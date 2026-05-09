# REVIEW.md

## Review 状态

Codex 已审查 MVP-03.9 最新提交与 Unity 编译热修：

```text
a2472bb fix: add Map namespace for main base ruin rule
```

结论：**MVP-03.9 代码审查通过**。

说明：大本营废墟已经不会通过普通 `BuildingRebuildService` 被重建；`RuinComponent` 的 `MapData` namespace 编译错误也已修复。本轮没有引入正式 UI、资源、占领、完整连地、区域奖励、传送阵或 AI。

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞问题。
```

### 已确认

- `RuinComponent.cs` 已补充 `using Map;`，解决 Unity `CS0246 MapData could not be found`。
- `RuinComponent.IsMainBaseRuin(MapData mapData)` 会通过 `mapData.GetPlot(sourcePlotId).isMainBase` 判断来源 plot 是否大本营。
- `BuildingRebuildService.Rebuild(...)` 遇到 main base ruin 时会 `return null`。
- Main base ruin 不会被销毁，也不会创建新建筑。
- 普通非大本营废墟仍可通过服务重建。
- `kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍未提交。

## 残留注意事项

### R 测试入口仍是临时入口

`GameEntry` 当前 `R` 键只尝试 `FindObjectsByType<RuinComponent>(...)[0]`。如果第一个废墟是大本营废墟，服务会正确拒绝重建，但 `R` 不会自动尝试下一个普通废墟。

这不阻塞 MVP-03.9，因为核心规则已落地；但后续调试入口不要依赖 Unity 返回顺序。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.9 代码审查通过。

进入 MVP-03.10：大本营废墟聚兵点最小原型。

用户规则继续保持：
- 最后的敌方大本营不能重建。
- 但它可以作为聚兵点。
- 后续还要能连接别的未占领地点并派兵。

本轮目标：
- 不做正式 UI。
- 不做资源、占领进度、升级、完整连地系统或 AI。
- 只做“大本营废墟可以作为聚兵点”的最小可见验证。

允许：
- 在 RuinComponent 上新增最小数据层方法，例如 CanUseAsRallyPoint(MapData mapData)，main base ruin 返回 true。
- 在 GameEntry 增加一个临时测试快捷键，例如 T。
- T 的行为：找到第一个 main base ruin，把当前存活的 Player 士兵聚到该废墟周围巡逻。
- 聚兵可以先复用 UnitPatrol.Setup(center, radius, staggerAngle)。
- 可以对这些单位调用 UnitMovement.Stop() / UnitCombat.ClearPushPath()，避免旧路径继续影响聚兵测试。
- 多个单位要用不同角度，避免完全重叠。
- 保持 R/K/L 测试快捷键现有行为。

必须保持：
- Main base ruin 仍不能被 R 重建。
- 普通非大本营废墟仍可重建。
- T 聚兵后，士兵围绕 main base ruin 巡逻。
- K / L 清场行为不变。
- Console 无明显错误。

禁止：
- 不做正式按钮或 UI。
- 不做连接未占领地的正式系统。
- 不做正式派兵系统。
- 不做资源、占领、升级、区域奖励、传送阵、AI。
- 不修改 ProjectSettings。
- 不提交 kingbattle/ProjectSettings/SceneTemplateSettings.json。

完成后更新 WORKLOG.md，说明修改文件、T/R/K/L Play Mode 验证步骤、是否修改 ProjectSettings，并 commit / push。
```
