using Buildings;
using Combat;
using Core;
using Map;
using Units;
using UnityEngine;

/// <summary>
/// Scene startup script that bootstraps the game.
///
/// Responsibilities:
/// 1. Creates fixed map data.
/// 2. Renders map visually.
/// 3. Creates buildings and links them.
/// 4. Activates test spawner.
/// 5. Initialises HUD, input, enemy AI, and debug shortcuts.
///
/// Does NOT contain game input logic — that is delegated to
/// PlayerInputController and DebugShortcutController.
/// </summary>
public class GameEntry : MonoBehaviour
{
    private MapData mapData;
    private MapRenderer mapRenderer;
    private EnemyPressureController enemyController;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoInitialize()
    {
        var go = new GameObject("GameEntry");
        go.AddComponent<GameEntry>();
    }

    private void Start()
    {
        // ── Build fixed map data ──
        mapData = MapData.CreateFixedMap();
        PlotCaptureService.Reset();
        StrategicDispatchService.Reset();
        MatchResultService.Reset();
        GameStatusService.Reset();
        Debug.Log($"[GameEntry] Map loaded: {mapData.Plots.Count} plots, {mapData.Roads.Count} roads.");

        // ── Render map visuals ──
        var rendererObj = new GameObject("MapRenderer");
        mapRenderer = rendererObj.AddComponent<MapRenderer>();
        mapRenderer.Initialize(mapData);

        // ── Create buildings ──
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        var (playerBaseHp, enemyBaseHp) = SetupBuildings(mapData);
#else
        SetupBuildings(mapData);
#endif

        // ── Test spawner (editor/development only) ──
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        var spawnerObj = new GameObject("TestUnitSpawner");
        var spawner = spawnerObj.AddComponent<TestUnitSpawner>();
        spawner.Initialize(mapData);
#endif

        // ── Player input controller (created before HUD so it can be passed) ──
        var inputObj = new GameObject("PlayerInputController");
        var inputCtrl = inputObj.AddComponent<PlayerInputController>();
        inputCtrl.Initialize(mapData, mapRenderer);

        // ── Game HUD (Canvas/uGUI) ──
        var hudObj = new GameObject("GameHud");
        var hud = hudObj.AddComponent<GameHud>();
        hud.Initialize(mapData, mapRenderer, inputCtrl);

        // ── Enemy pressure controller (MVP-04.1) ──
        var enemyCtrlObj = new GameObject("EnemyPressureController");
        enemyController = enemyCtrlObj.AddComponent<EnemyPressureController>();
        enemyController.Initialize(mapData);

        // ── Debug shortcut controller (editor/development only, MVP-04.5) ──
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        var debugObj = new GameObject("DebugShortcutController");
        var debugCtrl = debugObj.AddComponent<DebugShortcutController>();
        debugCtrl.Initialize(mapData, mapRenderer, enemyController, playerBaseHp, enemyBaseHp);
#endif

        Debug.Log("[GameEntry] Ready. Click source plot → target plot to dispatch.");
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log("[GameEntry] Debug keys active: K/L/E/N/R/T/Y/U/I/O/P/Q.");
#endif
    }

    // ── Building setup ─────────────────────────────────────────────────

    /// <summary>Returns (playerBaseHealth, enemyBaseHealth).</summary>
    private (HealthComponent, HealthComponent) SetupBuildings(MapData mapData)
    {
        var playerBarracks = CreateBuilding("PlayerBase",   mapData, Faction.Player, BuildingType.Barracks);
        var enemyBarracks  = CreateBuilding("EnemyBase",    mapData, Faction.Enemy,  BuildingType.Barracks);
        CreateBuilding("Crossroads",   mapData, Faction.Player, BuildingType.Tower);
        CreateBuilding("EnemyOutpost", mapData, Faction.Enemy,  BuildingType.Tower);
        CreateBuilding("Village",      mapData, Faction.Player, BuildingType.Granary);
        // Farmland intentionally left empty

        // Link Barracks
        if (playerBarracks != null && enemyBarracks != null)
        {
            var pbSpawner = playerBarracks.GetComponent<BarracksSpawner>();
            if (pbSpawner != null)
            {
                pbSpawner.SetEnemyTarget(enemyBarracks.GetComponent<HealthComponent>());
                pbSpawner.rallyPlotId = "Village";
                pbSpawner.pushTargetPlotId = "EnemyBase";
                pbSpawner.rallyThreshold = 3;
            }
        }

        if (enemyBarracks != null && playerBarracks != null)
        {
            var ebSpawner = enemyBarracks.GetComponent<BarracksSpawner>();
            if (ebSpawner != null)
            {
                ebSpawner.SetEnemyTarget(playerBarracks.GetComponent<HealthComponent>());
                ebSpawner.rallyPlotId = "EnemyOutpost";
                ebSpawner.pushTargetPlotId = "PlayerBase";
                ebSpawner.rallyThreshold = 3;
            }
        }

        // Faction defeat handlers
        HealthComponent playerBaseHp = playerBarracks?.GetComponent<HealthComponent>();
        HealthComponent enemyBaseHp = enemyBarracks?.GetComponent<HealthComponent>();

        if (playerBaseHp != null)
        {
            var playerHandlerGo = new GameObject("PlayerDefeatHandler");
            var playerHandler = playerHandlerGo.AddComponent<FactionDefeatHandler>();
            playerHandler.Initialize(Faction.Player);
            playerBaseHp.OnDeath += (hc) => playerHandler.OnMainBaseDefeated();
        }

        if (enemyBaseHp != null)
        {
            var enemyHandlerGo = new GameObject("EnemyDefeatHandler");
            var enemyHandler = enemyHandlerGo.AddComponent<FactionDefeatHandler>();
            enemyHandler.Initialize(Faction.Enemy);
            enemyBaseHp.OnDeath += (hc) => enemyHandler.OnMainBaseDefeated();
        }

        Debug.Log("[GameEntry] Buildings placed, defeat handlers active.");
        return (playerBaseHp, enemyBaseHp);
    }

    private static GameObject CreateBuilding(string plotId, MapData mapData, Faction faction, BuildingType type)
    {
        return BuildingFactory.CreateBuilding(plotId, mapData, faction, type);
    }
}
