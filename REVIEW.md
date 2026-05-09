# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
712cd39 refactor: extract dispatch + capture handler to StrategicDispatchService
```

结论：**MVP-03.14 代码审查通过，允许进入 MVP-03.15**。

说明：U 派兵、附近士兵筛选、capture handler 管理、到达后占领触发已经从 `GameEntry` 下沉到 `StrategicDispatchService`。`GameEntry` 的 U 分支明显变薄，玩法表现保持不变。

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞问题。
```

### 已确认

- 新增 `kingbattle/Assets/Scripts/Combat/StrategicDispatchService.cs`。
- 新增脚本 `.meta` 已提交。
- `StrategicDispatchService.DispatchToPlot(...)` 负责筛选附近 Player 存活士兵。
- 服务内部负责 `ClearPushPath()`、`UnitMovement.Stop()`、`SetPushPath(...)`。
- 服务内部负责 U 专用 capture handler 注册、替换和 self cleanup。
- 到达后仍调用 `PlotCaptureService.TryCapture(...)`。
- `GameEntry.Start()` 已调用 `StrategicDispatchService.Reset()`。
- `GameEntry` 的 U 分支只保留 main base ruin 查询、Neutral 目标选择、路径计算和服务调用。
- K / L / R / T / Y / U 行为保持。
- `kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍未提交。

### 说明

`git show --check HEAD` 会报告 Unity `.meta` 里的空值字段存在尾随空格，例如 `userData:`。这是当前仓库 Unity `.meta` 文件的一贯格式，本轮不作为阻塞问题，也不要求为此批量改动历史 `.meta` 文件。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.14 代码审查通过。

进入 MVP-03.15：占领后的下一层可连接 Neutral 查询。

背景：
- 当前 main base ruin 可以通过 Y 查询相邻 Neutral。
- U 可以从 main base ruin 派兵占领 Crossroads。
- Crossroads 被占领后变为 Player。
- 下一步先不要做正式 UI，也不要直接扩展复杂派兵。
- 先做数据层/临时验证：已占领的 Player plot 能查询它相邻的 Neutral plot。

本轮目标：
- 不改变当前 K / L / R / T / Y / U 行为。
- 不做正式 UI。
- 不做资源、升级、区域奖励、传送阵、AI。
- 不做占领进度条。
- 新增一个最小查询能力：给定一个 Player-owned plot，返回它相邻的 Neutral plot。

允许：
- 新增小服务类，例如：
  `kingbattle/Assets/Scripts/Map/StrategicConnectionService.cs`
  或放在更合适的现有命名空间，但职责必须清晰。
- 提供方法，例如：
  `GetConnectableNeutralPlotsFromOwnedPlot(MapData mapData, string sourcePlotId, Faction faction)`
- 方法规则：
  1. source plot 必须存在。
  2. source plot 必须属于传入 faction，例如 Faction.Player。
  3. source plot 不能是 Neutral。
  4. 只返回相邻且 `plot.faction == Faction.Neutral` 的 plotId。
  5. 不改变任何 plot 归属。
- 可以在 GameEntry 增加临时测试快捷键，例如 I。
- I 的行为：
  1. 找到第一个 Player-owned 且非 main base 的 plot，例如 Crossroads。
  2. 打印它可连接的 Neutral 邻居。
  3. 不派兵，不占领。
- 如果 Crossroads 还没被占领，I 可以输出 no owned frontier plot。
- 新增脚本必须提交 `.meta`。
- 更新 WORKLOG.md。

必须保持：
- Y 仍只查询 main base ruin 相邻 Neutral。
- U 仍只从 main base ruin 派兵到第一个相邻 Neutral。
- Crossroads 被 U 占领后，再按 I 应能看到它下一层 Neutral 连接，例如 Village / Farmland（按当前地图邻接关系）。
- I 不改变归属，不派兵。
- PlotCaptureService 规则不变。
- StrategicDispatchService 行为不变。
- 不修改 ProjectSettings。
- 不提交 kingbattle/ProjectSettings/SceneTemplateSettings.json。

禁止：
- 不做正式选择目标 UI。
- 不做多点派兵。
- 不做自动扩张。
- 不做资源、升级、区域奖励、传送阵、AI。
- 不改变 MapData 初始地图。
- 不重构 UnitCombat。
- 不改 K/L/R/T/Y/U 的现有行为。

完成后更新 WORKLOG.md，说明修改文件、I/Y/U/K/T/R/L Play Mode 验证结果、是否新增 .meta、是否修改 ProjectSettings，并 commit / push。
```
