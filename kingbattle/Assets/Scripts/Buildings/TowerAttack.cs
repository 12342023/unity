using Combat;
using Core;
using UnityEngine;

namespace Buildings
{
    /// <summary>
    /// Tower building: automatically attacks the nearest enemy unit within range.
    /// Uses Physics2D.OverlapCircleNonAlloc for efficient enemy scanning.
    /// </summary>
    [RequireComponent(typeof(HealthComponent))]
    public class TowerAttack : MonoBehaviour
    {
        [Header("Combat Stats")]
        public float damage = 15f;
        public float attackRange = 4f;
        public float attackInterval = 1.5f;
        public Faction faction;

        private HealthComponent myHealth;
        private float attackTimer;

        // Cache for physics overlap
        private readonly Collider2D[] hitBuffer = new Collider2D[32];

        // ── MonoBehaviour ───────────────────────────────────────────────

        private void Start()
        {
            myHealth = GetComponent<HealthComponent>();
        }

        private void Update()
        {
            if (myHealth.IsDead) return;

            attackTimer += Time.deltaTime;
            if (attackTimer < attackInterval) return;

            // Find nearest enemy unit with health component
            var target = FindNearestEnemy();
            if (target != null)
            {
                attackTimer = 0f;
                target.TakeDamage(damage);
            }
        }

        // ── Targeting ───────────────────────────────────────────────────

        private HealthComponent FindNearestEnemy()
        {
            HealthComponent nearest = null;
            float nearestDist = attackRange;

            // Use non-alloc physics overlap for efficiency
            int count = Physics2D.OverlapCircleNonAlloc(
                transform.position, attackRange, hitBuffer);

            for (int i = 0; i < count; i++)
            {
                var health = hitBuffer[i].GetComponent<HealthComponent>();
                if (health == null || health == myHealth || health.IsDead)
                    continue;
                if (health.faction == faction)
                    continue; // same faction = not enemy

                float dist = Vector2.Distance(transform.position, health.transform.position);
                if (dist <= nearestDist)
                {
                    nearestDist = dist;
                    nearest = health;
                }
            }

            return nearest;
        }

        // ── Gizmos (editor only) ────────────────────────────────────────

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
