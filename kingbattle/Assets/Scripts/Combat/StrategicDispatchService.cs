using System.Collections.Generic;
using Buildings;
using Core;
using Map;
using Units;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Structured result for a dispatch operation.
    /// </summary>
    public class DispatchResult
    {
        public string targetPlotId;
        public int dispatchedCount;
        public int requiredSoldierCount;
        public bool hasEnoughSoldiers => dispatchedCount >= requiredSoldierCount;
        public bool captureWillBeAttemptedOnArrival => hasEnoughSoldiers && dispatchedCount > 0;
        public string message;
    }

    /// <summary>
    /// Handles dispatching nearby Player soldiers from a rally point
    /// to a target plot via the road network. On arrival, triggers
    /// PlotCaptureService.TryCapture only if dispatched count meets
    /// the requiredSoldierCount threshold.
    ///
    /// Manages its own capture-handler dictionary so GameEntry can
    /// stay thin — it only finds the rally ruin, picks a target,
    /// and calls DispatchToPlot.
    /// </summary>
    public static class StrategicDispatchService
    {
        // Tracks active capture handlers to clean up on re-dispatch
        private static readonly Dictionary<UnitCombat, System.Action> captureHandlers = new();

        /// <summary>Call on game start to clear stale handler state.</summary>
        public static void Reset()
        {
            captureHandlers.Clear();
        }

        /// <summary>
        /// Find non-dead Player soldiers within <paramref name="gatherRadius"/>
        /// of <paramref name="rallyPos"/>, clear their current orders, and send
        /// them along <paramref name="waypoints"/>.
        ///
        /// Returns a DispatchResult with structured status.
        /// </summary>
        public static DispatchResult DispatchToPlot(
            Vector3 rallyPos,
            List<Vector3> waypoints,
            string targetPlotId,
            MapData mapData,
            MapRenderer mapRenderer,
            int requiredSoldierCount = 1,
            float gatherRadius = 5f)
        {
            if (waypoints == null || waypoints.Count < 2)
                return new DispatchResult
                {
                    targetPlotId = targetPlotId,
                    dispatchedCount = 0,
                    requiredSoldierCount = requiredSoldierCount,
                    message = $"Path too short or null, cannot dispatch to {targetPlotId}."
                };

            int totalDispatched = 0;
            bool captureConsidered = false; // shared across handlers via closure
            int dispatchedIndex = 0;

            // Target world position (last waypoint)
            Vector3 targetWorldPos = waypoints.Count > 0 ? waypoints[waypoints.Count - 1] : rallyPos;

            foreach (var u in Object.FindObjectsByType<UnitCombat>(FindObjectsInactive.Exclude))
            {
                if (u.faction != Faction.Player) continue;
                if (u.GetComponent<HealthComponent>().IsDead) continue;
                if (Vector3.Distance(u.transform.position, rallyPos) > gatherRadius) continue;

                u.ClearPushPath();
                u.GetComponent<UnitMovement>()?.Stop();

                // Remove any old capture handler for this unit
                if (captureHandlers.TryGetValue(u, out var oldHandler))
                {
                    u.OnPushDestinationReached -= oldHandler;
                    captureHandlers.Remove(u);
                }

                totalDispatched++; // increment BEFORE creating the handler
                int idx = dispatchedIndex++; // capture by value per soldier for patrol stagger
                var capturedTarget = targetPlotId;
                var capturedRequired = requiredSoldierCount;

                // One-shot handler. totalDispatched is captured by reference —
                // all lambdas share the same variable, so when the first soldier
                // arrives later, totalDispatched already holds the FINAL count.
                System.Action localHandler = null;
                localHandler = () =>
                {
                    u.OnPushDestinationReached -= localHandler;
                    captureHandlers.Remove(u);

                    // ── Update patrol center to target plot ──
                    // Whether capture succeeds or is blocked, dispatched
                    // soldiers stay near the target and do not return to source.
                    var patrol = u.GetComponent<UnitPatrol>();
                    if (patrol != null)
                    {
                        patrol.Setup(targetWorldPos, 0.9f, (idx % 12) * 30f);
                    }
                    u.SetHomePosition(targetWorldPos);

                    if (!captureConsidered)
                    {
                        captureConsidered = true;
                        if (totalDispatched >= capturedRequired)
                        {
                            Debug.Log($"[StrategicDispatchService] Capture attempt allowed: dispatched {totalDispatched}/{capturedRequired} to {capturedTarget}.");
                            bool captured = PlotCaptureService.TryCapture(capturedTarget, mapData, mapRenderer, Faction.Player);
                            GameStatusService.LastActionResult = captured
                                ? $"{capturedTarget} captured! ({totalDispatched}/{capturedRequired})"
                                : $"{capturedTarget} capture failed ({totalDispatched}/{capturedRequired})";
                        }
                        else
                        {
                            Debug.Log($"[StrategicDispatchService] Capture blocked: dispatched {totalDispatched}/{capturedRequired} to {capturedTarget}.");
                            GameStatusService.LastActionResult = $"Capture blocked: {totalDispatched}/{capturedRequired} to {capturedTarget}";
                        }
                    }
                };

                captureHandlers[u] = localHandler;
                u.OnPushDestinationReached += localHandler;
                u.SetPushPath(new List<Vector3>(waypoints));
            }

            var result = new DispatchResult
            {
                targetPlotId = targetPlotId,
                dispatchedCount = totalDispatched,
                requiredSoldierCount = requiredSoldierCount,
                message = totalDispatched > 0
                    ? $"dispatch {totalDispatched}/{requiredSoldierCount} to {targetPlotId}, willCapture={totalDispatched >= requiredSoldierCount}."
                    : $"No soldiers near rally point to dispatch to {targetPlotId}."
            };
            if (totalDispatched == 0)
                GameStatusService.LastActionResult = result.message;
            return result;
        }
    }
}
