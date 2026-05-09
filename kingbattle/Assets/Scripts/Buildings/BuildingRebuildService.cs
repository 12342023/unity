using Core;
using Map;
using UnityEngine;

namespace Buildings
{
    /// <summary>
    /// Rebuilds a building from a ruin.
    ///
    /// Usage:
    ///   BuildingRebuildService.Rebuild(ruinComponent, mapData, faction);
    ///
    /// The service reads the ruin's metadata (sourcePlotId, sourceBuildingType),
    /// calls BuildingFactory.CreateBuilding to spawn the new building,
    /// then destroys the old ruin GameObject.
    ///
    /// The new building is automatically registered in BuildingRegistry
    /// (via BuildingFactory), so FactionDefeatHandler and other systems
    /// see it immediately.
    /// </summary>
    public static class BuildingRebuildService
    {
        /// <summary>
        /// Rebuild the building that was originally at this ruin's location.
        /// Returns the new building GameObject, or null if the ruin isn't rebuildable.
        /// </summary>
        public static GameObject Rebuild(RuinComponent ruin, MapData mapData, Faction faction)
        {
            if (ruin == null)
            {
                Debug.LogWarning("[BuildingRebuildService] Ruin is null.");
                return null;
            }

            if (mapData == null)
            {
                Debug.LogWarning("[BuildingRebuildService] MapData is null, cannot rebuild.");
                return null;
            }

            if (!ruin.CanRebuildFor(faction))
            {
                Debug.LogWarning("[BuildingRebuildService] Ruin cannot be rebuilt (no sourcePlotId).");
                return null;
            }

            // Main base ruins cannot be rebuilt via the normal service
            if (ruin.IsMainBaseRuin(mapData))
            {
                Debug.LogWarning($"[BuildingRebuildService] Main base ruin at {ruin.sourcePlotId} cannot be rebuilt.");
                return null;
            }

            var plotId = ruin.sourcePlotId;
            var buildingType = ruin.GetRebuildBuildingType();

            // Create the new building via the factory (auto-registers in BuildingRegistry)
            var newBuilding = BuildingFactory.CreateBuilding(plotId, mapData, faction, buildingType);
            if (newBuilding == null)
            {
                Debug.LogError($"[BuildingRebuildService] Failed to create building at plot {plotId}.");
                return null;
            }

            // Destroy the old ruin
            GameObject.Destroy(ruin.gameObject);

            Debug.Log($"[BuildingRebuildService] Rebuilt {buildingType} at {plotId} for {faction}.");
            return newBuilding;
        }
    }
}
