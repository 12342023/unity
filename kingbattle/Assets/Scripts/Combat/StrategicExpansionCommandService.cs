using System.Collections.Generic;
using Core;
using Map;
using Units;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Single entry point for dispatching an expansion to a specific target.
    /// Validates the candidate, finds the road path, calculates the requirement,
    /// and calls StrategicDispatchService.DispatchToPlot.
    ///
    /// This service has NO dependency on keyboard, mouse, OnGUI, or platform APIs.
    /// It is called by:
    ///   - GameEntry (O shortcut)
    ///   - GameHud (Dispatch buttons)
    ///   - StrategicExpansionService.ExpandNext (for backward compatibility)
    /// </summary>
    public static class StrategicExpansionCommandService
    {
        /// <summary>
        /// Dispatch Player soldiers from <paramref name="sourcePlotId"/> to capture
        /// <paramref name="targetPlotId"/>. Returns an ExpansionResult with structured
        /// status. Does NOT depend on UI or platform APIs.
        /// </summary>
        public static StrategicExpansionService.ExpansionResult DispatchCandidate(
            MapData mapData,
            MapRenderer mapRenderer,
            string sourcePlotId,
            string targetPlotId)
        {
            if (mapData == null)
                return Fail("MapData is null.");

            var sourcePlot = mapData.GetPlot(sourcePlotId);
            if (sourcePlot == null)
                return Fail($"Source plot '{sourcePlotId}' not found.");

            if (sourcePlot.faction != Faction.Player)
                return Fail($"Source '{sourcePlotId}' is not Player-owned (current: {sourcePlot.faction}).");

            if (sourcePlot.isMainBase)
                return Fail($"Source '{sourcePlotId}' is a main base.");

            var targetPlot = mapData.GetPlot(targetPlotId);
            if (targetPlot == null)
                return Fail($"Target plot '{targetPlotId}' not found.");

            if (targetPlot.faction != Faction.Neutral)
                return Fail($"Target '{targetPlotId}' is not Neutral (current: {targetPlot.faction}).");

            // Validate adjacency
            bool adjacent = false;
            foreach (var nId in mapData.GetNeighbors(sourcePlotId))
            {
                if (nId == targetPlotId) { adjacent = true; break; }
            }
            if (!adjacent)
                return Fail($"'{sourcePlotId}' is not adjacent to '{targetPlotId}'.", sourcePlotId, targetPlotId);

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
            int required = PlotCaptureRequirementService.GetRequiredSoldierCount(targetPlot);

            var dispatchResult = StrategicDispatchService.DispatchToPlot(
                sourcePos, waypoints, targetPlotId, mapData, mapRenderer, required);

            return new StrategicExpansionService.ExpansionResult
            {
                success = dispatchResult.dispatchedCount > 0,
                sourcePlotId = sourcePlotId,
                targetPlotId = targetPlotId,
                dispatchedCount = dispatchResult.dispatchedCount,
                requiredCount = required,
                message = dispatchResult.message
            };
        }

        private static StrategicExpansionService.ExpansionResult Fail(string msg, string sourceId = "", string targetId = "")
        {
            GameStatusService.LastActionResult = msg;
            return new StrategicExpansionService.ExpansionResult
            {
                success = false,
                message = msg,
                sourcePlotId = sourceId,
                targetPlotId = targetId
            };
        }
    }
}
