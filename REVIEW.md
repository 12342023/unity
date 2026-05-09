# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
2424573 feat: GetConnectableNeutralPlots on main-base ruin, Y shortcut prints them
```

结论：**MVP-03.11 代码审查通过**。

说明：`RuinComponent.GetConnectableNeutralPlots(MapData)` 已提供 main base ruin 到相邻 Neutral 地点的数据查询；`GameEntry` 的 Y 临时测试快捷键只打印结果，不派兵、不改变 plot 归属。本轮没有引入正式 UI、资源、占领、升级、区域奖励、传送阵或 AI。

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞问题。
```

### 已确认

- `GetConnectableNeutralPlots(mapData)` 只在 `CanUseAsRallyPoint(mapData)` 为 true 时返回连接结果。
- 查询使用 `MapData.GetNeighbors(sourcePlotId)`。
- 只返回 `plot.faction == Faction.Neutral` 的邻居 plotId。
- Y 只打印连接数据，不派兵、不改归属。
- K / L / R / T 测试入口仍保留。
- `kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍未提交。

## 文档更正

Claude 的 `WORKLOG.md` 验证说明里写到 EnemyBase 可连接 `Farmland`。按当前 `MapData.CreateFixedMap()`：

```text
EnemyBase neighbours = Crossroads, EnemyOutpost
```

其中 `Crossroads` 是 Neutral，`EnemyOutpost` 是 Enemy，所以正确结果是：

```text
EnemyBase can connect to: Crossroads
```

Unity 日志也显示了这个正确结果。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.11 代码审查通过。

进入 MVP-03.12：大本营废墟临时派兵测试入口。

用户规则继续保持：
- 最后的敌方大本营不能重建。
- 它可以作为聚兵点。
- 它可以连接别的未占领地点，并从这里派兵。

本轮目标：
- 不做正式 UI。
- 不做资源、占领进度、升级、区域奖励、传送阵或 AI。
- 只做临时测试入口：从 main base ruin 派当前聚集的 Player 士兵前往第一个可连接 Neutral 地点。

允许：
- 在 GameEntry 增加临时测试快捷键，例如 U。
- U 的行为：
  1. 找到第一个 main base ruin。
  2. 调用 GetConnectableNeutralPlots(mapData)。
  3. 选择第一个 connectable neutral plot。
  4. 找到当前靠近 main base ruin 的 Player 士兵。
  5. 使用 RoadPathFinder.FindPath(mapData, ruin.sourcePlotId, targetPlotId) 计算路径。
  6. 将 path plotId 转成 world waypoint。
  7. 对这些士兵调用 UnitCombat.ClearPushPath() 后 SetPushPath(waypoints)，让他们沿路前往目标。
- 士兵筛选可以先用距离 main base ruin <= 3f 或同等简单阈值。
- 可以输出日志说明派出多少士兵、从哪个 ruin 派往哪个 plot。
- 保持 K / L / R / T / Y 行为不变。

必须保持：
- U 只是临时测试入口。
- U 不改变 plot 归属。
- U 不做占领判定。
- U 不生成 UI。
- R 仍跳过大本营废墟。
- T 仍能聚兵到大本营废墟。
- Y 仍只打印可连接 Neutral 地点。
- Console 无明显错误。

禁止：
- 不做正式派兵 UI。
- 不改变地图归属。
- 不做占领进度。
- 不做资源、升级、区域奖励、传送阵、AI。
- 不修改 ProjectSettings。
- 不提交 kingbattle/ProjectSettings/SceneTemplateSettings.json。

完成后更新 WORKLOG.md，说明修改文件、U/Y/T/R/K/L Play Mode 验证步骤、是否修改 ProjectSettings，并 commit / push。
```
