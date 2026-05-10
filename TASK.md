# TASK.md

## 当前任务

发布 MVP-03.21：占领反馈与结果状态批量任务。

Codex 当前仍作为 Tech Lead / Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
681abb4 fix: use shared totalDispatched instead of per-iteration capturedCount
```

Codex Review 结论：

```text
MVP-03.20 修复通过；允许进入 MVP-03.21
```

## 本轮目标

继续采用“每轮 2-3 个强相关任务”的节奏。本轮整理占领反馈与结果状态，方便后续 UI、失败重试和行为收口。

目标：

```text
为 U/O 派兵和占领结果补结构化状态，统一成功/失败日志。
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

## MVP-03.21 批量允许范围

任务 A：新增派兵结果数据

- 在 `StrategicDispatchService` 增加 `DispatchResult` 或等价小数据类型。
- 字段建议：
  - targetPlotId。
  - dispatchedCount。
  - requiredSoldierCount。
  - hasEnoughSoldiers。
  - captureWillBeAttemptedOnArrival。
  - message。

任务 B：O/U 使用结构化结果

- `StrategicExpansionService.ExpandNext(...)` 使用 `DispatchResult` 填充 `ExpansionResult`。
- `GameEntry` 的 U 分支使用 `DispatchResult` 打印统一日志。
- O/U 日志格式尽量一致，例如 `dispatch 3/2 to Crossroads, willCapture=True`。

任务 C：到达后的成功/失败日志统一

- 允许占领时打印 `Capture attempt allowed: dispatched 3/2 to Crossroads`。
- 不足时打印 `Capture blocked: dispatched 1/2 to Crossroads`。
- 不修改 `PlotCaptureService.TryCapture(...)` 的规则。

任务 D：验证

- P 仍打印需求。
- U/O 不足人数 blocked。
- U/O 足够人数可以占领。
- 重复按 U/O 不触发旧 handler。

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
  - O/U 日志格式一致，包含 dispatched / required / willCapture。
  - 到达后成功/失败日志清晰。
  - 不足人数不占领。
  - 足够人数可占领。
  - 重复按 U/O 不触发旧 handler。
- `StrategicDispatchService` handler 清理逻辑仍正确。
- `PlotCaptureService` 的基本规则不变。
- I 不派兵、不占领。
- Y / U 原有行为不变。
- R 仍跳过大本营废墟。
- K / L 清场行为不变。
- Console 无明显错误。
- 仍未修改或提交 `ProjectSettings`。
