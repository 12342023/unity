using Map;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Periodically triggers enemy attacks via EnemyAttackCommandService
    /// to create minimum player pressure. Stops after Victory/Defeat.
    ///
    /// Controlled entirely by timers — no UI or platform dependency.
    /// Initiated by GameEntry; E shortcut calls TriggerAttack().
    /// </summary>
    public class EnemyPressureController : MonoBehaviour
    {
        private MapData mapData;
        private float timer;
        private float cooldown = 10f; // seconds until next attack
        private bool isFirstAttack = true;

        public void Initialize(MapData data)
        {
            mapData = data;
            // First attack (from GameBalanceConfig)
            cooldown = Random.Range(GameBalanceConfig.EnemyFirstAttackMin, GameBalanceConfig.EnemyFirstAttackMax);
            timer = 0f;
            Debug.Log($"[EnemyPressureController] Started. First attack in ~{cooldown:F1}s.");
        }

        private void Update()
        {
            GameStatusService.TimeUntilNextEnemyAttack = Mathf.Max(0f, cooldown - timer);

            // Stop after match decided
            if (MatchResultService.CurrentResult != MatchResult.None)
                return;

            if (mapData == null) return;

            timer += Time.deltaTime;
            if (timer >= cooldown)
            {
                timer = 0f;
                TriggerAttack();
                // Subsequent attacks (from GameBalanceConfig)
                cooldown = Random.Range(GameBalanceConfig.EnemyRepeatAttackMin, GameBalanceConfig.EnemyRepeatAttackMax);
                isFirstAttack = false;
            }
        }

        /// <summary>Immediately trigger an enemy attack. Called by timer
        /// or by the E debug shortcut.</summary>
        public void TriggerAttack()
        {
            if (mapData == null)
            {
                Debug.Log("[EnemyPressureController] TriggerAttack skipped: MapData is null.");
                return;
            }

            if (MatchResultService.CurrentResult != MatchResult.None)
            {
                Debug.Log("[EnemyPressureController] TriggerAttack skipped: match already decided.");
                return;
            }

            var result = EnemyAttackCommandService.DispatchAttackToBestTarget(mapData);
            if (!result.success)
            {
                string label = isFirstAttack ? "First" : "Periodic";
                Debug.Log($"[EnemyPressureController] {label} enemy attack: {result.message}");
            }
            // On success, DispatchAttack already logs the result.
        }
    }
}
