using System.Collections.Generic;
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
    private MapData mapData;
    private MapRenderer mapRenderer;

    // Tracks active U capture handlers so re-pressing U removes old handlers
    // before the dispatched units reach their destination.
    private readonly System.Collections.Generic.Dictionary<UnitCombat, System.Action> uCaptureHandlers
        = new System.Collections.Generic.Dictionary<UnitCombat, System.Action>();

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
        PlotCaptureService.Reset(); // fresh capture state per game
        Debug.Log($"[GameEntry] Map loaded: {mapData.Plots.Count} plots, {mapData.Roads.Count} roads.");

        // ── Render map visuals ──
        var rendererObj = new GameObject("MapRenderer");
        mapRenderer = rendererObj.AddComponent<MapRenderer>();
        mapRenderer.Initialize(mapData);

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

        // Y = print connectable neutral plots for main-base ruin (test MVP-03.11)
        if (Input.GetKeyDown(KeyCode.Y) && mapData != null)
        {
            var ruins = FindObjectsByType<RuinComponent>(FindObjectsSortMode.None);
            bool found = false;
            foreach (var r in ruins)
            {
                if (!r.CanUseAsRallyPoint(mapData)) continue;
                var plots = r.GetConnectableNeutralPlots(mapData);
                if (plots.Count > 0)
                {
                    Debug.Log($"[GameEntry] Test shortcut Y: {r.sourcePlotId} can connect to: {string.Join(", ", plots)}");
                    found = true;
                }
                else
                {
                    Debug.Log($"[GameEntry] Test shortcut Y: {r.sourcePlotId} has no neutral neighbours.");
                    found = true;
                }
            }
            if (!found)
                Debug.Log("[GameEntry] Test shortcut Y: no main-base ruin found.");
        }

        // U = dispatch nearby Player soldiers from main-base ruin to first
        //     connectable neutral plot (test MVP-03.12)
        if (Input.GetKeyDown(KeyCode.U) && mapData != null)
        {
            // Find first main-base ruin
            RuinComponent rallyRuin = null;
            foreach (var r in FindObjectsByType<RuinComponent>(FindObjectsSortMode.None))
            {
                if (r.CanUseAsRallyPoint(mapData)) { rallyRuin = r; break; }
            }
            if (rallyRuin == null)
            {
                Debug.Log("[GameEntry] Test shortcut U: no main-base ruin.");
            }
            else
            {
                var plots = rallyRuin.GetConnectableNeutralPlots(mapData);
                if (plots.Count == 0)
                {
                    Debug.Log($"[GameEntry] Test shortcut U: {rallyRuin.sourcePlotId} has no connectable neutral plots.");
                }
                else
                {
                    string targetPlotId = plots[0];
                    // Find path from ruin's plot to target plot
                    var pathIds = RoadPathFinder.FindPath(mapData, rallyRuin.sourcePlotId, targetPlotId);
                    if (pathIds == null || pathIds.Count < 2)
                    {
                        Debug.Log($"[GameEntry] Test shortcut U: no road path to {targetPlotId}.");
                    }
                    else
                    {
                        var waypoints = new System.Collections.Generic.List<Vector3>();
                        foreach (var id in pathIds)
                        {
                            var p = mapData.GetPlot(id);
                            if (p != null)
                                waypoints.Add(new Vector3(p.worldPosition.x, p.worldPosition.y, -0.2f));
                        }
                        // Find Player soldiers near the ruin
                        Vector3 ruinPos = rallyRuin.transform.position;
                        int count = 0;
                        bool captureOnce = false;
                        foreach (var u in FindObjectsByType<UnitCombat>(FindObjectsSortMode.None))
                        {
                            if (u.faction != Faction.Player) continue;
                            if (u.GetComponent<HealthComponent>().IsDead) continue;
                            float d = Vector3.Distance(u.transform.position, ruinPos);
                            if (d > 5f) continue; // only nearby soldiers
                            u.ClearPushPath();
                            u.GetComponent<UnitMovement>()?.Stop();
                            // Remove any old U capture handler for this unit
                            if (uCaptureHandlers.TryGetValue(u, out var oldHandler))
                            {
                                u.OnPushDestinationReached -= oldHandler;
                                uCaptureHandlers.Remove(u);
                            }
                            // One-shot handler: self-unsubscribes + removes from dict
                            System.Action localHandler = null;
                            localHandler = () =>
                            {
                                u.OnPushDestinationReached -= localHandler;
                                uCaptureHandlers.Remove(u);
                                if (!captureOnce)
                                {
                                    captureOnce = true;
                                    PlotCaptureService.TryCapture(
                                        targetPlotId, mapData, mapRenderer, Faction.Player);
                                }
                            };
                            uCaptureHandlers[u] = localHandler;
                            u.OnPushDestinationReached += localHandler;
                            u.SetPushPath(new List<Vector3>(waypoints));
                            count++;
                        }
                        Debug.Log($"[GameEntry] Test shortcut U: dispatched {count} soldiers to {targetPlotId} ({string.Join("->", pathIds)}).");
                    }
                }
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
