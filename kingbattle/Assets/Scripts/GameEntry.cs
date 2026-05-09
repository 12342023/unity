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
    // Test-shortcut references for MVP-03.2 faction defeat verification
    private HealthComponent playerBaseHealth;
    private HealthComponent enemyBaseHealth;

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

        Debug.Log("[GameEntry] MVP-02.1 ready. K=kill EnemyBase, L=kill PlayerBase.");
    }

    // ── Test shortcuts for MVP-03.2 faction defeat ─────────────────────

    private void Update()
    {
        // K = instantly destroy EnemyBase (test faction defeat cleanup)
        if (Input.GetKeyDown(KeyCode.K) && enemyBaseHealth != null && !enemyBaseHealth.IsDead)
        {
            Debug.Log("[GameEntry] Test shortcut K: destroying EnemyBase...");
            enemyBaseHealth.TakeDamage(enemyBaseHealth.CurrentHealth);
        }

        // L = instantly destroy PlayerBase (test faction defeat cleanup)
        if (Input.GetKeyDown(KeyCode.L) && playerBaseHealth != null && !playerBaseHealth.IsDead)
        {
            Debug.Log("[GameEntry] Test shortcut L: destroying PlayerBase...");
            playerBaseHealth.TakeDamage(playerBaseHealth.CurrentHealth);
        }
    }

    // ── Building setup ─────────────────────────────────────────────────

    private void SetupBuildings(MapData mapData)
    {
        // ── Track buildings per faction for defeat-handler cleanup ──
        var playerBuildings = new System.Collections.Generic.List<GameObject>();
        var enemyBuildings = new System.Collections.Generic.List<GameObject>();

        // ── Create building GameObjects ──
        var playerBarracks = CreateBuilding("PlayerBase",   mapData, Faction.Player, BuildingType.Barracks);
        var enemyBarracks  = CreateBuilding("EnemyBase",    mapData, Faction.Enemy,  BuildingType.Barracks);
        var playerTower    = CreateBuilding("Crossroads",   mapData, Faction.Player, BuildingType.Tower);
        var enemyTower     = CreateBuilding("EnemyOutpost", mapData, Faction.Enemy,  BuildingType.Tower);
        var playerGranary  = CreateBuilding("Village",      mapData, Faction.Player, BuildingType.Granary);
        // Farmland intentionally left empty

        // ── Group by faction ──
        if (playerBarracks != null) playerBuildings.Add(playerBarracks);
        if (playerTower != null)    playerBuildings.Add(playerTower);
        if (playerGranary != null)  playerBuildings.Add(playerGranary);
        if (enemyBarracks != null)  enemyBuildings.Add(enemyBarracks);
        if (enemyTower != null)     enemyBuildings.Add(enemyTower);

        // ── Link Barracks to enemy targets, set rally points ──
        if (playerBarracks != null && enemyBarracks != null)
        {
            var pbSpawner = playerBarracks.GetComponent<BarracksSpawner>();
            if (pbSpawner != null)
            {
                pbSpawner.SetEnemyTarget(enemyBarracks.GetComponent<HealthComponent>());
                pbSpawner.rallyPlotId = "Village";
                pbSpawner.pushTargetPlotId = "EnemyBase";
                pbSpawner.rallyThreshold = 3;
            }
        }

        if (enemyBarracks != null && playerBarracks != null)
        {
            var ebSpawner = enemyBarracks.GetComponent<BarracksSpawner>();
            if (ebSpawner != null)
            {
                ebSpawner.SetEnemyTarget(playerBarracks.GetComponent<HealthComponent>());
                ebSpawner.rallyPlotId = "EnemyOutpost";
                ebSpawner.pushTargetPlotId = "PlayerBase";
                ebSpawner.rallyThreshold = 3;
            }
        }

        // ── Faction defeat handlers ──
        if (playerBarracks != null)
        {
            playerBaseHealth = playerBarracks.GetComponent<HealthComponent>();
            var playerHandlerGo = new GameObject("PlayerDefeatHandler");
            var playerHandler = playerHandlerGo.AddComponent<FactionDefeatHandler>();
            playerHandler.Initialize(Faction.Player, playerBuildings);
            playerBaseHealth.OnDeath += (hc) => playerHandler.OnMainBaseDefeated();
        }

        if (enemyBarracks != null)
        {
            enemyBaseHealth = enemyBarracks.GetComponent<HealthComponent>();
            var enemyHandlerGo = new GameObject("EnemyDefeatHandler");
            var enemyHandler = enemyHandlerGo.AddComponent<FactionDefeatHandler>();
            enemyHandler.Initialize(Faction.Enemy, enemyBuildings);
            enemyBaseHealth.OnDeath += (hc) => enemyHandler.OnMainBaseDefeated();
        }

        Debug.Log("[GameEntry] Buildings placed, defeat handlers active.");
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

        // ── Building identity (for future reconstruction) ──
        var identity = go.AddComponent<BuildingIdentity>();
        identity.plotId = plotId;
        identity.buildingType = type;
        identity.faction = faction;

        // ── Building death → ruin (self-contained in BuildingDeathHandler) ──
        go.AddComponent<BuildingDeathHandler>();

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
