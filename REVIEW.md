# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
2e19281 feat: PlotCaptureService + MapRenderer.RefreshPlotColor, U triggers capture
```

结论：**MVP-03.13 暂不通过，需要一轮小修后再继续**。

说明：主路径已经接近目标，U 派兵到达后可以调用占领服务并刷新地块颜色。但当前占领服务与到达回调还有几个边界问题，会影响后续连地/派兵系统稳定性。

## CODEX PROJECT REVIEW

Gate: **FAIL**

Findings:

### [P1] 静态 capturedPlots 没有 reset，可能导致 Play Mode 第二次无法占领

File: `kingbattle/Assets/Scripts/Combat/PlotCaptureService.cs:15`

Problem: `capturedPlots` 是 static，`Reset()` 已写但没有任何地方调用。如果 Unity 关闭 Domain Reload，或者后续切换场景/重启玩法但静态状态没清掉，新的 `MapData` 里 Crossroads 仍是 Neutral，但 `capturedPlots` 还记着它已占领，导致 `TryCapture` 返回 false，颜色和归属都不会更新。

Fix: 在 `GameEntry.Start()` 初始化新一局玩法时调用 `PlotCaptureService.Reset()`。更长期可以考虑去掉 `capturedPlots`，只用 `plot.faction != Faction.Neutral` 判断重复占领。

### [P2] TryCapture 没有强制只能 Player 占领，也没有拒绝 main base

File: `kingbattle/Assets/Scripts/Combat/PlotCaptureService.cs:29`

Problem: 任务要求“只允许 Neutral -> Player”和“不允许占领 main base ruin 本身”。但当前 public API 接收任意 `capturingFaction`，只要目标是 Neutral，就会改成该阵营；同时没有检查 `plot.isMainBase`。虽然当前 `GameEntry` 只传 `Faction.Player`，但服务边界本身不符合任务合同，后续复用时容易引入错误。

Fix: 在 `TryCapture` 入口明确拒绝 `capturingFaction != Faction.Player`，并在拿到 plot 后拒绝 `plot.isMainBase`。

### [P2] OnPushDestinationReached 回调会累积，后续派兵可能触发旧占领逻辑

File: `kingbattle/Assets/Scripts/GameEntry.cs:213`

Problem: U 每次派兵都会 `+=` 一个 lambda，但 lambda 到达后不会 unsubscribe。当前只有一个目标时影响还小；后续一旦支持多个 Neutral 目标或多次派兵，旧回调可能在下一次到达时再次执行，造成日志噪音或错误占领尝试。

Fix: 使用 one-shot handler：注册前创建 `System.Action handler = null; handler = () => { u.OnPushDestinationReached -= handler; ... }`，到达后先反订阅，再执行 `captureOnce` 判断。

### [P3] WORKLOG 的二次 U 验证描述与当前逻辑不一致

File: `WORKLOG.md`

Problem: WORKLOG 写“再次 U 会再次走向 Crossroads 并显示 already captured”。但第一次占领后 Crossroads 已变 Player，`GetConnectableNeutralPlots` 不再返回 Crossroads，所以再次 U 更可能输出没有可连接 Neutral plot。

Fix: 更新 WORKLOG 验证描述，保持和代码行为一致。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.13 暂不通过，需要小修后再继续。

本轮只修 MVP-03.13，占领边界与回调生命周期，不做新功能。

必须修复：

1. PlotCaptureService static 状态重置
   - 在 GameEntry.Start() 新一局初始化时调用 PlotCaptureService.Reset()。
   - 保证反复进入 Play Mode 时，Crossroads 不会因为旧 capturedPlots 状态而无法再次占领。

2. PlotCaptureService 占领边界
   - TryCapture 必须明确只允许 capturingFaction == Faction.Player。
   - 如果 capturingFaction 不是 Player，返回 false 并输出清晰日志。
   - 如果目标 plot.isMainBase == true，返回 false 并输出清晰日志。
   - 仍然只允许 plot.faction == Faction.Neutral 时占领。
   - 不允许占领 Enemy plot。

3. U 派兵到达回调改成 one-shot
   - 不要一直累积 OnPushDestinationReached lambda。
   - 使用可反订阅的一次性 handler。
   - handler 到达后先取消订阅，再执行 captureOnce / TryCapture。
   - 保持多个士兵里只有第一个到达者触发占领。

4. 修正 WORKLOG 验证描述
   - 第一次 U 到达 Crossroads 后，Crossroads 变 Player。
   - 再次 Y / U 时，EnemyBase 可能没有可连接 Neutral plot，因为 Crossroads 已不是 Neutral。
   - 不要写“再次 U 会再次走向 Crossroads”。

禁止：
- 不做正式 UI。
- 不做占领进度条。
- 不做资源、升级、区域奖励、传送阵、AI。
- 不重构 GameEntry。
- 不修改 ProjectSettings。
- 不提交 kingbattle/ProjectSettings/SceneTemplateSettings.json。

完成后更新 WORKLOG.md，说明修改文件、K/T/Y/U/R/L Play Mode 验证结果，并 commit / push。
```
