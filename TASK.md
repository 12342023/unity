# TASK.md

## 当前任务

发布 MVP-03.16：从已占领 plot 临时派兵到相邻 Neutral。

Codex 当前仍作为 Tech Lead / Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
966a408 feat: StrategicConnectionService + I shortcut for frontier neutral plots
```

Codex Review 结论：

```text
MVP-03.15 代码审查通过；允许进入 MVP-03.16
```

## 本轮目标

只做临时测试入口，不做正式 UI。

目标：

```text
Crossroads 被占领为 Player 后，可以从 Crossroads 派兵到它相邻的 Neutral plot。
```

## 当前工作区注意事项

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件仍是未跟踪 Unity Editor 生成文件。除非用户明确批准，否则不要提交。

## 已完成基础能力

- U 可以从 main base ruin 派兵去第一个相邻 Neutral plot。
- U 到达 Crossroads 后可将 Crossroads 从 Neutral 改为 Player。
- Crossroads 颜色会刷新为 Player 颜色。
- Crossroads 变 Player 后，I 可以查询它相邻的 Neutral plot。
- U 派兵与 capture handler 管理已下沉到 `StrategicDispatchService`。
- `StrategicConnectionService` 已提供 Player-owned frontier 查询。

## 文档更正

当前 `MapData.CreateFixedMap()` 中：

```text
Village = Neutral
Farmland = Neutral
```

所以 Crossroads 被占领后，I 的合理输出应包含：

```text
Village, Farmland
```

不要把 Village 写成 Player，除非本轮代码显式改变了 `Village.faction`。

## MVP-03.16 允许范围

- 可以在 `GameEntry` 增加临时测试快捷键，例如 `O`。
- `O` 的行为：
  - 调用 `StrategicConnectionService.GetPlayerFrontierPlots(mapData)`。
  - 选择第一个 frontier plot。
  - 选择第一个相邻 Neutral target plot。
  - 使用 `RoadPathFinder.FindPath(mapData, sourcePlotId, targetPlotId)` 算路径。
  - 将 path 转成 world waypoints。
  - 调用 `StrategicDispatchService.DispatchToPlot(sourcePlot.worldPosition, waypoints, targetPlotId, mapData, mapRenderer)`。
  - 输出日志说明派出多少士兵、source -> target。
- 可以小幅整理辅助代码，但不要做大重构。
- 更新 `WORKLOG.md`。

## 禁止范围

- 不改变 K / L / R / T / Y / U / I 行为。
- 不做正式派兵 UI。
- 不做自动扩张。
- 不做占领进度条。
- 不做敌方反夺。
- 不做资源、升级、区域奖励、传送阵、AI。
- 不改变 `MapData.CreateFixedMap()`。
- 不重构 `UnitCombat`。
- 不修改 `ProjectSettings`。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。

## 验收标准

- Play Mode：
  - K -> T -> Y -> U。
  - U 到达 Crossroads 后，Crossroads 变 Player。
  - I 能打印 Crossroads 可连接的 Neutral，例如 Village / Farmland。
  - O 能从 Crossroads 附近派 Player 士兵到第一个 Neutral target。
  - O 到达后 target 从 Neutral 变 Player，并刷新颜色。
- O 不应占领 Enemy plot。
- O 不应占领 main base plot。
- I 不派兵、不占领。
- Y / U 原有行为不变。
- R 仍跳过大本营废墟。
- K / L 清场行为不变。
- Console 无明显错误。
- 仍未修改或提交 `ProjectSettings`。
