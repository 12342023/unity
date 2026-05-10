# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
692c4a7 refactor: add ExpansionCandidate data model and GetExpansionCandidates query
```

结论：**MVP-03.18 代码审查通过，允许进入 MVP-03.19**。

说明：`StrategicConnectionService` 已提供显式 `ExpansionCandidate` 数据和 `GetExpansionCandidates(mapData)` 查询；`StrategicExpansionService` 已改为从第一个 candidate 派兵，O 外部行为保持不变。

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞问题。
```

### 已确认

- `StrategicConnectionService.ExpansionCandidate` 包含 `sourcePlotId` 和 `targetPlotId`。
- `GetExpansionCandidates(mapData)` 基于已有 frontier 查询展开 source/target pair。
- 候选规则继承 `GetPlayerFrontierPlots`：source 是 Player-owned 且非 main base，target 是相邻 Neutral。
- 查询过程不改变任何 `plot.faction`。
- `StrategicExpansionService.ExpandNext(...)` 已改用 `candidates[0]`。
- `ExpansionResult` 已补充 `sourcePlotId`、`targetPlotId`、`dispatchedCount`。
- K / L / R / T / Y / U / I / O 外部行为未被改动。
- `MapData.CreateFixedMap()` 未被改动。
- `kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍未提交。

### 观察

扩张候选数据已经成形。下一步可以开始补“不同大小地块需要不同占领兵力”的数据层，但先不改变当前到达即占领的玩法，避免一次跨太大。

### 说明

`git show --check HEAD` 本轮未发现问题。`kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍是未跟踪 Unity Editor 生成文件，不应提交。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.18 代码审查通过。

进入 MVP-03.19：占领需求数据层批量任务。

背景：
- 用户希望之后每轮多布置一些任务，加快进度。
- 原始目标里有“大的区块需要更多兵去占领”。
- 当前 `PlotCaptureService` 仍是一个 Player 士兵到达就占领。
- 本轮只做数据层，不改变当前占领行为。

本轮目标：
- 新增占领需求查询服务，让代码能按 PlotSize 查询占领所需兵力。
- 增加 P 日志验证。
- 让 O 的结果里带上 dispatched / required 预览数据。
- 不接入 `PlotCaptureService`，不改变 O/U 到达即占领。

允许：
任务 A：占领需求服务
- 新增 `PlotCaptureRequirementService` 或等价小服务，建议放在 `kingbattle/Assets/Scripts/Combat/`。
- 提供方法：
  - `GetRequiredSoldierCount(PlotData plot)`
  - `GetRequiredSoldierCount(PlotSize size)`
- 建议最小规则：
  - Small = 1
  - Medium = 2
  - Large = 3
- 对 null plot 做安全处理。

任务 B：Play Mode 验证入口
- 在 `GameEntry` 增加临时日志快捷键 `P`，打印所有 plot 的 capture requirement。
- 日志至少包含 plotId、PlotSize、requiredSoldierCount。

任务 C：扩张结果预览数据
- `StrategicExpansionService.ExpansionResult` 可以补充：
  - `requiredSoldierCount`
  - `hasEnoughDispatchedSoldiers` 或等价 bool
- `ExpandNext(...)` 可以计算 `dispatchedCount >= requiredSoldierCount`，仅用于日志/预览。
- O 日志可以带上 dispatched / required。

- 如果新增脚本，必须提交对应 `.meta`。
- 更新 WORKLOG.md。

必须保持：
- K / L / R / T / Y / U / I / O 外部行为不变。
- 不改变当前 Neutral -> Player 捕获行为。
- 不改变 `MapData.CreateFixedMap()`。
- 不修改 `PlotCaptureService` 的 Player-only / no-main-base / Neutral-only 规则。
- 不修改 `StrategicDispatchService` 的 handler 生命周期逻辑。
- 新服务不持有长期静态游戏状态。
- 不修改 ProjectSettings。
- 不提交 kingbattle/ProjectSettings/SceneTemplateSettings.json。

禁止：
- 不做正式选择目标 UI。
- 不做自动扩张。
- 不做资源、升级、区域奖励、传送阵、AI。
- 不做占领进度条。
- 不把占领需求接入实际 capture 判定；本轮只做数据与日志预览。
- 不重构 UnitCombat。
- 不重构无关建筑/移动/战斗代码。

完成后更新 WORKLOG.md，说明三项任务完成情况、修改文件、P/I/O Play Mode 验证结果、是否新增 .meta、是否修改 ProjectSettings，并 commit / push。
```
