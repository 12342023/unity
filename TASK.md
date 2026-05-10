# TASK.md

## 当前任务

发布 MVP-03.23：四周完整游戏冲刺第一轮。

Codex 当前仍作为 Tech Lead / Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
f82cbd7 feat: expansion previews with live soldier count
```

Codex Review 结论：

```text
MVP-03.22 通过；项目目标切换为 4 周左右完成 Unity 完整小游戏，不做移植版。
```

## 四周目标

用户当前四周目标：

```text
四周内先集中做出完整 Unity 小游戏；本阶段不实际开发微信小程序 / macOS / Android 移植版本。
```

长期目标仍包含后续移植。因此本阶段必须保持架构边界清楚，不能为了赶进度把平台、输入、UI 和核心玩法耦合在一起。

开发节奏调整：

- 每轮给 Claude 布置 3-5 个强相关任务。
- 优先做“能玩的一条完整链路”，少做长期框架。
- 保持 `GameEntry` 尽量变薄，但可以接受临时测试 HUD / 快捷键过渡。
- 仍禁止大规模重构、无关系统、ProjectSettings 变更。

## 移植边界要求

即使当前不做移植实现，也必须按后续微信小程序、macOS、Android 的方向保留边界：

- 核心玩法逻辑只能放在小服务 / 组件中，例如 dispatch、capture、match result、AI decision。
- 输入层只负责把“点击 / 快捷键 / 触摸”翻译成 gameplay command。
- HUD / UI 只能读取状态并发起命令，不直接改 map/building/unit 内部状态。
- 平台能力必须后置封装，例如存档、音频、震动、分享、广告、包体资源加载。
- 不在 `Update()` 中散落大量平台判断或 UI 逻辑。
- 不把 Unity Editor 临时配置、ProjectSettings、生成目录作为玩法代码的一部分提交。
- 新增临时 HUD 或 debug 快捷键时，要标明 debug / temporary，后续可替换为正式 UI。

## 已完成基础能力

- 建筑被击败后变成废墟。
- 敌方大本营被击败后敌方建筑变废墟、敌方士兵死亡。
- 士兵可以围绕废墟巡逻。
- 大本营废墟不能普通重建，但可以作为聚兵点。
- U 可以从 main base ruin 派兵去第一个相邻 Neutral plot。
- Neutral -> Player 最小占领主路径已实现。
- O 可以从第一个 Player-owned frontier plot 派兵到第一个相邻 Neutral。
- `StrategicDispatchService` 已返回 `DispatchResult`。
- `StrategicConnectionService` 已提供 `ExpansionCandidate`。
- `PlotCaptureRequirementService` 已提供 Small/Medium/Large = 1/2/3。
- P 可打印占领需求。
- Q 可打印全部扩张候选的 available / required / enough。

## 当前工作区注意事项

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件仍是未跟踪 Unity Editor 生成文件。除非用户明确批准，否则不要提交。

## MVP-03.23 批量允许范围

本轮目标：

```text
把当前“可验证机制”推进成更接近可玩的核心闭环：占领后行为、胜负状态、最小游戏提示。
```

任务 A：占领后士兵行为收口

- 当 Player 士兵成功占领 Neutral plot 后，本次派出的存活士兵应围绕新占领 plot 巡逻。
- 如果兵力不足导致 capture blocked，本次派出的存活士兵应停留在目标附近巡逻，作为“失败后集结”状态。
- 不要让士兵短暂折返到旧来源点。
- 不重构 `UnitCombat`；优先在 dispatch/capture 到达回调附近做小范围处理。

任务 B：最小胜负状态

- 增加一个很小的 match/game result 状态服务或组件。
- 当 EnemyBase 被击败并完成敌方清场后，标记 Player Victory。
- 当 PlayerBase 被击败并完成蓝方清场后，标记 Defeat。
- 重复触发 K/L 或死亡事件时不能重复刷屏。
- 先用日志验证即可，不做正式结算界面。

任务 C：最小可玩提示 HUD

- 增加临时 in-game debug HUD 或简单 `OnGUI` 显示，不做正式美术 UI。
- 显示：
  - 当前目标：占领 Neutral plots / 击败 EnemyBase。
  - 当前扩张候选数量。
  - 最近一次 dispatch / capture 结果。
  - 当前胜负状态。
- HUD 必须是临时可删组件，不要把 UI 逻辑散落到战斗/建筑服务里。

任务 D：Q/O/U/P 验证保留

- Q 仍只读，不派兵、不占领。
- O 仍派第一个 expansion candidate。
- U 仍从 main-base ruin 派第一个可连接 Neutral。
- P 仍打印占领需求。

任务 E：更新文档

- 更新 `WORKLOG.md`，写明 A/B/C/D 完成情况。
- 记录 Play Mode 验证结果。
- 记录是否新增 `.meta`，是否修改 `ProjectSettings`。

## 禁止范围

- 不做正式 UI 美术。
- 不做完整敌方 AI。
- 不做资源、升级、区域奖励、传送阵。
- 不做移动端/微信/macOS/Android 移植实现，但保持后续移植边界。
- 不改变 `MapData.CreateFixedMap()`，除非为了胜负验证必须做极小配置并说明原因。
- 不重构 `UnitCombat`。
- 不重构 `PlotCaptureService`。
- 不修改 `ProjectSettings`。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。

## 验收标准

- Play Mode：
  - Q 能打印候选预览。
  - O/U 足够人数占领后，士兵围绕新占领 plot 巡逻。
  - O/U 不足人数 blocked 后，士兵不折返旧来源点。
  - K 击败 EnemyBase 后只触发一次 Victory。
  - L 击败 PlayerBase 后只触发一次 Defeat。
  - 临时 HUD 能显示目标、候选数量、最近结果、胜负状态。
  - Console 无明显错误。
- 代码：
  - `GameEntry` 不继续膨胀核心业务逻辑。
  - 新状态服务保持小而清晰。
  - 不引入长期全局复杂状态。
  - HUD / 输入逻辑不直接修改核心数据，只调用服务或读取状态。
  - 新增代码不引入平台耦合，后续可替换为触摸 UI。
  - 未修改或提交 `ProjectSettings`。
