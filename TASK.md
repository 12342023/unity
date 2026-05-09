# TASK.md

## 当前任务

发布 MVP-03.12：大本营废墟临时派兵测试入口。

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

MVP-03.11 最新提交：

```text
2424573 feat: GetConnectableNeutralPlots on main-base ruin, Y shortcut prints them
```

Codex Review 结论：

```text
MVP-03.11 代码审查通过；允许进入 MVP-03.12
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

本轮只处理：

- 从大本营废墟临时派兵到第一个可连接 Neutral 地点。

暂不处理：

- 正式派兵 UI。
- 占领和归属变化。

## 当前工作区注意事项

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件仍是未跟踪 Unity Editor 生成文件。除非用户明确批准，否则不要提交。

## MVP-03.12 目标

提供一个临时 Play Mode 测试入口，验证：

```text
main base ruin -> connected neutral plot -> dispatch nearby Player soldiers
```

## MVP-03.12 允许范围

- 在 `GameEntry` 增加临时测试快捷键，例如 `U`。
- `U` 的行为：
  - 找到第一个 main base ruin。
  - 调用 `GetConnectableNeutralPlots(mapData)`。
  - 选择第一个可连接 Neutral plot。
  - 找到当前靠近 main base ruin 的 Player 士兵。
  - 使用 `RoadPathFinder.FindPath(mapData, ruin.sourcePlotId, targetPlotId)` 计算路径。
  - 将 path plotId 转成 world waypoint。
  - 对这些士兵调用 `UnitCombat.ClearPushPath()` 后 `UnitCombat.SetPushPath(waypoints)`。
- 士兵筛选先用简单距离，例如离 main base ruin `<= 3f`。
- 输出日志：派出多少士兵、从哪个 ruin 到哪个目标 plot。
- 保持 `K` / `L` / `R` / `T` / `Y` 行为不变。
- 更新 `WORKLOG.md`。

## MVP-03.12 禁止范围

- 不做正式派兵 UI。
- 不改变 plot 归属。
- 不做占领进度。
- 不做资源、升级、区域奖励、传送阵、AI。
- 不修改 `ProjectSettings`。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。

## MVP-03.12 验收标准

- 按 K 后出现 EnemyBase main base ruin。
- 按 T 后 Player 士兵聚到 EnemyBase 废墟附近。
- 按 Y 能打印 EnemyBase 可连接的 Neutral 地点。
- 按 U 后，靠近 EnemyBase 废墟的 Player 士兵沿道路向第一个可连接 Neutral 地点移动。
- U 不改变任何 plot 归属。
- R 仍跳过大本营废墟并重建普通废墟。
- K / L 清场行为不变。
- Console 无明显错误。
- 仍未修改或提交 `ProjectSettings`。
