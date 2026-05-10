# TASK.md

## 当前任务

发布 MVP-03.22：扩张预览与可派兵统计批量任务。

Codex 当前仍作为 Tech Lead / Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
ac2af48 feat: DispatchResult structured data, unified capture logs
```

Codex Review 结论：

```text
MVP-03.21 通过；允许进入 MVP-03.22
```

## 本轮目标

继续采用“每轮 2-4 个强相关任务”的节奏。本轮不改正式 UI，先补“扩张预览 + 可派兵统计”，让后续选择目标、占领失败提示、移动端 UI 都有只读数据基础。

目标：

```text
为战略扩张增加只读预览数据：候选 source -> target、占领需求、当前可派兵数量、是否足够占领。
```

## 当前工作区注意事项

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件仍是未跟踪 Unity Editor 生成文件。除非用户明确批准，否则不要提交。

## 已完成基础能力

- U 可以从 main base ruin 派兵去第一个相邻 Neutral plot。
- U 到达 Crossroads 后可将 Crossroads 从 Neutral 改为 Player。
- Crossroads 颜色会刷新为 Player 颜色。
- Crossroads 变 Player 后，I 可以查询它相邻的 Neutral plot。
- O 可以从第一个 Player-owned frontier plot 派兵到第一个相邻 Neutral。
- O 到达目标后复用现有 capture 流程，将目标 Neutral 改为 Player。
- U 派兵与 capture handler 管理已下沉到 `StrategicDispatchService`。
- `StrategicConnectionService` 已提供 Player-owned frontier 查询。
- `StrategicExpansionService` 已承接 O 的扩张编排。
- `StrategicConnectionService` 已提供 `ExpansionCandidate` 和 `GetExpansionCandidates(mapData)`。
- `PlotCaptureRequirementService` 已提供 Small/Medium/Large = 1/2/3。
- P 可打印每个 plot 的占领需求。
- O 日志已能显示 dispatched / required 预览。
- `StrategicDispatchService.DispatchToPlot(...)` 已返回 `DispatchResult`。
- U/O 日志已统一为 dispatched / required / willCapture。

## 文档更正

当前 `MapData.CreateFixedMap()` 中：

```text
Village = Neutral
Farmland = Neutral
```

所以 Crossroads 被占领后，I 的合理输出应包含：

```text
Village, Farmland
```

不要把 Village 写成 Player，除非本轮代码显式改变了 `Village.faction`。

## MVP-03.22 批量允许范围

任务 A：提取可派兵统计

- 在 `StrategicDispatchService` 增加只读统计方法，或新增一个很小的 query service。
- 统计逻辑必须复用 `DispatchToPlot(...)` 的筛选条件：
  - `Faction.Player`。
  - `HealthComponent` 未死亡。
  - 与 rally/source 位置距离 `<= gatherRadius`。
- 统计方法不能清空路径、不能 Stop、不能注册 handler、不能改变游戏状态。

任务 B：新增扩张预览数据

- 在 `StrategicExpansionService` 增加 `ExpansionPreview` 或等价小数据类型。
- 字段建议：
  - sourcePlotId。
  - targetPlotId。
  - requiredSoldierCount。
  - availableSoldierCount。
  - hasEnoughSoldiers。
  - message。
- 增加 `GetExpansionPreviews(MapData mapData, float gatherRadius = 5f)` 或等价方法。
- 预览来源应基于 `StrategicConnectionService.GetExpansionCandidates(mapData)`。
- 每个候选都应计算 target 的 required count，并统计 source 附近可派兵数量。

任务 C：新增 Q 快捷键打印全部扩张预览

- 在 `GameEntry.Update()` 增加 Q 测试快捷键。
- Q 只打印，不派兵、不占领、不改变状态。
- 日志格式建议：
  - `Crossroads -> Village: available 2/2, canCapture=True`
  - `Crossroads -> Farmland: available 1/3, canCapture=False`
- 如果没有候选，打印清晰提示。

任务 D：顺手修正误导注释

- `StrategicExpansionService` 里关于 required count 的 `preview only` 注释已经过期。
- 改成准确描述：required count 会传入 dispatch/capture 判定。
- 不做额外重构。

任务 E：验证

- P 仍打印需求。
- Q 能打印全部候选预览。
- Q 连续按多次不派兵、不触发占领、不注册旧 handler。
- O 仍按原逻辑派出第一个候选。
- U 仍按原逻辑从 main-base ruin 派兵。
- U/O 足够人数可以占领，不足人数 blocked。
- 更新 `WORKLOG.md`。

## 禁止范围

- 不改变 K / L / R / T / Y / U / I / O 的外部行为。
- 不做正式派兵 UI。
- 不做自动扩张。
- 不做正式多目标选择策略。
- 不做占领进度条。
- 不做敌方反夺。
- 不做资源、升级、区域奖励、传送阵、AI。
- 不改变 `MapData.CreateFixedMap()`。
- 不重构 `UnitCombat`。
- 不重构 `StrategicDispatchService`。
- 不重构 `PlotCaptureService`。
- 不重构无关建筑、移动、战斗代码。
- 不修改 `ProjectSettings`。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。

## 验收标准

- Play Mode：
  - P 能打印每个 plot 的占领需求。
  - Q 能打印所有扩张候选的 available / required / canCapture。
  - Q 不派兵、不占领、不改变任何 plot faction。
  - O/U 日志格式一致，包含 dispatched / required / willCapture。
  - 到达后成功/失败日志清晰。
  - 不足人数不占领。
  - 足够人数可占领。
  - 重复按 U/O 不触发旧 handler。
- `StrategicDispatchService` handler 清理逻辑仍正确。
- `PlotCaptureService` 的基本规则不变。
- I 不派兵、不占领。
- Y / U 原有行为不变。
- R 仍跳过大本营废墟。
- K / L 清场行为不变。
- Console 无明显错误。
- 仍未修改或提交 `ProjectSettings`。
