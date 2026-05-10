using System.Collections.Generic;
using Combat;
using Core;
using Map;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Canvas/uGUI-based HUD created at runtime (no scene prefab required).
///
/// Porting boundary: HUD only calls command service and reads shared state.
/// It does NOT directly modify map/building/unit data.
///
/// Created by GameEntry on start.
/// </summary>
public class GameHud : MonoBehaviour
{
    private MapData mapData;
    private MapRenderer mapRenderer;
    private PlayerInputController inputController; // cached, no per-frame Find
    private float refreshTimer;
    private List<StrategicConnectionService.ExpansionPreview> cachedPreviews = new();
    private Font uiFont;

    // ── UI references ──────────────────────────────────────────────────

    // Panels
    private GameObject hudPanelRoot;
    private GameObject endPanelRoot;

    // Status texts
    private Text objectiveText;
    private Text enemyTimerText;
    private Text actionText;
    private Text statsText;
    private Text selectionText;

    // Candidate list root (for clearing/recreating rows)
    private Transform candidateListParent;
    private readonly List<CandidateRowUI> candidateRows = new();

    // End-panel texts
    private Text endResultText;
    private Text endStatsText;

    // ── Initialisation ──────────────────────────────────────────────────

    public void Initialize(MapData data, MapRenderer renderer, PlayerInputController inputCtrl)
    {
        mapData = data;
        mapRenderer = renderer;
        inputController = inputCtrl;
        uiFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (uiFont == null) uiFont = Font.CreateDynamicFontFromOSFont("Arial", 14);

        CreateCanvas();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log("[GameHud] uGUI HUD initialized.");
#endif
    }

