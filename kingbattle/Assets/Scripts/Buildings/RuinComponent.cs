using Core;
using UnityEngine;

namespace Buildings
{
    /// <summary>
    /// Marks a GameObject as a building ruin.
    /// Ruins are purely visual — no attack, no spawn, no health.
    /// Soldiers can patrol around ruins after defeating a building.
    /// </summary>
    public class RuinComponent : MonoBehaviour
    {
        [Header("Appearance")]
        public Color ruinColor = new Color(0.25f, 0.25f, 0.25f);

        private void Start()
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = ruinColor;
        }
    }
}
