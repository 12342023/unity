# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
4c51488 feat: HUD readability polish — bigger fonts, compact layout, player-friendly status text
```

结论：**MVP-05.1 暂不通过，需要返修。**

## CODEX PROJECT REVIEW

Gate: **FAIL**

Findings:

```text
[P1] HUD 在当前 Unity Game view 下仍不可读，只显示为左上角小深色块。
[P2] Claude push 时使用了错误仓库 remote / cwd，仍出现旧账号仓库 hahaaaw/-.git。
```

### [P1] HUD 缩放策略导致可读性目标未达成

File: `kingbattle/Assets/Scripts/UI/GameHud.cs`

Play Mode 观察：

- `[GameHud] uGUI HUD initialized.` 正常出现。
- 最近 Editor log 无 `error CS`、`ArgumentException`、`NullReferenceException`。
- 但 HUD 在当前 Game view 下只像左上角一个很小的深色块，文字不可读。

原因判断：

- Claude 增大了字体和间距，但 `CanvasScaler` 仍是：

```csharp
scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
scaler.referenceResolution = new Vector2(1920, 1080);
```

- 当前 Editor Game view 较小，UI 被 1920x1080 reference resolution 缩得过小。
- 所以“字体 14 -> 16”在实际屏幕上仍不可读。

Fix:

- 优先调整 `CanvasScaler` 缩放策略。
- 建议改为 `referenceResolution = 960x540` / `800x450`，或使用 `ConstantPixelSize`。
- 验收必须看当前 Unity Game view：Objective、stats、candidate rows、Dispatch 按钮都能读。

### [P2] Claude push 目录 / remote 错误

Claude 报错：

```text
error: src refspec claude/ecstatic-tu-0afd0f does not match any
error: failed to push some refs to 'https://github.com:hahaaaw/-.git'
```

Codex 已检查：

- `/Users/jianghao/unity` remote 正确。
- `/Users/jianghao/unity/.claude/worktrees/ecstatic-tu-0afd0f` remote 正确。
- 全局 `.gitconfig` 没有 `hahaaaw/-.git`。
- Codex 已成功推送 Claude 分支到正确仓库：

```text
git push origin claude/ecstatic-tu-0afd0f
d85d121..4c51488  claude/ecstatic-tu-0afd0f -> claude/ecstatic-tu-0afd0f
```

判断：

- Claude 很可能在错误目录 / 错误 Git 仓库里执行了 push。

Fix:

- Claude 每次 push 前必须执行：

```bash
pwd
git status -sb
git remote -v
```

- 看到 `hahaaaw/-.git` 必须停止。
- 推荐命令：

```bash
git push origin HEAD:claude/ecstatic-tu-0afd0f
```

## 已确认做对的部分

- 代码修改范围小。
- 没有改 ProjectSettings / scene / gameplay service。
- UI 仍只读状态并调用 command service。
- Candidate rows 没回到每帧 Destroy/Recreate。
- `LastActionResult` 只在 HUD 显示层截断。

## 给 Claude 的返修任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

MVP-05.1 暂不通过。脚本无错误，但 HUD 在当前 Unity Game view 下不可读。

任务 1：修复 CanvasScaler 缩放
- 不要只继续加大字体。
- 调整 referenceResolution 到 960x540 / 800x450，或改用 ConstantPixelSize。
- Play 后必须看得到 Objective、stats、candidate rows、Dispatch 按钮文字。

任务 2：保持 UI / gameplay 边界
- 不改玩法 service。
- HUD Dispatch 继续调用 StrategicExpansionCommandService.DispatchCandidate。
- Candidate rows 继续触发式刷新。

任务 3：Play 验证
- Console 无 error CS。
- 无 ArgumentException / NullReferenceException。
- HUD 不再只是左上角深色小块，文字清晰可读。
- HUD Dispatch、地图点击派兵、O、K/L/E/N、Victory/Defeat、Restart 继续正常。

任务 4：修正 push 流程
- push 前执行 pwd、git status -sb、git remote -v。
- origin 必须是 https://12342023@github.com/12342023/unity.git。
- 使用 git push origin HEAD:claude/ecstatic-tu-0afd0f。
- 如果看到 hahaaaw/-.git，立即停止。

禁止：
- 不重写整个 UI。
- 不改 ProjectSettings。
- 不提交 .claude、.idea、kingbattle.slnx、Library、Logs、UserSettings、要求.md 删除。

完成后 commit / push。
```
