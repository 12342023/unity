using System.Collections.Generic;
using Combat;
using Core;
using UnityEngine;

namespace Buildings
{
    /// <summary>
    /// Runtime registry of all building GameObjects, grouped by faction.
    ///
    /// BuildingFactory.Register() is called after each building is created,
    /// so the registry stays in sync without manual tracking in GameEntry.
    /// Queries filter out null / destroyed entries automatically.
    /// </summary>
    public static class BuildingRegistry
    {
        private static readonly Dictionary<Faction, List<GameObject>> buildings = new()
        {
            { Faction.Player, new List<GameObject>() },
            { Faction.Enemy,  new List<GameObject>() },
            { Faction.Neutral, new List<GameObject>() },
        };

        /// <summary>Register a newly created building.</summary>
        public static void Register(GameObject building, Faction faction)
        {
            if (building == null) return;
            if (!buildings.ContainsKey(faction))
                buildings[faction] = new List<GameObject>();
            buildings[faction].Add(building);
        }

        /// <summary>
        /// Return all living buildings of the given faction.
        /// Filters out null (destroyed) and dead (Health <= 0) objects.
        /// </summary>
        public static IReadOnlyList<GameObject> GetBuildings(Faction faction)
        {
            if (!buildings.TryGetValue(faction, out var list))
                return System.Array.Empty<GameObject>();

            // Compact on read: remove null and dead entries
            list.RemoveAll(b => b == null || IsDead(b));
            return list;
        }

        private static bool IsDead(GameObject b)
        {
            var health = b.GetComponent<HealthComponent>();
            return health == null || health.IsDead;
        }
    }
}
