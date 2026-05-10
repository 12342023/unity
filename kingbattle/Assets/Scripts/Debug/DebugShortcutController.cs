using System.Collections.Generic;
using Buildings;
using Combat;
using Core;
using Map;
using Units;
using UnityEngine;

/// <summary>
/// Hosts all debug/test keyboard shortcuts formerly in GameEntry.Update().
/// GameEntry.Start() initialises this component and passes the needed references.
///
/// All shortcuts behave identically to the original inline code in GameEntry.
/// After MVP this component can be removed or hidden behind a debug flag.
/// </summary>
public class DebugShortcutController : MonoBehaviour
{
    private MapData mapData;
    private MapRenderer mapRenderer;
    private EnemyPressureController enemyController;
    private HealthComponent playerBaseHealth;
    private HealthComponent enemyBaseHealth;

    public void Initialize(
        MapData data,
        MapRenderer renderer,
        EnemyPressureController enemyCtrl,
        HealthComponent playerBaseHp,
        HealthComponent enemyBaseHp)
    {
        mapData = data;
        mapRenderer = renderer;
        enemyController = enemyCtrl;
        playerBaseHealth = playerBaseHp;
        enemyBaseHealth = enemyBaseHp;
    }

    private void Update()
    {
        if (mapData == null) return;

        // K = destroy EnemyBase (test faction defeat cleanup)
        if (Input.GetKeyDown(KeyCode.K) && enemyBaseHealth != null && !enemyBaseHealth.IsDead)
        {
            Debug.Log("[Debug] K: destroying EnemyBase...");
            enemyBaseHealth.TakeDamage(enemyBaseHealth.CurrentHealth);
        }

        // L = destroy PlayerBase (test faction defeat cleanup)
        if (Input.GetKeyDown(KeyCode.L) && playerBaseHealth != null && !playerBaseHealth.IsDead)
        {
            Debug.Log("[Debug] L: destroying PlayerBase...");
            playerBaseHealth.TakeDamage(playerBaseHealth.CurrentHealth);
        }

        // E = trigger enemy attack immediately
        if (Input.GetKeyDown(KeyCode.E) && enemyController != null)
        {
            Debug.Log("[Debug] E: triggering enemy attack...");
            enemyController.TriggerAttack();
        }

        // N = restart after match ended
        if (Input.GetKeyDown(KeyCode.N) && MatchResultService.CurrentResult != MatchResult.None)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.Log("[Debug] N: no active scene name, cannot reload. Add the scene to Build Settings.");
            }
            else
            {
                Debug.Log($"[Debug] N: restarting via {sceneName}");
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            }
        }

        // R = rebuild first rebuildable ruin (skips main base ruins)
        if (Input.GetKeyDown(KeyCode.R))
        {
            var ruins = FindObjectsByType<RuinComponent>(FindObjectsSortMode.None);
            bool rebuilt = false;
            foreach (var r in ruins)
            {
                if (r.IsMainBaseRuin(mapData)) continue;
                var result = BuildingRebuildService.Rebuild(r, mapData, Faction.Player);
                if (result != null)
                {
                    Debug.Log("[Debug] R: rebuild OK.");
                    rebuilt = true;
                    break;
                }
            }
            if (!rebuilt)
                Debug.Log("[Debug] R: no rebuildable ruins.");
        }

        // T = rally all Player soldiers to first main-base ruin
        if (Input.GetKeyDown(KeyCode.T))
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
                Debug.Log("[Debug] T: no main-base ruin found.");
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

                    u.ClearPushPath();
                    u.GetComponent<UnitMovement>()?.Stop();

                    var patrol = u.GetComponent<UnitPatrol>();
                    if (patrol != null)
                        patrol.Setup(new Vector3(rallyPos.x, rallyPos.y, -0.2f), 0.9f, (count % 12) * 30f);
                    count++;
                }
                Debug.Log($"[Debug] T: rallied {count} soldiers to main-base ruin.");
            }
        }

        // Y = print connectable neutral plots for main-base ruin
        if (Input.GetKeyDown(KeyCode.Y))
        {
            var ruins = FindObjectsByType<RuinComponent>(FindObjectsSortMode.None);
            bool found = false;
            foreach (var r in ruins)
            {
                if (!r.CanUseAsRallyPoint(mapData)) continue;
                var plots = r.GetConnectableNeutralPlots(mapData);
                if (plots.Count > 0)
                {
                    Debug.Log($"[Debug] Y: {r.sourcePlotId} can connect to: {string.Join(", ", plots)}");
                    found = true;
                }
                else
                {
                    Debug.Log($"[Debug] Y: {r.sourcePlotId} has no neutral neighbours.");
                    found = true;
                }
            }
            if (!found)
                Debug.Log("[Debug] Y: no main-base ruin found.");
        }

        // U = dispatch from main-base ruin to first connectable neutral
        if (Input.GetKeyDown(KeyCode.U))
        {
            RuinComponent rallyRuin = null;
            foreach (var r in FindObjectsByType<RuinComponent>(FindObjectsSortMode.None))
            {
                if (r.CanUseAsRallyPoint(mapData)) { rallyRuin = r; break; }
            }
            if (rallyRuin == null)
            {
                GameStatusService.LastActionResult = "U: no main-base ruin.";
                Debug.Log("[Debug] U: no main-base ruin.");
            }
            else
            {
                var plots = rallyRuin.GetConnectableNeutralPlots(mapData);
                if (plots.Count == 0)
                {
                    GameStatusService.LastActionResult = $"U: {rallyRuin.sourcePlotId} has no connectable neutral plots.";
                    Debug.Log($"[Debug] U: {rallyRuin.sourcePlotId} has no connectable neutral plots.");
                }
                else
                {
                    string targetPlotId = plots[0];
                    var pathIds = RoadPathFinder.FindPath(mapData, rallyRuin.sourcePlotId, targetPlotId);
                    if (pathIds == null || pathIds.Count < 2)
                    {
                        GameStatusService.LastActionResult = $"U: no road path to {targetPlotId}.";
                        Debug.Log($"[Debug] U: no road path to {targetPlotId}.");
                    }
                    else
                    {
                        var waypoints = new List<Vector3>();
                        foreach (var id in pathIds)
                        {
                            var p = mapData.GetPlot(id);
                            if (p != null)
                                waypoints.Add(new Vector3(p.worldPosition.x, p.worldPosition.y, -0.2f));
                        }
                        Vector3 ruinPos = rallyRuin.transform.position;
                        var targetPlot = mapData.GetPlot(targetPlotId);
                        int required = PlotCaptureRequirementService.GetRequiredSoldierCount(targetPlot);
                        var dispatchResult = StrategicDispatchService.DispatchToPlot(
                            ruinPos, waypoints, targetPlotId, mapData, mapRenderer, required);
                        GameStatusService.LastActionResult = dispatchResult.message;
                        Debug.Log($"[Debug] U: {dispatchResult.message}");
                    }
                }
            }
        }

        // I = print Player-owned frontier plots with neutral neighbours
        if (Input.GetKeyDown(KeyCode.I))
        {
            var frontiers = StrategicConnectionService.GetPlayerFrontierPlots(mapData);
            if (frontiers.Count == 0)
            {
                Debug.Log("[Debug] I: no Player-owned frontier plot with neutral neighbours.");
            }
            else
            {
                foreach (var f in frontiers)
                {
                    Debug.Log($"[Debug] I: {f.plotId} can connect to: {string.Join(", ", f.connectableNeutralPlots)}");
                }
            }
        }

        // O = dispatch from first Player frontier plot to its first neutral neighbour
        if (Input.GetKeyDown(KeyCode.O))
        {
            var candidates = StrategicConnectionService.GetExpansionCandidates(mapData);
            if (candidates.Count == 0)
            {
                GameStatusService.LastActionResult = "O: no expansion candidates.";
                Debug.Log("[Debug] O: no expansion candidates.");
            }
            else
            {
                var c = candidates[0];
                var result = StrategicExpansionCommandService.DispatchCandidate(
                    mapData, mapRenderer, c.sourcePlotId, c.targetPlotId);
                GameStatusService.LastActionResult = result.message;
                Debug.Log($"[Debug] O: {result.message}");
            }
        }

        // P = print capture requirements per plot
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("[Debug] P: capture requirements:");
            foreach (var plot in mapData.Plots)
            {
                int req = PlotCaptureRequirementService.GetRequiredSoldierCount(plot);
                Debug.Log($"  {plot.plotId} ({plot.size}, {plot.faction}) → requires {req} soldier(s)");
            }
        }

        // Q = print all expansion previews
        if (Input.GetKeyDown(KeyCode.Q))
        {
            var previews = StrategicConnectionService.GetExpansionPreviews(mapData);
            if (previews.Count == 0)
            {
                Debug.Log("[Debug] Q: no expansion previews.");
            }
            else
            {
                Debug.Log("[Debug] Q: expansion previews:");
                foreach (var p in previews)
                {
                    Debug.Log($"  {p.sourcePlotId} → {p.targetPlotId}: avail={p.availableCount}, req={p.requiredCount}, enough={p.hasEnough}");
                }
            }
        }
    }
}
