using Core;
using UnityEngine;

namespace Units
{
    /// <summary>
    /// Static configuration for unit movement speed.
    /// Samurai = slowest, ElfArcher = medium, Soldier = fastest.
    /// </summary>
    public static class UnitConfig
    {
        /// <summary>Movement speed in world-units per second.</summary>
        public static float GetSpeed(UnitType type) => type switch
        {
            UnitType.Samurai   => 1.5f,
            UnitType.ElfArcher => 2.5f,
            UnitType.Soldier   => 3.5f,
            _                  => 2.0f
        };

        /// <summary>Display colour used for the unit sprite.</summary>
        public static Color GetColor(UnitType type) => type switch
        {
            UnitType.Samurai   => new Color(0.9f, 0.3f, 0.3f),  // red
            UnitType.ElfArcher => new Color(0.3f, 0.8f, 0.3f),  // green
            UnitType.Soldier   => new Color(0.3f, 0.5f, 0.9f),  // blue
            _                  => Color.white
        };

        /// <summary>Display name for debug / labels.</summary>
        public static string GetDisplayName(UnitType type) => type switch
        {
            UnitType.Samurai   => "Samurai",
            UnitType.ElfArcher => "Elf Archer",
            UnitType.Soldier   => "Soldier",
            _                  => "Unknown"
        };
    }
}
