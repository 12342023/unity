using Combat;
using Core;
using Map;
using UnityEngine;

namespace Buildings
{
    /// <summary>
    /// Creates and assembles building GameObjects with all required components.
    ///
    /// GameEntry calls this factory instead of duplicating the assembly logic.
    /// Future reconstruction code can also call this factory to rebuild ruins.
    /// </summary>
    public static class BuildingFactory
    {
        /// <summary>
        /// Create a fully assembled building GameObject with visuals, health,
        /// identity, death-handler, collider, and type-specific behaviour.
        /// </summary>
        public static GameObject CreateBuilding(string plotId, MapData mapData, Faction faction, BuildingType type)
        {
            var plot = mapData.GetPlot(plotId);
            if (plot == null) return null;

            var go = new GameObject($"{type}_{faction}_{plotId}");
            go.transform.position = new Vector3(plot.worldPosition.x, plot.worldPosition.y, -0.05f);

            // ── Visual ──
            var sr = go.AddComponent<SpriteRenderer>();
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 1f);
            sr.sortingOrder = 2;

            switch (type)
            {
                case BuildingType.Barracks:
                    sr.color = faction == Faction.Player
                        ? new Color(0.2f, 0.5f, 1.0f)
                        : new Color(1.0f, 0.3f, 0.3f);
                    go.transform.localScale = Vector3.one * 0.8f;
                    break;

                case BuildingType.Tower:
                    sr.color = faction == Faction.Player
                        ? new Color(0.3f, 0.7f, 1.0f)
                        : new Color(1.0f, 0.5f, 0.2f);
                    go.transform.localScale = new Vector3(0.6f, 0.9f, 1f);
                    break;

                case BuildingType.Granary:
                    sr.color = new Color(0.9f, 0.85f, 0.3f);
                    go.transform.localScale = Vector3.one * 0.7f;
                    break;
            }

            // ── Health ──
            var health = go.AddComponent<HealthComponent>();
            health.maxHealth = type == BuildingType.Tower ? 80f : 50f;
            health.faction = faction;

            // ── Building identity (for future reconstruction) ──
            var identity = go.AddComponent<BuildingIdentity>();
            identity.plotId = plotId;
            identity.buildingType = type;
            identity.faction = faction;

            // ── Building death → ruin ──
            go.AddComponent<BuildingDeathHandler>();

            // ── Collider (needed for Tower's physics scan) ──
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = Vector2.one * 0.6f;

            // ── Type-specific component ──
            switch (type)
            {
                case BuildingType.Barracks:
                    var barracks = go.AddComponent<BarracksSpawner>();
                    barracks.Initialize(mapData, faction, plotId);
                    break;

                case BuildingType.Tower:
                    var tower = go.AddComponent<TowerAttack>();
                    tower.faction = faction;
                    tower.damage = GameBalanceConfig.TowerDamage;
                    tower.attackRange = GameBalanceConfig.TowerAttackRange;
                    tower.attackInterval = GameBalanceConfig.TowerAttackInterval;
                    break;
                // Granary: no special component yet (just visual + health)
            }

            // ── Register in runtime registry ──
            BuildingRegistry.Register(go, faction);

            return go;
        }
    }
}
