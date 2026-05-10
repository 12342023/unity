using System.Collections.Generic;
using Core;
using Map;
using Units;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// UI-free / platform-free command service for dispatching Enemy soldiers
    /// from an Enemy-owned plot toward a Player-owned target.
    ///
    /// This service does NOT handle capture — enemy soldiers only attack
    /// player buildings/units after arrival. Capture is for future iterations.
    ///
    /// Has NO dependency on HUD, keyboard, mouse, OnGUI, or platform APIs.
    /// </summary>
    public static class EnemyAttackCommandService
    {
        public class EnemyAttackResult
        {
            public bool success;
            public int dispatchedCount;
            public string sourcePlotId;
            public string targetPlotId;
            public string message;
        }

        /// <summary>
        /// Dispatch Enemy soldiers near <paramref name="sourcePlotId"/>
        /// toward <paramref name="targetPlotId"/> along the road network.
        /// Validates both plots, finds the road path, gathers nearby soldiers.
        /// </summary>
        public static EnemyAttackResult DispatchAttack(
            MapData mapData,
            string sourcePlotId,
            string targetPlotId,
            float gatherRadius = 5f)
        {
            if (mapData == null)
                return Fail("MapData is null.");

            // Reject if match already decided (MVP-04.3)
            if (MatchResultService.CurrentResult != MatchResult.None)
                return Fail($"Match ended ({MatchResultService.CurrentResult}), attack rejected.");

            var sourcePlot = mapData.GetPlot(sourcePlotId);
            if (sourcePlot == null)
                return Fail($"Source plot '{sourcePlotId}' not found.");

            if (sourcePlot.faction != Faction.Enemy)
                return Fail($"Source '{sourcePlotId}' is not Enemy-owned (current: {sourcePlot.faction}).");

            var targetPlot = mapData.GetPlot(targetPlotId);
            if (targetPlot == null)
                return Fail($"Target plot '{targetPlotId}' not found.");

            if (targetPlot.faction != Faction.Player)
                return Fail($"Target '{targetPlotId}' is not Player-owned (current: {targetPlot.faction}).");

            // Find road path
            var pathIds = RoadPathFinder.FindPath(mapData, sourcePlotId, targetPlotId);
            if (pathIds == null || pathIds.Count < 2)
                return Fail($"No road path from '{sourcePlotId}' to '{targetPlotId}'.", sourcePlotId, targetPlotId);

            // Build waypoints
            var waypoints = new List<Vector3>();
            foreach (var id in pathIds)
            {
                var p = mapData.GetPlot(id);
                if (p != null)
                    waypoints.Add(new Vector3(p.worldPosition.x, p.worldPosition.y, -0.2f));
            }

            Vector3 sourcePos = new Vector3(sourcePlot.worldPosition.x, sourcePlot.worldPosition.y, -0.2f);

            // Gather non-dead Enemy soldiers near source
            int count = 0;
            foreach (var u in GameObject.FindObjectsByType<UnitCombat>(FindObjectsSortMode.None))
            {
                if (u.faction != Faction.Enemy) continue;
                if (u.GetComponent<HealthComponent>().IsDead) continue;
                if (Vector3.Distance(u.transform.position, sourcePos) > gatherRadius) continue;

                u.ClearPushPath();
                u.GetComponent<UnitMovement>()?.Stop();
                u.SetPushPath(new List<Vector3>(waypoints));
                count++;
            }

            bool anyDispatched = count > 0;
            if (anyDispatched)
            {
                string msg = $"Enemy attack: {count} soldiers from {sourcePlotId} → {targetPlotId}.";
                Debug.Log($"[EnemyAttackCommandService] {msg}");
                GameStatusService.LastEnemyActionResult = msg;
                return new EnemyAttackResult
                {
                    success = true,
                    dispatchedCount = count,
                    sourcePlotId = sourcePlotId,
                    targetPlotId = targetPlotId,
                    message = msg
                };
            }
            else
            {
                string msg = $"Enemy attack: no available soldiers near {sourcePlotId} for attack on {targetPlotId}.";
                Debug.Log($"[EnemyAttackCommandService] {msg}");
                GameStatusService.LastEnemyActionResult = msg;
                return new EnemyAttackResult
                {
                    success = false,
                    dispatchedCount = 0,
                    sourcePlotId = sourcePlotId,
                    targetPlotId = targetPlotId,
                    message = msg
                };
            }
        }

        /// <summary>
        /// Convenience: pick the best source (Enemy plot with most soldiers)
        /// and best target (PlayerBase first, then Player frontier), then dispatch.
        /// Returns false with a descriptive message if no valid attack is possible.
        /// </summary>
        public static EnemyAttackResult DispatchAttackToBestTarget(MapData mapData, float gatherRadius = 5f)
        {
            if (mapData == null)
                return Fail("MapData is null.");

            // Reject if match already decided (MVP-04.3)
            if (MatchResultService.CurrentResult != MatchResult.None)
                return Fail($"Match ended ({MatchResultService.CurrentResult}), attack rejected.");

            // ── Pick target: PlayerBase first, then any Player-owned frontier ──
            string targetId = null;
            var playerBase = mapData.GetPlot("PlayerBase");
            if (playerBase != null && playerBase.faction == Faction.Player)
                targetId = "PlayerBase";

            if (targetId == null)
            {
                // Fallback: any Player-owned non-main-base plot
                foreach (var plot in mapData.Plots)
                {
                    if (plot.faction == Faction.Player && !plot.isMainBase)
                    {
                        targetId = plot.plotId;
                        break;
                    }
                }
            }

            if (targetId == null)
                return Fail("No Player-owned target plot for enemy attack.");

            // ── Pick source: Enemy plot with the most nearby soldiers ──
            string bestSourceId = null;
            int bestCount = 0;

            foreach (var plot in mapData.Plots)
            {
                if (plot.faction != Faction.Enemy) continue;

                Vector3 plotPos = new Vector3(plot.worldPosition.x, plot.worldPosition.y, -0.2f);
                int nearby = 0;
                foreach (var u in GameObject.FindObjectsByType<UnitCombat>(FindObjectsSortMode.None))
                {
                    if (u.faction != Faction.Enemy) continue;
                    if (u.GetComponent<HealthComponent>().IsDead) continue;
                    if (Vector3.Distance(u.transform.position, plotPos) <= gatherRadius)
                        nearby++;
                }

                if (nearby > bestCount)
                {
                    bestCount = nearby;
                    bestSourceId = plot.plotId;
                }
            }

            if (bestSourceId == null || bestCount == 0)
            {
                string msg = "Enemy attack skipped: no Enemy soldiers available near any Enemy plot.";
                Debug.Log($"[EnemyAttackCommandService] {msg}");
                GameStatusService.LastEnemyActionResult = msg;
                return new EnemyAttackResult { success = false, message = msg };
            }

            // Check road path before dispatching
            var pathIds = RoadPathFinder.FindPath(mapData, bestSourceId, targetId);
            if (pathIds == null || pathIds.Count < 2)
            {
                string msg = $"Enemy attack skipped: no road path from '{bestSourceId}' to '{targetId}'.";
                Debug.Log($"[EnemyAttackCommandService] {msg}");
                GameStatusService.LastEnemyActionResult = msg;
                return new EnemyAttackResult
                {
                    success = false,
                    sourcePlotId = bestSourceId,
                    targetPlotId = targetId,
                    message = msg
                };
            }

            return DispatchAttack(mapData, bestSourceId, targetId, gatherRadius);
        }

        private static EnemyAttackResult Fail(string msg, string sourceId = "", string targetId = "")
        {
            return new EnemyAttackResult
            {
                success = false,
                sourcePlotId = sourceId,
                targetPlotId = targetId,
                message = msg
            };
        }
    }
}
