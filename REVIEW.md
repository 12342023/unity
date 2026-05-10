# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
9979941 refactor: extract O expansion orchestration to StrategicExpansionService
```

结论：**MVP-03.17 代码审查通过，允许进入 MVP-03.18**。

说明：O 的扩张编排已从 `GameEntry` 下沉到 `StrategicExpansionService`，`GameEntry` 的 O 分支只负责按键入口和日志输出，外部行为保持不变。

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞问题。
```

### 已确认

- 新增 `kingbattle/Assets/Scripts/Combat/StrategicExpansionService.cs`。
- 新增脚本 `.meta` 已提交。
- `StrategicExpansionService.ExpandNext(mapData, mapRenderer)` 复用 `StrategicConnectionService`、`RoadPathFinder` 和 `StrategicDispatchService`。
- `GameEntry` 的 O 分支已缩小为调用服务并打印 `result.message`。
- O 的 source / target 选择逻辑保持与 MVP-03.16 一致。
- 到达后的实际占领仍由 `PlotCaptureService.TryCapture(...)` 执行。
- K / L / R / T / Y / U / I / O 外部行为未被改动。
- `MapData.CreateFixedMap()` 未被改动。
- `kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍未提交。

### 观察

扩张编排已经下沉。下一步可以把“哪些 source/target 可扩张”整理成显式候选数据，减少 `ExpandNext` 对 `frontiers[0]` 和 `connectableNeutralPlots[0]` 的直接索引依赖，为后续正式 UI 的目标选择做准备。

### 说明

`git show --check HEAD` 报告新增 Unity `.meta` 文件中空字段尾随空格；这是当前 Unity `.meta` 文件的一贯格式，本轮不作为阻塞问题。`kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍是未跟踪 Unity Editor 生成文件，不应提交。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.17 代码审查通过。

进入 MVP-03.18：连地网络规则整理 - 显式扩张候选数据。

背景：
- `StrategicExpansionService` 已经承接 O 的扩张编排。
- 当前 `ExpandNext` 仍直接拿 `frontiers[0]` 和 `connectableNeutralPlots[0]`。
- 后续正式 UI 需要能看到可选 source/target，而不是只隐藏在 O 的临时逻辑里。

本轮目标：
- 在数据层整理“可扩张候选”。
- O 外部行为保持不变，仍选择第一个候选派兵。

允许：
- 可以在 `StrategicConnectionService` 增加小数据类型，例如 `ExpansionCandidate`：
  - sourcePlotId
  - targetPlotId
- 可以增加方法，例如：
  `GetExpansionCandidates(MapData mapData)`
- 候选规则：
  - source 必须是 Player-owned。
  - source 不能是 main base。
  - target 必须是相邻 `Faction.Neutral`。
  - 不改变任何 faction。
  - 输出顺序保持当前 map / neighbor 遍历顺序，确保 O 行为不变。
- `StrategicExpansionService.ExpandNext(...)` 改为使用第一个 `ExpansionCandidate`。
- `ExpansionResult` 可以补充 `sourcePlotId`、`targetPlotId`、`dispatchedCount` 字段，方便以后 UI 和 review。
- 保持 I 现有输出行为不变。
- 更新 WORKLOG.md。

必须保持：
- K / L / R / T / Y / U / I / O 外部行为不变。
- O 仍只派第一个候选，不做选择 UI。
- 不改变 `MapData.CreateFixedMap()`。
- 不修改 `PlotCaptureService` 的 Player-only / no-main-base / Neutral-only 规则。
- 不修改 `StrategicDispatchService` 的 handler 生命周期逻辑。
- 不新增长期静态游戏状态。
- 不修改 ProjectSettings。
- 不提交 kingbattle/ProjectSettings/SceneTemplateSettings.json。

禁止：
- 不做正式选择目标 UI。
- 不做自动扩张。
- 不做资源、升级、区域奖励、传送阵、AI。
- 不做占领进度条。
- 不重构 UnitCombat。
- 不重构无关建筑/移动/战斗代码。

完成后更新 WORKLOG.md，说明修改文件、I/O Play Mode 验证结果、是否新增 .meta、是否修改 ProjectSettings，并 commit / push。
```
