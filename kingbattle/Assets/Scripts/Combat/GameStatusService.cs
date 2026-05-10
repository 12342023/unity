namespace Combat
{
    /// <summary>
    /// Lightweight shared state for the temporary HUD.
    /// Updated by dispatch/capture/enemy services, read by GameHud.
    /// </summary>
    public static class GameStatusService
    {
        public static string LastActionResult { get; set; } = "";
        public static string LastEnemyActionResult { get; set; } = "";
        public static float TimeUntilNextEnemyAttack { get; set; } = 0f;

        public static void Reset()
        {
            LastActionResult = "";
            LastEnemyActionResult = "";
            TimeUntilNextEnemyAttack = 0f;
        }
    }
}
