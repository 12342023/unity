using System.Collections.Generic;
using System.Linq;
using Combat;
using Core;
using Map;
using Units;
using UnityEngine;

namespace Buildings
{
    /// <summary>
    /// Barracks building: spawns Soldiers that rally then patrol,
    /// and periodically pushes gathered units in a wave toward the enemy.
    /// </summary>
    [RequireComponent(typeof(HealthComponent))]
    public class BarracksSpawner : MonoBehaviour
    {
        [Header("Spawn")]
        public float spawnInterval = 5f;

        [Header("Rally & Wave")]
        public string rallyPlotId = "";
        public string pushTargetPlotId = "";
        public int rallyThreshold = 3;

        [Header("Unit Stats")]
        public float unitHealth = 30f;
        public float unitDamage = 10f;
        public float unitAttackRange = 1.5f;
        public float unitAttackInterval = 1.0f;
        public float unitAggroRange = 4f;
        public float unitChaseRange = 7f;

        private MapData mapData;
        private Faction faction;
        private string currentPlotId;
        private HealthComponent enemyTarget;   // building to destroy in push waves
        private float timer;
        private float waveCheckTimer;
        private float throttleLogCooldown; // prevents spam when supply-capped
        private Sprite unitSprite;

        // Track spawned units for wave management
        private readonly List<GameObject> spawnedUnits = new();

        // ── Public API ──────────────────────────────────────────────────

        public void Initialize(MapData map, Faction fact, string plotId)
        {
            mapData = map;
            faction = fact;
            currentPlotId = plotId;
            GenerateUnitSprite();
        }

        /// <summary>Set the enemy building that push waves should attack.</summary>
        public void SetEnemyTarget(HealthComponent target)
        {
            enemyTarget = target;
        }

        // ── MonoBehaviour ───────────────────────────────────────────────

        private void Update()
        {
            if (enemyTarget == null || enemyTarget.IsDead)
                return;

            // Remove destroyed units from tracking
            spawnedUnits.RemoveAll(u => u == null);

            // Throttle log cooldown
            throttleLogCooldown -= Time.deltaTime;

            // Spawn timer (respects supply cap — MVP-04.2)
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                timer = 0f;
                if (FactionStatsService.CanSpawn(faction))
                {
                    SpawnUnit();
                }
                else if (throttleLogCooldown <= 0f)
                {
                    throttleLogCooldown = 10f;
                    int current = FactionStatsService.CountAliveUnits(faction);
                    int cap = FactionStatsService.GetSupplyCap(faction);
                    Debug.Log($"[Barracks] {faction} spawn throttled: {current}/{cap} at supply cap.");
                }
            }

            // Wave check every 2 seconds
            waveCheckTimer += Time.deltaTime;
            if (waveCheckTimer >= 2f)
            {
                waveCheckTimer = 0f;
                TryPushWave();
            }
        }

        // ── Unit creation ───────────────────────────────────────────────

        private void SpawnUnit()
        {
            string destPlotId = string.IsNullOrEmpty(rallyPlotId) ? currentPlotId : rallyPlotId;
            bool needRoadMove = destPlotId != currentPlotId;
            string logPath = destPlotId;

            // ── GameObject ──
            var go = new GameObject($"{faction}_Soldier_{Time.frameCount}");
            go.transform.position = new Vector3(transform.position.x, transform.position.y, -0.2f);
            go.transform.localScale = Vector3.one * 0.4f;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = unitSprite;
            sr.color = UnitConfig.GetColor(UnitType.Soldier);
            sr.sortingOrder = 3;

            // ── Health ──
            var health = go.AddComponent<HealthComponent>();
            health.maxHealth = unitHealth;
            health.faction = faction;

            // ── Movement ──
            var movement = go.AddComponent<UnitMovement>();
            movement.speed = UnitConfig.GetSpeed(UnitType.Soldier);

            // ── Collider ──
            var col = go.AddComponent<CircleCollider2D>();
            col.radius = 0.2f;
            col.isTrigger = true;

            // ── Patrol center ──
            Vector3 patrolCenter = mapData.GetPlot(destPlotId)?.worldPosition ?? transform.position;
            Vector3 homePos = new Vector3(patrolCenter.x, patrolCenter.y, -0.2f);

            // ── Circular patrol ──
            var patrol = go.AddComponent<UnitPatrol>();
            float startAngle = (spawnedUnits.Count % 12) * 30f;
            patrol.Setup(homePos, 0.9f, startAngle);

            // ── Combat ──
            var combat = go.AddComponent<UnitCombat>();
            combat.damage = unitDamage;
            combat.attackRange = unitAttackRange;
            combat.attackInterval = unitAttackInterval;
            combat.aggroRange = unitAggroRange;
            combat.chaseRange = unitChaseRange;
            combat.faction = faction;
            combat.SetHomePosition(homePos);

            // ── Road movement to rally point (if different from current plot) ──
            if (needRoadMove)
            {
                var pathIds = RoadPathFinder.FindPath(mapData, currentPlotId, destPlotId);
                if (pathIds != null && pathIds.Count >= 2)
                {
                    var waypoints = pathIds
                        .Select(id => mapData.GetPlot(id))
                        .Where(p => p != null)
                        .Select(p =>
                        {
                            var pos = p.worldPosition;
                            return new Vector3(pos.x, pos.y, -0.2f);
                        })
                        .ToList();

                    movement.StartMoving(waypoints);
                    logPath = string.Join(" -> ", pathIds);
                    Debug.Log($"[Barracks] Spawned {faction} Soldier, moving to rally {destPlotId}: {logPath}");
                }
                else
                {
                    // Rally plot unreachable — patrol at spawn position
                    Debug.Log($"[Barracks] Spawned {faction} Soldier at {currentPlotId} (rally {destPlotId} unreachable), patrolling locally.");
                }
            }
            else
            {
                // No rally point — patrol around Barracks immediately
                Debug.Log($"[Barracks] Spawned {faction} Soldier at {currentPlotId}, patrolling locally (no rally point).");
            }

            // Track
            spawnedUnits.Add(go);
        }

