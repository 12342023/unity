# REVIEW.md

## Review 状态

Codex 已审查 MVP-03.12 当前代码：

```text
8b61f54 fix: compile U dispatch shortcut
```

结论：**MVP-03.12 静态代码审查通过，允许进入 MVP-03.13**。

说明：`GameEntry` 已提供 U 临时测试快捷键，可以从 main base ruin 找到第一个可连接 Neutral plot，并将附近 Player 士兵沿道路派过去。U 不改变 plot 归属，不做占领判定，不生成正式 UI。

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞问题。
```

### 已确认

- `GameEntry` 已包含 `using System.Collections.Generic;`，修复 U 快捷键新增 `List<Vector3>` 后的编译风险。
- U 使用 `RuinComponent.GetConnectableNeutralPlots(mapData)` 获取目标。
- U 使用 `RoadPathFinder.FindPath(mapData, rallyRuin.sourcePlotId, targetPlotId)` 计算道路路径。
- U 将 path plotId 转为 world waypoint。
- U 只筛选 Player 且存活的附近士兵。
- U 派兵前调用 `ClearPushPath()` 和 `UnitMovement.Stop()`，避免旧路径与新 push path 抢控制。
- U 只派兵，不改变任何 plot 的 `faction`。
- K / L / R / T / Y 行为仍保留。
- `kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍未提交。

### 验证限制

Codex 本机没有可用的 `dotnet` / `mcs` / `csc` 命令，且 shell 未找到可直接执行的 Unity Editor 二进制，所以本轮只完成静态审查与 diff 检查。

用户侧 Unity 需要确认：

```text
K -> T -> Y -> U
```

预期：

- K 后 EnemyBase 变 main base ruin。
- T 后 Player 士兵聚到 EnemyBase 废墟附近。
- Y 打印 `EnemyBase can connect to: Crossroads`。
- U 后附近 Player 士兵沿路前往 Crossroads。
- Console 无明显错误。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.12 静态代码审查通过。

进入 MVP-03.13：Neutral plot 最小占领与颜色刷新。

本轮目标：
- 不做正式 UI。
- 不做资源、升级、区域奖励、传送阵或 AI。
- 不做倒计时占领条。
- 不做敌方反夺。
- 只做最小闭环：U 派出的 Player 士兵到达第一个可连接 Neutral plot 后，该 plot 变成 Player，并且地图颜色刷新。

允许：
- 新增一个很小的服务类，例如 StrategicCaptureService 或 PlotCaptureService。
- 服务职责保持单一：
  1. 检查目标 plot 是否存在。
  2. 检查目标 plot 当前是否是 Faction.Neutral。
  3. 将目标 plot.faction 改为 Faction.Player。
  4. 输出日志。
- 可以给 MapRenderer 增加最小刷新方法，例如 RefreshPlotColor(string plotId, MapData mapData)。
- GameEntry 可以保留 MapRenderer 引用，用于 U 到达后刷新目标 plot 颜色。
- U 派兵时可以使用 UnitCombat.OnPushDestinationReached，在士兵到达目标后触发一次占领。
- 如果多个士兵都到达，只允许第一次把 Neutral 改为 Player，后续输出 already captured 或直接忽略。
- 保持 K / L / R / T / Y / U 行为不变。

必须保持：
- Main base ruin 仍不能通过 R 重建。
- Y 仍只打印可连接 Neutral 地点。
- U 仍是临时测试入口。
- 只允许占领 Neutral plot。
- 不允许占领 Enemy plot。
- 不允许占领 main base ruin 本身。
- 不新增正式 UI。
- 不修改 ProjectSettings。
- 不提交 kingbattle/ProjectSettings/SceneTemplateSettings.json。

禁止：
- 不做资源产出。
- 不做升级。
- 不做区域奖励。
- 不做传送阵。
- 不做 AI。
- 不做复杂占领进度。
- 不把平台相关逻辑写进战斗/地图/建筑脚本。

完成后更新 WORKLOG.md，说明修改文件、K/T/Y/U/R/L Play Mode 验证步骤、是否修改 ProjectSettings，并 commit / push。
```
