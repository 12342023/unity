using System.Collections.Generic;
using Core;
using Map;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Handles plot capture when a unit reaches a destination.
    /// Currently only supports Neutral → Player capture.
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
        /// Currently only Neutral → Player is allowed.
        /// Returns true on first successful capture, false if already captured
        /// or conditions aren't met.
        /// </summary>
        public static bool TryCapture(string plotId, MapData mapData, MapRenderer mapRenderer, Faction capturingFaction)
        {
            if (string.IsNullOrEmpty(plotId) || mapData == null)
                return false;

            var plot = mapData.GetPlot(plotId);
            if (plot == null)
            {
                Debug.LogWarning($"[PlotCaptureService] Plot '{plotId}' not found.");
                return false;
            }

            // Only allow Neutral → capturingFaction
            if (plot.faction != Faction.Neutral)
            {
                Debug.Log($"[PlotCaptureService] {plotId} is not neutral (current: {plot.faction}), skip.");
                return false;
            }

            // Prevent double-capture of the same plot
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
