# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
f9e4641 docs: balance tuning document, GameBalanceConfig, regression checklist
```

结论：**MVP-04.4 通过，允许进入 MVP-04.5**。

说明：调优文档、轻量 `GameBalanceConfig`、Play Mode 回归记录都已完成。未发现阻塞性玩法或架构问题。

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞问题。
```

### 已确认

- `BALANCE.md` 已记录 supply cap、Barracks、unit、Tower、enemy pressure、plot capture 等关键数值。
- `GameBalanceConfig` 只收口了 supply cap、enemy pressure、Tower 三组关键数值，范围符合任务要求。
- `FactionStatsService`、`EnemyPressureController`、`BuildingFactory` 使用配置后行为等价于原常量。
- `WORKLOG.md` 已记录 13 项 Play Mode 回归结果。
- 本轮未修改或提交 `ProjectSettings`。
- `git show --check HEAD` 曾发现 `BALANCE.md` 一处行尾空格；Codex 已用小补丁修复。
- `GameBalanceConfig.cs` 新增注释中的非 ASCII 装饰字符已由 Codex 改为 ASCII 注释。

### 残余风险

- Play Mode 回归由 Claude 手动记录，Codex 当前无法自动运行 Unity Editor 验证。
- 当前正式玩家输入仍依赖 HUD Dispatch 和 debug key，缺少地图点击选择/目标高亮。
- `GameEntry.Update()` 仍塞有大量 debug 快捷键，后续移植前必须集中隔离。
- `TestUnitSpawner` 仍保留 1-4 测试输入，后续应纳入 debug-only 边界。

### 当前完成度判断

- 技术底座：约 87%。
- 核心玩法闭环：约 83%。
- 完整游戏体验：约 72%-75%。

下一步应做 MVP-04.5：正式输入整理、地图点击派兵、目标高亮、debug 快捷键集中。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md、BALANCE.md。

Codex Review：MVP-04.4 通过。

进入 MVP-04.5：正式输入整理、地图点击派兵、目标高亮、debug 快捷键集中。

项目目标：
- 四周内先完成完整 Unity 小游戏。
- 后续仍可能做微信小程序、macOS、Android 移植。
- 本轮重点是输入/反馈边界，不是新增玩法系统。

任务 A：新增玩家输入控制器
- 新增 `PlayerInputController` 或等价组件。
- 只负责把鼠标点击/键盘正式输入翻译成 gameplay command。
- 不直接修改 map/building/unit 内部状态。
- 点击流程建议：
  1. 点击 Player-owned 可扩张 source plot，选中 source。
  2. 高亮该 source 可连接的 Neutral target。
  3. 再点击一个高亮 Neutral target，调用 `StrategicExpansionCommandService.DispatchCandidate(...)`。
  4. 派兵结果写入 `GameStatusService.LastActionResult`。
- 如果点击无效地块，给出清晰 message，但不要报错刷屏。
- match ended 后不允许派兵，仍走 command/service 的拒绝路径。

任务 B：地图命中和高亮
- 可以扩展 `MapRenderer`，维护 plotId -> GameObject/SpriteRenderer 查表。
- 新增最小高亮能力：
  - selected source。
  - valid target。
  - 清除高亮。
- 高亮只改变视觉，不改变 `PlotData` faction 或玩法状态。
- 可以新增只读 helper，例如 `TryGetPlotAtWorldPosition(...)`。
- 不引入 TextMeshPro、新 Input System package 或第三方框架。

任务 C：HUD 与点击输入保持同一命令路径
- HUD Dispatch、O debug、鼠标点击派兵都必须最终调用同一个 command service。
- 不复制派兵/占领逻辑。
- HUD 可以显示当前 selected source / valid target 数量，但不要做正式 UI 美术。

任务 D：debug 快捷键集中
- 新增 `DebugShortcutController` 或等价组件。
- 将 `GameEntry.Update()` 中 K/L/E/N/R/T/Y/U/I/O/P/Q 的 debug 输入迁移进去。
- 迁移后快捷键行为保持不变。
- `GameEntry` 只负责启动和 wiring，尽量不再承载大量输入分支。
- `TestUnitSpawner` 的 1-4 测试输入可以先保留，但要在 WORKLOG 标记为后续 debug-only 清理项。

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

禁止：
- 不做正式 UI 美术。
- 不做存档。
- 不做移植实现。
- 不引入新 input package。
- 不重构 `UnitCombat`。
- 不重构 `PlotCaptureService`。
- 不修改 `ProjectSettings`。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `.claude/`、`kingbattle/.idea/`、Unity 生成目录。

完成后 commit / push。
```
