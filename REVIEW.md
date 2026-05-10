# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
87dbb99 feat: expansion command service, HUD dispatch buttons, O reuses same path
```

结论：**MVP-04.0 通过，允许进入 MVP-04.1**。

说明：扩张 command 服务、HUD 候选按钮、O 快捷键复用同一路径均已完成。未发现阻塞问题。

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞问题。
```

### 已确认

- `StrategicExpansionCommandService.DispatchCandidate(...)` 是 UI-free / platform-free 的扩张命令入口。
- command 服务会验证 source/target 合法性、相邻关系、road path、占领需求，然后调用 `StrategicDispatchService.DispatchToPlot(...)`。
- `GameHud` 只读 previews 和共享状态，点击按钮只调用 command 服务。
- O 快捷键也调用 `StrategicExpansionCommandService.DispatchCandidate(...)`。
- `StrategicExpansionService.ExpandNext(...)` 已委托给 command 服务，减少重复 path/dispatch 编排。
- Q 仍只读，U/P/K/L 验证能力保留。
- `git show --check HEAD` 未发现 whitespace 或 patch 问题。
- `kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍未提交。

### 观察

HUD 现在使用 `OnGUI`，适合作为临时 debug UI。后续做正式 UI 或触摸输入时，应继续调用 command 服务，不要把按钮逻辑复制到玩法层。

### 当前完成度判断

- 技术底座：约 78%。
- 核心玩法闭环：约 68%。
- 完整游戏体验：约 50%-55%。

现在玩家已经可以通过 HUD 选择扩张目标。下一步需要加入最小敌方压力，否则游戏仍偏测试沙盒。

### 说明

工作区仍有非本轮项：

```text
D 要求.md
?? .claude/
?? kingbattle/ProjectSettings/SceneTemplateSettings.json
```

这些不应随本轮提交，除非用户明确确认。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-04.0 通过。

进入 MVP-04.1：敌方最小压力 AI。

项目目标：
- 四周内先完成完整 Unity 小游戏。
- 后续仍可能做微信小程序、macOS、Android 移植。
- 所以 AI 决策、输入/UI、平台能力必须继续分离。

本轮目标：
- 让敌人定时产生最低限度进攻压力。
- 玩家不再只是主动扩张，也需要防守。
- 先做 enemy attack，不做 enemy capture，不做完整 AI。

任务 A：新增敌方进攻命令服务
- 新增 `EnemyAttackCommandService` 或等价小服务。
- 提供类似：
  - `DispatchEnemyAttack(MapData mapData, string sourcePlotId, string targetPlotId)`
  - 或 `DispatchEnemyAttackToPlayerBase(MapData mapData)`
- 服务职责：
  - 验证 source 是 Enemy-owned plot。
  - 验证 target 是 Player-owned plot，优先 PlayerBase，允许后续扩展到 Player frontier。
  - 查找 road path。
  - 找 source 附近存活 Enemy 士兵。
  - 给这些士兵设置 push path。
  - 返回结构化结果或 message。
- 该服务不能依赖 HUD、键盘、鼠标、OnGUI 或平台 API。
- 本轮不做 enemy capture；敌人只进攻玩家建筑/士兵。

任务 B：新增敌方压力控制器
- 新增 `EnemyPressureController` 或等价 MonoBehaviour。
- 由 `GameEntry` 初始化，持有 `MapData`。
- 每隔一段时间触发一次敌方进攻，例如首次 8-12 秒，之后每 20-30 秒。
- MatchResult 已经 Victory/Defeat 时停止触发。
- 如果没有可用 Enemy 士兵、没有 road path、目标已不存在，要写清晰日志，不报错刷屏。

任务 C：HUD 显示敌方压力状态
- 扩展临时 `GameHud`，显示：
  - 最近一次敌方行动结果，或
  - 下一次敌方进攻倒计时。
- 保持 HUD 只读状态 / 调用服务，不直接改 unit/map/building。
- 可以在 `GameStatusService` 增加一个很小字段，例如 `LastEnemyActionResult`。

任务 D：Debug 验证快捷键
- 可新增一个 debug 快捷键，例如 `E`，立即触发一次 enemy attack。
- E 也必须调用同一个 enemy command 服务或 controller 方法。
- 不要让 E 写一套独立派兵逻辑。

任务 E：文档和验证
- 更新 WORKLOG.md。
- Play Mode 验证：
  - 敌方会按时间自动派兵进攻。
  - E 能立即触发一次敌方进攻。
  - Victory/Defeat 后敌方压力停止。
  - HUD 显示敌方行动状态。
  - 玩家 HUD Dispatch、O、Q、U、P、K、L 仍可用。
  - Console 无明显错误。
- 记录是否新增 .meta，是否修改 ProjectSettings。

禁止：
- 不做 enemy capture。
- 不做完整行为树 / 第三方 AI 框架。
- 不做资源、升级、区域奖励。
- 不做正式 UI 美术。
- 不做移动端/微信/macOS/Android 移植实现。
- 不修改 ProjectSettings。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。

完成后 commit / push。
```
