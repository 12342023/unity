using Buildings;
using Combat;
using Core;
using Map;
using Units;
using UnityEngine;

/// <summary>
/// Thin scene startup script that bootstraps MVP-01 through MVP-02.1.
///
/// Responsibilities:
/// 1. Creates fixed map data.
/// 2. Renders map visually.
/// 3. Creates buildings and links them.
/// 4. Sets Barracks rally points and push targets.
/// 5. Activates test spawner (Key 1-4).
///
/// Deliberately thin — only wires things up, no business logic.
/// </summary>
public class GameEntry : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoInitialize()
    {
        var go = new GameObject("GameEntry");
        go.AddComponent<GameEntry>();
    }

    private void Start()
    {
        // ── Build fixed map data ──
        var mapData = MapData.CreateFixedMap();
        Debug.Log($"[GameEntry] Map loaded: {mapData.Plots.Count} plots, {mapData.Roads.Count} roads.");

        // ── Render map visuals ──
        var rendererObj = new GameObject("MapRenderer");
        var renderer = rendererObj.AddComponent<MapRenderer>();
        renderer.Initialize(mapData);

        // ── Create buildings ──
        SetupBuildings(mapData);

        // ── Activate test spawner ──
        var spawnerObj = new GameObject("TestUnitSpawner");
        var spawner = spawnerObj.AddComponent<TestUnitSpawner>();
        spawner.Initialize(mapData);

        Debug.Log("[GameEntry] MVP-02.1 ready. Units patrol, aggro, and wave-push.");
    }

    // ── Building setup ─────────────────────────────────────────────────

    private void SetupBuildings(MapData mapData)
    {
        // ── Create building GameObjects ──
        var playerBarracks = CreateBuilding("PlayerBase",   mapData, Faction.Player, BuildingType.Barracks);
        var enemyBarracks  = CreateBuilding("EnemyBase",    mapData, Faction.Enemy,  BuildingType.Barracks);
        var playerTower    = CreateBuilding("Crossroads",   mapData, Faction.Player, BuildingType.Tower);
        var enemyTower     = CreateBuilding("EnemyOutpost", mapData, Faction.Enemy,  BuildingType.Tower);
        var playerGranary  = CreateBuilding("Village",      mapData, Faction.Player, BuildingType.Granary);
        // Farmland intentionally left empty

        // ── Link Barracks to enemy targets, set rally points ──
        if (playerBarracks != null && enemyBarracks != null)
        {
            var pbSpawner = playerBarracks.GetComponent<BarracksSpawner>();
            if (pbSpawner != null)
            {
                pbSpawner.SetEnemyTarget(enemyBarracks.GetComponent<HealthComponent>());
                pbSpawner.rallyPlotId = "Village";          // gather at Village
                pbSpawner.pushTargetPlotId = "EnemyBase";   // push toward enemy base
                pbSpawner.rallyThreshold = 3;
            }
        }

        if (enemyBarracks != null && playerBarracks != null)
        {
            var ebSpawner = enemyBarracks.GetComponent<BarracksSpawner>();
            if (ebSpawner != null)
            {
                ebSpawner.SetEnemyTarget(playerBarracks.GetComponent<HealthComponent>());
                ebSpawner.rallyPlotId = "EnemyOutpost";     // gather at Outpost
                ebSpawner.pushTargetPlotId = "PlayerBase";  // push toward player base
                ebSpawner.rallyThreshold = 3;
            }
        }

        Debug.Log("[GameEntry] Buildings placed, rally points set, wave threshold = 3.");
    }

    // ── Factory helpers ────────────────────────────────────────────────

    private static GameObject CreateBuilding(string plotId, MapData mapData, Faction faction, BuildingType type)
    {
        var plot = mapData.GetPlot(plotId);
        if (plot == null) return null;

        var go = new GameObject($"{type}_{faction}_{plotId}");
        go.transform.position = new Vector3(plot.worldPosition.x, plot.worldPosition.y, -0.05f);

        // ── Visual ──
        var sr = go.AddComponent<SpriteRenderer>();
        var tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 1f);
        sr.sortingOrder = 2;

        switch (type)
        {
            case BuildingType.Barracks:
                sr.color = faction == Faction.Player
                    ? new Color(0.2f, 0.5f, 1.0f)
                    : new Color(1.0f, 0.3f, 0.3f);
                go.transform.localScale = Vector3.one * 0.8f;
                break;

            case BuildingType.Tower:
                sr.color = faction == Faction.Player
                    ? new Color(0.3f, 0.7f, 1.0f)
                    : new Color(1.0f, 0.5f, 0.2f);
                go.transform.localScale = new Vector3(0.6f, 0.9f, 1f);
                break;

            case BuildingType.Granary:
                sr.color = new Color(0.9f, 0.85f, 0.3f);
                go.transform.localScale = Vector3.one * 0.7f;
                break;
        }

        // ── Health ──
        var health = go.AddComponent<HealthComponent>();
        health.maxHealth = type == BuildingType.Tower ? 80f : 50f;
        health.faction = faction;

        // ── Collider (needed for Tower's physics scan) ──
        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = Vector2.one * 0.6f;

        // ── Type-specific component ──
        switch (type)
        {
            case BuildingType.Barracks:
                var barracks = go.AddComponent<BarracksSpawner>();
                barracks.Initialize(mapData, faction, plotId);
                break;

            case BuildingType.Tower:
                var tower = go.AddComponent<TowerAttack>();
                tower.faction = faction;
                tower.damage = 15f;
                tower.attackRange = 3.5f;
                tower.attackInterval = 1.5f;
                break;
            // Granary: no special component yet (just visual + health)
        }

        return go;
    }
}
