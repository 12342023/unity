# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
404b86c feat: PlotCaptureRequirementService + P shortcut prints capture requirements
569c321 feat: show dispatched versus required capture count
```

结论：**MVP-03.19 代码审查通过，允许进入 MVP-03.20**。

说明：占领需求数据层、P 验证日志、O 的 dispatched / required 预览均已完成；当前实际 capture 判定仍未改变。

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞问题。
```

### 已确认

- 新增 `kingbattle/Assets/Scripts/Combat/PlotCaptureRequirementService.cs`。
- 新增脚本 `.meta` 已提交。
- `GetRequiredSoldierCount(PlotSize)` 返回 Small=1、Medium=2、Large=3。
- `GetRequiredSoldierCount(PlotData)` 对 null 返回 0。
- `GameEntry` 新增 P 临时日志快捷键，打印每个 plot 的占领需求。
- `StrategicExpansionService.ExpansionResult` 已补充 `requiredCount`。
- O 日志已显示 dispatched / required 预览，例如 `Dispatched 3/2 soldiers...`。
- 当前 `PlotCaptureService` 的到达即占领逻辑未被改变。
- K / L / R / T / Y / U / I / O 外部行为未被改动。
- `MapData.CreateFixedMap()` 未被改动。
- `kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍未提交。

### 观察

需求数据已经可用，下一步可以把它接入实际 U/O capture 判定。为加快进度，本轮可以打包处理：派兵服务支持需求判定、U/O 都传入需求值、日志提示不足兵力，但仍不做 UI、进度条、资源系统。

### 说明

`git show --check HEAD` 本轮未发现问题。`kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍是未跟踪 Unity Editor 生成文件，不应提交。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.19 代码审查通过。

进入 MVP-03.20：占领需求接入批量任务。

背景：
- `PlotCaptureRequirementService` 已提供 Small/Medium/Large 的需求值。
- O 日志已能显示 dispatched / required。
- 当前真正占领仍由 `StrategicDispatchService` 到达后直接调用 `PlotCaptureService.TryCapture(...)`。
- 本轮开始把需求值接入实际 U/O 捕获判定。

本轮目标：
- U/O 派兵时，如果派出的 Player 士兵数量低于目标 plot 需求，则到达后不占领。
- 如果派兵数量满足需求，则保持现有到达后占领行为。
- 增加清晰日志，便于 Play Mode 验证。

允许：
任务 A：派兵服务支持 capture requirement
- 在 `StrategicDispatchService.DispatchToPlot(...)` 增加可选参数，例如 `requiredSoldierCount = 1`。
- 在注册 arrival capture handler 时，只有 `dispatchedCount >= requiredSoldierCount` 才调用 `PlotCaptureService.TryCapture(...)`。
- 如果不足，打印清晰日志，例如 `Capture blocked: dispatched 1/2 to Crossroads`。
- 必须保持 handler 清理逻辑不泄漏、不重复触发。

任务 B：O 接入需求判定
- `StrategicExpansionService.ExpandNext(...)` 把目标 plot 的 required count 传给 `StrategicDispatchService.DispatchToPlot(...)`。
- `ExpansionResult` 补充或保留：
  - dispatchedCount
  - requiredCount
  - hasEnoughDispatchedSoldiers
- O 日志继续显示 dispatched / required。

任务 C：U 接入需求判定
- `GameEntry` 的 U 分支也查询 target plot required count，并传给 `StrategicDispatchService.DispatchToPlot(...)`。
- U 日志显示 dispatched / required。

任务 D：验证日志
- P 继续可打印每个 plot 的需求。
- I/O/U 现有测试入口保留。
- 更新 WORKLOG.md。

必须保持：
- K / L / R / T / Y / U / I / O 外部行为不变。
- 除“兵力不足时不占领”外，不改变当前 Neutral -> Player 捕获流程。
- 不改变 `MapData.CreateFixedMap()`。
- 不修改 `PlotCaptureService` 的 Player-only / no-main-base / Neutral-only 规则。
- 不引入长期静态游戏状态。
- 不修改 ProjectSettings。
- 不提交 kingbattle/ProjectSettings/SceneTemplateSettings.json。

禁止：
- 不做正式选择目标 UI。
- 不做自动扩张。
- 不做资源、升级、区域奖励、传送阵、AI。
- 不做占领进度条。
- 不重构 UnitCombat。
- 不重构无关建筑/移动/战斗代码。

完成后更新 WORKLOG.md，说明 A/B/C/D 完成情况、修改文件、P/U/O Play Mode 验证结果、是否新增 .meta、是否修改 ProjectSettings，并 commit / push。
```
