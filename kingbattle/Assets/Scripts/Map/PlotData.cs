using Core;
using UnityEngine;

namespace Map
{
    [System.Serializable]
    public class PlotData
    {
        /// <summary>Unique identifier for this plot (e.g. "PlayerBase").</summary>
        public string plotId;

        /// <summary>Display name shown in debug / tooltip.</summary>
        public string displayName;

        /// <summary>World position of the plot center.</summary>
        public Vector3 worldPosition;

        /// <summary>Small / Medium / Large – affects build slot count and capture difficulty.</summary>
        public PlotSize size;

        /// <summary>Current owning faction.</summary>
        public Faction faction;

        /// <summary>True if this is a main base for its faction.</summary>
        public bool isMainBase;

        /// <summary>
        /// Number of building slots this plot supports.
        /// For MVP-01 this is a data marker only – no building system yet.
        /// </summary>
        public int buildSlotCount;

        public PlotData(string id, string name, Vector3 position, PlotSize size, Faction faction, bool isMainBase)
        {
            this.plotId = id;
            this.displayName = name;
            this.worldPosition = position;
            this.size = size;
            this.faction = faction;
            this.isMainBase = isMainBase;

            // Slot count derived from plot size
            this.buildSlotCount = size switch
            {
                PlotSize.Small => 1,
                PlotSize.Medium => 2,
                PlotSize.Large => 3,
                _ => 1
            };
        }
    }
}
