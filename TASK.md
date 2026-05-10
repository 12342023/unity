# TASK.md

## 当前任务

发布 MVP-03.18：连地网络规则整理 - 显式扩张候选数据。

Codex 当前仍作为 Tech Lead / Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
9979941 refactor: extract O expansion orchestration to StrategicExpansionService
```

Codex Review 结论：

```text
MVP-03.17 代码审查通过；允许进入 MVP-03.18
```

## 本轮目标

不做新玩法，只整理连地扩张的数据层候选，方便后续正式 UI 和多目标选择。

目标：

```text
将“可扩张 source/target”整理成显式候选数据；O 仍只选择第一个候选，外部行为不变。
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

## MVP-03.18 允许范围

- 可以在 `StrategicConnectionService` 增加小数据类型，例如 `ExpansionCandidate`：
  - sourcePlotId。
  - targetPlotId。
- 可以增加方法，例如 `GetExpansionCandidates(MapData mapData)`。
- 候选规则：
  - source 必须是 Player-owned。
  - source 不能是 main base。
  - target 必须是相邻 `Faction.Neutral`。
  - 查询不改变任何 faction。
  - 输出顺序保持当前 plot / neighbor 遍历顺序，确保 O 行为不变。
- `StrategicExpansionService.ExpandNext(...)` 改为使用第一个 `ExpansionCandidate`。
- `ExpansionResult` 可以补充 `sourcePlotId`、`targetPlotId`、`dispatchedCount` 字段。
- 保持 I 现有输出行为不变。
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
  - K -> T -> Y -> U。
  - U 到达 Crossroads 后，Crossroads 变 Player。
  - I 能打印 Crossroads 可连接的 Neutral，例如 Village / Farmland。
  - O 能从 Crossroads 附近派 Player 士兵到第一个 Neutral target。
  - O 到达后 target 从 Neutral 变 Player，并刷新颜色。
- `StrategicConnectionService.GetExpansionCandidates(mapData)` 或等价方法返回显式 source/target 候选。
- `StrategicExpansionService` 通过候选数据派兵，而不是直接索引 frontier 内部结构。
- 新增查询不持有长期静态游戏状态。
- O 不应占领 Enemy plot。
- O 不应占领 main base plot。
- I 不派兵、不占领。
- Y / U 原有行为不变。
- R 仍跳过大本营废墟。
- K / L 清场行为不变。
- Console 无明显错误。
- 仍未修改或提交 `ProjectSettings`。
