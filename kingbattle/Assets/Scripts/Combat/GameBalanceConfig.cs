namespace Combat
{
    /// <summary>
    /// Centralised gameplay balance values. Makes key magic numbers
    /// discoverable and changeable without hunting through source files.
    ///
    /// Only the most obvious values are collected here — individual
    /// component defaults (BarracksSpawner, TowerAttack, UnitConfig, etc.)
    /// remain on those types where they are inspector-public.
    ///
    /// See BALANCE.md for full tuning documentation.
    /// </summary>
    public static class GameBalanceConfig
    {
        // Supply cap
        /// <summary>Base supply cap for every faction.</summary>
        public const int BaseSupplyCap = 8;

        /// <summary>Additional supply cap per alive Granary.</summary>
        public const int GranarySupplyBonus = 4;

        // Enemy pressure timing (seconds)
        /// <summary>First enemy attack delay (min).</summary>
        public const float EnemyFirstAttackMin = 8f;
        /// <summary>First enemy attack delay (max).</summary>
        public const float EnemyFirstAttackMax = 12f;
        /// <summary>Subsequent enemy attack interval (min).</summary>
        public const float EnemyRepeatAttackMin = 20f;
        /// <summary>Subsequent enemy attack interval (max).</summary>
        public const float EnemyRepeatAttackMax = 30f;

        // Tower combat
        /// <summary>Damage per Tower attack.</summary>
        public const float TowerDamage = 15f;
        /// <summary>Tower attack range (world units).</summary>
        public const float TowerAttackRange = 3.5f;
        /// <summary>Seconds between Tower attacks.</summary>
        public const float TowerAttackInterval = 1.5f;
    }
}
