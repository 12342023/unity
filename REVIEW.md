# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
681abb4 fix: use shared totalDispatched instead of per-iteration capturedCount
```

结论：**MVP-03.20 修复通过，允许进入 MVP-03.21**。

说明：`StrategicDispatchService` 已改用共享 `totalDispatched`，arrival handler 触发时能读取本次最终派兵总数，不再使用 per-iteration 序号判定 capture requirement。

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞问题。
```

### 已确认

- `StrategicDispatchService.DispatchToPlot(...)` 使用 `totalDispatched` 返回最终派兵数。
- arrival handler 读取共享 `totalDispatched` 判定 `totalDispatched >= requiredSoldierCount`。
- blocked 日志显示最终 dispatched / required。
- `captureConsidered` 仍防止重复 TryCapture / 重复 blocked log。
- 每个 handler 到达后仍移除自身。
- O/U 继续传入 target required count。
- `kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍未提交。

### 观察

占领需求已经接入主路径。下一轮可以补“占领成功/失败后的反馈与状态数据”，让后续 UI、失败重试、士兵停留/巡逻更容易做。

### 说明

`git show --check HEAD` 未发现问题。`kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍是未跟踪 Unity Editor 生成文件，不应提交。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.20 修复通过。

进入 MVP-03.21：占领反馈与结果状态批量任务。

背景：
- U/O 已按 target plot 的 required count 判定是否能占领。
- 现在成功/失败主要靠日志，后续 UI 或重试逻辑需要更清晰的结果状态。
- 用户希望每轮多布置一些任务，所以本轮继续打包 3 个强相关点。

本轮目标：
- 为派兵和占领结果补结构化状态。
- 统一成功/失败日志。
- 为未来 UI 准备只读结果数据。

允许：
任务 A：新增派兵结果数据
- 在 `StrategicDispatchService` 增加 `DispatchResult` 或等价小数据类型。
- 字段建议：
  - targetPlotId
  - dispatchedCount
  - requiredSoldierCount
  - hasEnoughSoldiers
  - captureWillBeAttemptedOnArrival
  - message
- 保留现有调用兼容性，或同步更新 U/O 调用点。

任务 B：O/U 使用结构化结果
- `StrategicExpansionService.ExpandNext(...)` 使用 `DispatchResult` 填充 `ExpansionResult`。
- `GameEntry` 的 U 分支使用 `DispatchResult` 打印统一日志。
- O/U 日志格式尽量一致，例如 `dispatch 3/2 to Crossroads, willCapture=True`。

任务 C：到达后的成功/失败日志统一
- 当到达后 capture 被允许，打印 `Capture attempt allowed: dispatched 3/2 to Crossroads`。
- 当不足被 blocked，打印 `Capture blocked: dispatched 1/2 to Crossroads`。
- 不改变 `PlotCaptureService.TryCapture(...)` 的规则。

任务 D：验证
- P 仍打印需求。
- U/O 不足人数 blocked。
- U/O 足够人数可以占领。
- 重复按 U/O 不触发旧 handler。
- 更新 WORKLOG.md。

必须保持：
- K / L / R / T / Y / U / I / O 外部行为不变。
- 除已有“兵力不足时不占领”外，不改变当前 Neutral -> Player 捕获流程。
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
