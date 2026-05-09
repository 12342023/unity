# TASK.md

## 当前任务

发布 MVP-03.9：大本营废墟特殊规则。

Codex 当前仍作为 Tech Lead / Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 项目定位

这是一个 Unity 小游戏项目，当前主工程位于：

```text
kingbattle/
```

项目定位：

```text
低操作、高战略、自动战争 RTS
```

后续规划包括：

- 微信小程序移植
- macOS 移植
- Android 移植

当前阶段优先保持 Unity 工程结构清晰，不把未来平台差异散落在业务逻辑中。

## 最新 Review 结论

Claude 最新提交：

```text
32bb717 fix: add mapData null guard in BuildingRebuildService.Rebuild
```

Codex Review 结论：

```text
MVP-03.8 代码审查通过；允许进入 MVP-03.9
```

## 新增用户规则

用户新增要求：

```text
最后的敌方大本营不能重建，但是可以聚兵，也可以连接别的未占领的地方可以派兵。
```

阶段解释：

- 当前先把“大本营废墟不能普通重建”落地。
- 大本营废墟未来不是普通建筑，而是战略据点。
- 聚兵、连接未占领地、派兵会作为后续系统继续拆分。

## 当前工作区注意事项

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件仍是未跟踪 Unity Editor 生成文件。除非用户明确批准，否则不要提交。

## MVP-03.9 目标

大本营废墟不能走普通 `BuildingRebuildService` 重建流程。

当前 `R` 测试入口会找到第一个废墟并调用 `BuildingRebuildService.Rebuild(...)`。当第一个废墟是 `EnemyBase` 时，它会被重建为 Player Barracks，这与用户新增规则冲突。

本轮目标是：

```text
MainBase ruin != rebuildable building
MainBase ruin == future rally / dispatch strategic point
```

## MVP-03.9 允许范围

- 修改 `BuildingRebuildService.Rebuild(...)`。
- 使用 `mapData.GetPlot(ruin.sourcePlotId)` 查询来源 plot。
- 如果来源 plot 的 `isMainBase == true`：
  - 输出 Warning。
  - 返回 `null`。
  - 不销毁废墟。
  - 不创建新建筑。
- 可以在 `RuinComponent` 新增最小数据层方法或字段，例如：
  - `CanUseAsRallyPoint(...)`
  - `CanDispatchFrom(...)`
  - `IsMainBaseRuin(...)`
- 如果新增上述方法，本轮只表达数据边界，不实现完整聚兵 / 派兵系统。
- 保持普通非大本营废墟仍可按当前 R 测试逻辑重建。
- 更新 `WORKLOG.md`。

## MVP-03.9 禁止范围

- 不做正式重建按钮。
- 不做选择废墟 UI。
- 不做资源消耗。
- 不做占领进度。
- 不做完整连地系统。
- 不做正式聚兵 UI 或派兵 UI。
- 不做区域奖励、传送阵、AI。
- 不修改 `ProjectSettings`。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。

## MVP-03.9 验收标准

- 按 K 击败 EnemyBase 后，EnemyBase 废墟不能被 R 重建成 Player Barracks。
- EnemyBase 废墟不会因为重建失败而被销毁。
- EnemyOutpost 等非大本营废墟仍可按当前测试逻辑重建。
- 大本营废墟保留为未来聚兵 / 派兵战略据点的数据入口。
- K / L 清场行为不变。
- Console 无明显错误。
- 仍未修改或提交 `ProjectSettings`。
