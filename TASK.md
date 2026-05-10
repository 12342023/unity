# TASK.md

## 当前任务

发布 MVP-04.5：正式输入整理、地图点击派兵、目标高亮、debug 快捷键集中。

Codex 当前仍作为 Tech Lead 和 Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
f9e4641 docs: balance tuning document, GameBalanceConfig, regression checklist
```

Codex Review 结论：

```text
MVP-04.4 通过；允许进入 MVP-04.5。
```

## 四周目标

用户当前四周目标：

```text
四周内先集中做出完整 Unity 小游戏；本阶段不实际开发微信小程序 / macOS / Android 移植版本。
```

长期目标仍包含后续移植。因此本阶段必须保持架构边界清楚，不能为了赶进度把平台、输入、UI 和核心玩法耦合在一起。

当前完成度粗估：

- 技术底座：约 87%。
- 核心玩法闭环：约 83%。
- 完整游戏体验：约 72%-75%。

## MVP-04.5 批量允许范围

本轮目标：

```text
把玩家操作从 debug/HUD-only 推进到可点击地图的试玩输入，同时集中 debug 快捷键，为后续移动端/微信输入适配留边界。
```

任务 A：新增玩家输入控制器

- 新增 `PlayerInputController` 或等价组件。
- 只负责把鼠标点击/键盘正式输入翻译成 gameplay command。
- 不直接修改 map/building/unit 内部状态。
- 点击 Player-owned 可扩张 source plot 时选中 source。
- 高亮该 source 可连接的 Neutral target。
- 再点击高亮 target 时调用 `StrategicExpansionCommandService.DispatchCandidate(...)`。
- 派兵结果写入 `GameStatusService.LastActionResult`。
- 点击无效地块时给出清晰 message，不报错刷屏。

任务 B：地图命中和高亮

- 可以扩展 `MapRenderer`，维护 plotId -> GameObject/SpriteRenderer 查表。
- 新增最小高亮能力：
  - selected source。
  - valid target。
  - 清除高亮。
- 高亮只改变视觉，不改变 `PlotData` faction 或玩法状态。
- 可以新增只读 helper，例如 `TryGetPlotAtWorldPosition(...)`。
- 不引入 TextMeshPro、新 Input System package 或第三方框架。

任务 C：统一命令路径

- HUD Dispatch、O debug、鼠标点击派兵都必须最终调用同一个 command service。
- 不复制派兵/占领逻辑。
- HUD 可以显示当前 selected source / valid target 数量，但不要做正式 UI 美术。

任务 D：debug 快捷键集中

- 新增 `DebugShortcutController` 或等价组件。
- 将 `GameEntry.Update()` 中 K/L/E/N/R/T/Y/U/I/O/P/Q 的 debug 输入迁移进去。
- 迁移后快捷键行为保持不变。
- `GameEntry` 只负责启动和 wiring，尽量不再承载大量输入分支。
- `TestUnitSpawner` 的 1-4 测试输入可以先保留，但要在 `WORKLOG.md` 标记为后续 debug-only 清理项。

任务 E：文档和验证

- 更新 `WORKLOG.md`，记录实现内容和 Play Mode 验证。
- 更新 `NEXT_STEPS.md`，把 MVP-04.5 完成/未完成项写清楚。
- 回归验证至少包含：
  - 点击 source 后出现 target 高亮。
  - 点击高亮 target 后派兵。
  - HUD Dispatch 仍可派兵。
  - O 仍可派兵且同路径。
  - Victory/Defeat 后点击和 O/HUD 都不再派兵。
  - K/L/E/N/R/T/Y/U/I/O/P/Q 行为没有回归。
  - Console 无明显错误。

## 禁止范围

- 不做正式 UI 美术。
- 不做复杂菜单系统。
- 不做存档。
- 不引入第三方框架。
- 不引入新 Input System package。
- 不做移动端/微信/macOS/Android 移植实现。
- 不重构 `UnitCombat`。
- 不重构 `PlotCaptureService`。
- 不修改 `ProjectSettings`。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `.claude/` 或 `kingbattle/.idea/`，除非用户明确要求。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。

## 验收标准

- 鼠标点击 source/target 可以完成一次派兵。
- 地图能显示 selected source 和 valid target 高亮。
- HUD Dispatch 和 O 仍走同一 command service。
- match ended 后点击输入不会继续派兵。
- Debug 快捷键集中到独立 controller，行为不回归。
- `WORKLOG.md` 记录 Play Mode 验证。
- Console 无明显错误。
- 未修改或提交 `ProjectSettings`。
