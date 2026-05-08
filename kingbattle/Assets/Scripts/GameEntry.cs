using Map;
using Units;
using UnityEngine;

/// <summary>
/// Thin scene startup script that bootstraps MVP-01.
///
/// When entering Play Mode, this script automatically:
/// 1. Creates the fixed map data (6 plots, 7 roads).
/// 2. Renders the map visually (coloured plot rectangles + road lines).
/// 3. Activates the test spawner so you can press 1-4 to spawn units.
///
/// This is deliberately thin – it only "wires things up" instead of
/// carrying business logic.
/// </summary>
public class GameEntry : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoInitialize()
    {
        var go = new GameObject("GameEntry");
        go.AddComponent<GameEntry>();
    }

    private void Start()
    {
        // ── Build fixed map data ──
        var mapData = MapData.CreateFixedMap();
        Debug.Log($"[GameEntry] Map loaded: {mapData.Plots.Count} plots, {mapData.Roads.Count} roads.");

        // ── Render map visuals ──
        var rendererObj = new GameObject("MapRenderer");
        var renderer = rendererObj.AddComponent<MapRenderer>();
        renderer.Initialize(mapData);

        // ── Activate test spawner ──
        var spawnerObj = new GameObject("TestUnitSpawner");
        var spawner = spawnerObj.AddComponent<TestUnitSpawner>();
        spawner.Initialize(mapData);

        Debug.Log("[GameEntry] MVP-01 ready. Press 1 (Samurai) / 2 (Elf Archer) / 3 (Soldier) / 4 (Samurai→Crossroads).");
    }
}
