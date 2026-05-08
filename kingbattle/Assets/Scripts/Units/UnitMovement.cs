using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    /// <summary>
    /// Moves a unit along a list of world-space waypoints.
    /// The unit moves toward the next waypoint each frame at its configured speed.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class UnitMovement : MonoBehaviour
    {
        /// <summary>Speed in world-units per second. Set by the spawner.</summary>
        public float speed = 2f;

        private List<Vector3> waypoints;
        private int currentIndex;
        private bool isMoving;

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
            isMoving = true;

            // Snap to first waypoint
            transform.position = waypointPositions[0];
        }

        /// <summary>True while the unit is still travelling.</summary>
        public bool IsMoving => isMoving;

        // ── MonoBehaviour ───────────────────────────────────────────────

        private void Update()
        {
            if (!isMoving || waypoints == null) return;

            var target = waypoints[currentIndex];
            var step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);

            if (Vector3.Distance(transform.position, target) < 0.01f)
            {
                transform.position = target; // snap to avoid floating-point drift
                currentIndex++;

                if (currentIndex >= waypoints.Count)
                {
                    isMoving = false;
                    Debug.Log($"[UnitMovement] {name} reached destination.");
                }
            }
        }
    }
}
