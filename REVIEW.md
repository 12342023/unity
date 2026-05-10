# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
780358a feat: replace OnGUI HUD with Canvas/uGUI runtime UI
```

结论：**MVP-05.0 暂不通过，需要返修。**

## CODEX PROJECT REVIEW

Gate: **FAIL**

Findings:

```text
[P1] GameHud.cs 编译失败：CreateSeparator 返回 void 却赋值给 var。
[P2] Candidate rows 每帧 Destroy/Recreate，会造成 UI GC/抖动。
[P3] Dispatch 按钮仍显示 Dsp，不符合正式 UI 文案要求。
```

### [P1] `GameHud.cs` 编译失败

Unity Console / Editor log 当前错误：

```text
Assets/Scripts/UI/GameHud.cs(95,13): error CS0815: Cannot assign void to an implicitly-typed variable
Assets/Scripts/UI/GameHud.cs(100,13): error CS0815: Cannot assign void to an implicitly-typed variable
Assets/Scripts/UI/GameHud.cs(108,13): error CS0815: Cannot assign void to an implicitly-typed variable
```

问题代码：

```csharp
var sep1 = CreateSeparator(hudPanelRoot);
var sep2 = CreateSeparator(hudPanelRoot);
var sep3 = CreateSeparator(hudPanelRoot);
```

但 `CreateSeparator` 当前签名是：

```csharp
private void CreateSeparator(GameObject parent)
```

最小修复：

```csharp
CreateSeparator(hudPanelRoot);
CreateSeparator(hudPanelRoot);
CreateSeparator(hudPanelRoot);
```

或让 `CreateSeparator` 返回 `GameObject` / `Text`。本轮建议用最小修复，不扩展。

### [P2] Candidate rows 每帧重建

当前 `Update()` 每帧调用 `UpdateHudUI()`，而 `UpdateHudUI()` 每帧调用：

```csharp
RebuildCandidateRows();
```

`RebuildCandidateRows()` 会 Destroy 旧 rows 并创建新 rows。这样在正式 UI 中会造成：

- 每帧分配。
- GC 噪音。
- UI 可能抖动。
- 按钮 listener 每帧重建。

建议：

- 只在 `RefreshData()` 后重建候选列表。
- `HandleDispatchClick()` 后刷新数据并重建。
- 普通每帧只更新状态文本、倒计时、选择提示。

### [P3] 按钮仍是 debug 文案

当前：

```csharp
var btnGo = new GameObject("DspBtn");
var btnText = CreateLinkedText(btnGo, "Label", "Dsp", 11, TextAnchor.MiddleCenter);
```

要求：

- 用户可见文字改为 `Dispatch`。
- 按钮宽度相应加大，不要挤压。
- GameObject 名称可以改为 `DispatchButton`。

## 已确认做对的部分

- `GameHud` 已从 OnGUI 改为运行时 Canvas/uGUI 方向。
- `GameEntry` 已传入缓存的 `PlayerInputController`，避免每帧 `FindAnyObjectByType`。
- UI 触发派兵仍走 `StrategicExpansionCommandService.DispatchCandidate`。
- 没看到 UI 直接修改 `MapData` / `PlotData.faction` / building health / unit state。

## 给 Claude 的返修任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

MVP-05.0 暂不通过，当前 Unity 编译失败。请只做小返修，不要重写整个 UI。

任务 1：修复 GameHud 编译错误
- 当前错误：
  Assets/Scripts/UI/GameHud.cs(95,13): error CS0815
  Assets/Scripts/UI/GameHud.cs(100,13): error CS0815
  Assets/Scripts/UI/GameHud.cs(108,13): error CS0815
- 原因：CreateSeparator 返回 void，却写成 var sep = CreateSeparator(...)
- 最小修复：
  CreateSeparator(hudPanelRoot);
  不要赋值给 var。

任务 2：避免 candidate rows 每帧 Destroy/Recreate
- 当前 UpdateHudUI 每帧调用 RebuildCandidateRows。
- 请改为只在 RefreshData 后、HandleDispatchClick 后，或者 candidate 数据变化时重建 rows。
- 普通每帧只更新倒计时、状态文本、选择提示。
- 不要引入复杂 diff 系统，保持简单。

任务 3：修正式 UI 文案
- Dispatch 按钮不要显示 Dsp。
- 改为 Dispatch，并适当加宽按钮。
- GameObject 名称建议从 DspBtn 改为 DispatchButton。

任务 4：补验证和 WORKLOG
- 触发 Unity 重新编译。
- Console 无 error CS。
- Play 初始显示 Canvas/uGUI HUD，不显示旧 OnGUI debug 框。
- HUD Dispatch、点击地图派兵、O 快捷键仍正常。
- K/L/E/N 在 Editor Play Mode 仍可用。
- Victory/Defeat 面板与 Restart 可用。
- 更新 WORKLOG.md，记录返修和验证。

禁止：
- 不重写整个 UI。
- 不改战斗/占领/派兵 service 行为。
- 不改 ProjectSettings。
- 不提交 .claude、.idea、kingbattle.slnx、Library、Logs、UserSettings、要求.md 删除。

完成后 commit / push。
```
