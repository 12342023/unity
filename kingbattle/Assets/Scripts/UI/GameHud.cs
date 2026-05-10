using System.Collections.Generic;
using Combat;
using Map;
using UnityEngine;

/// <summary>
/// Temporary in-game HUD displaying match state, objective,
/// expansion candidate list (with Dispatch buttons), and recent results.
/// OnGUI-based — deliberately simple, no formal UI system.
///
/// Porting boundary: HUD only calls command service and reads shared state.
/// It does NOT directly modify map/building/unit data.
///
/// This is a temporary component. Replace with proper UI when
/// moving out of MVP phase.
/// </summary>
public class GameHud : MonoBehaviour
{
    private MapData mapData;
    private MapRenderer mapRenderer;
    private float refreshTimer;
    private List<StrategicConnectionService.ExpansionPreview> cachedPreviews = new();

    public void Initialize(MapData data, MapRenderer renderer)
    {
        mapData = data;
        mapRenderer = renderer;
        RefreshPreviews();
    }

    private void RefreshPreviews()
    {
        if (mapData == null) return;
        cachedPreviews = StrategicConnectionService.GetExpansionPreviews(mapData);
    }

    private void Update()
    {
        refreshTimer += Time.deltaTime;
        if (refreshTimer >= 2f)
        {
            refreshTimer = 0f;
            RefreshPreviews();
        }
    }

    private void OnGUI()
    {
        var result = MatchResultService.CurrentResult;

        // ── Background box (taller to fit candidate list + stats) ──
        GUI.Box(new Rect(10, 10, 390, 390), "Game Status (debug)");

        GUILayout.BeginArea(new Rect(15, 28, 375, 370));

        // ── Status section ──
        string objective = result == MatchResult.PlayerVictory ? "Victory!"
            : result == MatchResult.PlayerDefeat ? "Defeated."
            : "Capture neutral plots → Defeat Enemy base";
        GUILayout.Label($"Objective: {objective}");
        GUILayout.Label($"Expansion Candidates: {cachedPreviews.Count}");

        string lastAction = GameStatusService.LastActionResult;
        if (!string.IsNullOrEmpty(lastAction))
            GUILayout.Label($"Last Action: {lastAction}");

        // ── Enemy attack status ──
        if (result == MatchResult.None)
        {
            float timeLeft = GameStatusService.TimeUntilNextEnemyAttack;
            if (timeLeft > 0f)
                GUILayout.Label($"Enemy Attack: ~{timeLeft:F0}s");
            else
                GUILayout.Label("Enemy Attack: imminent");

            string enemyLast = GameStatusService.LastEnemyActionResult;
            if (!string.IsNullOrEmpty(enemyLast))
            {
                string shortMsg = enemyLast.Length > 50
                    ? enemyLast[..50] + "…"
                    : enemyLast;
                GUILayout.Label($"Enemy Last: {shortMsg}");
            }
        }

        // ── Faction stats (MVP-04.2) ──
        if (result == MatchResult.None)
        {
            int pUnits = FactionStatsService.CountAliveUnits(Core.Faction.Player);
            int pCap = FactionStatsService.GetSupplyCap(Core.Faction.Player);
            int pGran = FactionStatsService.CountAliveGranaries(Core.Faction.Player);
            int pTow = FactionStatsService.CountAliveTowers(Core.Faction.Player);

            int eUnits = FactionStatsService.CountAliveUnits(Core.Faction.Enemy);
            int eCap = FactionStatsService.GetSupplyCap(Core.Faction.Enemy);
            int eGran = FactionStatsService.CountAliveGranaries(Core.Faction.Enemy);
            int eTow = FactionStatsService.CountAliveTowers(Core.Faction.Enemy);

            GUILayout.Space(2);
            GUILayout.Label($"Units: Player {pUnits}/{pCap} | Enemy {eUnits}/{eCap}");
            GUILayout.Label($"Bldgs: Player G:{pGran} T:{pTow} | Enemy G:{eGran} T:{eTow}");
        }

        if (result != MatchResult.None)
            GUILayout.Label($"Result: {result}");

        // ── Candidate list ──
        GUILayout.Space(4);
        GUILayout.Label("── Expansion Candidates ──");

        if (cachedPreviews.Count == 0)
        {
            GUILayout.Label("  (none)");
        }
        else
        {
            int maxToShow = Mathf.Min(cachedPreviews.Count, 4);
            for (int i = 0; i < maxToShow; i++)
            {
                var p = cachedPreviews[i];

                GUILayout.BeginHorizontal();

                string label = $"[{i + 1}] {p.sourcePlotId} → {p.targetPlotId}: {p.availableCount}/{p.requiredCount}";
                string status = p.hasEnough ? "enough" : "short";

                GUILayout.Label(label, GUILayout.Width(240));
                GUILayout.Label(status, GUILayout.Width(45));

                // Capture by value for closure
                string src = p.sourcePlotId;
                string tgt = p.targetPlotId;

                if (GUILayout.Button("Dsp", GUILayout.Width(40)))
                {
                    HandleDispatchClick(src, tgt);
                }

                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndArea();
    }

    private void HandleDispatchClick(string sourcePlotId, string targetPlotId)
    {
        if (mapData == null || mapRenderer == null) return;

        var result = StrategicExpansionCommandService.DispatchCandidate(mapData, mapRenderer, sourcePlotId, targetPlotId);
        GameStatusService.LastActionResult = result.message;
        Debug.Log($"[GameHud] Dispatch button: {result.message}");
        RefreshPreviews();

        // If no soldiers were dispatched, refresh again after a short delay
        // to give the game state time to settle
        if (result.dispatchedCount == 0)
            Debug.Log($"[GameHud] Dispatch to {targetPlotId} failed: {result.message}");
    }
}
