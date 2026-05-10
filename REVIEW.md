# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
d76e602 feat: O shortcut dispatches from frontier plot to adjacent neutral
```

结论：**MVP-03.16 代码审查通过，允许进入 MVP-03.17**。

说明：O 临时测试快捷键已能复用 `StrategicConnectionService`、`RoadPathFinder`、`StrategicDispatchService` 与 `PlotCaptureService`，从第一个 Player-owned frontier plot 派兵到第一个相邻 Neutral，并在到达后触发 Neutral -> Player 占领。

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞问题。
```

### 已确认

- `GameEntry` 新增 O 临时测试入口。
- O 使用 `StrategicConnectionService.GetPlayerFrontierPlots(mapData)` 选择 source。
- O 使用 source 的第一个 `connectableNeutralPlots` 作为 target。
- O 使用 `RoadPathFinder.FindPath(mapData, source.plotId, targetPlotId)` 生成 path。
- O 将 path 转成 world waypoints 后调用 `StrategicDispatchService.DispatchToPlot(...)`。
- 到达后的实际占领仍由 `PlotCaptureService.TryCapture(...)` 执行。
- `PlotCaptureService` 的 Player-only、Neutral-only、no-main-base 规则未被放宽。
- K / L / R / T / Y / U / I 行为未被改动。
- `MapData.CreateFixedMap()` 未被改动。
- 本轮没有新增 `.meta`。
- `kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍未提交。

### 观察

O 当前逻辑可用，但 `GameEntry.Update()` 继续承载 frontier 选择、target 选择、路径转换和派兵编排。下一步应做小范围架构收口，把这段 O 编排下沉到独立服务，避免后续正式 UI、微信小程序、macOS、Android 移植时把输入层和业务规则绑死在 `GameEntry`。

### 说明

`git show --check HEAD` 本轮未发现问题。`kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍是未跟踪 Unity Editor 生成文件，不应提交。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.16 代码审查通过。

进入 MVP-03.17：整理已占领地块的扩张编排服务。

背景：
- O 已能从第一个 Player-owned frontier plot 派兵到第一个相邻 Neutral。
- 现在 O 的选择 source、选择 target、算路、转 waypoints、派兵编排都写在 GameEntry.Update() 中。
- 为后续正式 UI 和跨平台移植，需要让 GameEntry 更薄，把业务编排放进服务。

本轮目标：
- 新增 `StrategicExpansionService` 或等价小服务。
- 将 O 当前的扩张编排下沉到这个服务。
- O 外部行为保持不变。

允许：
- 新服务建议放在 `kingbattle/Assets/Scripts/Combat/`。
- 新服务可以提供一个方法，例如：
  `TryDispatchFirstFrontierToNeutral(MapData mapData, MapRenderer mapRenderer, out ExpansionDispatchResult result)`
- 方法内部复用：
  - `StrategicConnectionService.GetPlayerFrontierPlots(mapData)`
  - `RoadPathFinder.FindPath(mapData, sourcePlotId, targetPlotId)`
  - `StrategicDispatchService.DispatchToPlot(...)`
- result 类型保持小而明确，可包含：
  - success
  - sourcePlotId
  - targetPlotId
  - dispatchedCount
  - message
- GameEntry 的 O 分支只做：
  1. 调用新服务。
  2. 打印 result.message。
- 更新 WORKLOG.md。

必须保持：
- K / L / R / T / Y / U / I / O 外部行为不变。
- O 仍只通过现有 PlotCaptureService 占领 Neutral。
- PlotCaptureService 的 Player-only / no-main-base / Neutral-only 规则不变。
- StrategicDispatchService 行为不变，不要重构它。
- 不新增长期静态游戏状态。
- 不修改 ProjectSettings。
- 不提交 kingbattle/ProjectSettings/SceneTemplateSettings.json。

禁止：
- 不做正式选择目标 UI。
- 不做自动扩张。
- 不做多 source / 多 target 策略。
- 不做资源、升级、区域奖励、传送阵、AI。
- 不做占领进度条。
- 不改变 MapData 初始地图。
- 不重构 UnitCombat。

完成后更新 WORKLOG.md，说明修改文件、O/I/Y/U/K/T/R/L Play Mode 验证结果、是否新增 .meta、是否修改 ProjectSettings，并 commit / push。
```
