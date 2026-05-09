# TASK.md

## 当前任务

发布 MVP-03.11：大本营废墟连接未占领地点数据层。

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

MVP-03.10 最新提交：

```text
6f1deab feat: main-base ruin rally and rebuildable ruin scan
```

Codex Review 结论：

```text
MVP-03.10 代码审查通过；允许进入 MVP-03.11
```

## 用户规则

用户要求：

```text
最后的敌方大本营不能重建，但是可以聚兵，也可以连接别的未占领的地方可以派兵。
```

已经完成：

- 大本营废墟不能通过普通 `BuildingRebuildService` 重建。
- 大本营废墟可以作为聚兵点。
- R 会跳过大本营废墟，继续重建普通废墟。

本轮只处理：

- 大本营废墟可以连接哪些未占领地点的数据查询。

暂不处理：

- 正式派兵系统。
- 正式 UI。

## 当前工作区注意事项

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件仍是未跟踪 Unity Editor 生成文件。除非用户明确批准，否则不要提交。

## MVP-03.11 目标

为 main base ruin 提供最小“可连接未占领地点”数据层。

```text
MainBase ruin -> neighboring neutral plots
```

## MVP-03.11 允许范围

- 修改 `RuinComponent.cs`，新增最小查询方法，例如：

```csharp
public IReadOnlyList<string> GetConnectableNeutralPlots(MapData mapData)
```

- 方法规则：
  - 如果不是 `CanUseAsRallyPoint(mapData)`，返回空列表。
  - 使用 `MapData.GetNeighbors(sourcePlotId)` 获取相邻 plot。
  - 只返回 `plot.faction == Faction.Neutral` 的邻居。
- 可以使用 `List<string>` 实现，返回 `IReadOnlyList<string>`。
- 可以在 `GameEntry` 增加临时测试快捷键，例如 `Y`。
- `Y` 的行为：
  - 找到第一个 main base ruin。
  - 打印它可连接的未占领地点列表。
  - 不派兵，不改变归属。
- 保持 `K` / `L` / `R` / `T` 行为不变。
- 更新 `WORKLOG.md`。

## MVP-03.11 禁止范围

- 不做正式连接 UI。
- 不做正式派兵系统。
- 不改变 plot 归属。
- 不做资源、占领进度、升级、区域奖励、传送阵、AI。
- 不修改 `ProjectSettings`。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。

## MVP-03.11 验收标准

- 按 K 后出现 EnemyBase main base ruin。
- 按 Y 后能在 Console 看到 EnemyBase 相邻且未占领的地点列表。
- 当前地图上 EnemyBase 的相邻点包括 `Crossroads` 和 `EnemyOutpost`；只有 `Faction.Neutral` 的点应被列出。
- Y 不派兵，不改变归属。
- R 仍跳过大本营废墟，重建普通废墟。
- T 仍能聚兵到大本营废墟。
- K / L 清场行为不变。
- Console 无明显错误。
- 仍未修改或提交 `ProjectSettings`。
