using Core;
using UnityEngine;

namespace Buildings
{
    /// <summary>
    /// Stores the identity of a building: which plot it sits on, what type it is,
    /// and which faction it belongs to.
    ///
    /// Used by BuildingDeathHandler to pass this data to the RuinComponent,
    /// enabling future reconstruction to know what was originally here.
    /// </summary>
    public class BuildingIdentity : MonoBehaviour
    {
        public string plotId;
        public BuildingType buildingType;
        public Faction faction;
    }
}
