# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
966a408 feat: StrategicConnectionService + I shortcut for frontier neutral plots
```

结论：**MVP-03.15 代码审查通过，允许进入 MVP-03.16**。

说明：`StrategicConnectionService` 已提供从 Player-owned non-main-base plot 查询相邻 Neutral plot 的最小数据能力；`GameEntry` 的 I 临时测试快捷键只打印结果，不派兵、不占领、不改变归属。

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞问题。
```

### 已确认

- 新增 `kingbattle/Assets/Scripts/Combat/StrategicConnectionService.cs`。
- 新增脚本 `.meta` 已提交。
- `GetPlayerFrontierPlots(mapData)` 只返回 Player-owned 且非 main base 的前线 plot。
- 只收集相邻 `Faction.Neutral` plotId。
- 查询方法不改变任何 `plot.faction`。
- I 只打印 frontier 查询结果，不派兵、不占领。
- K / L / R / T / Y / U 行为未被改动。
- `kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍未提交。

### 文档更正

Claude 的 WORKLOG 写到 Crossroads 被占领后只连接 `Farmland`，并称 `Village=Player`。按当前 `MapData.CreateFixedMap()` 和当前代码：

```text
Village: Neutral
Farmland: Neutral
Crossroads: U 到达后变 Player
EnemyOutpost: Enemy
EnemyBase: Enemy / main base ruin 后 mapData faction 仍不是 Neutral
```

因此 Crossroads 被占领后，I 的合理输出应是：

```text
Crossroads can connect to: Village, Farmland
```

除非后续另有代码显式改变 `Village.faction`，否则不要把 Village 写成 Player。

### 说明

`git show --check HEAD` 仍会报告 Unity `.meta` 空值字段的尾随空格。这是当前仓库 Unity `.meta` 文件的一贯格式，本轮不作为阻塞问题。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.15 代码审查通过。

进入 MVP-03.16：从已占领 plot 临时派兵到相邻 Neutral。

背景：
- U 仍只负责从 main base ruin 派兵到第一个相邻 Neutral。
- I 已能打印 Player-owned frontier plot 的相邻 Neutral。
- Crossroads 被 U 占领后，应能查询到 Village / Farmland。
- 本轮仍不做正式 UI，只做临时测试入口。

本轮目标：
- 新增一个临时快捷键，例如 O。
- O 从第一个 Player-owned frontier plot 派附近 Player 士兵到它的第一个相邻 Neutral plot。
- 到达后复用现有 capture 流程，让目标 Neutral 变 Player 并刷新颜色。

允许：
- 复用 `StrategicConnectionService.GetPlayerFrontierPlots(mapData)`。
- 复用 `RoadPathFinder.FindPath(mapData, sourcePlotId, targetPlotId)`。
- 复用 `StrategicDispatchService.DispatchToPlot(...)`。
- O 的高层流程：
  1. 获取 frontier plots。
  2. 选择第一个 frontier。
  3. 选择它的第一个 connectableNeutralPlots 目标。
  4. 从 source plot 到 target plot 算 path。
  5. 转换 waypoints。
  6. 调用 StrategicDispatchService.DispatchToPlot(sourcePlot.worldPosition, waypoints, targetPlotId, mapData, mapRenderer)。
  7. 输出日志说明派出多少士兵、source -> target。
- 保持 I 只打印，不派兵。
- 保持 U 只从 main base ruin 派兵。
- 新增逻辑尽量薄，不要做正式 UI。
- 更新 WORKLOG.md。

必须保持：
- O 不改变 Enemy plot。
- O 不改变 main base plot。
- O 只通过现有 PlotCaptureService 占领 Neutral。
- PlotCaptureService 的 Player-only / no-main-base / Neutral-only 规则不变。
- StrategicDispatchService 行为不变。
- K / L / R / T / Y / U / I 行为不变。
- 不修改 ProjectSettings。
- 不提交 kingbattle/ProjectSettings/SceneTemplateSettings.json。

禁止：
- 不做正式选择目标 UI。
- 不做自动扩张。
- 不做资源、升级、区域奖励、传送阵、AI。
- 不做占领进度条。
- 不改变 MapData 初始地图。
- 不重构 UnitCombat。

完成后更新 WORKLOG.md，说明修改文件、O/I/Y/U/K/T/R/L Play Mode 验证结果、是否新增 .meta、是否修改 ProjectSettings，并 commit / push。
```
