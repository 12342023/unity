using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Map
{
    /// <summary>
    /// Holds the fixed map definition: plots + road connections.
    /// For MVP-01 the map is hard-coded via CreateFixedMap().
    /// </summary>
    public class MapData
    {
        public IReadOnlyList<PlotData> Plots => plots;
        public IReadOnlyList<RoadConnection> Roads => roads;

        private readonly List<PlotData> plots;
        private readonly List<RoadConnection> roads;

        // Adjacency list used by RoadPathFinder
        private readonly Dictionary<string, List<string>> adjacency;

        // Quick lookup: plotId → PlotData
        private readonly Dictionary<string, PlotData> plotLookup;

        private MapData(List<PlotData> plots, List<RoadConnection> roads)
        {
            this.plots = plots;
            this.roads = roads;
            this.adjacency = new Dictionary<string, List<string>>();
            this.plotLookup = new Dictionary<string, PlotData>();

            foreach (var p in plots)
                plotLookup[p.plotId] = p;

            BuildAdjacency();
        }

        // ── Build the fixed MVP-01 map ───────────────────────────────────

        /// <summary>
        /// Creates the fixed map with 6 plots and 7 bidirectional roads.
        /// Layout:
        ///   PlayerBase ── Village ── Crossroads ── EnemyOutpost ── EnemyBase
        ///       │                      │                            │
        ///       └── Farmland ──────────┘────────────────────────────┘
        /// </summary>
        public static MapData CreateFixedMap()
        {
            var plots = new List<PlotData>
            {
                new("PlayerBase",   "Player Base",   new Vector3(-4.0f,  1.5f),  PlotSize.Medium, Faction.Player, isMainBase: true),
                new("Village",      "Village",       new Vector3(-1.0f,  2.5f),  PlotSize.Small,  Faction.Neutral, isMainBase: false),
                new("Farmland",     "Farmland",      new Vector3(-2.5f, -1.5f),  PlotSize.Small,  Faction.Neutral, isMainBase: false),
                new("Crossroads",   "Crossroads",    new Vector3( 1.5f,  0.5f),  PlotSize.Medium, Faction.Neutral, isMainBase: false),
                new("EnemyOutpost", "Enemy Outpost", new Vector3( 4.0f,  2.5f),  PlotSize.Small,  Faction.Enemy,   isMainBase: false),
                new("EnemyBase",    "Enemy Base",    new Vector3( 4.0f, -1.5f),  PlotSize.Large,  Faction.Enemy,   isMainBase: true),
            };

            var roads = new List<RoadConnection>
            {
                new("PlayerBase",   "Village"),
                new("PlayerBase",   "Farmland"),
                new("Village",      "Crossroads"),
                new("Farmland",     "Crossroads"),
                new("Crossroads",   "EnemyOutpost"),
                new("Crossroads",   "EnemyBase"),
                new("EnemyOutpost", "EnemyBase"),
            };

            var map = new MapData(plots, roads);
            return map;
        }

        // ── Queries ─────────────────────────────────────────────────────

        public PlotData GetPlot(string plotId)
        {
            return plotLookup.TryGetValue(plotId, out var plot) ? plot : null;
        }

        /// <summary>Returns neighbour plot IDs for a given plot.</summary>
        public IReadOnlyList<string> GetNeighbors(string plotId)
        {
            return adjacency.TryGetValue(plotId, out var list)
                ? list
                : System.Array.Empty<string>();
        }

        // ── Adjacency builder ───────────────────────────────────────────

        private void BuildAdjacency()
        {
            foreach (var p in plots)
                adjacency[p.plotId] = new List<string>();

            foreach (var r in roads)
            {
                adjacency[r.fromPlotId].Add(r.toPlotId);
                adjacency[r.toPlotId].Add(r.fromPlotId);
            }
        }
    }
}