        // ── Wave push ───────────────────────────────────────────────────

        private void TryPushWave()
        {
            if (spawnedUnits.Count == 0) return;
            if (string.IsNullOrEmpty(pushTargetPlotId)) return;

            // Count units near the rally point and NOT currently engaged in combat
            var rallyPlot = mapData.GetPlot(string.IsNullOrEmpty(rallyPlotId) ? currentPlotId : rallyPlotId);
            if (rallyPlot == null) return;

            var gathered = new List<GameObject>();
            float rallyDist = rallyPlot.size == PlotSize.Small ? 3f : 4f;

            foreach (var u in spawnedUnits)
            {
                if (u == null) continue;
                var combat = u.GetComponent<UnitCombat>();
                if (combat == null || combat.IsEngaged) continue; // skip fighting units
                float d = Vector3.Distance(u.transform.position, rallyPlot.worldPosition);
                if (d <= rallyDist)
                    gathered.Add(u);
            }

            if (gathered.Count < rallyThreshold)
                return;

            // Find path from rally point to push target
            var targetPlotId = pushTargetPlotId;
            if (string.IsNullOrEmpty(targetPlotId)) return;

            // Calculate enemy target plot position (for combat reference)
            string targetPlot = FindTargetPlotId();
            if (targetPlot == null) return;

            var pathIds = RoadPathFinder.FindPath(mapData, rallyPlot.plotId, targetPlot);
            if (pathIds == null || pathIds.Count < 2) return;

            var pushWaypoints = pathIds
                .Select(id => mapData.GetPlot(id))
                .Where(p => p != null)
                .Select(p =>
                {
                    var pos = p.worldPosition;
                    return new Vector3(pos.x, pos.y, -0.2f);
                })
                .ToList();

            // Command gathered units to push
            foreach (var u in gathered)
            {
                if (u == null) continue;
                var combat = u.GetComponent<UnitCombat>();
                if (combat != null)
                {
                    combat.SetPushPath(new List<Vector3>(pushWaypoints));
                }
            }

            Debug.Log($"[Barracks] Wave push! {gathered.Count} units from {rallyPlot.plotId} -> {targetPlot}");
        }

        private string FindTargetPlotId()
        {
            if (enemyTarget == null || mapData == null) return null;

            foreach (var plot in mapData.Plots)
            {
                float dist = Vector3.Distance(
                    enemyTarget.transform.position, plot.worldPosition);
                if (dist < 1.5f)
                    return plot.plotId;
            }
            return pushTargetPlotId;
        }

        // ── Sprite ──────────────────────────────────────────────────────

        private void GenerateUnitSprite()
        {
            int size = 12;
            var tex = new Texture2D(size, size);
            float center = (size - 1) / 2f;
            float radius = center - 1;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - center;
                    float dy = y - center;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    tex.SetPixel(x, y, dist <= radius ? Color.white : Color.clear);
                }
            }
            tex.Apply();
            unitSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 8f);
        }
    }
}
