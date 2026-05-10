# TASK.md

## 当前任务

发布 MVP-03.17：整理已占领地块的扩张编排服务。

Codex 当前仍作为 Tech Lead / Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
d76e602 feat: O shortcut dispatches from frontier plot to adjacent neutral
```

Codex Review 结论：

```text
MVP-03.16 代码审查通过；允许进入 MVP-03.17
```

## 本轮目标

不做新玩法，只做架构收口：把 O 的扩张编排从 `GameEntry` 下沉到清晰的小服务中。

目标：

```text
O 的外部行为保持不变，但 GameEntry 不再直接承担 frontier 选择、target 选择、路径生成、派兵编排。
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
- O 可以从第一个 Player-owned frontier plot 派兵到第一个相邻 Neutral。
- O 到达目标后复用现有 capture 流程，将目标 Neutral 改为 Player。
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

## MVP-03.17 允许范围

- 新增 `StrategicExpansionService` 或等价小服务，建议放在 `kingbattle/Assets/Scripts/Combat/`。
- 服务负责 O 当前已有的高层编排：
  - 调用 `StrategicConnectionService.GetPlayerFrontierPlots(mapData)`。
  - 选择第一个 frontier plot。
  - 选择第一个相邻 Neutral target plot。
  - 使用 `RoadPathFinder.FindPath(mapData, sourcePlotId, targetPlotId)` 算路径。
  - 将 path 转成 world waypoints。
  - 调用 `StrategicDispatchService.DispatchToPlot(sourcePlot.worldPosition, waypoints, targetPlotId, mapData, mapRenderer)`。
- `GameEntry` 的 O 分支只负责接收按键、调用新服务、打印结果。
- 可以定义一个很小的 result 类型，返回：
  - 是否成功派兵。
  - sourcePlotId。
  - targetPlotId。
  - dispatchedCount。
  - 失败原因或日志 message。
- 更新 `WORKLOG.md`。

## 禁止范围

- 不改变 K / L / R / T / Y / U / I / O 的外部行为。
- 不做正式派兵 UI。
- 不做自动扩张。
- 不做多目标选择策略。
- 不做占领进度条。
- 不做敌方反夺。
- 不做资源、升级、区域奖励、传送阵、AI。
- 不改变 `MapData.CreateFixedMap()`。
- 不重构 `UnitCombat`。
- 不重构 `StrategicDispatchService`。
- 不重构 `PlotCaptureService`。
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
- `GameEntry` 的 O 分支明显变薄，只调用扩张服务并打印结果。
- 新服务不持有长期静态游戏状态。
- O 不应占领 Enemy plot。
- O 不应占领 main base plot。
- I 不派兵、不占领。
- Y / U 原有行为不变。
- R 仍跳过大本营废墟。
- K / L 清场行为不变。
- Console 无明显错误。
- 仍未修改或提交 `ProjectSettings`。
