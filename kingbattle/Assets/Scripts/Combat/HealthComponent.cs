using Core;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Health + damage + death for any combatant (units and buildings).
    /// When health reaches zero the GameObject is destroyed.
    /// </summary>
    public class HealthComponent : MonoBehaviour
    {
        [Header("Stats")]
        public float maxHealth = 100f;

        public Faction faction;

        [Header("Visual")]
        [SerializeField] private Color fullHealthColor = Color.white;
        [SerializeField] private Color lowHealthColor = Color.red;
        [SerializeField] private SpriteRenderer targetRenderer;

        public float CurrentHealth { get; private set; }
        public bool IsDead => CurrentHealth <= 0f;

        /// <summary>Called when this object dies (before Destroy).</summary>
        public System.Action<HealthComponent> OnDeath;

        private void Start()
        {
            CurrentHealth = maxHealth;

            if (targetRenderer == null)
                targetRenderer = GetComponent<SpriteRenderer>();
        }

        public void TakeDamage(float amount)
        {
            if (IsDead) return;

            CurrentHealth -= amount;

            // Visual feedback — tint redder as health drops
            if (targetRenderer != null)
            {
                float t = Mathf.Clamp01(CurrentHealth / maxHealth);
                targetRenderer.color = Color.Lerp(lowHealthColor, fullHealthColor, t);
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
