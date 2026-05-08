using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Map
{
    /// <summary>
    /// Creates visual representations of plots and roads at runtime.
    /// All visuals are generated programmatically – no scene editing required.
    /// </summary>
    public class MapRenderer : MonoBehaviour
    {
        [Header("Colours")]
        [SerializeField] private Color playerColor = new Color(0.2f, 0.4f, 0.9f);
        [SerializeField] private Color enemyColor = new Color(0.9f, 0.2f, 0.2f);
        [SerializeField] private Color neutralColor = new Color(0.6f, 0.6f, 0.6f);
        [SerializeField] private Color baseHighlightColor = new Color(1f, 0.8f, 0.2f);
        [SerializeField] private Color roadColor = new Color(0.5f, 0.5f, 0.5f);

        [Header("Sizes")]
        [SerializeField] private Vector2 smallPlotSize = new Vector2(1.5f, 1.5f);
        [SerializeField] private Vector2 mediumPlotSize = new Vector2(2.5f, 2.5f);
        [SerializeField] private Vector2 largePlotSize = new Vector2(3.5f, 3.5f);

        private Sprite whiteSprite;
        private Sprite baseBorderSprite;

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

                    // Thick border (2px) on a transparent square
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

            // ── Base highlight border ──
            if (plot.isMainBase)
            {
                var border = new GameObject("BaseBorder");
                border.transform.SetParent(go.transform);
                border.transform.localPosition = Vector3.zero;
                border.transform.localScale = Vector3.one * 1.15f; // slightly larger

                var borderSr = border.AddComponent<SpriteRenderer>();
                borderSr.sprite = baseBorderSprite;
                borderSr.color = baseHighlightColor;
                borderSr.sortingOrder = 1;
            }

            // ── Build-slot markers (small dots) ──
            for (int i = 0; i < plot.buildSlotCount; i++)
            {
                var slot = new GameObject($"Slot_{i}");
                slot.transform.SetParent(go.transform);

                // Arrange slots in a row at the bottom of the plot
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

            // ── Name label (just a sprite alt – no TextMeshPro dependency) ──
            // For MVP we skip text labels to keep dependencies minimal.
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
