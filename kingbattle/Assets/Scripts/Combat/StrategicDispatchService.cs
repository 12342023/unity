using System.Collections.Generic;
using Buildings;
using Core;
using Map;
using Units;
using UnityEngine;

namespace Combat
{
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
        /// The first soldier to arrive triggers a capture attempt only if
        /// <paramref name="dispatchedCount"/> >= <paramref name="requiredSoldierCount"/>.
        /// The dispatched count is counted internally and passed to the handler
        /// via closure.
        /// </summary>
        public static int DispatchToPlot(
            Vector3 rallyPos,
            List<Vector3> waypoints,
            string targetPlotId,
            MapData mapData,
            MapRenderer mapRenderer,
            int requiredSoldierCount = 1,
            float gatherRadius = 5f)
        {
            if (waypoints == null || waypoints.Count < 2)
                return 0;

            int totalDispatched = 0;
            bool captureConsidered = false; // shared across handlers via closure

            foreach (var u in GameObject.FindObjectsByType<UnitCombat>(FindObjectsSortMode.None))
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
                var capturedRequired = requiredSoldierCount;

                // One-shot handler. totalDispatched is captured by reference —
                // all lambdas share the same variable, so when the first soldier
                // arrives later, totalDispatched already holds the FINAL count.
                System.Action localHandler = null;
                localHandler = () =>
                {
                    u.OnPushDestinationReached -= localHandler;
                    captureHandlers.Remove(u);
                    if (!captureConsidered)
                    {
                        captureConsidered = true;
                        if (totalDispatched >= capturedRequired)
                        {
                            PlotCaptureService.TryCapture(targetPlotId, mapData, mapRenderer, Faction.Player);
                        }
                        else
                        {
                            Debug.Log($"[StrategicDispatchService] Capture blocked: dispatched {totalDispatched}/{capturedRequired} to {targetPlotId}.");
                        }
                    }
                };

                captureHandlers[u] = localHandler;
                u.OnPushDestinationReached += localHandler;
                u.SetPushPath(new List<Vector3>(waypoints));
            }

            if (totalDispatched > 0 && totalDispatched < requiredSoldierCount)
            {
                Debug.Log($"[StrategicDispatchService] Dispatched {totalDispatched}/{requiredSoldierCount} — capture will be blocked on arrival.");
            }

            return totalDispatched;
        }
    }
}
