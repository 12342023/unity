using System.Collections.Generic;

namespace Map
{
    /// <summary>
    /// Simple BFS pathfinder on the road graph.
    /// Returns the shortest path (fewest road hops) between two plots.
    /// </summary>
    public static class RoadPathFinder
    {
        /// <summary>
        /// Returns a list of plot IDs from <paramref name="fromPlotId"/>
        /// to <paramref name="toPlotId"/> inclusive, or null if unreachable.
        /// </summary>
        public static List<string> FindPath(MapData mapData, string fromPlotId, string toPlotId)
        {
            if (fromPlotId == toPlotId)
                return new List<string> { fromPlotId };

            var visited = new HashSet<string> { fromPlotId };
            var queue = new Queue<string>();
            var parent = new Dictionary<string, string>();

            queue.Enqueue(fromPlotId);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current == toPlotId)
                    break;

                foreach (var neighbor in mapData.GetNeighbors(current))
                {
                    if (visited.Add(neighbor)) // returns true if newly added
                    {
                        parent[neighbor] = current;
                        queue.Enqueue(neighbor);
                    }
                }
            }

            // If we never reached the destination
            if (!parent.ContainsKey(toPlotId))
                return null;

            // Reconstruct path
            var path = new List<string>();
            var node = toPlotId;
            while (node != fromPlotId)
            {
                path.Add(node);
                node = parent[node];
            }
            path.Add(fromPlotId);
            path.Reverse();
            return path;
        }
    }
}
