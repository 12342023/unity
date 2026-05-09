using Combat;
using Core;
using UnityEngine;

namespace Buildings
{
    /// <summary>
    /// Attached to every building. Hooks the building's HealthComponent.OnDeath
    /// and spawns a ruin at the building's position when it dies.
    ///
    /// This decouples ruin logic from GameEntry — GameEntry only creates the
    /// component, it no longer carries the SpawnRuin method.
    /// </summary>
    [RequireComponent(typeof(HealthComponent))]
    public class BuildingDeathHandler : MonoBehaviour
    {
        private void Start()
        {
            var health = GetComponent<HealthComponent>();
            var faction = health.faction;
            var position = transform.position;

            health.OnDeath += (hc) => SpawnRuin(position, faction);
        }

        /// <summary>Spawn a ruin GameObject at the given position.
        /// Ruins are purely visual — no attack, no spawn, no health.</summary>
        private static void SpawnRuin(Vector3 position, Faction faction)
        {
            var ruin = new GameObject($"Ruin_{faction}_{Time.frameCount}");
            ruin.transform.position = new Vector3(position.x, position.y, -0.04f);

            var sr = ruin.AddComponent<SpriteRenderer>();
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 1f);
            sr.sortingOrder = 1;

            // Darkened tint: hint of faction colour but mostly gray
            Color ruinColor = faction == Faction.Player
                ? new Color(0.2f, 0.2f, 0.35f)
                : new Color(0.35f, 0.2f, 0.2f);
            sr.color = ruinColor;
            ruin.transform.localScale = Vector3.one * 0.7f;

            var col = ruin.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = Vector2.one * 0.6f;

            ruin.AddComponent<RuinComponent>();

            Debug.Log($"[BuildingDeathHandler] Ruin spawned at {position}");
        }
    }
}
