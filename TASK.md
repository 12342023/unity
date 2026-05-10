# TASK.md

## 当前任务

发布 MVP-03.20：占领需求接入批量任务。

Codex 当前仍作为 Tech Lead / Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
404b86c feat: PlotCaptureRequirementService + P shortcut prints capture requirements
569c321 feat: show dispatched versus required capture count
```

Codex Review 结论：

```text
MVP-03.19 代码审查通过；允许进入 MVP-03.20
```

## 本轮目标

继续采用“每轮 2-3 个强相关任务”的节奏。MVP-03.20 把占领需求接入实际 U/O 捕获判定，但仍不做 UI、占领进度条、资源、升级或 AI。

目标：

```text
U/O 派兵数量不足目标 plot 需求时，到达后不占领；满足需求时保持现有到达后占领。
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
- `StrategicExpansionService` 已承接 O 的扩张编排。
- `StrategicConnectionService` 已提供 `ExpansionCandidate` 和 `GetExpansionCandidates(mapData)`。
- `PlotCaptureRequirementService` 已提供 Small/Medium/Large = 1/2/3。
- P 可打印每个 plot 的占领需求。
- O 日志已能显示 dispatched / required 预览。

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

## MVP-03.20 批量允许范围

任务 A：派兵服务支持 capture requirement

- 在 `StrategicDispatchService.DispatchToPlot(...)` 增加可选参数，例如 `requiredSoldierCount = 1`。
- arrival capture handler 里只有 `dispatchedCount >= requiredSoldierCount` 才调用 `PlotCaptureService.TryCapture(...)`。
- 如果不足，打印清晰日志，例如 `Capture blocked: dispatched 1/2 to Crossroads`。
- 必须保持 capture handler 清理逻辑不泄漏、不重复触发。

任务 B：O 接入需求判定

- `StrategicExpansionService.ExpandNext(...)` 查询 target plot 的 required count。
- 调用 `StrategicDispatchService.DispatchToPlot(...)` 时传入 required count。
- `ExpansionResult` 保留或补充：
  - dispatchedCount。
  - requiredCount。
  - hasEnoughDispatchedSoldiers。
- O 日志继续显示 dispatched / required。

任务 C：U 接入需求判定

- `GameEntry` 的 U 分支查询 target plot 的 required count。
- 调用 `StrategicDispatchService.DispatchToPlot(...)` 时传入 required count。
- U 日志显示 dispatched / required。

任务 D：验证入口保持

- P 继续打印每个 plot 的占领需求。
- I/O/U 现有测试入口保留。

- 新增脚本必须提交对应 `.meta`。
- 更新 `WORKLOG.md`。

## 禁止范围

- 不改变 K / L / R / T / Y / U / I / O 的外部行为。
- 不做正式派兵 UI。
- 不做自动扩张。
- 不做正式多目标选择策略。
- 不做占领进度条。
- 不做敌方反夺。
- 不做资源、升级、区域奖励、传送阵、AI。
- 不改变 `MapData.CreateFixedMap()`。
- 不重构 `UnitCombat`。
- 不重构 `StrategicDispatchService`。
- 不重构 `PlotCaptureService`。
- 不重构无关建筑、移动、战斗代码。
- 不修改 `ProjectSettings`。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。

## 验收标准

- Play Mode：
  - P 能打印每个 plot 的占领需求。
  - O 日志显示 dispatched / required。
  - U 日志显示 dispatched / required。
  - 当 dispatchedCount >= requiredCount 时，目标仍能被占领。
  - 当 dispatchedCount < requiredCount 时，目标不应被占领，并有清晰日志。
- `StrategicDispatchService` handler 清理逻辑仍正确。
- `PlotCaptureService` 的基本规则不变。
- I 不派兵、不占领。
- Y / U 原有行为不变。
- R 仍跳过大本营废墟。
- K / L 清场行为不变。
- Console 无明显错误。
- 仍未修改或提交 `ProjectSettings`。