    private void CreateCanvas()
    {
        // ── Root Canvas ──
        var canvasGo = new GameObject("GameHudCanvas");
        canvasGo.transform.SetParent(transform, false);

        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

        canvasGo.AddComponent<GraphicRaycaster>();

        // ── HUD panel (left side, visible during gameplay) ──
        hudPanelRoot = CreatePanel(canvasGo, "HudPanel", new Vector2(0, 1), new Vector2(0, 1),
            new Vector2(8, -8), new Vector2(400, 480));
        var hudBg = hudPanelRoot.AddComponent<Image>();
        hudBg.color = new Color(0, 0, 0, 0.65f);
        hudBg.raycastTarget = false;

        var vlg = hudPanelRoot.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 3;
        vlg.padding = new RectOffset(8, 8, 8, 8);
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        // Status section
        objectiveText = CreateText(hudPanelRoot, "Objective", "Capture neutral plots → Defeat Enemy base", 14, TextAnchor.UpperLeft);

        var sep1 = CreateSeparator(hudPanelRoot);

        enemyTimerText = CreateText(hudPanelRoot, "EnemyTimer", "", 13, TextAnchor.UpperLeft);
        actionText = CreateText(hudPanelRoot, "Action", "", 13, TextAnchor.UpperLeft);

        var sep2 = CreateSeparator(hudPanelRoot);

        statsText = CreateText(hudPanelRoot, "Stats", "", 13, TextAnchor.UpperLeft);

        // Selection info (hidden by default)
        selectionText = CreateText(hudPanelRoot, "Selection", "", 13, TextAnchor.UpperLeft);
        selectionText.gameObject.SetActive(false);

        var sep3 = CreateSeparator(hudPanelRoot);

        CreateText(hudPanelRoot, "CandidateHeader", "── Expansion Targets ──", 13, TextAnchor.UpperLeft);

        // Candidate list rows
        var listGo = new GameObject("CandidateList");
        listGo.transform.SetParent(hudPanelRoot.transform, false);
        candidateListParent = listGo.transform;
        var listVlg = listGo.AddComponent<VerticalLayoutGroup>();
        listVlg.spacing = 2;
        listVlg.padding = new RectOffset(0, 0, 0, 0);
        listVlg.childForceExpandWidth = true;
        listVlg.childForceExpandHeight = false;

        // Spacer to push content to top
        var spacer = new GameObject("Spacer");
        spacer.transform.SetParent(hudPanelRoot.transform, false);
        var le = spacer.AddComponent<LayoutElement>();
        le.flexibleHeight = 1;

        // ── End-game panel (centred, initially hidden) ──
        endPanelRoot = CreatePanel(canvasGo, "EndPanel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(-200, -150), new Vector2(400, 300));
        endPanelRoot.SetActive(false);

        var endBg = endPanelRoot.AddComponent<Image>();
        endBg.color = new Color(0, 0, 0, 0.85f);

        var endVlg = endPanelRoot.AddComponent<VerticalLayoutGroup>();
        endVlg.spacing = 6;
        endVlg.padding = new RectOffset(16, 16, 16, 16);
        endVlg.childForceExpandWidth = true;
        endVlg.childForceExpandHeight = false;

        endResultText = CreateText(endPanelRoot, "Result", "", 26, TextAnchor.MiddleCenter);
        endStatsText = CreateText(endPanelRoot, "FinalStats", "", 14, TextAnchor.UpperLeft);
        CreateRestartButton(endPanelRoot);
    }

    // ── Data refresh ──────────────────────────────────────────────────

    private void RefreshData()
    {
        if (mapData == null) return;
        cachedPreviews = StrategicConnectionService.GetExpansionPreviews(mapData);
    }

    // ── Update loop ────────────────────────────────────────────────────

    private void Update()
    {
        refreshTimer += Time.deltaTime;
        if (refreshTimer >= 2f)
        {
            refreshTimer = 0f;
            RefreshData();
        }

        var result = MatchResultService.CurrentResult;
        bool matchEnded = result != MatchResult.None;

        // Toggle panels
        if (endPanelRoot.activeSelf != matchEnded)
        {
            endPanelRoot.SetActive(matchEnded);
            hudPanelRoot.SetActive(!matchEnded);
            if (matchEnded)
                UpdateEndPanel(result);
        }

        // Always update HUD content (even while end panel is shown,
        // so stats are up-to-date when we switch back)
        UpdateHudUI();
    }

    // ── HUD UI update ──────────────────────────────────────────────────

    private void UpdateHudUI()
    {
        var result = MatchResultService.CurrentResult;

        // Objective
        string objText = result == MatchResult.PlayerVictory ? "Victory!"
            : result == MatchResult.PlayerDefeat ? "Defeated."
            : "Capture neutral plots → Defeat Enemy base";
        objectiveText.text = objText;

        // Enemy timer
        if (result == MatchResult.None)
        {
            float timeLeft = GameStatusService.TimeUntilNextEnemyAttack;
            enemyTimerText.text = timeLeft > 0f
                ? $"Enemy Attack: ~{timeLeft:F0}s"
                : "Enemy Attack: imminent";

            string enemyLast = GameStatusService.LastEnemyActionResult;
            if (!string.IsNullOrEmpty(enemyLast))
            {
                string shortMsg = enemyLast.Length > 55 ? enemyLast[..55] + "…" : enemyLast;
                enemyTimerText.text += $"\n{shortMsg}";
            }
        }
        else
        {
            enemyTimerText.text = "";
        }

        // Last action
        string lastAction = GameStatusService.LastActionResult;
        actionText.text = !string.IsNullOrEmpty(lastAction) ? $"Last: {lastAction}" : "";
        actionText.gameObject.SetActive(!string.IsNullOrEmpty(lastAction));

        // Faction stats
        int pUnits = FactionStatsService.CountAliveUnits(Faction.Player);
        int pCap = FactionStatsService.GetSupplyCap(Faction.Player);
        int pGran = FactionStatsService.CountAliveGranaries(Faction.Player);
        int pTow = FactionStatsService.CountAliveTowers(Faction.Player);

        int eUnits = FactionStatsService.CountAliveUnits(Faction.Enemy);
        int eCap = FactionStatsService.GetSupplyCap(Faction.Enemy);
        int eGran = FactionStatsService.CountAliveGranaries(Faction.Enemy);
        int eTow = FactionStatsService.CountAliveTowers(Faction.Enemy);

        statsText.text = $"Units: Player {pUnits}/{pCap}  |  Enemy {eUnits}/{eCap}\n"
            + $"Bldgs: Player G:{pGran} T:{pTow}  |  Enemy G:{eGran} T:{eTow}";

        // Selection info
        if (inputController != null && inputController.HasSelection)
        {
            selectionText.text = $"Selected: {inputController.SelectedSourcePlotId}  |  Targets: {inputController.ValidTargets.Count}";
            selectionText.gameObject.SetActive(true);
        }
        else
        {
            selectionText.gameObject.SetActive(false);
        }

        // Candidate rows
        RebuildCandidateRows();
    }

    // ── Candidate rows ─────────────────────────────────────────────────

    private struct CandidateRowUI
    {
        public GameObject root;
        public Text infoText;
        public Text statusText;
        public Button dispatchButton;
        public string sourcePlotId;
        public string targetPlotId;
    }

    private void RebuildCandidateRows()
    {
        // Remove old rows — detach from hierarchy first so new rows
        // don't compete with soon-to-be-destroyed objects in the same parent.
        foreach (var row in candidateRows)
        {
            if (row.root != null)
            {
                row.root.transform.SetParent(null);
                Destroy(row.root);
            }
        }
        candidateRows.Clear();

        if (cachedPreviews.Count == 0)
        {
            // Show "(none)" placeholder
            var noneGo = new GameObject("NoneLabel");
            noneGo.transform.SetParent(candidateListParent, false);
            var noneText = noneGo.AddComponent<Text>();
            noneText.font = uiFont;
            noneText.fontSize = 12;
            noneText.color = Color.gray;
            noneText.text = "  (none)";
            noneText.alignment = TextAnchor.UpperLeft;

            // Track it for cleanup
            candidateRows.Add(new CandidateRowUI { root = noneGo });
            return;
        }

        int maxToShow = Mathf.Min(cachedPreviews.Count, 4);
        for (int i = 0; i < maxToShow; i++)
        {
            var p = cachedPreviews[i];

            var rowGo = new GameObject($"Candidate_{i}");
            rowGo.transform.SetParent(candidateListParent, false);

            var hlg = rowGo.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 4;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = false;

            // Info label: "[1] Crossroads → Village: 3/1"
            string info = $"[{i + 1}] {p.sourcePlotId} → {p.targetPlotId}: {p.availableCount}/{p.requiredCount}";
            var infoText = CreateLinkedText(rowGo, "Info", info, 12, TextAnchor.MiddleLeft);
            var infoLe = infoText.gameObject.AddComponent<LayoutElement>();
            infoLe.minWidth = 220;

            // Status label: "enough" / "short"
            string statusStr = p.hasEnough ? "enough" : "short";
            var statusText = CreateLinkedText(rowGo, "Status", statusStr, 12, TextAnchor.MiddleLeft);
            var statusLe = statusText.gameObject.AddComponent<LayoutElement>();
            statusLe.minWidth = 42;

            // Dispatch button
            var btnGo = new GameObject("DspBtn");
            btnGo.transform.SetParent(rowGo.transform, false);
            var btn = btnGo.AddComponent<Button>();
            var btnImage = btnGo.AddComponent<Image>();
            btnImage.color = new Color(0.2f, 0.5f, 0.2f);
            btn.targetGraphic = btnImage;

            var btnText = CreateLinkedText(btnGo, "Label", "Dsp", 11, TextAnchor.MiddleCenter);
            var btnLe = btnGo.AddComponent<LayoutElement>();
            btnLe.minWidth = 38;
            btnLe.minHeight = 20;

            // Capture by value for closure
            string src = p.sourcePlotId;
            string tgt = p.targetPlotId;
            btn.onClick.AddListener(() => HandleDispatchClick(src, tgt));

            candidateRows.Add(new CandidateRowUI
            {
                root = rowGo,
                infoText = infoText,
                statusText = statusText,
                dispatchButton = btn,
                sourcePlotId = p.sourcePlotId,
                targetPlotId = p.targetPlotId
            });
        }
    }

    // ── End panel ─────────────────────────────────────────────────────

    private void UpdateEndPanel(MatchResult result)
    {
        string resultText = result == MatchResult.PlayerVictory ? "PLAYER VICTORY!" : "DEFEAT!";
        endResultText.text = resultText;

        int pUnits = FactionStatsService.CountAliveUnits(Faction.Player);
        int pCap = FactionStatsService.GetSupplyCap(Faction.Player);
        int pGran = FactionStatsService.CountAliveGranaries(Faction.Player);
        int pTow = FactionStatsService.CountAliveTowers(Faction.Player);

        int eUnits = FactionStatsService.CountAliveUnits(Faction.Enemy);
        int eCap = FactionStatsService.GetSupplyCap(Faction.Enemy);
        int eGran = FactionStatsService.CountAliveGranaries(Faction.Enemy);
        int eTow = FactionStatsService.CountAliveTowers(Faction.Enemy);

        endStatsText.text = $"Final Stats:\n"
            + $"  Player Units: {pUnits}/{pCap}\n"
            + $"  Enemy  Units: {eUnits}/{eCap}\n"
            + $"  Player Bldgs: G:{pGran} T:{pTow}  |  Enemy Bldgs: G:{eGran} T:{eTow}";
    }

    // ── Actions ────────────────────────────────────────────────────────

    private void HandleDispatchClick(string sourcePlotId, string targetPlotId)
    {
        if (mapData == null || mapRenderer == null) return;

        var result = StrategicExpansionCommandService.DispatchCandidate(mapData, mapRenderer, sourcePlotId, targetPlotId);
        GameStatusService.LastActionResult = result.message;
        Debug.Log($"[GameHud] Dispatch: {result.message}");
        RefreshData();

        if (result.dispatchedCount == 0)
            Debug.Log($"[GameHud] Dispatch to {targetPlotId} failed: {result.message}");
    }

    private void HandleRestart()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.Log("[GameHud] Restart: no active scene name. Add the scene to Build Settings.");
        }
        else
        {
            Debug.Log($"[GameHud] Restarting scene: {sceneName}");
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }

