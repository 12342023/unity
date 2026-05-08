using UnityEngine;

namespace Units
{
    /// <summary>
    /// Circular patrol around a center point.
    /// Units circle around their building (Barracks / rally point) in a small loop.
    /// Multiple units are staggered by startAngle to avoid visual overlap.
    /// </summary>
    public class UnitPatrol : MonoBehaviour
    {
        private Vector3 center;
        private float radius = 0.9f;
        private float patrolSpeed = 1.2f;
        private float currentAngleDeg;
        private bool isActive;

        // ── Public API ──────────────────────────────────────────────────

        /// <summary>
        /// Set up circular patrol.
        /// </summary>
        /// <param name="patrolCenter">World position to circle around.</param>
        /// <param name="patrolRadius">Radius of the circle (0.6–1.2 recommended).</param>
        /// <param name="startAngleDeg">Starting angle in degrees. Stagger per unit.</param>
        /// <param name="speed">Movement speed while patrolling.</param>
        public void Setup(Vector3 patrolCenter, float patrolRadius, float startAngleDeg, float speed = 1.2f)
        {
            center = patrolCenter;
            radius = patrolRadius;
            currentAngleDeg = startAngleDeg;
            patrolSpeed = speed;
            isActive = true;

            // Do NOT snap position here — Tick() will apply the circle position
            // when the unit is ready (i.e. after road movement is complete).
        }

        /// <summary>Pause patrolling (e.g. when chasing / fighting).</summary>
        public void Pause()
        {
            isActive = false;
        }

        /// <summary>Resume circling from current angle.</summary>
        public void Resume()
        {
            isActive = true;
        }

        public bool IsActive => isActive;

        /// <summary>
        /// Call each frame while idling.
        /// Advances the angle and moves the unit along the circle.
        /// </summary>
        public void Tick(float deltaTime)
        {
            if (!isActive) return;

            // Angular speed: linear-speed / circumference * 360
            float circumference = 2f * Mathf.PI * Mathf.Max(radius, 0.1f);
            float angularSpeed = (patrolSpeed / circumference) * 360f; // deg / sec

            currentAngleDeg += angularSpeed * deltaTime;

            ApplyCirclePosition();
        }

        /// <summary>Get the world position the unit should be at right now.</summary>
        public Vector3 CurrentPatrolPosition
        {
            get
            {
                float rad = currentAngleDeg * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * radius;
                return center + offset;
            }
        }

        /// <summary>The center point of the patrol circle.</summary>
        public Vector3 Center => center;

        /// <summary>Radius of the patrol circle.</summary>
        public float Radius => radius;

        // ── Internal ────────────────────────────────────────────────────

        private void ApplyCirclePosition()
        {
            transform.position = CurrentPatrolPosition;
        }
    }
}
