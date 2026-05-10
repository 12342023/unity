# TASK.md

## 当前任务

发布 MVP-04.2：游戏性最小系统第一步，人口/补给 + 建筑作用。

Codex 当前仍作为 Tech Lead 和 Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
92bb7f7 feat: enemy pressure AI with timed attacks, E debug shortcut
```

Codex Review 结论：

```text
MVP-04.1 通过；允许进入 MVP-04.2。
```

## 四周目标

用户当前四周目标：

```text
四周内先集中做出完整 Unity 小游戏；本阶段不实际开发微信小程序 / macOS / Android 移植版本。
```

长期目标仍包含后续移植。因此本阶段必须保持架构边界清楚，不能为了赶进度把平台、输入、UI 和核心玩法耦合在一起。

当前完成度粗估：

- 技术底座：约 80%。
- 核心玩法闭环：约 72%。
- 完整游戏体验：约 55%-60%。

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
- Tower 已自动攻击敌方单位。
- Granary 目前还没有玩法作用。

## 当前工作区注意事项

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
.claude/
kingbattle/.idea/
```

这些仍是未跟踪项。除非用户明确批准，否则不要提交。

## MVP-04.2 批量允许范围

本轮目标：

```text
增加最小人口/补给规则，让 Granary 有明确作用，并在 HUD 显示双方单位和建筑收益状态。
```

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

任务 E：更新文档

- 更新 `WORKLOG.md`，写明 A/B/C/D 完成情况。
- 记录 Play Mode 验证结果。
- 记录是否新增 `.meta`，是否修改 `ProjectSettings`。

## 禁止范围

- 不做复杂经济系统。
- 不做金币/粮食库存。
- 不做建筑升级。
- 不做区域奖励。
- 不做正式 UI 美术。
- 不做移动端/微信/macOS/Android 移植实现。
- 不改变 `MapData.CreateFixedMap()`，除非为了验证必须做极小配置并说明原因。
- 不重构 `UnitCombat`。
- 不重构 `PlotCaptureService`。
- 不修改 `ProjectSettings`。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `.claude/` 或 `kingbattle/.idea/`，除非用户明确要求。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。

## 验收标准

- Play Mode：
  - 单位数量达到 cap 后，Barracks 停止生成。
  - 摧毁 Granary 后 cap 下降，HUD 更新。
  - 重建 Granary 后 cap 上升，HUD 更新。
  - Player/Enemy 都遵守 supply cap。
  - HUD Dispatch 仍可派兵。
  - O/Q/U/P/E/K/L 仍可验证。
  - Enemy pressure 仍能工作。
  - Console 无明显错误。
- 代码：
  - rule/stat 服务不依赖 UI / 输入 / 平台 API。
  - BarracksSpawner 只调用规则服务，不承担 UI 状态展示。
  - HUD 只读取规则/统计服务，不直接修改核心数据。
  - 未修改或提交 `ProjectSettings`。
