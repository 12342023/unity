using System.Collections.Generic;
using Core;
using Map;
using Units;
using UnityEngine;

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

        // ── Expansion candidates ────────────────────────────────────

        /// <summary>A single expansion move: from a Player-owned plot
        /// to an adjacent Neutral target.</summary>
        public class ExpansionCandidate
        {
            public string sourcePlotId;
            public string targetPlotId;
        }

        /// <summary>Returns all <see cref="ExpansionCandidate"/>s:
        /// every Player-owned (non-main-base) plot paired with each
        /// adjacent Neutral neighbour.</summary>
        public static List<ExpansionCandidate> GetExpansionCandidates(MapData mapData)
        {
            var candidates = new List<ExpansionCandidate>();
            var frontiers = GetPlayerFrontierPlots(mapData);
            foreach (var f in frontiers)
            {
                foreach (var target in f.connectableNeutralPlots)
                {
                    candidates.Add(new ExpansionCandidate
                    {
                        sourcePlotId = f.plotId,
                        targetPlotId = target
                    });
                }
            }
            return candidates;
        }

        // ── Expansion previews ──────────────────────────────────────

        /// <summary>A live preview of an expansion move, including
        /// the current available soldier count near the source plot.</summary>
        public class ExpansionPreview
        {
            public string sourcePlotId;
            public string targetPlotId;
            public int requiredCount;
            public int availableCount;
            public bool hasEnough => availableCount >= requiredCount;
        }

        /// <summary>Count non-dead Player soldiers within
        /// <paramref name="gatherRadius"/> of the given world position.</summary>
        private static int CountSoldiersNear(Vector3 worldPos, float gatherRadius)
        {
            int count = 0;
            foreach (var u in Object.FindObjectsByType<UnitCombat>(FindObjectsInactive.Exclude))
            {
                if (u.faction != Faction.Player) continue;
                if (u.GetComponent<HealthComponent>().IsDead) continue;
                if (Vector3.Distance(u.transform.position, worldPos) <= gatherRadius)
                    count++;
            }
            return count;
        }

        /// <summary>Returns all expansion previews — each candidate paired
        /// with its current available soldier count and requirement.</summary>
        public static List<ExpansionPreview> GetExpansionPreviews(MapData mapData, float gatherRadius = 5f)
        {
            var previews = new List<ExpansionPreview>();
            var candidates = GetExpansionCandidates(mapData);
            foreach (var c in candidates)
            {
                var sourcePlot = mapData.GetPlot(c.sourcePlotId);
                var targetPlot = mapData.GetPlot(c.targetPlotId);
                if (sourcePlot == null || targetPlot == null) continue;

                Vector3 sourcePos = new Vector3(sourcePlot.worldPosition.x, sourcePlot.worldPosition.y, -0.2f);
                int available = CountSoldiersNear(sourcePos, gatherRadius);
                int required = PlotCaptureRequirementService.GetRequiredSoldierCount(targetPlot);

                previews.Add(new ExpansionPreview
                {
                    sourcePlotId = c.sourcePlotId,
                    targetPlotId = c.targetPlotId,
                    requiredCount = required,
                    availableCount = available
                });
            }
            return previews;
        }
    }
}
