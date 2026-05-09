# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
224db1d fix: capture guards (Player-only, no main base, Reset, one-shot handler)
```

结论：**MVP-03.13 仍暂不通过，需要最后一轮很小的收口修复**。

说明：Player-only、main base 拒绝、`PlotCaptureService.Reset()` 都已补上；普通一次 U 派兵到达后的占领主路径基本成立。但 U 的回调生命周期还没有完全解决“到达前重复派兵/改派”场景，WORKLOG 最新验证描述也仍与代码行为不一致。

## CODEX PROJECT REVIEW

Gate: **FAIL**

Findings:

### [P2] 到达前重复按 U 时，旧 OnPushDestinationReached handler 仍可能残留

File: `kingbattle/Assets/Scripts/GameEntry.cs:214`

Problem: 当前 handler 会在触发后 self-unsubscribe，这修复了“到达后继续残留”的一部分问题。但如果玩家在士兵到达目标前再次按 U，`ClearPushPath()` 会取消旧 push path，却不会移除上一次注册的 handler。随后新的 U 又注册一个新 handler。等士兵最终到达时，旧 handler 和新 handler 都可能触发，只是各自 self-unsubscribe。当前只有 Crossroads 一个目标时影响较小，但后续多目标派兵会造成旧目标占领尝试、日志噪音或错误占领。

Fix: 在 `GameEntry` 内维护 U 专用的 handler 记录，例如：

```csharp
private readonly Dictionary<UnitCombat, System.Action> uCaptureHandlers = new();
```

给某个单位注册新的 U capture handler 前：

```csharp
if (uCaptureHandlers.TryGetValue(u, out var previous))
{
    u.OnPushDestinationReached -= previous;
    uCaptureHandlers.Remove(u);
}
```

新 handler 触发时先取消订阅并从字典移除，再执行 `captureOnce` / `TryCapture`。不要清空 `OnPushDestinationReached`，避免影响其他系统可能注册的回调。

### [P3] WORKLOG 最新验证描述仍与当前代码行为不一致

File: `WORKLOG.md`

Problem: 最新 WORKLOG 写“再次 U → 蓝兵走向 Crossroads，到达后 TryCapture 因 not neutral 拒绝”。但当前 `GameEntry.U` 会先调用 `GetConnectableNeutralPlots(mapData)`；Crossroads 第一次占领后已变 Player，不再是 Neutral，所以再次 U 应该在派兵前输出 `EnemyBase has no connectable neutral plots`，不会再次派兵去 Crossroads，也不会触发 `TryCapture not neutral`。

Fix: 更新 WORKLOG 最新验证描述：

- 第一次 U：蓝兵到 Crossroads，Crossroads Neutral -> Player，颜色变蓝。
- 再次 Y：EnemyBase 没有可连接 Neutral。
- 再次 U：输出 no connectable neutral plots，不派兵，不触发旧回调。

## 已确认通过的部分

- `GameEntry.Start()` 已调用 `PlotCaptureService.Reset()`。
- `TryCapture` 已拒绝 `capturingFaction != Faction.Player`。
- `TryCapture` 已拒绝 `plot.isMainBase`。
- `TryCapture` 仍只允许 `plot.faction == Faction.Neutral`。
- `MapRenderer.RefreshPlotColor` 保持最小范围。
- `kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍未提交。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.13 还差最后一轮很小的收口修复，暂不进入新功能。

本轮只修两个点：

1. U capture handler 的取消/替换
   - 当前 self-unsubscribe 只解决“到达后移除”。
   - 还需要解决“到达前重复按 U / 重新派兵”时旧 handler 残留的问题。
   - 在 GameEntry 内维护 U 专用 handler 字典，例如：
     `private readonly Dictionary<UnitCombat, System.Action> uCaptureHandlers = new();`
   - 给某个单位注册新的 U capture handler 前，如果字典里已有旧 handler，先：
     `u.OnPushDestinationReached -= previous;`
     `uCaptureHandlers.Remove(u);`
   - 新 handler 触发时先：
     `u.OnPushDestinationReached -= localHandler;`
     `uCaptureHandlers.Remove(u);`
   - 然后再执行 captureOnce / PlotCaptureService.TryCapture。
   - 不要清空整个 OnPushDestinationReached，避免影响其他系统。

2. 修正 WORKLOG 最新验证描述
   - Crossroads 第一次被占领后已经是 Player，不再是 Neutral。
   - 再次 Y 应显示 EnemyBase 没有可连接 Neutral。
   - 再次 U 应输出 no connectable neutral plots，不应写成再次走向 Crossroads。
   - 不要写 TryCapture 因 not neutral 拒绝，除非代码真的绕过 GetConnectableNeutralPlots 去调用 TryCapture。

禁止：
- 不做正式 UI。
- 不做占领进度条。
- 不做资源、升级、区域奖励、传送阵、AI。
- 不重构 GameEntry。
- 不修改 ProjectSettings。
- 不提交 kingbattle/ProjectSettings/SceneTemplateSettings.json。

完成后更新 WORKLOG.md，说明修改文件、重复按 U 的验证、K/T/Y/U/R/L Play Mode 验证结果，并 commit / push。
```
