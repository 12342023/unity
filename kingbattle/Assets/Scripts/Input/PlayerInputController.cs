using System.Collections.Generic;
using Combat;
using Core;
using Map;
using UnityEngine;

/// <summary>
/// Translates mouse clicks into gameplay commands for expansion.
/// Flow:
///   1. Click Player-owned frontier plot → select it, highlight valid Neutral targets.
///   2. Click a highlighted target → dispatch via StrategicExpansionCommandService.
///   3. Click elsewhere → clear selection.
///
/// Porting boundary: only calls command service and reads state.
/// Does NOT directly modify map/building/unit data.
/// No dependency on OnGUI, debug keys, or platform APIs.
/// </summary>
public class PlayerInputController : MonoBehaviour
{
    private MapData mapData;
    private MapRenderer mapRenderer;

    // Current selection state
    private string selectedSourcePlotId;
    private readonly List<string> validTargetPlotIds = new();

    // Highlight colours
    private static readonly Color HighlightSource = new Color(0.2f, 0.6f, 1.0f);
    private static readonly Color HighlightTarget = new Color(0.7f, 0.9f, 0.2f);

    public void Initialize(MapData data, MapRenderer renderer)
    {
        mapData = data;
        mapRenderer = renderer;
        ClearSelection();
    }

    /// <summary>True while the player has a source plot selected.</summary>
    public bool HasSelection => !string.IsNullOrEmpty(selectedSourcePlotId);

    /// <summary>The currently selected source plotId, or null.</summary>
    public string SelectedSourcePlotId => selectedSourcePlotId;

    /// <summary>Read-only list of valid target plotIds for the current selection.</summary>
    public IReadOnlyList<string> ValidTargets => validTargetPlotIds;

    /// <summary>Programmatically clear the current selection (e.g. after dispatch).</summary>
    public void ClearSelection()
    {
        if (mapRenderer != null && mapData != null)
            mapRenderer.ClearAllHighlights(mapData);

        selectedSourcePlotId = null;
        validTargetPlotIds.Clear();
    }

    private void Update()
    {
        // Only process left-click during gameplay (not after match end)
        if (!Input.GetMouseButtonDown(0)) return;
        if (MatchResultService.CurrentResult != MatchResult.None) return;
        if (mapData == null || mapRenderer == null) return;

        // Convert screen click to world position
        Vector3 worldPos = Camera.main != null
            ? Camera.main.ScreenToWorldPoint(Input.mousePosition)
            : Vector3.zero;
        worldPos.z = 0f;

        // Find which plot was clicked
        string clickedPlotId = mapRenderer.GetPlotAtWorldPosition(worldPos, mapData);

        if (string.IsNullOrEmpty(clickedPlotId))
        {
            // Clicked empty space → clear selection
            if (HasSelection)
            {
                ClearSelection();
                GameStatusService.LastActionResult = "Selection cleared.";
            }
            return;
        }

        // ── CASE 1: No selection → try to select a valid source ──
        if (!HasSelection)
        {
            TrySelectSource(clickedPlotId);
            return;
        }

        // ── CASE 2: Has selection, clicked a valid target → dispatch ──
        if (validTargetPlotIds.Contains(clickedPlotId))
        {
            HandleDispatch(selectedSourcePlotId, clickedPlotId);
            ClearSelection();
            return;
        }

        // ── CASE 3: Has selection, clicked the same source again → deselect ──
        if (clickedPlotId == selectedSourcePlotId)
        {
            ClearSelection();
            GameStatusService.LastActionResult = "Selection cleared.";
            return;
        }

        // ── CASE 4: Has selection, clicked something else → try to re-select ──
        ClearSelection();
        TrySelectSource(clickedPlotId);
    }

    private void TrySelectSource(string plotId)
    {
        var plot = mapData.GetPlot(plotId);
        if (plot == null) return;

        // Must be Player-owned, non-main-base
        if (plot.faction != Faction.Player || plot.isMainBase)
        {
            GameStatusService.LastActionResult = $"Cannot expand from '{plotId}'.";
            return;
        }

        // Must have at least one Neutral neighbour
        var candidates = StrategicConnectionService.GetExpansionCandidates(mapData);
        validTargetPlotIds.Clear();
        foreach (var c in candidates)
        {
            if (c.sourcePlotId == plotId)
                validTargetPlotIds.Add(c.targetPlotId);
        }

        if (validTargetPlotIds.Count == 0)
        {
            GameStatusService.LastActionResult = $"'{plotId}' has no expandable Neutral neighbours.";
            return;
        }

        // Select this source
        selectedSourcePlotId = plotId;

        // Visual: highlight source and all valid targets
        mapRenderer.ClearAllHighlights(mapData);
        mapRenderer.SetPlotHighlight(plotId, HighlightSource);
        foreach (var tId in validTargetPlotIds)
            mapRenderer.SetPlotHighlight(tId, HighlightTarget);

        GameStatusService.LastActionResult = $"Selected '{plotId}'. {validTargetPlotIds.Count} target(s) highlighted. Click a target to dispatch.";
    }

    private void HandleDispatch(string sourcePlotId, string targetPlotId)
    {
        var result = StrategicExpansionCommandService.DispatchCandidate(
            mapData, mapRenderer, sourcePlotId, targetPlotId);
        GameStatusService.LastActionResult = result.message;
        Debug.Log($"[PlayerInputController] Dispatch: {result.message}");
    }
}
