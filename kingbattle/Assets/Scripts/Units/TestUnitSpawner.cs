using System.Collections.Generic;
using Core;
using Map;
using UnityEngine;

namespace Units
{
    /// <summary>
    /// Temporary test entry point for MVP-01.
    ///
    /// Usage in Play Mode:
    ///   Key 1 → Spawn Samurai from PlayerBase → EnemyBase
    ///   Key 2 → Spawn Elf Archer from PlayerBase → EnemyBase
    ///   Key 3 → Spawn Soldier from PlayerBase → EnemyBase
    ///   Key 4 → Spawn Samurai from PlayerBase → Crossroads
    ///
    /// Console logs show spawned unit type, path, and arrival.
    /// </summary>
    public class TestUnitSpawner : MonoBehaviour
    {
        private MapData mapData;
        private Sprite unitSprite;

        // ── Initialisation ──────────────────────────────────────────────

        public void Initialize(MapData map)
        {
            mapData = map;
            GenerateUnitSprite();
            Debug.Log("[TestUnitSpawner] Ready. Press 1-4 to spawn units (Play Mode).");
        }

        private void GenerateUnitSprite()
        {
            int size = 16;
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

        // ── Input ───────────────────────────────────────────────────────

        private void Update()
        {
            if (mapData == null) return;

            if (Input.GetKeyDown(KeyCode.Alpha1))
                SpawnUnit(UnitType.Samurai, "PlayerBase", "EnemyBase");

            if (Input.GetKeyDown(KeyCode.Alpha2))
                SpawnUnit(UnitType.ElfArcher, "PlayerBase", "EnemyBase");

            if (Input.GetKeyDown(KeyCode.Alpha3))
                SpawnUnit(UnitType.Soldier, "PlayerBase", "EnemyBase");

            if (Input.GetKeyDown(KeyCode.Alpha4))
                SpawnUnit(UnitType.Samurai, "PlayerBase", "Crossroads");
        }

        // ── Spawn logic ─────────────────────────────────────────────────

        private void SpawnUnit(UnitType type, string fromPlotId, string toPlotId)
        {
            var fromPlot = mapData.GetPlot(fromPlotId);
            var toPlot = mapData.GetPlot(toPlotId);
            if (fromPlot == null || toPlot == null)
            {
                Debug.LogError($"[TestUnitSpawner] Plot not found: {fromPlotId} / {toPlotId}");
                return;
            }

            // Find road path
            var pathIds = RoadPathFinder.FindPath(mapData, fromPlotId, toPlotId);
            if (pathIds == null)
            {
                Debug.LogError($"[TestUnitSpawner] No road path from {fromPlotId} to {toPlotId}");
                return;
            }

            // Convert path to world positions
            var waypoints = new List<Vector3>();
            foreach (var id in pathIds)
            {
                var p = mapData.GetPlot(id);
                waypoints.Add(new Vector3(p.worldPosition.x, p.worldPosition.y, -0.2f));
            }

            // Log the path for debugging
            var pathStr = string.Join(" → ", pathIds);
            Debug.Log($"[TestUnitSpawner] Spawning {UnitConfig.GetDisplayName(type)}: {pathStr}");

            // Create unit GameObject
            var unit = new GameObject(UnitConfig.GetDisplayName(type));
            unit.transform.position = waypoints[0];

            var sr = unit.AddComponent<SpriteRenderer>();
            sr.sprite = unitSprite;
            sr.color = UnitConfig.GetColor(type);
            sr.sortingOrder = 3;
            unit.transform.localScale = Vector3.one * 0.5f;

            // Add movement
            var movement = unit.AddComponent<UnitMovement>();
            movement.speed = UnitConfig.GetSpeed(type);
            movement.StartMoving(waypoints);
        }
    }
}
