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
    // Test-shortcut references
    private HealthComponent playerBaseHealth;
    private HealthComponent enemyBaseHealth;
    private MapData mapData; // kept for test shortcuts (R = rebuild)

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoInitialize()
    {
        var go = new GameObject("GameEntry");
        go.AddComponent<GameEntry>();
    }

    private void Start()
    {
        // ── Build fixed map data ──
        mapData = MapData.CreateFixedMap();
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

        // R = rebuild the first rebuildable ruin (skips main base ruins)
        if (Input.GetKeyDown(KeyCode.R) && mapData != null)
        {
            var ruins = FindObjectsByType<RuinComponent>(FindObjectsSortMode.None);
            bool rebuilt = false;
            foreach (var r in ruins)
            {
                if (r.IsMainBaseRuin(mapData)) continue; // skip main base
                var result = BuildingRebuildService.Rebuild(r, mapData, Faction.Player);
                if (result != null)
                {
                    Debug.Log("[GameEntry] Test shortcut R: rebuild OK.");
                    rebuilt = true;
                    break;
                }
            }
            if (!rebuilt)
                Debug.Log("[GameEntry] Test shortcut R: no rebuildable ruins.");
        }

        // T = rally all Player soldiers to the first main-base ruin (test MVP-03.10)
        if (Input.GetKeyDown(KeyCode.T) && mapData != null)
        {
            var ruins = FindObjectsByType<RuinComponent>(FindObjectsSortMode.None);
            RuinComponent rallyRuin = null;
            foreach (var r in ruins)
            {
                if (r.CanUseAsRallyPoint(mapData))
                {
                    rallyRuin = r;
                    break;
                }
            }
            if (rallyRuin == null)
            {
                Debug.Log("[GameEntry] Test shortcut T: no main-base ruin found.");
            }
            else
            {
                Vector3 rallyPos = rallyRuin.transform.position;
                int count = 0;
                var units = FindObjectsByType<UnitCombat>(FindObjectsSortMode.None);
                foreach (var u in units)
                {
                    if (u.faction != Faction.Player) continue;
                    if (u.GetComponent<HealthComponent>().IsDead) continue;

                    // Clear any active orders
                    u.ClearPushPath();
                    u.GetComponent<UnitMovement>()?.Stop();

                    // Set patrol around the ruin with staggered angle
                    var patrol = u.GetComponent<UnitPatrol>();
                    if (patrol != null)
                        patrol.Setup(new Vector3(rallyPos.x, rallyPos.y, -0.2f), 0.9f, (count % 12) * 30f);

                    count++;
                }
                Debug.Log($"[GameEntry] Test shortcut T: rallied {count} soldiers to main-base ruin.");
            }
        }
    }

    // ── Building setup ─────────────────────────────────────────────────

    private void SetupBuildings(MapData mapData)
    {
        // Buildings register themselves via BuildingFactory → BuildingRegistry.
        // No manual tracking needed here.

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

        // ── Faction defeat handlers (queries BuildingRegistry internally) ──
        if (playerBarracks != null)
        {
            playerBaseHealth = playerBarracks.GetComponent<HealthComponent>();
            var playerHandlerGo = new GameObject("PlayerDefeatHandler");
            var playerHandler = playerHandlerGo.AddComponent<FactionDefeatHandler>();
            playerHandler.Initialize(Faction.Player);
            playerBaseHealth.OnDeath += (hc) => playerHandler.OnMainBaseDefeated();
        }

        if (enemyBarracks != null)
        {
            enemyBaseHealth = enemyBarracks.GetComponent<HealthComponent>();
            var enemyHandlerGo = new GameObject("EnemyDefeatHandler");
            var enemyHandler = enemyHandlerGo.AddComponent<FactionDefeatHandler>();
            enemyHandler.Initialize(Faction.Enemy);
            enemyBaseHealth.OnDeath += (hc) => enemyHandler.OnMainBaseDefeated();
        }

        Debug.Log("[GameEntry] Buildings placed, defeat handlers active.");
    }

    // ── Factory helpers ────────────────────────────────────────────────

    private static GameObject CreateBuilding(string plotId, MapData mapData, Faction faction, BuildingType type)
    {
        return BuildingFactory.CreateBuilding(plotId, mapData, faction, type);
    }

}
