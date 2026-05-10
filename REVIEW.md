# REVIEW.md

## Review 状态

Codex 已审查 Claude 当前工作区实现：

```text
MVP-03.20 本地未提交实现
```

结论：**MVP-03.20 暂不通过，需要修复 capture requirement 判定**。

说明：Claude 已把需求值接入 U/O，但 `StrategicDispatchService` 使用了“注册 handler 时的单兵序号”来判定是否满足需求，会导致派出人数足够时仍可能被第一名到达士兵错误阻止占领。

## CODEX PROJECT REVIEW

Gate: **FAIL**

Findings:

```text
[P1] 派出人数足够时仍可能无法占领。
File: kingbattle/Assets/Scripts/Combat/StrategicDispatchService.cs
Problem: handler 内使用 capturedCount + 1 判断需求。capturedCount 是每个士兵注册 handler 时的循环序号，不是最终 dispatchedCount。若派出 3 个兵、目标需求 2，第一名注册的士兵先到达时会按 1/2 判定失败，并设置 captureConsidered=true，后续士兵到达也不会再触发占领。
Fix: handler 应使用本次 DispatchToPlot 的最终派出总数判断，例如 foreach 结束后确定 totalDispatched，再让所有 handler 闭包读取同一个 finalTotal / canCapture 值；或者只在 count 统计完成后注册 handler。必须保证第一次到达时用的是总派兵数，而不是当前士兵序号。
```

### 已确认

- P 快捷键和占领需求服务已存在。
- O/U 已开始传入 required count。
- 问题集中在 `StrategicDispatchService.DispatchToPlot(...)` 的 arrival capture gate。
- `kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍未提交。

### 观察

这不是文案问题，而是核心占领判定错误。必须先修复再进入下一轮功能。

### 说明

当前代码未提交；本轮只发布修复要求。`kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍是未跟踪 Unity Editor 生成文件，不应提交。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.20 暂不通过。

进入 MVP-03.20 修复包：占领需求判定收口。

背景：
- 你已把 required count 传入 `StrategicDispatchService.DispatchToPlot(...)`。
- 但当前 capture handler 用 `capturedCount + 1` 判定需求，capturedCount 是当前士兵注册时的序号，不是最终派兵总数。
- 这会导致 dispatchedCount >= requiredCount 时仍可能占领失败。

本轮目标：
- 修复 requirement 判定，让 arrival handler 使用本次派出的最终总数。
- 保持 U/O/P 行为和日志。
- 不做新玩法。

允许：
任务 A：修复总数判定
- `StrategicDispatchService.DispatchToPlot(...)` 必须使用最终 totalDispatched 判断是否满足 `requiredSoldierCount`。
- 不要用每个士兵注册 handler 时的局部序号做判定。
- 可选实现：
  - 先收集符合条件的 units 到列表，得到 totalDispatched，再注册 handlers。
  - 或保留循环，但让 handler 闭包读取 foreach 完成后的 `count` / finalTotal。
- 第一个到达的士兵触发 capture 时，应按 `totalDispatched >= requiredSoldierCount` 判定。

任务 B：保持 one-shot handler 清理
- 每个 handler 到达后仍要移除自身。
- `captureConsidered` 仍应防止重复 TryCapture / 重复 blocked log。
- 重复按 U/O 时旧 handler 仍要被移除。

任务 C：日志和结果字段一致
- blocked 日志必须显示最终 `totalDispatched/requiredSoldierCount`。
- O 的 `ExpansionResult.hasEnoughDispatchedSoldiers` 必须等价于 `dispatchedCount >= requiredCount`。
- U 日志继续显示 dispatched / required。

任务 D：Play Mode 反向验证
- 不足人数：例如 dispatched 1/2，到达后不占领，Console 有 blocked log。
- 足够人数：例如 dispatched 3/2，第一个兵到达也必须占领成功。
- 重复按 U/O 不应出现旧 handler 误触发。
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

完成后更新 WORKLOG.md，说明 A/B/C/D 修复情况、修改文件、P/U/O Play Mode 验证结果、是否新增 .meta、是否修改 ProjectSettings，并 commit / push。
```
