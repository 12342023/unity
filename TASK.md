# TASK.md

## 当前任务

MVP-03.13 收口小修：U capture handler 替换与 WORKLOG 验证修正。

Codex 当前仍作为 Tech Lead / Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
224db1d fix: capture guards (Player-only, no main base, Reset, one-shot handler)
```

Codex Review 结论：

```text
MVP-03.13 仍暂不通过；还需修复到达前重复 U 时旧 handler 残留的问题，并修正 WORKLOG 验证描述
```

## 本轮目标

只做最后收口，不加玩法。

成功标准：

```text
重复按 U / 到达前重新派兵不会留下旧 capture handler；
Crossroads 被占领后，再次 Y/U 的行为描述与代码一致；
MVP-03.13 可以进入通过状态。
```

## 当前工作区注意事项

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件仍是未跟踪 Unity Editor 生成文件。除非用户明确批准，否则不要提交。

## 必须修复

### 1. U capture handler 到达前替换

当前已有 self-unsubscribe：

```csharp
localHandler = () =>
{
    u.OnPushDestinationReached -= localHandler;
    ...
};
```

但如果单位还没到达时再次按 U，旧 handler 没有触发，也就不会取消订阅。

要求：

- 在 `GameEntry` 中新增 U 专用 handler 记录，例如：

```csharp
private readonly Dictionary<UnitCombat, System.Action> uCaptureHandlers = new();
```

- 给某个单位注册新的 U capture handler 前：

```csharp
if (uCaptureHandlers.TryGetValue(u, out var previous))
{
    u.OnPushDestinationReached -= previous;
    uCaptureHandlers.Remove(u);
}
```

- 新 handler 触发时：

```csharp
u.OnPushDestinationReached -= localHandler;
uCaptureHandlers.Remove(u);
```

- 然后再执行 `captureOnce` / `PlotCaptureService.TryCapture(...)`。
- 不要清空整个 `OnPushDestinationReached`。
- 不要重构 `UnitCombat`。

### 2. WORKLOG 最新验证描述修正

当前代码逻辑是：

- 第一次 U 到达 Crossroads 后，Crossroads 从 Neutral 变 Player。
- 之后 `GetConnectableNeutralPlots(mapData)` 不再返回 Crossroads。
- 所以再次 Y / U 不会再次派兵到 Crossroads。

WORKLOG 应写：

- 再次 Y：EnemyBase 没有可连接 Neutral。
- 再次 U：输出 no connectable neutral plots，不派兵。
- 不要写“再次 U 走向 Crossroads”。
- 不要写“TryCapture 因 not neutral 拒绝”，除非代码真的绕过 Neutral 查询。

## 禁止范围

- 不做正式派兵 UI。
- 不做占领进度条。
- 不做敌方反夺。
- 不做资源、升级、区域奖励、传送阵、AI。
- 不重构 `GameEntry`。
- 不重构 `UnitCombat`。
- 不修改 `ProjectSettings`。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。

## 验收标准

- Play Mode：K -> T -> Y -> U。
- U 到达 Crossroads 后：
  - Crossroads 从 Neutral 变 Player。
  - Crossroads 颜色刷新为 Player 颜色。
  - Console 有清晰占领日志。
- 到达前重复按 U：
  - 不应留下旧 capture handler。
  - 到达后只触发当前 U 的 capture handler。
- Crossroads 被占领后再次 Y / U：
  - Y / U 不应再把 Crossroads 当作 Neutral。
  - U 应输出 no connectable neutral plots 或同等日志。
  - 不应再次派兵去 Crossroads。
- R 仍跳过大本营废墟。
- K / L 清场行为不变。
- Console 无明显错误。
- 仍未修改或提交 `ProjectSettings`。
