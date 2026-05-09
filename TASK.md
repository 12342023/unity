# TASK.md

## 当前任务

MVP-03.13 小修：占领边界与 U 到达回调生命周期。

Codex 当前仍作为 Tech Lead / Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
2e19281 feat: PlotCaptureService + MapRenderer.RefreshPlotColor, U triggers capture
```

Codex Review 结论：

```text
MVP-03.13 暂不通过；需要修复占领边界、static reset、one-shot callback
```

## 本轮目标

只修 MVP-03.13 的质量问题，不做新玩法。

成功标准：

```text
U 派兵到达 Neutral plot 后可稳定占领；
重复 Play Mode / 重复派兵不会被旧静态状态或旧回调干扰；
服务层明确只允许 Neutral -> Player，且拒绝 main base。
```

## 当前工作区注意事项

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件仍是未跟踪 Unity Editor 生成文件。除非用户明确批准，否则不要提交。

## 必须修复

### 1. Reset PlotCaptureService static state

- `PlotCaptureService` 已有 `Reset()`。
- 在 `GameEntry.Start()` 初始化新一局时调用。
- 目标：反复进入 Play Mode 时，不受旧 `capturedPlots` 状态影响。

### 2. 收紧 TryCapture 边界

`PlotCaptureService.TryCapture(...)` 必须：

- 只允许 `capturingFaction == Faction.Player`。
- 拒绝 `plot.isMainBase == true`。
- 只允许 `plot.faction == Faction.Neutral` 时占领。
- 不允许占领 Enemy plot。
- 输出清晰日志，方便 Play Mode 验证。

### 3. U 到达回调必须 one-shot

当前 `GameEntry` 中：

```csharp
u.OnPushDestinationReached += () => { ... };
```

需要改为可反订阅的一次性 handler，避免旧回调累积。

要求：

- 到达后先 unsubscribe。
- 多个士兵里仍只有第一个到达者触发 `TryCapture`。
- 不要清掉其他系统可能注册的回调。

### 4. 修正 WORKLOG 验证描述

- 第一次 U 到达 Crossroads 后，Crossroads 变 Player。
- Crossroads 变 Player 后，不再是 EnemyBase 的 connectable Neutral。
- 再次 Y / U 的预期应改成“没有可连接 Neutral”或同等实际行为。
- 不要写“再次 U 会再次走向 Crossroads”。

## 禁止范围

- 不做正式派兵 UI。
- 不做占领进度条。
- 不做敌方反夺。
- 不做资源、升级、区域奖励、传送阵、AI。
- 不重构 `GameEntry`。
- 不修改 `ProjectSettings`。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。

## 验收标准

- Play Mode：K -> T -> Y -> U。
- U 到达 Crossroads 后：
  - Crossroads 从 Neutral 变 Player。
  - Crossroads 颜色刷新为 Player 颜色。
  - Console 有清晰占领日志。
- 再次 Y / U 后：
  - 不应重复把 Crossroads 当作 Neutral。
  - 不应触发旧回调造成额外占领尝试。
- 不能占领 Enemy plot。
- 不能占领 main base plot。
- R 仍跳过大本营废墟。
- K / L 清场行为不变。
- Console 无明显错误。
- 仍未修改或提交 `ProjectSettings`。
