# TASK.md

## 当前任务

发布 MVP-04.0：玩家正式操作第一步，候选按钮 + gameplay command 边界。

Codex 当前仍作为 Tech Lead / Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
d5b46ed feat: patrol after capture, victory/defeat state, and game HUD
```

Codex Review 结论：

```text
MVP-03.23 通过；允许进入 MVP-04.0。
```

## 四周目标

用户当前四周目标：

```text
四周内先集中做出完整 Unity 小游戏；本阶段不实际开发微信小程序 / macOS / Android 移植版本。
```

长期目标仍包含后续移植。因此本阶段必须保持架构边界清楚，不能为了赶进度把平台、输入、UI 和核心玩法耦合在一起。

当前完成度粗估：

- 技术底座：约 75%。
- 核心玩法闭环：约 65%。
- 完整游戏体验：约 45%-50%。

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
- 大本营废墟不能普通重建，但可以作为聚兵点。
- Neutral -> Player 最小占领主路径已实现。
- `StrategicDispatchService` 已返回 `DispatchResult`。
- P 可打印占领需求。
- Q 可打印全部扩张候选的 available / required / enough。
- O 可以从第一个 Player-owned frontier plot 派兵。
- 最小 PlayerVictory / PlayerDefeat 状态已完成。
- 临时 `GameHud` 已显示目标、候选数量、最近结果、胜负状态。

## 当前工作区注意事项

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
.claude/
```

这些仍是未跟踪项。除非用户明确批准，否则不要提交。

## MVP-04.0 批量允许范围

本轮目标：

```text
把“按 O 自动派第一个候选”推进成“玩家从 HUD 候选按钮里选择目标并派兵”，同时建立可移植的 gameplay command 边界。
```

任务 A：新增扩张命令服务

- 新增 `StrategicExpansionCommandService` 或等价小服务。
- 提供类似：
  - `DispatchExpansionCandidate(MapData mapData, MapRenderer mapRenderer, string sourcePlotId, string targetPlotId)`
  - 或 `DispatchExpansionCandidate(..., StrategicConnectionService.ExpansionPreview preview)`
- 该服务负责：
  - 验证 source/target 仍是合法 expansion candidate。
  - 查找 road path。
  - 计算 target required count。
  - 调用 `StrategicDispatchService.DispatchToPlot(...)`。
  - 返回 `ExpansionResult` 或等价结构化结果。
- 该服务不能依赖键盘、鼠标、OnGUI 或平台 API。

任务 B：HUD 候选按钮

- 扩展临时 `GameHud`。
- 显示当前 expansion previews 列表，至少显示前 4 个候选。
- 每项显示：
  - source -> target。
  - available / required。
  - enough / not enough。
  - Dispatch 按钮。
- 点击按钮调用任务 A 的 command 服务派兵。
- HUD 不直接修改 map/building/unit，只调用 command 服务并显示返回结果。

任务 C：O 快捷键复用 command 服务

- `GameEntry` 的 O 分支改为：
  - 获取第一个 expansion candidate / preview。
  - 调用同一个 command 服务。
- 不再在 `StrategicExpansionService` 和 `GameEntry` 中保留两套路径/派兵编排逻辑。
- U 可以暂时保留为 main-base ruin debug 快捷键。

任务 D：保持移植边界

- HUD 仍标记为 temporary/debug。
- 本轮不做正式美术 UI。
- 本轮不做世界点击 raycast。
- 本轮不做触摸输入实现。
- 但结构要保证后续可替换为手机触摸按钮。

任务 E：更新文档

- 更新 `WORKLOG.md`，写明 A/B/C/D 完成情况。
- 记录 Play Mode 验证结果。
- 记录是否新增 `.meta`，是否修改 `ProjectSettings`。

## 禁止范围

- 不做正式 UI 美术。
- 不做完整敌方 AI。
- 不做资源、升级、区域奖励、传送阵。
- 不做移动端/微信/macOS/Android 移植实现。
- 不改变 `MapData.CreateFixedMap()`。
- 不重构 `UnitCombat`。
- 不重构 `PlotCaptureService`。
- 不修改 `ProjectSettings`。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `.claude/`，除非用户明确要求。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。

## 验收标准

- Play Mode：
  - HUD 能列出至少前 4 个扩张候选。
  - HUD 每个候选显示 source -> target、available / required、enough 状态。
  - 点击 HUD Dispatch 能派兵到对应目标。
  - O 与 HUD Dispatch 使用同一条 command 服务路径。
  - Q 仍只读，不派兵、不占领。
  - U/P/K/L 仍能用于验证。
  - Console 无明显错误。
- 代码：
  - command 服务不依赖 UI / 输入 / 平台 API。
  - HUD 不直接修改核心数据。
  - `GameEntry` 不继续膨胀路径/派兵业务逻辑。
  - 未修改或提交 `ProjectSettings`。
