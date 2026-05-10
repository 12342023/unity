using System.Collections.Generic;
using Map;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Orchestrates expansion from a Player-owned frontier plot to an adjacent
    /// Neutral plot: queries the expansion candidates, calculates the road path,
    /// and dispatches nearby soldiers.
    ///
    /// GameEntry's O shortcut calls this service instead of duplicating the
    /// orchestration logic.
    /// </summary>
    public static class StrategicExpansionService
    {
        public class ExpansionResult
        {
            public bool success;
            public string message;
            public string sourcePlotId;
            public string targetPlotId;
            public int dispatchedCount;
            public int requiredCount;
            public bool hasEnoughDispatchedSoldiers => dispatchedCount >= requiredCount;
        }

        /// <summary>
        /// Take the first ExpansionCandidate, calculate the road path,
        /// and dispatch nearby soldiers.
        /// </summary>
        public static ExpansionResult ExpandNext(MapData mapData, MapRenderer mapRenderer)
        {
            if (mapData == null)
                return Fail("MapData is null.");

            var candidates = StrategicConnectionService.GetExpansionCandidates(mapData);
            if (candidates.Count == 0)
                return Fail("No expansion candidates.");

            var candidate = candidates[0];
            var pathIds = RoadPathFinder.FindPath(mapData, candidate.sourcePlotId, candidate.targetPlotId);
            if (pathIds == null || pathIds.Count < 2)
                return Fail($"No road path from {candidate.sourcePlotId} to {candidate.targetPlotId}.",
                    candidate.sourcePlotId, candidate.targetPlotId);

            var waypoints = new List<Vector3>();
            foreach (var id in pathIds)
            {
                var p = mapData.GetPlot(id);
                if (p != null)
                    waypoints.Add(new Vector3(p.worldPosition.x, p.worldPosition.y, -0.2f));
            }

            Vector3 sourcePos = mapData.GetPlot(candidate.sourcePlotId).worldPosition;
            sourcePos.z = -0.2f;

            // Query the requirement (used to gate DispatchToPlot)
            var targetPlot = mapData.GetPlot(candidate.targetPlotId);
            int required = PlotCaptureRequirementService.GetRequiredSoldierCount(targetPlot);

            var dispatchResult = StrategicDispatchService.DispatchToPlot(
                sourcePos, waypoints, candidate.targetPlotId, mapData, mapRenderer, required);

            return new ExpansionResult
            {
                success = dispatchResult.dispatchedCount > 0,
                sourcePlotId = candidate.sourcePlotId,
                targetPlotId = candidate.targetPlotId,
                dispatchedCount = dispatchResult.dispatchedCount,
                requiredCount = required,
                message = dispatchResult.message
            };
        }

        private static ExpansionResult Fail(string msg, string sourceId = "", string targetId = "")
        {
            GameStatusService.LastActionResult = msg;
            return new ExpansionResult { success = false, message = msg, sourcePlotId = sourceId, targetPlotId = targetId };
        }
    }
}
