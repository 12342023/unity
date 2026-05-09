using System.Collections.Generic;
using Core;
using Map;

namespace Combat
{
    /// <summary>
    /// Queries the map for Player-owned frontier plots that have adjacent
    /// Neutral plots, enabling the next layer of expansion after capture.
    ///
    /// Pure data layer — no dispatch, no capture, no UI.
    /// </summary>
    public static class StrategicConnectionService
    {
        /// <summary>
        /// Find all Player-owned non-main-base plots that have at least one
        /// Neutral neighbour. Each result includes the plotId and its list
        /// of connectable Neutral neighbour plotIds.
        /// </summary>
        public static List<FrontierInfo> GetPlayerFrontierPlots(MapData mapData)
        {
            var results = new List<FrontierInfo>();

            if (mapData == null) return results;

            foreach (var plot in mapData.Plots)
            {
                // Must be Player-owned and not a main base
                if (plot.faction != Faction.Player) continue;
                if (plot.isMainBase) continue;

                var neutralNeighbours = new List<string>();
                foreach (var nId in mapData.GetNeighbors(plot.plotId))
                {
                    var nPlot = mapData.GetPlot(nId);
                    if (nPlot != null && nPlot.faction == Faction.Neutral)
                        neutralNeighbours.Add(nId);
                }

                if (neutralNeighbours.Count > 0)
                {
                    results.Add(new FrontierInfo
                    {
                        plotId = plot.plotId,
                        connectableNeutralPlots = neutralNeighbours
                    });
                }
            }

            return results;
        }

        public class FrontierInfo
        {
            public string plotId;
            public List<string> connectableNeutralPlots;
        }
    }
}
