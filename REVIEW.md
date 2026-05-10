# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
92bb7f7 feat: enemy pressure AI with timed attacks, E debug shortcut
```

结论：**MVP-04.1 通过，允许进入 MVP-04.2**。

说明：敌方进攻命令服务、定时压力控制器、HUD 敌方状态、E debug 快捷键均已完成。未发现阻塞问题。

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞问题。
```

### 已确认

- `EnemyAttackCommandService` 不依赖 HUD、键盘、鼠标、OnGUI 或平台 API。
- `EnemyAttackCommandService.DispatchAttackToBestTarget(...)` 优先攻击 PlayerBase，找不到时回退到 Player-owned non-main-base plot。
- `EnemyPressureController` 只负责计时和调用 enemy attack command。
- MatchResult 已经 Victory/Defeat 后，敌方压力停止。
- E debug 快捷键复用 `EnemyPressureController.TriggerAttack()`。
- HUD 显示下一次敌方进攻倒计时和最近敌方行动。
- 新增 `.meta` 已随代码提交。
- `git show --check HEAD` 未发现 whitespace 或 patch 问题。
- `kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍未提交。

### 缺失检查

当前没有阻塞 bug，但作为完整游戏还缺这些关键项：

- 没有最小资源/人口规则，单位会持续生成，缺少约束。
- `Granary` 目前没有玩法作用，只是 visual + health。
- `Tower` 已有攻击作用，但 HUD 没展示建筑收益/状态。
- 没有胜负界面，当前只有 HUD 文字。
- 没有数值调优/回归清单。
- 工作区仍有未确认项：`.claude/`、`kingbattle/.idea/`、`kingbattle/ProjectSettings/SceneTemplateSettings.json`、`要求.md` 删除。

### 当前完成度判断

- 技术底座：约 80%。
- 核心玩法闭环：约 72%。
- 完整游戏体验：约 55%-60%。

现在游戏已经具备玩家派兵和敌方压力。下一步应该补“最小游戏性规则”：人口/补给、粮仓作用、HUD 规则状态，让它更像一局有约束的游戏。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-04.1 通过。

进入 MVP-04.2：游戏性最小系统第一步，人口/补给 + 建筑作用。

项目目标：
- 四周内先完成完整 Unity 小游戏。
- 后续仍可能做微信小程序、macOS、Android 移植。
- 所以规则计算、HUD、输入、平台能力必须继续分离。

本轮目标：
- 给单位持续生成加一个最小约束。
- 让 Granary 有明确玩法作用。
- 在 HUD 中显示玩家和敌方的单位数量 / 人口上限。
- 保持系统轻量，不做复杂资源经济。

任务 A：新增轻量规则/统计服务
- 新增 `GameRuleService`、`FactionStatsService` 或等价小服务。
- 提供只读查询：
  - 当前某 faction 存活单位数量。
  - 当前某 faction 存活 Granary 数量。
  - 当前某 faction 人口上限。
- 建议规则：
  - base supply cap = 8。
  - 每个存活 Granary +4 supply cap。
- 服务不能依赖 HUD、键盘、鼠标、OnGUI 或平台 API。

任务 B：BarracksSpawner 接入人口上限
- `BarracksSpawner` 在 SpawnUnit 前检查当前 faction 单位数是否达到 supply cap。
- 达到上限时不生成新兵，并输出节流/清晰日志。
- Player 和 Enemy 都使用同一规则。
- 不要把 HUD 逻辑写进 BarracksSpawner。
- 不要引入复杂资源、金币、粮食库存。

任务 C：HUD 显示人口/建筑状态
- 扩展临时 `GameHud`。
- 显示：
  - Player Units: current / cap。
  - Enemy Units: current / cap。
  - Player Granaries / Enemy Granaries。
  - 可选：Towers count。
- HUD 只读取规则/统计服务，不直接扫描和修改核心数据。

任务 D：建筑作用文档/日志
- 在 WORKLOG 和必要注释中明确：
  - Barracks = 生成士兵。
  - Tower = 自动攻击敌方单位。
  - Granary = 增加 supply cap。
- 不做建筑升级，不做资源产出，不做区域奖励。

任务 E：验证
- Play Mode 验证：
  - 单位数量达到 cap 后，Barracks 停止生成。
  - 摧毁 Granary 后 cap 下降，HUD 更新。
  - 重建 Granary 后 cap 上升，HUD 更新。
  - Player/Enemy 都遵守 supply cap。
  - HUD Dispatch、O、Q、U、P、E、K、L 仍可用。
  - Enemy pressure 仍能工作。
  - Console 无明显错误。
- 更新 WORKLOG.md。
- 记录是否新增 .meta，是否修改 ProjectSettings。

禁止：
- 不做复杂经济系统。
- 不做金币/粮食库存。
- 不做建筑升级。
- 不做区域奖励。
- 不做正式 UI 美术。
- 不做移动端/微信/macOS/Android 移植实现。
- 不修改 ProjectSettings。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `.claude/`、`kingbattle/.idea/`。

完成后 commit / push。
```
