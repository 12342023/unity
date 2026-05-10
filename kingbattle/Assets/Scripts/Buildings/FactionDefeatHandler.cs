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
    /// Buildings are queried from BuildingRegistry instead of a manual list.
    /// </summary>
    public class FactionDefeatHandler : MonoBehaviour
    {
        private Faction faction;

        public void Initialize(Faction fact)
        {
            faction = fact;
        }

        /// <summary>Called when this faction's main base is destroyed.</summary>
        public void OnMainBaseDefeated()
        {
            Debug.Log($"[FactionDefeatHandler] {faction} base destroyed! Cleaning up...");

            // ── Kill all surviving buildings (their OnDeath → SpawnRuin → Destroy) ──
            var factionBuildings = BuildingRegistry.GetBuildings(faction);
            foreach (var b in factionBuildings)
            {
                if (b == null) continue;
                var health = b.GetComponent<HealthComponent>();
                if (health != null && !health.IsDead)
                {
                    health.Kill(); // triggers OnDeath → SpawnRuin
                }
            }

            // ── Kill all surviving units of this faction ──
            var allHealth = Object.FindObjectsByType<HealthComponent>(FindObjectsInactive.Exclude);
            foreach (var h in allHealth)
            {
                if (h.faction == faction && !h.IsDead && h.GetComponent<UnitCombat>() != null)
                {
                    h.Kill();
                }
            }

            Debug.Log($"[FactionDefeatHandler] {faction} cleanup complete.");

            // ── Declare game result ──
            if (faction == Faction.Enemy)
            {
                if (MatchResultService.TryDeclareVictory())
                    GameStatusService.LastActionResult = "Player Victory!";
            }
            else if (faction == Faction.Player)
            {
                if (MatchResultService.TryDeclareDefeat())
                    GameStatusService.LastActionResult = "Player Defeated!";
            }
        }
    }
}
