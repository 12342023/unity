using Core;
using Units;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Unit combat behaviour.
    /// When the unit is within attackRange of its target it stops moving
    /// and attacks at a fixed interval.
    /// </summary>
    [RequireComponent(typeof(UnitMovement))]
    [RequireComponent(typeof(HealthComponent))]
    public class UnitCombat : MonoBehaviour
    {
        [Header("Combat Stats")]
        public float damage = 10f;
        public float attackRange = 1.5f;
        public float attackInterval = 1.0f;
        public Faction faction;

        private HealthComponent target;
        private HealthComponent myHealth;
        private UnitMovement movement;
        private float attackTimer;

        // ── Public API ──────────────────────────────────────────────────

        /// <summary>Set the building or unit this unit should attack.</summary>
        public void SetTarget(HealthComponent enemyTarget)
        {
            target = enemyTarget;
            if (target != null && target.IsDead)
                target = null;
        }

        /// <summary>True while the unit has a valid target in range.</summary>
        public bool IsInCombat { get; private set; }

        // ── MonoBehaviour ───────────────────────────────────────────────

        private void Start()
        {
            myHealth = GetComponent<HealthComponent>();
            movement = GetComponent<UnitMovement>();
        }

        private void Update()
        {
            if (target == null || target.IsDead)
            {
                if (IsInCombat)
                {
                    IsInCombat = false;
                    movement?.Resume();
                }
                return;
            }

            float dist = Vector3.Distance(transform.position, target.transform.position);

            if (dist <= attackRange)
            {
                // In range — stop moving and attack
                IsInCombat = true;
                movement?.Pause();

                attackTimer += Time.deltaTime;
                if (attackTimer >= attackInterval)
                {
                    attackTimer = 0f;
                    target.TakeDamage(damage);
                }
            }
            else
            {
                // Not in range — keep moving
                if (IsInCombat)
                {
                    IsInCombat = false;
                    movement?.Resume();
                }
            }
        }
    }
}
