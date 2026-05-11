# TASK.md

## 当前任务

MVP-05.1 三次返修：HUD Text 仍不可见，需要用可见性探针定位并修复渲染链路。

Codex 当前仍作为 Tech Lead 和 Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
e9f19b5 fix: add LayoutElement preferredHeight to CreateText/CreateLinkedText for visible Text
```

Codex Review 结论：

```text
MVP-05.1 仍暂不通过。
Play Mode 无脚本错误，HUD 面板显示，但 Text 仍不可见。
```

## 已确认做对的部分

- 本次只改 `GameHud.cs` 和 `WORKLOG.md`。
- Play Mode 可进入。
- Console 出现 `[GameHud] uGUI HUD initialized.`。
- 最近 Editor log 无 `error CS`、`ArgumentException`、`NullReferenceException`。
- 最近 Editor log 未发现 LayoutElement duplicate / AddComponent 异常。
- Candidate rows 仍使用 `candidatesDirty` 触发式刷新，没有回到每帧 Destroy/Recreate。
- UI 派兵仍走 `StrategicExpansionCommandService.DispatchCandidate(...)`。
- Claude 这次 remote 正确，`origin/claude/ecstatic-tu-0afd0f` 已同步到 `e9f19b5`。

## 当前执行状态：Claude 停在未提交 WIP

Codex 检查到 Claude worktree：

```text
/Users/jianghao/unity/.claude/worktrees/ecstatic-tu-0afd0f
```

当前有未提交改动：

```text
M WORKLOG.md
M kingbattle/Assets/Scripts/UI/GameHud.cs
?? .claude/
```

其中 `GameHud.cs` 已临时加入：

```text
TestProbe
HUD TEXT TEST
TestProbe2
NULL FONT TEST
```

这说明 Claude 已开始做 Text 可见性探针，但尚未完成验证和正式修复。

重要要求：

- 不要直接提交当前 WIP。
- 必须先 Play 验证探针是否可见。
- 根据探针结果修正式 HUD Text。
- 最终提交前必须删除 `TestProbe` / `TestProbe2` / `HUD TEXT TEST` / `NULL FONT TEST`。
- 如果继续沿用当前 WIP，提交前必须再次 `rg "TestProbe|HUD TEXT TEST|NULL FONT TEST|TEMP" kingbattle/Assets/Scripts/UI/GameHud.cs`，结果必须为空。

## 阻塞问题 A：HUD Text 仍不可见

Play Mode 观察：

- 左上 HUD 面板可见。
- 面板内 Objective、Enemy Attack、stats、Expansion Targets、candidate rows、Dispatch 文案仍不可见。
- 这说明仅补 `LayoutElement.preferredHeight` 仍未修复 Text 渲染/布局链路。

要求下一轮不要继续盲调数值，必须先做最小可见性定位：

1. 在 `GameHudCanvas` 下临时创建一个固定位置的 `Text` 探针：
   - 文案：`HUD TEXT TEST`
   - 白色。
   - 字号 24 或更大。
   - `RectTransform` 固定在左上，例如 `anchoredPosition = (16, -16)`，`sizeDelta = (360, 40)`。
2. Play 验证：
   - 如果探针可见，说明字体/Canvas 没问题，问题在 HUD panel 的 layout/child Text 尺寸或层级。
   - 如果探针也不可见，说明字体/Canvas/Text 渲染链路有问题，需要优先修这个。
3. 定位后删除或隐藏探针，不要把测试文字作为正式 UI 留下。

建议修复方向：

- 对所有 Text GameObject 显式配置 `RectTransform.sizeDelta`，不要只依赖 layout preferred height。
- 对 panel 内 Text 设置 `LayoutElement.minHeight` 和 `preferredHeight`。
- Candidate row 的 `HorizontalLayoutGroup.childForceExpandWidth` 可以保持 false，但每个子 Text/Button 必须有明确宽高。
- 避免对同一个 GameObject 多次 `AddComponent<LayoutElement>()`；helper 可返回已有 LayoutElement 或提供参数设置宽高。
- 可先把 HUD 背景 alpha 临时调高、Text 改纯白，确保对比度。

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

任务 1：做 Text 可见性探针

- 在 `GameHudCanvas` 下临时创建固定 RectTransform 的白色 `Text`，显示 `HUD TEXT TEST`。
- Play 验证它是否可见。
- 用验证结果判断是 Canvas/Text 渲染问题，还是 HUD panel/layout 问题。
- 最终提交不要保留测试文字。
- 当前 Claude WIP 已经加了探针；下一步不是继续加探针，而是运行 Play、记录结论，然后删除探针。

任务 2：修复正式 HUD Text 显示

- 显式设置 Text RectTransform 尺寸。
- 给主要文本和 candidate row 设置合理 min/preferred 高度。
- 确保 Objective、stats、candidate rows、Dispatch 按钮肉眼可读。
- 避免同一个对象重复添加 `LayoutElement`。
- 不要只继续改 CanvasScaler 或继续加字体。

任务 3：保持已完成边界

- 保留 `ConstantPixelSize` 或同等可读缩放策略。
- 保留较短的 HUD feedback 文案。
- 保留 `ready` / `need more` 或更清晰短文案。
- 保持 candidate rows 触发式刷新。
- HUD Dispatch 继续调用 command service。

任务 4：Play 验证和 WORKLOG

- Console 无 `error CS`。
- Play 后无 `ArgumentException` / `NullReferenceException`。
- HUD 面板内文字清晰可见。
- Candidate rows 与 `Dispatch` 按钮可见。
- HUD Dispatch、地图点击派兵、`O` 快捷键继续同路径。
- `K/L/E/N` 正常。
- Victory/Defeat 面板与 Restart 正常。
- 更新 `WORKLOG.md`。

任务 5：Git push 前检查

- push 前执行 `pwd`、`git status -sb`、`git remote -v`。
- 确认 `origin` 是 `https://12342023@github.com/12342023/unity.git`。
- 使用 `git push origin HEAD:claude/ecstatic-tu-0afd0f`。
- push 前执行：

```bash
rg "TestProbe|HUD TEXT TEST|NULL FONT TEST|TEMP" kingbattle/Assets/Scripts/UI/GameHud.cs
```

该命令必须无输出。

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
