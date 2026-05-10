namespace Combat
{
    /// <summary>
    /// Lightweight shared state for the temporary HUD.
    /// Updated by dispatch/capture services, read by GameHud.
    /// </summary>
    public static class GameStatusService
    {
        public static string LastActionResult { get; set; } = "";

        public static void Reset()
        {
            LastActionResult = "";
        }
    }
}
