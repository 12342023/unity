using UnityEngine;

namespace Combat
{
    public enum MatchResult
    {
        None,
        PlayerVictory,
        PlayerDefeat
    }

    public static class MatchResultService
    {
        public static MatchResult CurrentResult { get; private set; } = MatchResult.None;
        private static bool isFinalized = false;

        public static void Reset()
        {
            CurrentResult = MatchResult.None;
            isFinalized = false;
        }

        /// <summary>Declare Player victory. Returns false if already finalized.</summary>
        public static bool TryDeclareVictory()
        {
            if (isFinalized) return false;
            isFinalized = true;
            CurrentResult = MatchResult.PlayerVictory;
            Debug.Log("[MatchResult] PLAYER VICTORY! Enemy base destroyed.");
            return true;
        }

        /// <summary>Declare Player defeat. Returns false if already finalized.</summary>
        public static bool TryDeclareDefeat()
        {
            if (isFinalized) return false;
            isFinalized = true;
            CurrentResult = MatchResult.PlayerDefeat;
            Debug.Log("[MatchResult] DEFEAT! Player base destroyed.");
            return true;
        }
    }
}
