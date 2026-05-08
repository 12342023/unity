using System.Collections.Generic;
using Core;
using Units;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Unit combat behaviour with self-directed aggro / chase / deaggro.
    ///
    /// State machine:
    ///   Idle   → patrol or stand by, scan aggroRange for enemies
    ///   Chase  → pursue enemy within chaseRange, attack when close enough
    ///   Attack → stop, attack at interval until target dies
    ///
    /// On deaggro (target dead or out of chaseRange) → return to Idle.
    /// </summary>
    [RequireComponent(typeof(UnitMovement))]
    [RequireComponent(typeof(HealthComponent))]
    public class UnitCombat : MonoBehaviour
    {
        [Header("Combat Stats")]
        public float damage = 10f;
        public float attackRange = 1.5f;
        public float attackInterval = 1.0f;

        [Header("Aggro")]
        public float aggroRange = 4f;
        public float chaseRange = 7f;

        [Header("Faction")]
        public Faction faction;

        // ── State machine ──────────────────────────────────────────────
        private enum CState { Idle, Chase, Attack }
        private CState state = CState.Idle;

        private HealthComponent target;      // current attack target (Attack state)
        private HealthComponent chaseTarget; // enemy we're pursuing (Chase state)
        private HealthComponent myHealth;
        private UnitMovement movement;
        private float attackTimer;

        // Patrol fallback — set by BarracksSpawner
        private UnitPatrol patrol;
        private Vector3 homePosition;
        private bool hasHome;

        // External push command — path waypoints to enemy
        private List<Vector3> pushPath;
        private int pushIndex;

        // ── Public API ──────────────────────────────────────────────────

        /// <summary>Set the home position to return to after deaggro.
        /// Patrol should already be set up with proper stagger angle by the spawner.</summary>
        public void SetHomePosition(Vector3 pos)
        {
            homePosition = pos;
            hasHome = true;
        }

        /// <summary>Assign a push path: unit will traverse this path toward the enemy,
        /// scanning for aggro along the way.</summary>
        public void SetPushPath(List<Vector3> waypoints)
        {
            pushPath = waypoints;
            pushIndex = 1; // skip first (current position)
            state = CState.Idle; // will pick up push movement naturally
        }

        /// <summary>Clear any push orders.</summary>
        public void ClearPushPath()
        {
            pushPath = null;
        }

        /// <summary>True while the unit has a visible combat target.</summary>
        public bool IsInCombat => state == CState.Attack;

        /// <summary>True while the unit is actively engaged (chase or attack).</summary>
        public bool IsEngaged => state != CState.Idle;

        // Callback invoked when a push-path waypoint is reached
        public System.Action OnPushDestinationReached;

        // ── MonoBehaviour ───────────────────────────────────────────────

        private void Start()
        {
            myHealth = GetComponent<HealthComponent>();
            movement = GetComponent<UnitMovement>();
            patrol = GetComponent<UnitPatrol>();
        }

        private void Update()
        {
            if (myHealth.IsDead) return;

            switch (state)
            {
                case CState.Idle:   UpdateIdle();   break;
                case CState.Chase:  UpdateChase();  break;
                case CState.Attack: UpdateAttack(); break;
            }
        }

        // ── Idle: patrol or stand by, scan for enemies ──────────────────

        private void UpdateIdle()
        {
            // 1. Scan for enemies in aggro range
            var enemy = FindNearestEnemy(aggroRange);
            if (enemy != null)
            {
                chaseTarget = enemy;
                state = CState.Chase;
                movement?.Stop(); // stop patrol movement
                return;
            }

            // 2. Follow push path if assigned
            if (pushPath != null && pushIndex < pushPath.Count)
            {
                // Move directly toward next push waypoint
                var targetPos = pushPath[pushIndex];
                float dist = Vector3.Distance(transform.position, targetPos);
                if (dist < 0.3f)
                {
                    pushIndex++;
                    if (pushIndex >= pushPath.Count)
                    {
                        pushPath = null;
                        OnPushDestinationReached?.Invoke();
                    }
                }
                else
                {
                    var step = movement.speed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
                }
                return;
            }

            // 3. Circular patrol (only when NOT moving along road path AND no push path)
            if (patrol != null && pushPath == null && movement != null && !movement.HasRemainingPath)
            {
                patrol.Tick(Time.deltaTime);
            }
        }

        // ── Chase: pursue enemy, attack when in range ───────────────────

        private void UpdateChase()
        {
            // Deaggro checks
            if (chaseTarget == null || chaseTarget.IsDead)
            {
                Deaggro();
                return;
            }

            float dist = Vector3.Distance(transform.position, chaseTarget.transform.position);

            if (dist > chaseRange)
            {
                Deaggro();
                return;
            }

            if (dist <= attackRange)
            {
                // Enter attack mode
                target = chaseTarget;
                chaseTarget = null;
                state = CState.Attack;
                attackTimer = 0f;
                return;
            }

            // Move toward target (direct chase, not road-based)
            var step = movement.speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(
                transform.position, chaseTarget.transform.position, step);
        }

        // ── Attack: existing combat loop ────────────────────────────────

        private void UpdateAttack()
        {
            if (target == null || target.IsDead)
            {
                Deaggro();
                return;
            }

            float dist = Vector3.Distance(transform.position, target.transform.position);

            if (dist > attackRange * 1.2f)
            {
                // Target moved out of range — chase again
                chaseTarget = target;
                target = null;
                state = CState.Chase;
                return;
            }

            attackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f;
                target.TakeDamage(damage);
            }
        }

        // ── Deaggro ─────────────────────────────────────────────────────

        private void Deaggro()
        {
            target = null;
            chaseTarget = null;
            state = CState.Idle;

            // Resume circular patrol around home
            if (patrol != null)
            {
                patrol.Resume();

                // Direct return toward the patrol circle
                var targetPos = patrol.CurrentPatrolPosition;
                var step = movement.speed * Time.deltaTime * 0.8f;
                transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
            }
            else if (hasHome)
            {
                var step = movement.speed * Time.deltaTime * 0.8f;
                transform.position = Vector3.MoveTowards(
                    transform.position, homePosition, step);
            }
        }

        // ── Enemy detection ─────────────────────────────────────────────

        private HealthComponent FindNearestEnemy(float range)
        {
            HealthComponent nearest = null;
            float nearestDist = range;

            var hits = Physics2D.OverlapCircleAll(transform.position, range);
            foreach (var hit in hits)
            {
                var health = hit.GetComponent<HealthComponent>();
                if (health == null || health == myHealth || health.IsDead)
                    continue;
                if (health.faction == faction)
                    continue;

                // Priority: units (has UnitCombat) over buildings
                float dist = Vector2.Distance(transform.position, health.transform.position);
                if (dist <= nearestDist)
                {
                    // If we already have a candidate and this one is a building
                    // while current is a unit, skip (unit priority)
                    if (nearest != null)
                    {
                        bool currentIsUnit = nearest.GetComponent<UnitCombat>() != null;
                        bool thisIsUnit = health.GetComponent<UnitCombat>() != null;
                        if (currentIsUnit && !thisIsUnit)
                            continue;
                    }
                    nearestDist = dist;
                    nearest = health;
                }
            }

            return nearest;
        }
    }
}
