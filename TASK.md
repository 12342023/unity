# TASK.md

## 当前任务

发布 MVP-03.20 修复包：占领需求判定收口。

Codex 当前仍作为 Tech Lead / Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
MVP-03.20 本地未提交实现
```

Codex Review 结论：

```text
MVP-03.20 暂不通过；必须修复 requirement 判定
```

## 本轮目标

继续采用“每轮 2-3 个强相关任务”的节奏，但当前有阻塞 bug，先做修复包。

目标：

```text
修复 U/O 派兵数量满足目标需求时仍可能无法占领的问题。
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

## MVP-03.20 修复范围

任务 A：修复总派兵数判定

- `StrategicDispatchService.DispatchToPlot(...)` 必须用本次最终 totalDispatched 判断是否满足 `requiredSoldierCount`。
- 不允许使用每个士兵注册 handler 时的局部序号作为派兵总数。
- 第一个到达的士兵触发 capture 时，应按 `totalDispatched >= requiredSoldierCount` 判定。

任务 B：保持 handler 生命周期

- 每个 handler 到达后仍移除自身。
- `captureConsidered` 仍防止重复 TryCapture / 重复 blocked log。
- 重复按 U/O 时旧 handler 仍要被移除。

任务 C：日志和结果字段一致

- blocked 日志必须显示最终 totalDispatched / requiredSoldierCount。
- O 的 `ExpansionResult.hasEnoughDispatchedSoldiers` 必须等价于 `dispatchedCount >= requiredCount`。
- U 日志继续显示 dispatched / required。

任务 D：Play Mode 验证

- 不足人数：dispatched 1/2，到达后不占领，Console 有 blocked log。
- 足够人数：dispatched 3/2，第一个兵到达也必须占领成功。
- 重复按 U/O 不应出现旧 handler 误触发。

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
  - 足够人数场景下，第一名士兵到达也不能错误 blocked。
- `StrategicDispatchService` handler 清理逻辑仍正确。
- `PlotCaptureService` 的基本规则不变。
- I 不派兵、不占领。
- Y / U 原有行为不变。
- R 仍跳过大本营废墟。
- K / L 清场行为不变。
- Console 无明显错误。
- 仍未修改或提交 `ProjectSettings`。
