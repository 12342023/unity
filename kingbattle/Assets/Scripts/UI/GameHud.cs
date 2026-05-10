using Combat;
using Map;
using UnityEngine;

/// <summary>
/// Temporary in-game HUD displaying match state, objective,
/// expansion candidate count, and recent action results.
/// OnGUI-based — deliberately simple, no formal UI system.
///
/// This is a temporary component. Replace with proper UI when
/// moving out of MVP phase.
/// </summary>
public class GameHud : MonoBehaviour
{
    private MapData mapData;
    private float candidateRefreshTimer;
    private int cachedCandidateCount;

    public void Initialize(MapData data)
    {
        mapData = data;
        cachedCandidateCount = CountExpansionCandidates();
    }

    private int CountExpansionCandidates()
    {
        if (mapData == null) return 0;
        return StrategicConnectionService.GetExpansionCandidates(mapData).Count;
    }

    private void Update()
    {
        // Refresh candidate count every 2 seconds (not every frame)
        candidateRefreshTimer += Time.deltaTime;
        if (candidateRefreshTimer >= 2f)
        {
            candidateRefreshTimer = 0f;
            cachedCandidateCount = CountExpansionCandidates();
        }
    }

    private void OnGUI()
    {
        var result = MatchResultService.CurrentResult;

        // ── Background box ──
        GUI.Box(new Rect(10, 10, 360, 135), "Game Status");

        GUILayout.BeginArea(new Rect(15, 30, 350, 115));

        // Objective
        string objective = result == MatchResult.PlayerVictory ? "Victory!"
            : result == MatchResult.PlayerDefeat ? "Defeated."
            : "Capture neutral plots → Defeat Enemy base";
        GUILayout.Label($"Objective: {objective}");

        // Expansion candidates
        GUILayout.Label($"Expansion Candidates: {cachedCandidateCount}");

        // Last action result
        string lastAction = GameStatusService.LastActionResult;
        if (!string.IsNullOrEmpty(lastAction))
            GUILayout.Label($"Last Action: {lastAction}");

        // Match result
        if (result != MatchResult.None)
            GUILayout.Label($"Result: {result}");

        GUILayout.EndArea();
    }
}
