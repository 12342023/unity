# TASK.md

## 当前任务

MVP-05.0 返修：修复正式 UI 编译错误与候选列表刷新。

Codex 当前仍作为 Tech Lead 和 Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
780358a feat: replace OnGUI HUD with Canvas/uGUI runtime UI
```

Codex Review 结论：

```text
MVP-05.0 暂不通过。
当前 Unity 编译失败，需要先返修 GameHud.cs。
```

## 阻塞问题 A：`GameHud.cs` 编译失败

Unity Console / Editor log：

```text
Assets/Scripts/UI/GameHud.cs(95,13): error CS0815: Cannot assign void to an implicitly-typed variable
Assets/Scripts/UI/GameHud.cs(100,13): error CS0815: Cannot assign void to an implicitly-typed variable
Assets/Scripts/UI/GameHud.cs(108,13): error CS0815: Cannot assign void to an implicitly-typed variable
```

当前问题：

```csharp
var sep1 = CreateSeparator(hudPanelRoot);
var sep2 = CreateSeparator(hudPanelRoot);
var sep3 = CreateSeparator(hudPanelRoot);
```

但 helper 是：

```csharp
private void CreateSeparator(GameObject parent)
```

要求最小修复：

```csharp
CreateSeparator(hudPanelRoot);
CreateSeparator(hudPanelRoot);
CreateSeparator(hudPanelRoot);
```

不要为了这个重写 UI。

## 问题 B：候选列表每帧重建

当前：

- `Update()` 每帧调用 `UpdateHudUI()`。
- `UpdateHudUI()` 每帧调用 `RebuildCandidateRows()`。
- `RebuildCandidateRows()` 会 Destroy 旧 rows 并创建新 rows。

要求：

- 不要每帧 Destroy/Recreate UI rows。
- 只在 `RefreshData()` 后、`HandleDispatchClick()` 后，或 candidate 数据变化时重建。
- 普通每帧只更新：
  - 敌方进攻倒计时。
  - 最近操作。
  - 阵营统计。
  - 选择提示。
- 不要引入复杂 diff 系统。

## 问题 C：按钮文案仍是 debug 风格

当前：

```csharp
var btnGo = new GameObject("DspBtn");
var btnText = CreateLinkedText(btnGo, "Label", "Dsp", 11, TextAnchor.MiddleCenter);
```

要求：

- 用户可见文案改为 `Dispatch`。
- 按钮宽度适当加大。
- GameObject 名称建议改为 `DispatchButton`。

## 已通过部分

- `GameHud` 已改向 Canvas/uGUI。
- `GameEntry` 已传入 `PlayerInputController`，避免每帧查找。
- UI 派兵仍走 `StrategicExpansionCommandService.DispatchCandidate`。
- 未发现 UI 直接修改 `MapData` / `PlotData.faction` / building health / unit state。

## 验证要求

- Unity Console 无 `error CS`。
- Play 初始显示 Canvas/uGUI HUD。
- 不显示旧 OnGUI debug 框。
- HUD `Dispatch` 按钮可派兵。
- 点击地图派兵仍正常。
- `O` 快捷键与 HUD Dispatch / 点击派兵同路径。
- `K/L/E/N` 在 Editor Play Mode 仍可用。
- Victory/Defeat 后显示正式结束面板。
- Restart 按钮可用。
- Victory/Defeat 后点击、HUD Dispatch、`O/E` 不再执行 gameplay command。

## 禁止范围

- 不重写整个 UI。
- 不做新玩法系统。
- 不改战斗 / 占领 / 派兵 service 行为。
- 不修改 `ProjectSettings`。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。
- 不提交 `.claude/`、`kingbattle/.idea/`。
- 不提交 `kingbattle/kingbattle.slnx`。
- 不提交 `要求.md` 删除。

## 验收标准

- Console 无编译错误。
- Candidate rows 不再每帧 Destroy/Recreate。
- Dispatch 按钮文案正式。
- `WORKLOG.md` 已记录返修和验证。
