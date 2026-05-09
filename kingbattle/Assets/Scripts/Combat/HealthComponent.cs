using Core;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Health + damage + death for any combatant (units and buildings).
    /// When health reaches zero the GameObject is destroyed.
    /// Uses the SpriteRenderer's original color as the full-health colour,
    /// so faction colours (blue Player / red Enemy) are preserved on damage.
    /// </summary>
    public class HealthComponent : MonoBehaviour
    {
        [Header("Stats")]
        public float maxHealth = 100f;

        public Faction faction;

        [Header("Visual")]
        [SerializeField] private Color lowHealthColor = Color.red;

        private SpriteRenderer targetRenderer;
        private Color originalColor; // captured in Start — used as fullHealthColor

        public float CurrentHealth { get; private set; }
        public bool IsDead => CurrentHealth <= 0f;

        /// <summary>Called when this object dies (before Destroy).</summary>
        public System.Action<HealthComponent> OnDeath;

        private void Start()
        {
            CurrentHealth = maxHealth;

            if (targetRenderer == null)
                targetRenderer = GetComponent<SpriteRenderer>();

            // Capture the original colour so damage-tinting doesn't wash out
            // faction colours (blue Player, red Enemy, etc.).
            if (targetRenderer != null)
                originalColor = targetRenderer.color;
        }

        /// <summary>Instant kill without visual feedback (used for faction-defeat cleanup).
        /// Triggers OnDeath then destroys the GameObject.</summary>
        public void Kill()
        {
            if (IsDead) return;
            CurrentHealth = 0f;
            OnDeath?.Invoke(this);
            Destroy(gameObject);
        }

        public void TakeDamage(float amount)
        {
            if (IsDead) return;

            CurrentHealth -= amount;

            // Visual feedback — tint toward lowHealthColor, preserving original colour
            if (targetRenderer != null)
            {
                float t = Mathf.Clamp01(CurrentHealth / maxHealth);
                targetRenderer.color = Color.Lerp(lowHealthColor, originalColor, t);
            }

            if (CurrentHealth <= 0f)
            {
                CurrentHealth = 0f;
                OnDeath?.Invoke(this);
                Destroy(gameObject);
            }
        }
    }
}
