# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
d38fb3e feat: supply cap system and building roles (Granary +4 cap)
```

结论：**MVP-04.2 通过，允许进入 MVP-04.3**。

说明：人口/补给规则、Granary 作用、HUD units/cap 显示均已完成。未发现阻塞问题。

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞问题。
```

### 已确认

- `FactionStatsService` 是无状态只读查询服务，不依赖 HUD、输入或平台 API。
- supply cap 规则已实现：base 8，每个存活 Granary +4。
- `BarracksSpawner` 在生成单位前调用 `FactionStatsService.CanSpawn(faction)`。
- Player 和 Enemy 都使用同一套 supply cap 规则。
- supply-capped 日志已节流，避免刷屏。
- `GameHud` 显示 Player/Enemy units/cap、Granary/Tower 数量。
- `git show --check HEAD` 未发现 whitespace 或 patch 问题。
- `kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍未提交。

### 缺失检查

当前没有阻塞 bug，但完整游戏还缺这些关键项：

- 胜利/失败后仍可能继续点 HUD Dispatch，match end 状态没有完全收口。
- 还没有明显的一局结束面板 / restart 提示。
- gameplay command 服务还需要在 match ended 后统一拒绝执行。
- 还缺数值调优和完整 Play Mode 回归清单。
- 工作区仍有未确认项：`.claude/`、`kingbattle/.idea/`、`kingbattle/ProjectSettings/SceneTemplateSettings.json`、`要求.md` 删除。

### 当前完成度判断

- 技术底座：约 82%。
- 核心玩法闭环：约 76%。
- 完整游戏体验：约 62%-65%。

下一步应收口“一局结束体验”：Victory/Defeat 显示更明确，结束后禁用正式 gameplay commands，并准备回归清单。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-04.2 通过。

进入 MVP-04.3：胜负界面和一局结束体验。

项目目标：
- 四周内先完成完整 Unity 小游戏。
- 后续仍可能做微信小程序、macOS、Android 移植。
- 所以 match flow、UI、输入和平台能力必须继续分离。

本轮目标：
- 胜利/失败后给玩家清晰反馈。
- 一局结束后阻止继续派兵/进攻等 gameplay command。
- 增加最小 restart debug 能力。
- 开始整理 Play Mode 回归清单。

任务 A：match end 命令收口
- 在玩家扩张 command、敌方进攻 command 等 gameplay command 入口检查 `MatchResultService.CurrentResult`。
- 如果 match 已经 PlayerVictory / PlayerDefeat，返回失败结果或清晰 message。
- HUD Dispatch、O、E 都应自然走到同一套拒绝逻辑。
- K/L 作为 debug 触发胜负可以保留。

任务 B：胜负结束面板
- 扩展临时 `GameHud` 或新增小型 `GameEndHud`。
- Victory / Defeat 时显示更明显的结束区域：
  - Result: Victory / Defeat。
  - 最终 Player Units / Cap。
  - 最终 Enemy Units / Cap。
  - 简短提示：Press N to restart / 或 Restart 按钮。
- 不做正式 UI 美术，不做复杂动画。
- HUD 仍只读状态，不直接改核心数据。

任务 C：Restart debug 能力
- 新增一个 debug 快捷键，例如 `N`，在 match ended 后重启当前场景或重新初始化当前 GameEntry。
- 优先选择最小、安全的方式。
- 不要修改 ProjectSettings。
- 如果用 SceneManager，需要确保当前场景可 reload；如果不可行，先用日志提示并在 WORKLOG 说明。

任务 D：结束后输入/按钮表现
- match ended 后：
  - HUD Dispatch 按钮不可用或点击返回 “match ended”。
  - O/E 不再真正派兵。
  - 敌方压力 controller 不再触发。
  - Q/P 这类只读 debug 可以保留。

任务 E：回归清单雏形
- 新增或更新 `NEXT_STEPS.md` / `WORKLOG.md`，列出 Play Mode 回归清单。
- 至少包含：
  - Play 初始 HUD。
  - HUD Dispatch。
  - supply cap。
  - enemy pressure。
  - Victory。
  - Defeat。
  - Restart debug。

验证：
- Play Mode 验证：
  - Victory 后 HUD 显示明显结果。
  - Defeat 后 HUD 显示明显结果。
  - Victory/Defeat 后 HUD Dispatch、O、E 不再派兵。
  - Q/P 仍可只读验证。
  - N 能 restart，或明确记录为什么暂不可做。
  - Console 无明显错误。
- 更新 WORKLOG.md。
- 记录是否新增 .meta，是否修改 ProjectSettings。

禁止：
- 不做正式 UI 美术。
- 不做复杂菜单系统。
- 不做存档。
- 不做移动端/微信/macOS/Android 移植实现。
- 不修改 ProjectSettings。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `.claude/`、`kingbattle/.idea/`。

完成后 commit / push。
```
