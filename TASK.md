# TASK.md

## 当前任务

发布 MVP-03.13：Neutral plot 最小占领与颜色刷新。

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

MVP-03.12 当前代码：

```text
8b61f54 fix: compile U dispatch shortcut
```

Codex Review 结论：

```text
MVP-03.12 静态代码审查通过；允许进入 MVP-03.13
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
- Y 可以打印大本营废墟可连接的相邻 Neutral 地点。
- U 可以从大本营废墟向第一个可连接 Neutral plot 临时派兵。

本轮只处理：

- U 派出的 Player 士兵到达 Neutral plot 后，该 plot 变成 Player，并刷新地图颜色。

暂不处理：

- 正式派兵 UI。
- 占领进度条。
- 敌方反夺。
- 资源、升级、区域奖励、传送阵、AI。

## 当前工作区注意事项

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件仍是未跟踪 Unity Editor 生成文件。除非用户明确批准，否则不要提交。

## MVP-03.13 目标

提供一个最小占领闭环：

```text
main base ruin -> U dispatch -> Player unit reaches Neutral plot -> plot faction becomes Player -> plot colour refreshes
```

## MVP-03.13 允许范围

- 可以新增小服务类，例如：

```text
kingbattle/Assets/Scripts/Map/PlotCaptureService.cs
```

- 服务职责：
  - 接收 `MapData`、`plotId`、`Faction.Player`。
  - 只允许目标 plot 当前是 `Faction.Neutral` 时占领。
  - 将 `plot.faction` 改为 `Faction.Player`。
  - 返回 bool 或简单结果，便于日志判断。
- 可以给 `MapRenderer` 增加最小刷新能力，例如：

```csharp
public void RefreshPlotColor(string plotId, MapData mapData)
```

- `GameEntry` 可以保留 `MapRenderer` 引用。
- U 派兵时可以使用 `UnitCombat.OnPushDestinationReached`：
  - 士兵到达目标后尝试占领。
  - 多个士兵到达时，只允许第一次从 Neutral 改为 Player。
  - 后续到达不应重复改变状态。
- 保持 `K` / `L` / `R` / `T` / `Y` / `U` 行为不变。
- 更新 `WORKLOG.md`。

## MVP-03.13 禁止范围

- 不做正式派兵 UI。
- 不做占领进度条。
- 不做敌方反夺。
- 不允许占领 Enemy plot。
- 不允许占领 main base ruin 本身。
- 不做资源、升级、区域奖励、传送阵、AI。
- 不修改 `ProjectSettings`。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。

## MVP-03.13 验收标准

- 按 K 后出现 EnemyBase main base ruin。
- 按 T 后 Player 士兵聚到 EnemyBase 废墟附近。
- 按 Y 能打印 EnemyBase 可连接的 Neutral 地点，例如 `Crossroads`。
- 按 U 后，靠近 EnemyBase 废墟的 Player 士兵沿道路向 Crossroads 移动。
- 士兵到达 Crossroads 后：
  - `Crossroads` 的 `faction` 从 Neutral 变为 Player。
  - Console 输出清晰占领日志。
  - Crossroads 颜色刷新为 Player 颜色。
- U 不应占领 Enemy plot。
- R 仍跳过大本营废墟并重建普通废墟。
- K / L 清场行为不变。
- Console 无明显错误。
- 仍未修改或提交 `ProjectSettings`。
