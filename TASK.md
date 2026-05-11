# TASK.md

## 当前任务

MVP-05.1 返修：修复 HUD 在当前 Unity Game view 下不可读的问题。

Codex 当前仍作为 Tech Lead 和 Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
4c51488 feat: HUD readability polish — bigger fonts, compact layout, player-friendly status text
```

Codex Review 结论：

```text
MVP-05.1 暂不通过。
Unity Play Mode 无脚本错误，但 HUD 在当前 Game view 下仍不可读，只看到左上角小深色块。
```

## 已确认做对的部分

- 提交范围较小，只改 `GameHud.cs` 和 `WORKLOG.md`。
- 未修改 `ProjectSettings`、场景文件、玩法 service。
- Candidate rows 仍使用 `candidatesDirty` 触发式刷新，没有回到每帧 Destroy/Recreate。
- UI 派兵仍走 `StrategicExpansionCommandService.DispatchCandidate(...)`。
- `LastActionResult` 只在 HUD 显示层截断，没有改 command service 语义。
- Claude 分支已由 Codex 推送成功：

```text
git push origin claude/ecstatic-tu-0afd0f
d85d121..4c51488  claude/ecstatic-tu-0afd0f -> claude/ecstatic-tu-0afd0f
```

## 阻塞问题 A：HUD 视觉仍不可读

Play Mode 观察：

- Console 出现 `[GameHud] uGUI HUD initialized.`。
- 最近 Editor log 无 `error CS`、`ArgumentException`、`NullReferenceException`。
- 但 Game view 左上角 HUD 只呈现为一个很小的深色块，文字不可读。

疑似原因：

- `CanvasScaler` 仍使用 `ScaleWithScreenSize` + `referenceResolution = 1920x1080`。
- 当前 Unity Game view 实际显示区域较小，400px 面板和 14/16 字体被按比例缩小，导致“增大字体”没有真正改善可读性。

要求最小返修：

- 优先修 `CanvasScaler` 的缩放策略，而不是继续只加字体。
- 建议二选一：
  - 改为更适合当前 16:9 小游戏 Game view 的 `referenceResolution = 960x540` 或 `800x450`。
  - 或改为 `CanvasScaler.ScaleMode.ConstantPixelSize` 并设置合理 `scaleFactor`，确保当前 Editor Game view 下文字可读。
- 保持 HUD 面板不遮挡地图核心操作区过多。
- Play 后必须肉眼可读：
  - Objective。
  - Enemy Attack。
  - Units / Bldgs stats。
  - Expansion Targets。
  - Candidate row 与 Dispatch 按钮。

## Git push 问题修正

Claude 报错：

```text
git push origin claude/ecstatic-tu-0afd0f
error: src refspec claude/ecstatic-tu-0afd0f does not match any
error: failed to push some refs to 'https://github.com:hahaaaw/-.git'
```

Codex 已检查：

- `/Users/jianghao/unity` 的 `origin` 正确：

```text
https://12342023@github.com/12342023/unity.git
```

- `/Users/jianghao/unity/.claude/worktrees/ecstatic-tu-0afd0f` 的 `origin` 也正确。
- 全局 `.gitconfig` 没有旧的 `hahaaaw/-.git`。
- 因此旧 URL 大概率来自 Claude 执行命令时所在的错误目录 / 错误仓库。

Claude 后续每次 push 前必须执行：

```bash
pwd
git status -sb
git remote -v
```

必须确认：

```text
pwd = /Users/jianghao/unity
或 pwd = /Users/jianghao/unity/.claude/worktrees/<当前分支>

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

任务 1：修复 HUD 缩放导致不可读

- 调整 `CanvasScaler` 缩放策略。
- 目标是在当前 Unity Game view 下，HUD 文字肉眼可读。
- 不要只继续加大字体。
- 不要引入 scene prefab。

任务 2：保持 MVP-05.1 已做的小改进

- 保留较短的 HUD feedback 文案。
- 保留 `ready` / `need more` 或更清晰短文案。
- 保持 candidate rows 触发式刷新。

任务 3：Play 验证和 WORKLOG

- Console 无 `error CS`。
- Play 后无 `ArgumentException` / `NullReferenceException`。
- HUD 文字清晰可读，不只是一个深色块。
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
- 当前 Unity Game view 下 HUD 文字肉眼可读。
- Candidate rows 不每帧 Destroy/Recreate。
- UI 不越过 command service 直接改业务状态。
- Claude 不再 push 到旧 remote。
- `WORKLOG.md` 已记录本轮修改和 Play 验证。
