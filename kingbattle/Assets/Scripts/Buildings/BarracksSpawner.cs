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
    /// Barracks building: periodically spawns a Soldier unit that moves
    /// along the road toward the enemy target building.
    /// </summary>
    [RequireComponent(typeof(HealthComponent))]
    public class BarracksSpawner : MonoBehaviour
    {
        [Header("Spawn")]
        public float spawnInterval = 5f;

        [Header("Unit Stats")]
        public float unitHealth = 30f;
        public float unitDamage = 10f;
        public float unitAttackRange = 1.5f;
        public float unitAttackInterval = 1.0f;

        private MapData mapData;
        private Faction faction;
        private string currentPlotId;
        private HealthComponent targetBuilding;
        private float timer;
        private Sprite unitSprite;

        // ── Public API ──────────────────────────────────────────────────

        public void Initialize(MapData map, Faction fact, string plotId)
        {
            mapData = map;
            faction = fact;
            currentPlotId = plotId;
            GenerateUnitSprite();
        }

        /// <summary>Set the building that spawned units should attack.</summary>
        public void SetTargetBuilding(HealthComponent target)
        {
            targetBuilding = target;
        }

        // ── MonoBehaviour ───────────────────────────────────────────────

        private void Update()
        {
            if (targetBuilding == null || targetBuilding.IsDead)
                return;

            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                timer = 0f;
                SpawnUnit();
            }
        }

        // ── Unit creation ───────────────────────────────────────────────

        private void SpawnUnit()
        {
            // ── Path ──
            // Find the target plot ID from the target building's position
            string targetPlotId = FindTargetPlotId();
            if (targetPlotId == null) return;

            var pathIds = RoadPathFinder.FindPath(mapData, currentPlotId, targetPlotId);
            if (pathIds == null || pathIds.Count < 2)
                return;

            var waypoints = pathIds
                .Select(id => mapData.GetPlot(id))
                .Where(p => p != null)
                .Select(p =>
                {
                    var pos = p.worldPosition;
                    return new Vector3(pos.x, pos.y, -0.2f);
                })
                .ToList();

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
            movement.StartMoving(waypoints);

            // ── Collider (for Tower physics detection) ──
            var col = go.AddComponent<CircleCollider2D>();
            col.radius = 0.2f;
            col.isTrigger = true;

            // ── Combat ──
            var combat = go.AddComponent<UnitCombat>();
            combat.damage = unitDamage;
            combat.attackRange = unitAttackRange;
            combat.attackInterval = unitAttackInterval;
            combat.faction = faction;
            combat.SetTarget(targetBuilding);

            // Log
            var pathStr = string.Join(" -> ", pathIds);
            Debug.Log($"[Barracks] Spawned {faction} Soldier: {pathStr}");
        }

        private string FindTargetPlotId()
        {
            if (targetBuilding == null || mapData == null) return null;

            foreach (var plot in mapData.Plots)
            {
                float dist = Vector3.Distance(
                    targetBuilding.transform.position,
                    plot.worldPosition);
                if (dist < 1.5f)
                    return plot.plotId;
            }
            return null;
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
