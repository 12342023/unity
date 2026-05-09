using System.Collections.Generic;
using Core;
using Map;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Handles plot capture when a unit reaches a destination.
    /// Only supports Neutral → Player capture.
    /// Rejects main base plots and non-Player capturers.
    /// Uses a static HashSet to prevent duplicate captures of the same plot.
    /// </summary>
    public static class PlotCaptureService
    {
        private static readonly HashSet<string> capturedPlots = new();

        /// <summary>Clear captured-plot tracking (e.g. on scene reload).</summary>
        public static void Reset()
        {
            capturedPlots.Clear();
        }

        /// <summary>
        /// Attempt to capture the given plot for <paramref name="capturingFaction"/>.
        /// Conditions:
        ///   - Only Faction.Player can capture.
        ///   - Only Neutral plots are eligible.
        ///   - Main base plots are never eligible.
        ///   - Each plot can only be captured once per game.
        /// </summary>
        public static bool TryCapture(string plotId, MapData mapData, MapRenderer mapRenderer, Faction capturingFaction)
        {
            if (string.IsNullOrEmpty(plotId) || mapData == null)
                return false;

            // Only Player can capture
            if (capturingFaction != Faction.Player)
            {
                Debug.Log($"[PlotCaptureService] Only Player can capture, but {capturingFaction} attempted.");
                return false;
            }

            var plot = mapData.GetPlot(plotId);
            if (plot == null)
            {
                Debug.LogWarning($"[PlotCaptureService] Plot '{plotId}' not found.");
                return false;
            }

            // Main base plots cannot be captured
            if (plot.isMainBase)
            {
                Debug.Log($"[PlotCaptureService] {plotId} is a main base, cannot be captured.");
                return false;
            }

            // Only Neutral plots are eligible
            if (plot.faction != Faction.Neutral)
            {
                Debug.Log($"[PlotCaptureService] {plotId} is not neutral (current: {plot.faction}), skip.");
                return false;
            }

            // Prevent double-capture
            if (!capturedPlots.Add(plotId))
            {
                Debug.Log($"[PlotCaptureService] {plotId} already captured, skip.");
                return false;
            }

            // Commit
            plot.faction = capturingFaction;
            mapRenderer?.RefreshPlotColor(plotId, mapData);

            Debug.Log($"[PlotCaptureService] {plotId} captured by {capturingFaction}!");
            return true;
        }
    }
}
