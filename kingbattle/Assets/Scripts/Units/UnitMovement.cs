using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    /// <summary>
    /// Moves a unit along a list of world-space waypoints.
    /// The unit moves toward the next waypoint each frame at its configured speed.
    /// Supports Pause / Resume so combat can temporarily halt movement.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class UnitMovement : MonoBehaviour
    {
        /// <summary>Speed in world-units per second. Set by the spawner.</summary>
        public float speed = 2f;

        private List<Vector3> waypoints;
        private int currentIndex;
        private bool hasPath;     // true while a path still needs traversing
        private bool isPaused;    // true while combat halts movement

        // ── Public API ──────────────────────────────────────────────────

        /// <summary>
        /// Begin moving this unit along the given list of positions.
        /// The first position should be the current position (instant snap).
        /// </summary>
        public void StartMoving(List<Vector3> waypointPositions)
        {
            if (waypointPositions == null || waypointPositions.Count < 2)
            {
                Debug.LogWarning("[UnitMovement] Path too short, nothing to do.");
                return;
            }

            waypoints = waypointPositions;
            currentIndex = 1; // skip the first waypoint (we're already there)
            hasPath = true;
            isPaused = false;

            // Snap to first waypoint
            transform.position = waypointPositions[0];
        }

        /// <summary>True while the unit is still travelling (not paused, not finished).</summary>
        public bool IsMoving => hasPath && !isPaused;

        /// <summary>True while the unit still has remaining waypoints (regardless of pause).</summary>
        public bool HasRemainingPath => hasPath;

        /// <summary>Pause movement (e.g. when attacking).</summary>
        public void Pause()
        {
            isPaused = true;
        }

        /// <summary>Resume movement after pause.</summary>
        public void Resume()
        {
            isPaused = false;
        }

        /// <summary>Stop movement entirely and clear path.</summary>
        public void Stop()
        {
            hasPath = false;
            isPaused = false;
            waypoints = null;
        }

        /// <summary>
        /// Replace the current path with a new one (used by push-wave commands).
        /// Retains the current position as the first waypoint.
        /// </summary>
        public void SetNewPath(List<Vector3> newWaypoints)
        {
            if (newWaypoints == null || newWaypoints.Count < 2)
            {
                Stop();
                return;
            }

            waypoints = newWaypoints;
            currentIndex = 1;
            hasPath = true;
            isPaused = false;
        }

        // ── MonoBehaviour ───────────────────────────────────────────────

        private void Update()
        {
            if (!hasPath || isPaused || waypoints == null) return;

            var target = waypoints[currentIndex];
            var step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);

            if (Vector3.Distance(transform.position, target) < 0.01f)
            {
                transform.position = target; // snap to avoid floating-point drift
                currentIndex++;

                if (currentIndex >= waypoints.Count)
                {
                    hasPath = false;
                    Debug.Log($"[UnitMovement] {name} reached destination.");
                }
            }
        }
    }
}
