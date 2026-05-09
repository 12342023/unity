# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
ab78ee4 fix: uCaptureHandlers dict prevents stale handler on re-press U
```

结论：**MVP-03.13 代码审查通过，允许进入 MVP-03.14**。

说明：Neutral -> Player 最小占领闭环已经成立；U 的 capture handler 生命周期也已补上“到达前重复按 U”的替换逻辑。当前仍是临时测试入口，但已经足够进入派兵边界整理。

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞问题。
```

### 已确认

- `GameEntry.Start()` 已调用 `PlotCaptureService.Reset()`。
- `PlotCaptureService.TryCapture()` 已限制只有 `Faction.Player` 可以占领。
- `PlotCaptureService.TryCapture()` 已拒绝 `plot.isMainBase`。
- `TryCapture` 仍只允许 `plot.faction == Faction.Neutral`。
- `MapRenderer.RefreshPlotColor()` 能按 plotId 刷新地块颜色。
- U 到达 Crossroads 后可以触发 Crossroads Neutral -> Player。
- U capture handler 已使用 `uCaptureHandlers` 字典记录。
- 给同一单位注册新的 U capture handler 前，会移除旧 handler。
- handler 触发后会从 `OnPushDestinationReached` 和字典中移除。
- 最新 WORKLOG 已说明 Crossroads 被占领后再次 Y / U 不再把 Crossroads 当作 Neutral。
- `kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍未提交。

### 残留风险

U / capture / handler 逻辑现在集中在 `GameEntry` 内，`GameEntry` 已经变厚。下一步应做边界整理：把临时 U 派兵流程下沉到小服务，保持行为不变，为后续微信小程序、macOS、Android 移植预留更清晰的业务边界。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.13 代码审查通过。

进入 MVP-03.14：派兵边界整理，把 U 临时派兵逻辑从 GameEntry 下沉到小服务。

本轮目标：
- 不改变玩法表现。
- 不做正式 UI。
- 不做资源、升级、区域奖励、传送阵、AI。
- 不做占领进度条。
- 只整理 U 派兵与 capture handler 的代码边界，让 GameEntry 变薄。

允许：
- 新增小服务类，例如：
  `kingbattle/Assets/Scripts/Combat/StrategicDispatchService.cs`
- 该服务可以负责：
  1. 找到靠近 main base ruin 的 Player 存活士兵。
  2. 根据 pathIds / waypoints 派兵。
  3. 管理 U 专用 capture handler 字典。
  4. 注册 one-shot capture handler。
  5. 在重复派兵时移除旧 U capture handler。
  6. 到达后调用 PlotCaptureService.TryCapture(...)。
- GameEntry 仍可以负责临时快捷键 K/L/R/T/Y/U 的入口判断。
- GameEntry 的 U 分支应尽量只做：
  1. 找 main base ruin。
  2. 取 GetConnectableNeutralPlots(mapData) 第一个目标。
  3. 算 RoadPathFinder path。
  4. 调用 StrategicDispatchService 执行派兵。
- 保持 K / L / R / T / Y / U 行为不变。
- 新增脚本必须提交 `.meta`。
- 更新 WORKLOG.md。

必须保持：
- U 仍只是临时测试入口。
- U 到达 Crossroads 后仍能占领并刷新颜色。
- Crossroads 变 Player 后，再次 Y / U 不应再把 Crossroads 当作 Neutral。
- Main base ruin 仍不能通过 R 重建。
- PlotCaptureService 的 Player-only / no-main-base / Neutral-only 规则不变。
- 不修改 ProjectSettings。
- 不提交 kingbattle/ProjectSettings/SceneTemplateSettings.json。

禁止：
- 不做正式派兵 UI。
- 不做占领进度条。
- 不做敌方反夺。
- 不做资源、升级、区域奖励、传送阵、AI。
- 不改变 MapData 初始地图。
- 不重构 UnitCombat。
- 不改 K/L/R/T/Y 的现有行为。

完成后更新 WORKLOG.md，说明修改文件、K/T/Y/U/R/L Play Mode 验证结果、是否新增 .meta、是否修改 ProjectSettings，并 commit / push。
```
