# TASK.md

## 当前任务

发布 MVP-03.19：占领需求数据层批量任务。

Codex 当前仍作为 Tech Lead / Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
692c4a7 refactor: add ExpansionCandidate data model and GetExpansionCandidates query
```

Codex Review 结论：

```text
MVP-03.18 代码审查通过；允许进入 MVP-03.19
```

## 本轮目标

从本轮开始，每次尽量给 Claude 布置 2-3 个强相关小任务，以加快进度；仍保持边界清楚、方便 review。

本轮不做新玩法，只补占领需求数据层和验证输出，为后续“大区块需要更多兵占领”做准备。

目标：

```text
按 PlotSize 查询占领所需兵力；P 可打印验证；O 的结果可携带目标需求预览；暂不接入实际 capture 判定。
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

## MVP-03.19 批量允许范围

任务 A：占领需求服务

- 新增 `PlotCaptureRequirementService` 或等价小服务，建议放在 `kingbattle/Assets/Scripts/Combat/`。
- 提供方法：
  - `GetRequiredSoldierCount(PlotData plot)`。
  - `GetRequiredSoldierCount(PlotSize size)`。
- 建议最小规则：
  - Small = 1。
  - Medium = 2。
  - Large = 3。
- 对 null plot 做安全处理。

任务 B：Play Mode 验证入口

- 在 `GameEntry` 增加临时日志快捷键 `P`，打印所有 plot 的 capture requirement。
- 日志至少包含 plotId、PlotSize、requiredSoldierCount。

任务 C：扩张结果预览数据

- `StrategicExpansionService.ExpansionResult` 可以补充：
  - `requiredSoldierCount`。
  - `hasEnoughDispatchedSoldiers` 或等价 bool，仅作为日志/预览数据。
- `ExpandNext(...)` 在派兵后可计算 `dispatchedCount >= requiredSoldierCount`，但不得改变当前 capture 行为。
- O 日志可以带上 dispatched / required，例如 `dispatched 2, required 3`。

- 新增脚本必须提交对应 `.meta`。
- 更新 `WORKLOG.md`。

## 禁止范围

- 不改变 K / L / R / T / Y / U / I / O 的外部行为。
- 不做正式派兵 UI。
- 不做自动扩张。
- 不做正式多目标选择策略。
- 不把占领需求接入实际 capture 判定；本轮只做数据与日志预览。
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
  - P 能打印每个 plot 的占领需求，例如 Small/Medium/Large 对应 1/2/3。
  - O 原有派兵和占领行为不变，但日志可显示 dispatched / required。
  - I 原有行为不变。
- 新服务可按 PlotSize 或 PlotData 返回需求值。
- `ExpansionResult` 可携带目标需求和是否满足需求的预览字段。
- 新服务不持有长期静态游戏状态。
- 当前 Neutral -> Player capture 行为不变。
- I 不派兵、不占领。
- Y / U 原有行为不变。
- R 仍跳过大本营废墟。
- K / L 清场行为不变。
- Console 无明显错误。
- 仍未修改或提交 `ProjectSettings`。
