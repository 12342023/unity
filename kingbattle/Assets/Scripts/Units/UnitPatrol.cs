using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    /// <summary>
    /// Manages a list of patrol waypoints for a unit.
    /// UnitBrain / UnitCombat reads from this when idling.
    /// </summary>
    public class UnitPatrol : MonoBehaviour
    {
        private List<Vector3> patrolPoints;
        private int currentIndex;

        /// <summary>Set patrol waypoints. Unit will cycle through them.</summary>
        public void Setup(List<Vector3> points)
        {
            patrolPoints = points;
            currentIndex = 0;
        }

        public bool HasPatrol => patrolPoints != null && patrolPoints.Count > 0;

        /// <summary>Current target patrol waypoint.</summary>
        public Vector3 CurrentTarget =>
            HasPatrol ? patrolPoints[currentIndex % patrolPoints.Count] : transform.position;

        /// <summary>Advance to the next patrol waypoint (cyclical).</summary>
        public void Advance()
        {
            if (!HasPatrol) return;
            currentIndex = (currentIndex + 1) % patrolPoints.Count;
        }
    }
}
