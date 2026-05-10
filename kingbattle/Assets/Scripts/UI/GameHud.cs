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

        if (result == MatchResult.None)
            DrawNormalHud();
        else
            DrawEndPanel(result);
    }

    // ── Normal HUD ─────────────────────────────────────────────────────

    private void DrawNormalHud()
    {
        // ── Background box (taller to fit candidate list + stats) ──
        GUI.Box(new Rect(10, 10, 390, 390), "Game Status (debug)");

        GUILayout.BeginArea(new Rect(15, 28, 375, 370));

        GUILayout.Label("Objective: Capture neutral plots → Defeat Enemy base");
        GUILayout.Label($"Expansion Candidates: {cachedPreviews.Count}");

        string lastAction = GameStatusService.LastActionResult;
        if (!string.IsNullOrEmpty(lastAction))
            GUILayout.Label($"Last Action: {lastAction}");

        // ── Enemy attack status ──
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

        // ── Faction stats (MVP-04.2) ──
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

        // ── Selection info (MVP-04.5) ──
        var inputCtrl = Object.FindAnyObjectByType<PlayerInputController>(FindObjectsInactive.Exclude);
        if (inputCtrl != null && inputCtrl.HasSelection)
        {
            GUILayout.Space(2);
            GUILayout.Label($"Selected: {inputCtrl.SelectedSourcePlotId}  |  Targets: {inputCtrl.ValidTargets.Count} highlighted");
        }

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

    // ── End panel (MVP-04.3) ──────────────────────────────────────────

    private void DrawEndPanel(MatchResult result)
    {
        int px = 60;
        int py = Screen.height / 2 - 130;
        int pw = Screen.width - 120;
        int ph = 260;

        GUI.Box(new Rect(px, py, pw, ph), "GAME OVER");

        GUILayout.BeginArea(new Rect(px + 15, py + 25, pw - 30, ph - 40));

        // Result title
        string resultText = result == MatchResult.PlayerVictory
            ? "PLAYER VICTORY!"
            : "DEFEAT!";
        GUILayout.Label(resultText, GUILayout.Height(30));

        GUILayout.Space(10);

        // Final stats
        int pUnits = FactionStatsService.CountAliveUnits(Core.Faction.Player);
        int pCap = FactionStatsService.GetSupplyCap(Core.Faction.Player);
        int pGran = FactionStatsService.CountAliveGranaries(Core.Faction.Player);
        int pTow = FactionStatsService.CountAliveTowers(Core.Faction.Player);

        int eUnits = FactionStatsService.CountAliveUnits(Core.Faction.Enemy);
        int eCap = FactionStatsService.GetSupplyCap(Core.Faction.Enemy);
        int eGran = FactionStatsService.CountAliveGranaries(Core.Faction.Enemy);
        int eTow = FactionStatsService.CountAliveTowers(Core.Faction.Enemy);

        GUILayout.Label("Final Stats:");
        GUILayout.Label($"  Player Units: {pUnits}/{pCap}");
        GUILayout.Label($"  Enemy  Units: {eUnits}/{eCap}");
        GUILayout.Label($"  Player Bldgs: G:{pGran} T:{pTow}  |  Enemy Bldgs: G:{eGran} T:{eTow}");

        GUILayout.Space(10);

        // Restart button
        if (GUILayout.Button("Restart (N)"))
        {
            HandleRestart();
        }

        GUILayout.EndArea();
    }

    // ── Actions ───────────────────────────────────────────────────────

    private void HandleDispatchClick(string sourcePlotId, string targetPlotId)
    {
        if (mapData == null || mapRenderer == null) return;

        var result = StrategicExpansionCommandService.DispatchCandidate(mapData, mapRenderer, sourcePlotId, targetPlotId);
        GameStatusService.LastActionResult = result.message;
        Debug.Log($"[GameHud] Dispatch button: {result.message}");
        RefreshPreviews();

        if (result.dispatchedCount == 0)
            Debug.Log($"[GameHud] Dispatch to {targetPlotId} failed: {result.message}");
    }

    private void HandleRestart()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.Log("[GameHud] Restart: no active scene name, cannot reload. Add the scene to Build Settings.");
        }
        else
        {
            Debug.Log($"[GameHud] Restarting scene: {sceneName}");
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}
