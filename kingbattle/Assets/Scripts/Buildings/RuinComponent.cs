using Core;
using UnityEngine;

namespace Buildings
{
    /// <summary>
    /// Marks a GameObject as a building ruin.
    /// Ruins are purely visual — no attack, no spawn, no health.
    /// Soldiers can patrol around ruins after defeating a building.
    ///
    /// Stores the original building's identity metadata so that
    /// future reconstruction knows what was here.
    ///
    /// Provides minimal rebuild-predicate methods for the data layer.
    /// No UI, no actual rebuild — just "can this be rebuilt" logic.
    /// </summary>
    public class RuinComponent : MonoBehaviour
    {
        [Header("Appearance")]
        public Color ruinColor = new Color(0.25f, 0.25f, 0.25f);

        [Header("Source Identity (set on spawn)")]
        public string sourcePlotId;
        public BuildingType sourceBuildingType;
        public Faction originalFaction;

        private void Start()
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = ruinColor;
        }

        // ── Rebuild-predicate helpers ─────────────────────────────────

        /// <summary>Returns true if this ruin can theoretically be rebuilt.
        /// A valid sourcePlotId is the minimum requirement.</summary>
        public bool CanRebuildFor(Faction faction)
        {
            return !string.IsNullOrEmpty(sourcePlotId);
        }

        /// <summary>Returns the type of building that was originally here.</summary>
        public BuildingType GetRebuildBuildingType()
        {
            return sourceBuildingType;
        }
    }
}
