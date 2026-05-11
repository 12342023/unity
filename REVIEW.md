# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
e9f19b5 fix: add LayoutElement preferredHeight to CreateText/CreateLinkedText for visible Text
```

结论：**MVP-05.1 仍暂不通过，需要三次返修。**

## CODEX PROJECT REVIEW

Gate: **FAIL**

Findings:

```text
[P1] HUD 面板可见，但所有 Text 内容仍不可见。
```

### [P1] LayoutElement 高度补丁未修复 Text 可见性

File: `kingbattle/Assets/Scripts/UI/GameHud.cs`

Play Mode 观察：

- `[GameHud] uGUI HUD initialized.` 正常出现。
- 最近 Editor log 无 `error CS`、`ArgumentException`、`NullReferenceException`。
- 最近 Editor log 未发现 LayoutElement duplicate / AddComponent 异常。
- HUD 面板可见。
- 但面板内 Objective、stats、candidate rows、Dispatch 文案仍不可见。

判断：

- 单纯补 `LayoutElement.preferredHeight` 没有修复问题。
- 下一步需要先做最小 Text 可见性探针，定位到底是 Canvas/Text 渲染链路，还是 panel/layout 子树问题。

Fix:

- 临时在 `GameHudCanvas` 下创建固定 RectTransform 白色 `Text`，显示 `HUD TEXT TEST`。
- 如果探针可见，再修 HUD panel 子树 layout。
- 如果探针不可见，先修 Canvas/Text/font 渲染链路。
- 最终提交不要保留测试文字。
- 修正式 HUD 时显式设置 Text RectTransform 尺寸和 LayoutElement，避免重复 AddComponent。

## 已确认做对的部分

- 代码修改范围小。
- 没有改 ProjectSettings / scene / gameplay service。
- UI 仍只读状态并调用 command service。
- Candidate rows 没回到每帧 Destroy/Recreate。
- Claude 这次已推送到正确 remote：`origin/claude/ecstatic-tu-0afd0f = e9f19b5`。

## 当前 WIP 状态

Claude worktree 当前有未提交改动：

```text
/Users/jianghao/unity/.claude/worktrees/ecstatic-tu-0afd0f

M WORKLOG.md
M kingbattle/Assets/Scripts/UI/GameHud.cs
?? .claude/
```

未提交 `GameHud.cs` 中已经出现测试探针：

```text
TestProbe
HUD TEXT TEST
TestProbe2
NULL FONT TEST
TEMP
```

这些只能用于 Play 定位，不允许进入正式提交。

## 探针验证结论

Codex 已临时把 Claude worktree 中带探针的 `GameHud.cs` 同步到当前 Unity 主工程进行 Play 验证，验证后已恢复主工程文件，未提交临时改动。

结果：

```text
绿色 HUD TEXT TEST：可见
红色 NULL FONT TEST：未看到
```

结论：

- Canvas + UnityEngine.UI.Text 渲染链路正常。
- `LegacyRuntime.ttf` 字体正常。
- 当前问题不是字体问题。
- 当前问题在 HUD panel/layout 子树。
- 下一步应删除探针，修 HUD panel 内 Text 的 RectTransform/LayoutGroup/层级，不要再排查字体。

## 给 Claude 的返修任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

MVP-05.1 仍暂不通过。面板可见，但 Text 内容不可见。

任务 1：删除探针并记录结论
- 删除 TestProbe / TestProbe2 / HUD TEXT TEST / NULL FONT TEST / TEMP 注释。
- 在 WORKLOG.md 记录：绿色 HUD TEXT TEST 可见，红色 NULL FONT TEST 未看到。
- 结论：LegacyRuntime.ttf 可用，问题在 HUD panel/layout 子树。

任务 2：修复正式 HUD Text
- 显式设置 Text RectTransform 尺寸。
- 给主要文本和 candidate row 设置合理 min/preferred 高度。
- 确保 Objective、stats、candidate rows、Dispatch 按钮肉眼可读。
- 避免同一个对象重复添加 LayoutElement。
- 不要只继续改 CanvasScaler 或继续加字体。
- 可以先用固定 anchoredPosition/sizeDelta 的简单 HUD layout 恢复可读性，不要求继续保留当前 VerticalLayoutGroup 方案。

任务 3：保持 UI / gameplay 边界
- 不改玩法 service。
- HUD Dispatch 继续调用 StrategicExpansionCommandService.DispatchCandidate。
- Candidate rows 继续触发式刷新。

任务 4：Play 验证
- Console 无 error CS。
- 无 ArgumentException / NullReferenceException。
- HUD 面板内文字清晰可见。
- HUD Dispatch、地图点击派兵、O、K/L/E/N、Victory/Defeat、Restart 继续正常。

任务 5：push 前检查
- push 前执行 pwd、git status -sb、git remote -v。
- origin 必须是 https://12342023@github.com/12342023/unity.git。
- 使用 git push origin HEAD:claude/ecstatic-tu-0afd0f。
- push 前执行 rg "TestProbe|HUD TEXT TEST|NULL FONT TEST|TEMP" kingbattle/Assets/Scripts/UI/GameHud.cs，必须无输出。

禁止：
- 不重写整个 UI。
- 不改 ProjectSettings。
- 不提交 .claude、.idea、kingbattle.slnx、Library、Logs、UserSettings、要求.md 删除。

完成后 commit / push。
```
