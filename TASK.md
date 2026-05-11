# TASK.md

## 当前任务

MVP-05.1 二次返修：HUD 面板变大后文字仍不可见，需要修复 uGUI Text 布局/渲染。

Codex 当前仍作为 Tech Lead 和 Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
4b6d255 fix: switch CanvasScaler to ConstantPixelSize for readable text at any resolution
```

Codex Review 结论：

```text
MVP-05.1 仍暂不通过。
ConstantPixelSize 已让 HUD 面板变大，但文字仍不可见/不可读。
```

## 已确认做对的部分

- 本次只改 `GameHud.cs` 和 `WORKLOG.md`。
- `CanvasScaler` 已从 `ScaleWithScreenSize + 1920x1080` 改成 `ConstantPixelSize + scaleFactor 1`。
- Play Mode 可进入。
- Console 出现 `[GameHud] uGUI HUD initialized.`。
- 最近 Editor log 无 `error CS`、`ArgumentException`、`NullReferenceException`。
- Candidate rows 仍使用 `candidatesDirty` 触发式刷新，没有回到每帧 Destroy/Recreate。
- UI 派兵仍走 `StrategicExpansionCommandService.DispatchCandidate(...)`。
- Claude 这次 remote 正确，`origin/claude/ecstatic-tu-0afd0f` 已同步到 `4b6d255`。

## 阻塞问题 A：Text 未显示

Play Mode 观察：

- 左上 HUD 面板已经比上一轮更大。
- 但面板内 Objective、Enemy Attack、stats、Expansion Targets、candidate rows、Dispatch 文案仍不可见/不可读。
- 当前视觉结果仍不能算“正式 UI 可读”。

疑似方向：

- 这已经不只是 CanvasScaler 缩放问题。
- 需要检查 `Text` 子物体是否有合理 `RectTransform` 尺寸。
- 需要检查 `VerticalLayoutGroup` 与 Text preferred/min height 的配合。
- 需要检查 panel `Image` 与 child `Text` 的渲染层级。
- 需要确认字体 `LegacyRuntime.ttf` 在当前 Text 组件中实际渲染。

要求最小返修：

- 不要重写整个 UI。
- 不要改玩法 service。
- 在 `CreateText(...)` / `CreateLinkedText(...)` / candidate row 创建处补齐必要的 uGUI layout 信息。
- 建议：
  - 给 Text GameObject 增加 `LayoutElement`，设置合理 `minHeight` / `preferredHeight`。
  - Candidate row 自身设置 `LayoutElement.minHeight`。
  - Dispatch button 设置足够 `minWidth` / `minHeight`。
  - 必要时显式设置 Text `rectTransform.sizeDelta`。
  - 让文本颜色临时保持高对比白色，不要降低透明度。
- Play 后必须能肉眼读到：
  - Objective。
  - Enemy Attack。
  - Units / Bldgs stats。
  - Expansion Targets。
  - Candidate row。
  - Dispatch 按钮。

## Git push 规则

Claude 后续每次 push 前必须执行：

```bash
pwd
git status -sb
git remote -v
```

必须确认：

```text
origin = https://12342023@github.com/12342023/unity.git
```

推荐 push 命令：

```bash
git push origin HEAD:claude/ecstatic-tu-0afd0f
```

如果看到 `hahaaaw/-.git`，立即停止，不要继续 push。

## 给 Claude 的返修任务

请先阅读 `AGENTS.md`、`TASK.md`、`REVIEW.md`、`NEXT_STEPS.md`、`WORKLOG.md`。

只做小返修，不要重写整个 UI，不要改玩法 service。

任务 1：修复 Text 可见性

- 检查 `CreateText(...)` 和 `CreateLinkedText(...)` 生成的 Text 是否有正确 RectTransform / LayoutElement。
- 给主要文本加合理高度，例如 18-24 px。
- 给 candidate row 加合理高度，例如 24-28 px。
- 确保白色文字在半透明黑底上可见。
- 不要只继续改 CanvasScaler 或继续加字体。

任务 2：保持已完成边界

- 保留 `ConstantPixelSize` 或同等可读缩放策略。
- 保留较短的 HUD feedback 文案。
- 保留 `ready` / `need more` 或更清晰短文案。
- 保持 candidate rows 触发式刷新。
- HUD Dispatch 继续调用 command service。

任务 3：Play 验证和 WORKLOG

- Console 无 `error CS`。
- Play 后无 `ArgumentException` / `NullReferenceException`。
- HUD 面板内文字清晰可见。
- Candidate rows 与 `Dispatch` 按钮可见。
- HUD Dispatch、地图点击派兵、`O` 快捷键继续同路径。
- `K/L/E/N` 正常。
- Victory/Defeat 面板与 Restart 正常。
- 更新 `WORKLOG.md`。

任务 4：Git push 前检查

- push 前执行 `pwd`、`git status -sb`、`git remote -v`。
- 确认 `origin` 是 `https://12342023@github.com/12342023/unity.git`。
- 使用 `git push origin HEAD:claude/ecstatic-tu-0afd0f`。

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
- 当前 Unity Game view 下 HUD 面板内文字肉眼可读。
- Candidate rows 不每帧 Destroy/Recreate。
- UI 不越过 command service 直接改业务状态。
- Claude 不再 push 到旧 remote。
- `WORKLOG.md` 已记录本轮修改和 Play 验证。
