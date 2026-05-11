# TASK.md

## 当前任务

MVP-05.0 二次返修：修复 Canvas/uGUI HUD 在 Unity 6 Play Mode 的运行时初始化错误。

Codex 当前仍作为 Tech Lead 和 Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
bafdbca fix: CS0815, per-frame candidate rebuild, Dispatch button label
```

Codex Review 结论：

```text
MVP-05.0 暂不通过。
CS0815 编译错误已修复，但 Play Mode 验证发现 GameHud 初始化失败，正式 HUD 无法显示。
```

## 已修复项

- `CreateSeparator(...)` 已改为直接调用，不再 `var sep = CreateSeparator(...)`。
- Candidate rows 已从每帧 Destroy/Recreate 改为 `candidatesDirty` 触发式刷新。
- Dispatch 按钮文案已从 `Dsp` 改为 `Dispatch`，按钮宽度已加大。
- 当前 Unity Console / Editor log 未再出现 `error CS0815` 或其他 `error CS`。

## 阻塞问题 A：Unity 6 builtin font 路径错误

Play Mode 日志：

```text
ArgumentException: Arial.ttf is no longer a valid built in font. Please use LegacyRuntime.ttf
  at UnityEngine.Resources.GetBuiltinResource[T] (System.String path)
  at GameHud.Initialize (...) (at Assets/Scripts/UI/GameHud.cs:54)
  at GameEntry.Start () (at Assets/Scripts/GameEntry.cs:71)
```

当前风险：

- `GameHud.Initialize(...)` 在取字体时抛异常。
- `CreateCanvas()` 没有完成。
- HUD 不显示。

要求最小修复：

```diff
- uiFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
+ uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
```

如需 fallback，可以保留 `Font.CreateDynamicFontFromOSFont(...)`，但不能再优先访问 Unity 6 已失效的 `Arial.ttf`。

## 阻塞问题 B：HUD 初始化失败后 Update 空引用

Play Mode 日志：

```text
NullReferenceException: Object reference not set to an instance of an object
  at GameHud.Update () (at Assets/Scripts/UI/GameHud.cs:172)
```

原因：

- 字体异常打断 `Initialize(...)`。
- `endPanelRoot` / `hudPanelRoot` 尚未创建。
- `Update()` 继续访问 UI root。

要求：

- 首要修复字体路径，让 `CreateCanvas()` 正常完成。
- 可加轻量防御：

```csharp
if (hudPanelRoot == null || endPanelRoot == null) return;
```

但不要用防御掩盖 HUD 创建失败；Play 验证必须看到正式 HUD。

## 建议小修正：首帧候选列表

当前 `cachedPreviews` 只有 2 秒定时 `RefreshData()` 后才更新。字体修复后，建议在 `Initialize(...)` / `CreateCanvas()` 完成后调用一次 `RefreshData()`，避免首帧 candidate list 短暂显示 `(none)`。

这不是本轮 P1，但属于正式 UI 体验的小收口。

## 给 Claude 的返修任务

请先阅读 `AGENTS.md`、`TASK.md`、`REVIEW.md`、`NEXT_STEPS.md`、`WORKLOG.md`。

只做小返修，不要重写整个 UI。

任务 1：修复 Unity 6 builtin font

- `GameHud.Initialize(...)` 不要再使用 `Resources.GetBuiltinResource<Font>("Arial.ttf")`。
- 改用 Unity 6 可用的 `LegacyRuntime.ttf`。
- 保留简单 fallback 可以，但不能让 HUD 初始化抛异常。

任务 2：避免初始化失败后的 NRE 噪音

- 字体修好后确认 `CreateCanvas()` 正常完成。
- 可在 `Update()` 开头加轻量 root null guard。
- 不要因此跳过 Play 验证。

任务 3：首帧刷新候选数据

- 在 HUD 初始化完成后触发一次 `RefreshData()`。
- Candidate rows 仍保持 `candidatesDirty` 触发式刷新，不要回到每帧 Destroy/Recreate。

任务 4：补完整 Play 验证和 WORKLOG

- Unity Console 无 `error CS`。
- Play 后无 `ArgumentException` / `NullReferenceException`。
- Canvas/uGUI HUD 正常显示，不显示旧 OnGUI debug 框。
- HUD `Dispatch` 按钮可派兵。
- 地图点击派兵仍正常。
- `O` 与 HUD Dispatch / 点击派兵同路径。
- `K/L/E/N` 在 Editor Play Mode 仍可用。
- Victory/Defeat 面板与 Restart 可用。
- Victory/Defeat 后点击、HUD Dispatch、`O/E` 不再执行 gameplay command。
- 更新 `WORKLOG.md`，记录返修、验证、未提交排除项。

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
- Play Mode 无 HUD runtime exception。
- Canvas/uGUI HUD 正常显示。
- Candidate rows 不每帧 Destroy/Recreate。
- Dispatch 按钮文案正式。
- `WORKLOG.md` 已记录返修和完整 Play 验证。
