using Map;

namespace Combat
{
    /// <summary>
    /// Orchestrates expansion from a Player-owned frontier plot to an adjacent
    /// Neutral plot. Now delegates the actual dispatch to
    /// StrategicExpansionCommandService to keep a single "validate + path +
    /// dispatch" code path.
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
        /// Take the first ExpansionCandidate and dispatch.
        /// Delegates to StrategicExpansionCommandService.
        /// </summary>
        public static ExpansionResult ExpandNext(MapData mapData, MapRenderer mapRenderer)
        {
            if (mapData == null)
            {
                GameStatusService.LastActionResult = "MapData is null.";
                return new ExpansionResult { success = false, message = "MapData is null." };
            }

            var candidates = StrategicConnectionService.GetExpansionCandidates(mapData);
            if (candidates.Count == 0)
            {
                GameStatusService.LastActionResult = "No expansion candidates.";
                return new ExpansionResult { success = false, message = "No expansion candidates." };
            }

            var c = candidates[0];
            return StrategicExpansionCommandService.DispatchCandidate(mapData, mapRenderer, c.sourcePlotId, c.targetPlotId);
        }
    }
}
