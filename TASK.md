# TASK.md

## 当前任务

MVP-05.1：正式 UI 视觉与交互反馈打磨。

Codex 当前仍作为 Tech Lead 和 Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
d85d121 fix: Unity 6 LegacyRuntime.ttf, first-frame RefreshData, Update NRE guard
```

Codex Review 结论：

```text
MVP-05.0 二次返修通过。
可以进入 MVP-05.1 UI 视觉与反馈打磨。
```

## MVP-05.0 已确认通过项

- `CreateSeparator(...)` 已改为直接调用，`CS0815` 编译错误消失。
- Candidate rows 已从每帧 Destroy/Recreate 改为 `candidatesDirty` 触发式刷新。
- Dispatch 按钮文案已从 `Dsp` 改为 `Dispatch`。
- `GameHud` 已改用 Unity 6 可用的 `LegacyRuntime.ttf`。
- `CreateCanvas()` 完成后立即 `RefreshData()`，避免首帧候选列表延迟。
- `Update()` 增加轻量 root null guard，避免初始化失败时持续 NRE 噪音。
- Play Mode 验证看到 `[GameHud] uGUI HUD initialized.`。
- Editor log 最近 500 行无 `error CS`、`ArgumentException`、`NullReferenceException`。
- UI 仍只读状态、调用 `StrategicExpansionCommandService.DispatchCandidate(...)`，未直接修改 `MapData` / `PlotData.faction` / building health / unit state。

## 当前给 Claude 的任务：MVP-05.1

请先阅读 `AGENTS.md`、`TASK.md`、`REVIEW.md`、`NEXT_STEPS.md`、`WORKLOG.md`。

只做正式 UI 小步打磨，不要重写整个 UI，不要改玩法 service。

任务 1：改善 HUD 可读性与布局

- 当前 HUD 在 Unity Game view 左上显示为较紧凑的深色面板。
- 调整 Canvas/HUD panel 的尺寸、字体、间距，让核心文字可读。
- 保持 runtime uGUI 创建方式，不引入 scene prefab。
- 不要遮挡地图核心操作区过多。

任务 2：改善 candidate rows 展示

- Candidate row 继续最多显示 4 条即可。
- 保持 `Dispatch` 文案。
- `source -> target available/required` 要清晰可读。
- `enough/short` 状态可以改成更玩家化的短文案，但不要引入复杂 UI 状态机。
- Candidate rows 仍不能每帧 Destroy/Recreate。

任务 3：改善反馈文案

- `LastActionResult` 中过长或偏 debug 的文案，可以在 HUD 层做短显示。
- 不要改 command service 的结构化结果语义。
- 鼠标点击选择、HUD Dispatch、`O` 快捷键应继续走同一 command service。

任务 4：完整 Play 回归并更新 WORKLOG

- Unity Console 无 `error CS`。
- Play 后无 `ArgumentException` / `NullReferenceException`。
- Canvas/uGUI HUD 显示清晰，不显示旧 OnGUI debug 框。
- HUD `Dispatch` 按钮可派兵。
- 地图点击派兵仍正常。
- `O` 与 HUD Dispatch / 点击派兵同路径。
- `K/L/E/N` 在 Editor Play Mode 仍可用。
- Victory/Defeat 面板与 Restart 可用。
- Victory/Defeat 后点击、HUD Dispatch、`O/E` 不再执行 gameplay command。
- 更新 `WORKLOG.md`，记录修改、验证、未提交排除项。

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

- Console 无编译错误和 HUD runtime exception。
- HUD 可读性明显优于 MVP-05.0 初版。
- Candidate rows 不每帧 Destroy/Recreate。
- UI 不越过 command service 直接改业务状态。
- `WORKLOG.md` 已记录本轮修改和 Play 验证。
