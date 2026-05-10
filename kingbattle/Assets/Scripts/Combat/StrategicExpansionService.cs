using System.Collections.Generic;
using Map;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Orchestrates expansion from a Player-owned frontier plot to an adjacent
    /// Neutral plot: queries the frontier, calculates the road path, and
    /// dispatches nearby soldiers.
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
        }

        /// <summary>
        /// Find the first Player-owned frontier plot with a Neutral neighbour,
        /// calculate the road path, and dispatch nearby soldiers.
        /// Returns a human-readable result message.
        /// </summary>
        public static ExpansionResult ExpandNext(MapData mapData, MapRenderer mapRenderer)
        {
            if (mapData == null)
                return new ExpansionResult { success = false, message = "MapData is null." };

            var frontiers = StrategicConnectionService.GetPlayerFrontierPlots(mapData);
            if (frontiers.Count == 0)
                return new ExpansionResult { success = false, message = "No Player-owned frontier plot with neutral neighbours." };

            var source = frontiers[0];
            string targetPlotId = source.connectableNeutralPlots[0];

            var pathIds = RoadPathFinder.FindPath(mapData, source.plotId, targetPlotId);
            if (pathIds == null || pathIds.Count < 2)
                return new ExpansionResult { success = false, message = $"No road path from {source.plotId} to {targetPlotId}." };

            var waypoints = new List<Vector3>();
            foreach (var id in pathIds)
            {
                var p = mapData.GetPlot(id);
                if (p != null)
                    waypoints.Add(new Vector3(p.worldPosition.x, p.worldPosition.y, -0.2f));
            }

            Vector3 sourcePos = mapData.GetPlot(source.plotId).worldPosition;
            sourcePos.z = -0.2f;

            int count = StrategicDispatchService.DispatchToPlot(
                sourcePos, waypoints, targetPlotId, mapData, mapRenderer);

            return new ExpansionResult
            {
                success = count > 0,
                message = count > 0
                    ? $"Dispatched {count} soldiers from {source.plotId} to {targetPlotId}."
                    : $"No soldiers near {source.plotId} to dispatch."
            };
        }
    }
}
