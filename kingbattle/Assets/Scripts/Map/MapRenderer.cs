using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Map
{
    /// <summary>
    /// Creates visual representations of plots and roads at runtime.
    /// All visuals are generated programmatically – no scene editing required.
    /// Also provides plot hit-testing and highlighting for PlayerInputController.
    /// </summary>
    public class MapRenderer : MonoBehaviour
    {
        [Header("Colours")]
        [SerializeField] private Color playerColor = new Color(0.2f, 0.4f, 0.9f);
        [SerializeField] private Color enemyColor = new Color(0.9f, 0.2f, 0.2f);
        [SerializeField] private Color neutralColor = new Color(0.6f, 0.6f, 0.6f);
        [SerializeField] private Color baseHighlightColor = new Color(1f, 0.8f, 0.2f);
        [SerializeField] private Color roadColor = new Color(0.5f, 0.5f, 0.5f);

        [Header("Highlight colours (MVP-04.5)")]
        [SerializeField] private Color selectedSourceColor = new Color(0.3f, 0.7f, 1.0f, 1f);
        [SerializeField] private Color validTargetColor = new Color(0.8f, 0.9f, 0.2f, 1f);

        [Header("Sizes")]
        [SerializeField] private Vector2 smallPlotSize = new Vector2(1.5f, 1.5f);
        [SerializeField] private Vector2 mediumPlotSize = new Vector2(2.5f, 2.5f);
        [SerializeField] private Vector2 largePlotSize = new Vector2(3.5f, 3.5f);

        private Sprite whiteSprite;
        private Sprite baseBorderSprite;

        // Plot lookup
        private readonly Dictionary<string, SpriteRenderer> plotRenderers = new();

        // ── Initialisation ──────────────────────────────────────────────

        public void Initialize(MapData mapData)
        {
            GenerateWhiteSprite();
            GenerateBaseBorderSprite();

            foreach (var plot in mapData.Plots)
                CreatePlotVisual(plot);

            foreach (var road in mapData.Roads)
                CreateRoadVisual(road, mapData);
        }

        // ── Plot hit-testing (MVP-04.5) ────────────────────────────────

        /// <summary>
        /// Find the plotId whose world position is closest to <paramref name="worldPos"/>
        /// within <paramref name="threshold"/> distance. Returns null if nothing is close enough.
        /// </summary>
        public string GetPlotAtWorldPosition(Vector3 worldPos, MapData mapData, float threshold = 1.5f)
        {
            string closestId = null;
            float closestDist = threshold;

            foreach (var plot in mapData.Plots)
            {
                Vector3 pPos = new Vector3(plot.worldPosition.x, plot.worldPosition.y, 0f);
                float dist = Vector3.Distance(worldPos, pPos);
                if (dist <= closestDist)
                {
                    closestDist = dist;
                    closestId = plot.plotId;
                }
            }
            return closestId;
        }

        // ── Highlighting (MVP-04.5) ────────────────────────────────────

        /// <summary>Apply a temporary highlight colour to a plot visual.
        /// Does not change the underlying PlotData.faction.</summary>
        public void SetPlotHighlight(string plotId, Color color)
        {
            if (plotRenderers.TryGetValue(plotId, out var sr))
            {
                sr.color = color;
                sr.sortingOrder = 5; // on top of normal plots
            }
        }

        /// <summary>Revert a single plot to its faction's normal colour.</summary>
        public void ClearPlotHighlight(string plotId, MapData mapData)
        {
            if (plotRenderers.TryGetValue(plotId, out var sr))
            {
                var plot = mapData?.GetPlot(plotId);
                if (plot != null)
                {
                    sr.color = FactionToColor(plot.faction);
                    sr.sortingOrder = 0;
                }
            }
        }

        /// <summary>Clear all plot highlights (revert to faction colours).</summary>
        public void ClearAllHighlights(MapData mapData)
        {
            foreach (var kvp in plotRenderers)
            {
                var plot = mapData?.GetPlot(kvp.Key);
                if (plot != null)
                {
                    kvp.Value.color = FactionToColor(plot.faction);
                    kvp.Value.sortingOrder = 0;
                }
            }
        }

        // ── Sprite generation ───────────────────────────────────────────

        private void GenerateWhiteSprite()
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            whiteSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        }

        private void GenerateBaseBorderSprite()
        {
            int size = 32;
            var tex = new Texture2D(size, size);
            int half = size / 2;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int dx = Mathf.Abs(x - half);
                    int dy = Mathf.Abs(y - half);
                    int edgeDist = Mathf.Max(dx, dy);

                    bool isBorder = edgeDist >= half - 2 && edgeDist <= half;
                    tex.SetPixel(x, y, isBorder ? Color.white : Color.clear);
                }
            }
            tex.Apply();
            baseBorderSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 16f);
        }

        // ── Plot visuals ────────────────────────────────────────────────

        private void CreatePlotVisual(PlotData plot)
        {
            var go = new GameObject($"Plot_{plot.plotId}");
            go.transform.SetParent(transform);
            go.transform.position = new Vector3(plot.worldPosition.x, plot.worldPosition.y, 0f);

            // ── Main coloured rectangle ──
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = whiteSprite;
            sr.color = FactionToColor(plot.faction);
            sr.sortingOrder = 0;

            var size = PlotSizeToVector(plot.size);
            go.transform.localScale = new Vector3(size.x, size.y, 1f);

            plotRenderers[plot.plotId] = sr;

            // ── Base highlight border ──
            if (plot.isMainBase)
            {
                var border = new GameObject("BaseBorder");
                border.transform.SetParent(go.transform);
                border.transform.localPosition = Vector3.zero;
                border.transform.localScale = Vector3.one * 1.15f;

                var borderSr = border.AddComponent<SpriteRenderer>();
                borderSr.sprite = baseBorderSprite;
                borderSr.color = baseHighlightColor;
                borderSr.sortingOrder = 1;
            }

            // ── Build-slot markers ──
            for (int i = 0; i < plot.buildSlotCount; i++)
            {
                var slot = new GameObject($"Slot_{i}");
                slot.transform.SetParent(go.transform);

                float spacing = 0.4f;
                float totalWidth = (plot.buildSlotCount - 1) * spacing;
                float slotX = -totalWidth / 2f + i * spacing;
                float slotY = -size.y / 2f + 0.3f;
                slot.transform.localPosition = new Vector3(slotX, slotY, 0f);

                var slotSr = slot.AddComponent<SpriteRenderer>();
                slotSr.sprite = whiteSprite;
                slotSr.color = new Color(1f, 1f, 1f, 0.4f);
                slotSr.transform.localScale = new Vector3(0.12f, 0.12f, 1f);
                slotSr.sortingOrder = 2;
            }
        }

        // ── Road visuals ────────────────────────────────────────────────

        private void CreateRoadVisual(RoadConnection road, MapData mapData)
        {
            var from = mapData.GetPlot(road.fromPlotId);
            var to = mapData.GetPlot(road.toPlotId);
            if (from == null || to == null) return;

            var go = new GameObject($"Road_{road.fromPlotId}_{road.toPlotId}");
            go.transform.SetParent(transform);
            go.transform.position = Vector3.zero;

            var lr = go.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.SetPosition(0, new Vector3(from.worldPosition.x, from.worldPosition.y, -0.1f));
            lr.SetPosition(1, new Vector3(to.worldPosition.x, to.worldPosition.y, -0.1f));
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.material = Resources.Load<Material>("Sprites-Default") ?? CreateDefaultMaterial();
            lr.startColor = roadColor;
            lr.endColor = roadColor;
            lr.sortingOrder = -1;
            lr.textureMode = LineTextureMode.Tile;
        }

        // ── Runtime plot colour refresh (used after capture) ─────────

        public void RefreshPlotColor(string plotId, MapData mapData)
        {
            var plot = mapData?.GetPlot(plotId);
            if (plot == null) return;

            if (plotRenderers.TryGetValue(plotId, out var sr))
            {
                sr.color = FactionToColor(plot.faction);
                sr.sortingOrder = 0;
            }
        }

        // ── Helpers ─────────────────────────────────────────────────────

        private Color FactionToColor(Faction f) => f switch
        {
            Faction.Player => playerColor,
            Faction.Enemy => enemyColor,
            Faction.Neutral => neutralColor,
            _ => neutralColor
        };

        private Vector2 PlotSizeToVector(PlotSize s) => s switch
        {
            PlotSize.Small => smallPlotSize,
            PlotSize.Medium => mediumPlotSize,
            PlotSize.Large => largePlotSize,
            _ => smallPlotSize
        };

        private static Material CreateDefaultMaterial()
        {
            var mat = new Material(Shader.Find("Sprites/Default"));
            return mat;
        }
    }
}
