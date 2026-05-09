# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新修复提交：

```text
32bb717 fix: add mapData null guard in BuildingRebuildService.Rebuild
```

结论：**MVP-03.8 代码审查通过**。

说明：`BuildingRebuildService.Rebuild(...)` 已补齐 `mapData == null` 防御，满足“错误入参安全返回”的服务边界要求。本轮仍未引入正式 UI、资源、占领、升级、连地、区域奖励、传送阵或 AI。

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞问题。
```

### 已确认

- `BuildingRebuildService.Rebuild(null, mapData, faction)` 会安全返回 `null`。
- `BuildingRebuildService.Rebuild(ruin, null, faction)` 会安全返回 `null` 并输出 Warning。
- 正常重建仍通过 `BuildingFactory.CreateBuilding(...)` 创建建筑。
- 新建筑仍通过 `BuildingFactory` 自动注册到 `BuildingRegistry`。
- `R` 测试快捷键保持为临时验证入口。
- 未提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。

## 新增用户规则

用户新增要求：

```text
最后的敌方大本营不能重建，但是可以聚兵，也可以连接别的未占领的地方可以派兵。
```

Codex 阶段拆分：

- 立即禁止大本营废墟走普通重建流程。
- 大本营废墟保留为可聚兵 / 可派兵的战略据点概念。
- 本轮先做数据和规则边界，不做正式 UI、资源、占领进度或完整派兵系统。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.8 代码审查通过。

进入 MVP-03.9：大本营废墟特殊规则。

用户新增规则：
- 最后的敌方大本营不能重建。
- 但它未来可以作为聚兵点。
- 也可以连接别的未占领地点，并从这里派兵。

本轮目标：
- 不做正式 UI。
- 不做资源、占领进度、升级、连地系统完整版或 AI。
- 先把“大本营废墟不能被普通重建服务重建”这个规则落地。
- 为“大本营废墟可作为聚兵 / 派兵战略据点”保留清晰数据边界。

允许：
- 在 BuildingRebuildService.Rebuild(...) 中使用 mapData.GetPlot(ruin.sourcePlotId).isMainBase 判断。
- 如果 source plot 是 main base，则 Debug.LogWarning 并 return null，不销毁废墟。
- R 测试快捷键按到 EnemyBase / PlayerBase 废墟时不能把它重建成 Barracks。
- 可以在 RuinComponent 上新增最小方法或字段，例如 CanUseAsRallyPoint / CanDispatchFrom，表达大本营废墟未来可聚兵 / 可派兵。
- 如果新增方法，只做数据层，不接入 UI 或完整派兵逻辑。
- 保持普通非大本营废墟仍可通过 R 测试重建。

必须保持：
- K 击败 EnemyBase 后，EnemyBase 废墟不能被 R 重建成 Player Barracks。
- EnemyOutpost 等非大本营废墟仍可按当前测试逻辑重建。
- 大本营废墟不被销毁，仍可作为士兵巡逻/未来聚兵中心。
- K / L 清场行为不变。
- Console 无明显错误。

禁止：
- 不做正式重建按钮。
- 不做选择废墟 UI。
- 不做资源消耗、占领进度、升级、连地系统完整版、区域奖励、传送阵、AI。
- 不修改 ProjectSettings。
- 不提交 kingbattle/ProjectSettings/SceneTemplateSettings.json。

完成后更新 WORKLOG.md，说明修改文件、规则边界、R/K/L Play Mode 验证步骤、是否修改 ProjectSettings，并 commit / push。
```
