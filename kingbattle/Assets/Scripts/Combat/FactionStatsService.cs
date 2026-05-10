using Buildings;
using Core;
using Units;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Lightweight stateless query service for faction-wide counts and rules.
    ///
    /// Roles:
    ///   Barracks = spawns soldiers (limited by supply cap).
    ///   Tower    = auto-attacks enemy units.
    ///   Granary  = increases supply cap.
    ///
    /// Rules:
    ///   base supply cap = 8.
    ///   each alive Granary owned by that faction adds +4.
    ///
    /// No dependency on HUD, keyboard, mouse, OnGUI, or platform APIs.
    /// Intended to be called occasionally (HUD refresh ~2s, Barracks spawn ~5s).
    /// </summary>
    public static class FactionStatsService
    {
        public const int BaseSupplyCap = 8;
        public const int GranarySupplyBonus = 4;

        /// <summary>Count non-dead units of the given faction.</summary>
        public static int CountAliveUnits(Faction faction)
        {
            int count = 0;
            foreach (var u in GameObject.FindObjectsByType<UnitCombat>(FindObjectsSortMode.None))
            {
                if (u.faction != faction) continue;
                if (u.GetComponent<HealthComponent>().IsDead) continue;
                count++;
            }
            return count;
        }

        /// <summary>Count alive Granary buildings owned by the given faction.</summary>
        public static int CountAliveGranaries(Faction faction)
        {
            return CountAliveBuildingsOfType(faction, BuildingType.Granary);
        }

        /// <summary>Count alive Tower buildings owned by the given faction.</summary>
        public static int CountAliveTowers(Faction faction)
        {
            return CountAliveBuildingsOfType(faction, BuildingType.Tower);
        }

        /// <summary>Supply cap for the given faction = base + Granary bonus.</summary>
        public static int GetSupplyCap(Faction faction)
        {
            return BaseSupplyCap + CountAliveGranaries(faction) * GranarySupplyBonus;
        }

        /// <summary>True if the faction can spawn a new unit (current < cap).</summary>
        public static bool CanSpawn(Faction faction)
        {
            return CountAliveUnits(faction) < GetSupplyCap(faction);
        }

        private static int CountAliveBuildingsOfType(Faction faction, BuildingType type)
        {
            int count = 0;
            var buildings = BuildingRegistry.GetBuildings(faction);
            foreach (var b in buildings)
            {
                if (b == null) continue;
                var health = b.GetComponent<HealthComponent>();
                if (health == null || health.IsDead) continue;
                var identity = b.GetComponent<BuildingIdentity>();
                if (identity != null && identity.buildingType == type)
                    count++;
            }
            return count;
        }
    }
}
