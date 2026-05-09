using System.Collections.Generic;
using Combat;
using Core;
using UnityEngine;

namespace Buildings
{
    /// <summary>
    /// Handles faction-wide cleanup when a main base is destroyed.
    ///
    /// When OnMainBaseDefeated() is called:
    ///   1. All surviving buildings of this faction become ruins.
    ///   2. All surviving units of this faction are killed.
    ///
    /// Ruins are not double-generated — buildings that already died are
    /// filtered out via IsDead / null checks.
    /// </summary>
    public class FactionDefeatHandler : MonoBehaviour
    {
        private Faction faction;
        private List<GameObject> factionBuildings;

        public void Initialize(Faction fact, List<GameObject> buildings)
        {
            faction = fact;
            factionBuildings = buildings;
        }

        /// <summary>Called when this faction's main base is destroyed.</summary>
        public void OnMainBaseDefeated()
        {
            Debug.Log($"[FactionDefeatHandler] {faction} base destroyed! Cleaning up...");

            // ── Kill all surviving buildings (their OnDeath → SpawnRuin → Destroy) ──
            foreach (var b in factionBuildings)
            {
                if (b == null) continue; // already destroyed
                var health = b.GetComponent<HealthComponent>();
                if (health != null && !health.IsDead)
                {
                    health.Kill(); // triggers OnDeath → SpawnRuin
                }
            }

            // ── Kill all surviving units of this faction ──
            var allHealth = FindObjectsByType<HealthComponent>(FindObjectsSortMode.None);
            foreach (var h in allHealth)
            {
                if (h.faction == faction && !h.IsDead && h.GetComponent<UnitCombat>() != null)
                {
                    h.Kill();
                }
            }

            Debug.Log($"[FactionDefeatHandler] {faction} cleanup complete.");
        }
    }
}
