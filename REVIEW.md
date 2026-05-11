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

## 给 Claude 的返修任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

MVP-05.1 仍暂不通过。面板可见，但 Text 内容不可见。

任务 1：做 Text 可见性探针
- 在 GameHudCanvas 下临时创建固定 RectTransform 的白色 Text，显示 HUD TEXT TEST。
- Play 验证它是否可见。
- 如果可见，说明 Canvas/font 没问题，修 HUD panel/layout 子树。
- 如果不可见，优先修 Canvas/Text/font 渲染链路。
- 最终提交不要保留测试文字。

任务 2：修复正式 HUD Text
- 显式设置 Text RectTransform 尺寸。
- 给主要文本和 candidate row 设置合理 min/preferred 高度。
- 确保 Objective、stats、candidate rows、Dispatch 按钮肉眼可读。
- 避免同一个对象重复添加 LayoutElement。
- 不要只继续改 CanvasScaler 或继续加字体。

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

禁止：
- 不重写整个 UI。
- 不改 ProjectSettings。
- 不提交 .claude、.idea、kingbattle.slnx、Library、Logs、UserSettings、要求.md 删除。

完成后 commit / push。
```
