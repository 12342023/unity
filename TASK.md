# TASK.md

## 当前任务

发布 MVP-04.1：敌方最小压力 AI。

Codex 当前仍作为 Tech Lead / Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
87dbb99 feat: expansion command service, HUD dispatch buttons, O reuses same path
```

Codex Review 结论：

```text
MVP-04.0 通过；允许进入 MVP-04.1。
```

## 四周目标

用户当前四周目标：

```text
四周内先集中做出完整 Unity 小游戏；本阶段不实际开发微信小程序 / macOS / Android 移植版本。
```

长期目标仍包含后续移植。因此本阶段必须保持架构边界清楚，不能为了赶进度把平台、输入、UI 和核心玩法耦合在一起。

当前完成度粗估：

- 技术底座：约 78%。
- 核心玩法闭环：约 68%。
- 完整游戏体验：约 50%-55%。

## 移植边界要求

- 核心玩法逻辑只能放在小服务 / 组件中，例如 dispatch、capture、match result、AI decision、gameplay command。
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
- Q 可打印全部扩张候选的 available / required / enough。
- HUD 已显示候选按钮，玩家可点击 Dispatch 派兵。
- O 与 HUD Dispatch 已复用 `StrategicExpansionCommandService`。
- 最小 PlayerVictory / PlayerDefeat 状态已完成。
- 临时 `GameHud` 已显示目标、候选数量、最近结果、胜负状态。

## 当前工作区注意事项

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
.claude/
```

这些仍是未跟踪项。除非用户明确批准，否则不要提交。

## MVP-04.1 批量允许范围

本轮目标：

```text
让敌人定时产生最低限度进攻压力，让玩家需要防守；先做 enemy attack，不做 enemy capture。
```

任务 A：新增敌方进攻命令服务

- 新增 `EnemyAttackCommandService` 或等价小服务。
- 提供类似：
  - `DispatchEnemyAttack(MapData mapData, string sourcePlotId, string targetPlotId)`。
  - 或 `DispatchEnemyAttackToPlayerBase(MapData mapData)`。
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

任务 E：更新文档

- 更新 `WORKLOG.md`，写明 A/B/C/D 完成情况。
- 记录 Play Mode 验证结果。
- 记录是否新增 `.meta`，是否修改 `ProjectSettings`。

## 禁止范围

- 不做 enemy capture。
- 不做完整行为树 / 第三方 AI 框架。
- 不做资源、升级、区域奖励、传送阵。
- 不做正式 UI 美术。
- 不做移动端/微信/macOS/Android 移植实现。
- 不改变 `MapData.CreateFixedMap()`，除非为了验证必须做极小配置并说明原因。
- 不重构 `UnitCombat`。
- 不重构 `PlotCaptureService`。
- 不修改 `ProjectSettings`。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `.claude/`，除非用户明确要求。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。

## 验收标准

- Play Mode：
  - 敌方会按时间自动派兵进攻。
  - E 能立即触发一次敌方进攻。
  - Victory/Defeat 后敌方压力停止。
  - HUD 显示敌方行动状态。
  - 玩家 HUD Dispatch 仍可派兵。
  - O/Q/U/P/K/L 仍可验证。
  - Console 无明显错误。
- 代码：
  - enemy command 服务不依赖 UI / 输入 / 平台 API。
  - controller 只负责计时和调用 command 服务。
  - HUD 不直接修改核心数据。
  - `GameEntry` 不继续膨胀敌方 AI 逻辑。
  - 未修改或提交 `ProjectSettings`。
