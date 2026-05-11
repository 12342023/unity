# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
4b6d255 fix: switch CanvasScaler to ConstantPixelSize for readable text at any resolution
```

结论：**MVP-05.1 仍暂不通过，需要二次返修。**

## CODEX PROJECT REVIEW

Gate: **FAIL**

Findings:

```text
[P1] HUD 面板变大了，但 Text 内容仍不可见/不可读。
```

### [P1] Text 布局 / 渲染未达到可读目标

File: `kingbattle/Assets/Scripts/UI/GameHud.cs`

Play Mode 观察：

- `[GameHud] uGUI HUD initialized.` 正常出现。
- 最近 Editor log 无 `error CS`、`ArgumentException`、`NullReferenceException`。
- 改成 `ConstantPixelSize` 后，HUD 面板变大。
- 但面板内看不到 Objective、stats、candidate rows、Dispatch 文案。

判断：

- 这轮已证明不是单纯 scaler 问题。
- 下一轮应检查 uGUI `Text` 子物体的 RectTransform / LayoutElement / preferred height / 渲染层级。

Fix:

- 不要继续只改字体或 CanvasScaler。
- 给 `CreateText(...)` / `CreateLinkedText(...)` 生成的 Text 补明确 layout 尺寸。
- 给 candidate row 自身补 `LayoutElement.minHeight`。
- 确保 Text 是高对比白色，且显示在 panel Image 之上。
- Play 验收必须能读到 Objective、stats、candidate rows、Dispatch。

## 已确认做对的部分

- 代码修改范围小。
- 没有改 ProjectSettings / scene / gameplay service。
- UI 仍只读状态并调用 command service。
- Candidate rows 没回到每帧 Destroy/Recreate。
- Claude 这次已推送到正确 remote：`origin/claude/ecstatic-tu-0afd0f = 4b6d255`。

## 给 Claude 的返修任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

MVP-05.1 仍暂不通过。ConstantPixelSize 让 HUD 面板变大了，但 Text 仍不可见。

任务 1：修复 Text 可见性
- 检查 CreateText / CreateLinkedText 生成的 Text 是否有正确 RectTransform / LayoutElement。
- 给主要文本加合理高度，例如 18-24 px。
- 给 candidate row 加合理高度，例如 24-28 px。
- 确保白色文字在半透明黑底上可见。
- 不要只继续改 CanvasScaler 或继续加字体。

任务 2：保持 UI / gameplay 边界
- 不改玩法 service。
- HUD Dispatch 继续调用 StrategicExpansionCommandService.DispatchCandidate。
- Candidate rows 继续触发式刷新。

任务 3：Play 验证
- Console 无 error CS。
- 无 ArgumentException / NullReferenceException。
- HUD 面板内文字清晰可见。
- HUD Dispatch、地图点击派兵、O、K/L/E/N、Victory/Defeat、Restart 继续正常。

任务 4：push 前检查
- push 前执行 pwd、git status -sb、git remote -v。
- origin 必须是 https://12342023@github.com/12342023/unity.git。
- 使用 git push origin HEAD:claude/ecstatic-tu-0afd0f。

禁止：
- 不重写整个 UI。
- 不改 ProjectSettings。
- 不提交 .claude、.idea、kingbattle.slnx、Library、Logs、UserSettings、要求.md 删除。

完成后 commit / push。
```
