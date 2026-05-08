namespace Map
{
    [System.Serializable]
    public class RoadConnection
    {
        /// <summary>Plot ID of one endpoint.</summary>
        public string fromPlotId;

        /// <summary>Plot ID of the other endpoint.</summary>
        public string toPlotId;

        public RoadConnection(string from, string to)
        {
            // Normalise order so (a, b) == (b, a) for dedup
            if (string.CompareOrdinal(from, to) <= 0)
            {
                fromPlotId = from;
                toPlotId = to;
            }
            else
            {
                fromPlotId = to;
                toPlotId = from;
            }
        }

        public bool Connects(string id1, string id2)
        {
            return (fromPlotId == id1 && toPlotId == id2)
                || (fromPlotId == id2 && toPlotId == id1);
        }
    }
}