    // ── UI helpers ─────────────────────────────────────────────────────

    private GameObject CreatePanel(GameObject parent, string name,
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 pos, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);

        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return go;
    }

    private Text CreateText(GameObject parent, string name, string content,
        int fontSize, TextAnchor alignment)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);

        var text = go.AddComponent<Text>();
        text.font = uiFont;
        text.fontSize = fontSize;
        text.color = Color.white;
        text.text = content;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        return text;
    }

    private Text CreateLinkedText(GameObject parent, string name, string content,
        int fontSize, TextAnchor alignment)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);

        var text = go.AddComponent<Text>();
        text.font = uiFont;
        text.fontSize = fontSize;
        text.color = Color.white;
        text.text = content;
        text.alignment = alignment;
        return text;
    }

    private void CreateSeparator(GameObject parent)
    {
        var sep = new GameObject("Sep");
        sep.transform.SetParent(parent.transform, false);
        var sepText = sep.AddComponent<Text>();
        sepText.font = uiFont;
        sepText.fontSize = 8;
        sepText.color = new Color(0.5f, 0.5f, 0.5f);
        sepText.text = "─";
    }

    private void CreateRestartButton(GameObject parent)
    {
        var btnGo = new GameObject("RestartBtn");
        btnGo.transform.SetParent(parent.transform, false);

        var btn = btnGo.AddComponent<Button>();
        var btnImage = btnGo.AddComponent<Image>();
        btnImage.color = new Color(0.3f, 0.3f, 0.6f);
        btn.targetGraphic = btnImage;

        var btnText = CreateLinkedText(btnGo, "Label", "Restart (N)", 16, TextAnchor.MiddleCenter);
        var le = btnGo.AddComponent<LayoutElement>();
        le.minHeight = 32;

        btn.onClick.AddListener(() => HandleRestart());
    }
}
