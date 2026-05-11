# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
bafdbca fix: CS0815, per-frame candidate rebuild, Dispatch button label
```

结论：**MVP-05.0 暂不通过，需要二次返修。**

## CODEX PROJECT REVIEW

Gate: **FAIL**

Findings:

```text
[P1] Unity 6 Play Mode 下 GameHud 使用失效 builtin font，HUD 初始化直接抛 ArgumentException。
[P1] HUD 初始化被打断后 GameHud.Update 访问 endPanelRoot，持续 NullReferenceException。
[P3] 首帧 candidate list 可能先显示 (none)，2 秒后才刷新真实候选。
```

### 已确认修复

- `CreateSeparator(...)` 不再赋值给 `var`，`CS0815` 已消失。
- `RebuildCandidateRows()` 已从 `UpdateHudUI()` 移出，不再每帧 Destroy/Recreate。
- Dispatch 按钮已显示完整文案 `Dispatch`。
- UI 派兵仍走 `StrategicExpansionCommandService.DispatchCandidate(...)`。
- 未发现 UI 直接修改 `MapData` / `PlotData.faction` / building health / unit state。

### [P1] Unity 6 builtin font 路径失效

Play Mode 日志：

```text
ArgumentException: Arial.ttf is no longer a valid built in font. Please use LegacyRuntime.ttf
  at UnityEngine.Resources.GetBuiltinResource[T] (System.String path)
  at GameHud.Initialize (...) (at Assets/Scripts/UI/GameHud.cs:54)
  at GameEntry.Start () (at Assets/Scripts/GameEntry.cs:71)
```

问题代码：

```csharp
uiFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
```

Unity 6 下应做最小修复：

```diff
- uiFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
+ uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
```

Fix:

- 不要继续访问 `Arial.ttf`。
- 可保留 `Font.CreateDynamicFontFromOSFont(...)` fallback。
- 修复后重新 Play，确认 HUD 正常显示。

### [P1] 初始化失败后的 Update NRE

Play Mode 日志：

```text
NullReferenceException: Object reference not set to an instance of an object
  at GameHud.Update () (at Assets/Scripts/UI/GameHud.cs:172)
```

问题：

- 字体异常打断 `Initialize(...)`。
- `CreateCanvas()` 没完成，`hudPanelRoot` / `endPanelRoot` 为空。
- `Update()` 继续访问 `endPanelRoot.activeSelf`。

Fix:

- 先修字体路径，让 HUD 初始化成功。
- 可加轻量 guard：

```csharp
if (hudPanelRoot == null || endPanelRoot == null) return;
```

但验收必须以 HUD 正常显示为准，不能只靠 guard 静默跳过。

### [P3] 首帧候选数据刷新较晚

当前：

- `candidatesDirty = true` 会让第一帧重建 rows。
- 但 `cachedPreviews` 初始为空。
- 真实候选要等 2 秒定时 `RefreshData()` 后才出现。

建议：

- `CreateCanvas()` 完成后调用一次 `RefreshData()`。
- 继续保留 `candidatesDirty` 触发式刷新。

## 给 Claude 的返修任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

MVP-05.0 仍暂不通过。CS0815 已修复，但 Play Mode 发现 HUD runtime blocker。

任务 1：修复 Unity 6 builtin font
- GameHud.Initialize 不要再调用 Resources.GetBuiltinResource<Font>("Arial.ttf")。
- 改用 LegacyRuntime.ttf。
- 保留 fallback 可以，但 HUD 初始化不能抛 ArgumentException。

任务 2：清理初始化失败后的 NRE
- 字体修复后确认 CreateCanvas 正常完成。
- 可在 GameHud.Update 开头加 hudPanelRoot/endPanelRoot null guard。
- 不能只用 guard 掩盖 HUD 未创建问题。

任务 3：首帧刷新 candidate previews
- HUD 初始化完成后调用一次 RefreshData()。
- Candidate rows 仍不要每帧 Destroy/Recreate。

任务 4：完整 Play 验证并更新 WORKLOG
- Console 无 error CS。
- Play 后无 ArgumentException / NullReferenceException。
- Canvas/uGUI HUD 显示，不显示旧 OnGUI debug 框。
- HUD Dispatch、地图点击派兵、O 快捷键都正常。
- K/L/E/N 正常。
- Victory/Defeat 面板与 Restart 正常。
- Victory/Defeat 后点击、HUD Dispatch、O/E 不再执行 gameplay command。

禁止：
- 不重写整个 UI。
- 不改战斗/占领/派兵 service 行为。
- 不改 ProjectSettings。
- 不提交 .claude、.idea、kingbattle.slnx、Library、Logs、UserSettings、要求.md 删除。

完成后 commit / push。
```
