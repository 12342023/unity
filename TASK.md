# TASK.md

## 当前任务

发布 MVP-04.3：胜负界面和一局结束体验。

Codex 当前仍作为 Tech Lead 和 Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
d38fb3e feat: supply cap system and building roles (Granary +4 cap)
```

Codex Review 结论：

```text
MVP-04.2 通过；允许进入 MVP-04.3。
```

## 四周目标

用户当前四周目标：

```text
四周内先集中做出完整 Unity 小游戏；本阶段不实际开发微信小程序 / macOS / Android 移植版本。
```

长期目标仍包含后续移植。因此本阶段必须保持架构边界清楚，不能为了赶进度把平台、输入、UI 和核心玩法耦合在一起。

当前完成度粗估：

- 技术底座：约 82%。
- 核心玩法闭环：约 76%。
- 完整游戏体验：约 62%-65%。

## 移植边界要求

- 核心玩法逻辑只能放在小服务 / 组件中，例如 dispatch、capture、match result、AI decision、gameplay command、rule/stat query。
- 输入层只负责把“点击 / 快捷键 / 触摸”翻译成 gameplay command。
- HUD / UI 只能读取状态并发起命令，不直接改 map/building/unit 内部状态。
- 平台能力必须后置封装，例如存档、音频、震动、分享、广告、包体资源加载。
- 不在 `Update()` 中散落大量平台判断或 UI 逻辑。
- 新增临时 HUD 或 debug 快捷键时，要标明 debug / temporary，后续可替换为正式 UI / 触摸 UI。

## 已完成基础能力

- 建筑被击败后变成废墟。
- 敌方大本营被击败后敌方建筑变废墟、敌方士兵死亡。
- 士兵可以围绕废墟和占领目标巡逻。
- Neutral -> Player 最小占领主路径已实现。
- HUD 已显示候选按钮，玩家可点击 Dispatch 派兵。
- O 与 HUD Dispatch 已复用 `StrategicExpansionCommandService`。
- 敌方会按时间自动派兵进攻，E 可立即触发。
- 最小 PlayerVictory / PlayerDefeat 状态已完成。
- supply cap 已完成：base 8，每个 Granary +4。
- Barracks 遵守 supply cap。
- HUD 已显示双方 units/cap、Granary/Tower 数。

## 当前工作区注意事项

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
.claude/
kingbattle/.idea/
```

这些仍是未跟踪项。除非用户明确批准，否则不要提交。

## MVP-04.3 批量允许范围

本轮目标：

```text
收口一局结束体验：胜负显示更明确，match ended 后阻止继续 gameplay command，并提供最小 restart debug 能力。
```

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

## 禁止范围

- 不做正式 UI 美术。
- 不做复杂菜单系统。
- 不做存档。
- 不做移动端/微信/macOS/Android 移植实现。
- 不改变 `MapData.CreateFixedMap()`。
- 不重构 `UnitCombat`。
- 不重构 `PlotCaptureService`。
- 不修改 `ProjectSettings`。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `.claude/` 或 `kingbattle/.idea/`，除非用户明确要求。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。

## 验收标准

- Play Mode：
  - Victory 后 HUD 显示明显结果。
  - Defeat 后 HUD 显示明显结果。
  - Victory/Defeat 后 HUD Dispatch、O、E 不再派兵。
  - 敌方压力 controller 不再触发。
  - Q/P 仍可只读验证。
  - N 能 restart，或明确记录为什么暂不可做。
  - Console 无明显错误。
- 代码：
  - gameplay command 统一检查 match ended。
  - HUD 不直接修改核心数据。
  - restart debug 不修改 ProjectSettings。
  - 未修改或提交 `ProjectSettings`。
