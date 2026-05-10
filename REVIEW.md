# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
f82cbd7 feat: expansion previews with live soldier count
```

结论：**MVP-03.22 通过，允许进入 MVP-03.23**。

说明：Q 扩张预览、可派兵统计、required 注释修正均已完成。本轮没有发现阻塞问题。

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞问题。
```

### 已确认

- `StrategicConnectionService` 已增加 `ExpansionPreview`。
- `GetExpansionPreviews(...)` 基于 `GetExpansionCandidates(...)` 返回所有候选预览。
- 每个 preview 包含 source、target、required、available、hasEnough。
- Q 快捷键只读打印，不派兵、不占领。
- `StrategicExpansionService` 过期的 `preview only` 注释已修正。
- `git diff --check` 未发现 whitespace 问题。
- `kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍未提交。

### 观察

`CountSoldiersNear(...)` 目前放在 `StrategicConnectionService` 中，短期可以接受；如果后面 dispatch 筛选条件继续扩展，可以再抽成单独 query service，避免统计与实际派兵规则漂移。

### 当前目标调整

用户已明确：

```text
四周内优先完成完整 Unity 小游戏，并要求加快节奏；后续仍可能做微信小程序、macOS、Android 移植。
```

因此后续任务从“小步底层能力”调整为“每轮 3-5 个强相关任务，优先可玩闭环”，但必须保持核心逻辑、输入/UI、平台能力边界清楚。

### 说明

工作区仍有两个非本轮项：

```text
D 要求.md
?? kingbattle/ProjectSettings/SceneTemplateSettings.json
```

前者不是本轮修改，后者是 Unity Editor 生成的 ProjectSettings 文件；均不应随本轮提交。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.22 通过。

项目目标调整：
- 四周内先完成一个完整 Unity 小游戏。
- 本阶段不实际开发微信小程序、macOS、Android 移植版本。
- 但后续仍可能移植，所以必须保持边界清楚。
- 从现在开始加快节奏，每轮做 3-5 个强相关任务。
- 但仍禁止大规模重构、无关系统、ProjectSettings 变更。

移植边界必须保持：
- 核心玩法服务不依赖键盘、鼠标、OnGUI 或平台 API。
- 输入层只把快捷键 / 点击 / 未来触摸翻译成 gameplay command。
- HUD / UI 只读取状态和发起命令，不直接改 map/building/unit 内部状态。
- 存档、音频、震动、分享、广告、资源加载等平台能力后置封装。
- 不在建筑、战斗、移动、地图逻辑里散落平台判断。

进入 MVP-03.23：四周冲刺第一轮，核心可玩闭环收口。

本轮目标：
- 占领成功/失败后的士兵行为收口。
- 最小 Victory / Defeat 状态。
- 临时 HUD 显示当前目标、候选、最近结果、胜负状态。
- 保留 Q/O/U/P 验证能力。

任务 A：占领后士兵行为收口
- 当 Player 士兵成功占领 Neutral plot 后，本次派出的存活士兵应围绕新占领 plot 巡逻。
- 如果兵力不足导致 capture blocked，本次派出的存活士兵应停留在目标附近巡逻，作为“失败后集结”状态。
- 不要让士兵短暂折返到旧来源点。
- 不重构 UnitCombat；优先在 dispatch/capture 到达回调附近小范围处理。

任务 B：最小胜负状态
- 增加一个很小的 match/game result 状态服务或组件。
- EnemyBase 被击败并完成敌方清场后，标记 Player Victory。
- PlayerBase 被击败并完成蓝方清场后，标记 Defeat。
- 重复触发 K/L 或死亡事件时不能重复刷屏。
- 先用日志验证即可，不做正式结算界面。

任务 C：最小可玩提示 HUD
- 增加临时 in-game debug HUD 或简单 OnGUI 显示，不做正式美术 UI。
- 显示：
  - 当前目标：占领 Neutral plots / 击败 EnemyBase
  - 当前扩张候选数量
  - 最近一次 dispatch / capture 结果
  - 当前胜负状态
- HUD 必须是临时可删组件，不要把 UI 逻辑散落到战斗/建筑服务里。

任务 D：Q/O/U/P 验证保留
- Q 仍只读，不派兵、不占领。
- O 仍派第一个 expansion candidate。
- U 仍从 main-base ruin 派第一个可连接 Neutral。
- P 仍打印占领需求。

任务 E：文档和验证
- 更新 WORKLOG.md。
- 记录 Play Mode 验证结果。
- 记录是否新增 .meta，是否修改 ProjectSettings。

禁止：
- 不做正式 UI 美术。
- 不做完整敌方 AI。
- 不做资源、升级、区域奖励、传送阵。
- 不做移植实现，但要保持移植边界。
- 不大改 MapData。
- 不重构 UnitCombat。
- 不重构 PlotCaptureService。
- 不修改 ProjectSettings。
- 不提交 kingbattle/ProjectSettings/SceneTemplateSettings.json。

完成后 commit / push。
```
