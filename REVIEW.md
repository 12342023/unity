# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
ac2af48 feat: DispatchResult structured data, unified capture logs
```

结论：**MVP-03.21 通过，允许进入 MVP-03.22**。

说明：`StrategicDispatchService` 已返回结构化 `DispatchResult`，U/O 已使用统一 dispatch message；arrival handler 的 allowed / blocked 日志也已按 dispatched / required 输出。

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞问题。
```

### 已确认

- `DispatchResult` 包含 target、dispatched、required、hasEnough、willCapture 和 message。
- `DispatchToPlot(...)` 在路径无效、无兵、派兵成功时都返回结构化结果。
- U 分支改用 `DispatchResult.message` 打印统一日志。
- O 分支通过 `StrategicExpansionService.ExpandNext(...)` 继续输出统一 message。
- arrival handler 的 `Capture attempt allowed` / `Capture blocked` 均显示 dispatched / required。
- `captureConsidered` 仍防止同一轮派兵重复 TryCapture / 重复 blocked log。
- `git show --check HEAD` 未发现 whitespace 或 patch 问题。
- `kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍未提交。

### 观察

`StrategicExpansionService` 中有一句关于 required count 的 `preview only` 注释已过期：required 现在已经传入 dispatch/capture 判定。它不是行为 bug，可以在下一轮顺手修正。

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

Codex Review：MVP-03.21 通过。

进入 MVP-03.22：扩张预览与可派兵统计批量任务。

背景：
- U/O 已有结构化 DispatchResult。
- P 能打印每个 plot 的占领需求。
- O 当前会派第一个 expansion candidate。
- 未来需要正式 UI / 移动端操作前，先要有只读扩张预览数据。
- 用户希望加快进度，所以本轮打包 4 个强相关点。

本轮目标：
- 为战略扩张增加只读预览数据。
- 能看到每个 source -> target 候选需要多少兵、当前可派多少兵、是否足够占领。
- 新增 Q 快捷键打印全部预览。
- 不改变当前 O/U 的实际派兵与占领行为。

允许：
任务 A：提取可派兵统计
- 在 `StrategicDispatchService` 增加只读统计方法，或新增一个很小的 query service。
- 统计条件必须与 `DispatchToPlot(...)` 当前派兵筛选一致：
  - Player faction
  - HealthComponent 未死亡
  - 距离 rally/source 位置 <= gatherRadius
- 统计方法不能 ClearPushPath、不能 Stop、不能注册 handler、不能修改任何状态。

任务 B：新增扩张预览数据
- 在 `StrategicExpansionService` 增加 `ExpansionPreview` 或等价小数据类型。
- 字段建议：
  - sourcePlotId
  - targetPlotId
  - requiredSoldierCount
  - availableSoldierCount
  - hasEnoughSoldiers
  - message
- 增加 `GetExpansionPreviews(MapData mapData, float gatherRadius = 5f)` 或等价方法。
- 预览来源使用 `StrategicConnectionService.GetExpansionCandidates(mapData)`。
- 每个候选都计算 target required count，并统计 source 附近可派兵数量。

任务 C：新增 Q 快捷键打印全部扩张预览
- 在 `GameEntry.Update()` 增加 Q。
- Q 只打印，不派兵、不占领、不改变状态。
- 日志格式建议：
  - `Crossroads -> Village: available 2/2, canCapture=True`
  - `Crossroads -> Farmland: available 1/3, canCapture=False`
- 没有候选时打印清晰提示。

任务 D：修正过期注释
- `StrategicExpansionService` 中 required count 的 `preview only` 注释已经过期。
- 改成准确描述：required count 会传入 dispatch/capture 判定。
- 不做额外重构。

任务 E：验证
- P 仍打印需求。
- Q 能打印全部候选预览。
- 连续按 Q 不派兵、不占领、不改变 plot faction。
- O 仍派第一个候选。
- U 仍从 main-base ruin 派第一个可连接 Neutral。
- U/O 足够人数可以占领，不足人数 blocked。
- 更新 WORKLOG.md。

必须保持：
- K / L / R / T / Y / U / I / O 外部行为不变。
- Q 是新的只读测试快捷键，不能触发派兵或占领。
- 不改变当前 Neutral -> Player 捕获流程。
- 不改变 `MapData.CreateFixedMap()`。
- 不修改 `PlotCaptureService` 的 Player-only / no-main-base / Neutral-only 规则。
- 不引入长期静态游戏状态。
- 不修改 ProjectSettings。
- 不提交 kingbattle/ProjectSettings/SceneTemplateSettings.json。

禁止：
- 不做正式选择目标 UI。
- 不做自动扩张。
- 不做资源、升级、区域奖励、传送阵、AI。
- 不做占领进度条。
- 不重构 UnitCombat。
- 不重构无关建筑/移动/战斗代码。

完成后更新 WORKLOG.md，说明 A/B/C/D/E 完成情况、修改文件、P/Q/U/O Play Mode 验证结果、是否新增 .meta、是否修改 ProjectSettings，并 commit / push。
```
