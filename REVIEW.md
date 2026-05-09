# REVIEW.md

## Review 状态

Codex 已审查并上传 MVP-03.10 修复与聚兵点代码：

```text
6f1deab feat: main-base ruin rally and rebuildable ruin scan
```

结论：**MVP-03.10 代码审查通过**。

说明：`R` 测试入口已经不再依赖 `ruins[0]`，会遍历所有废墟并跳过 main base ruin；`T` 临时测试入口可以把 Player 士兵聚到大本营废墟周围巡逻。本轮没有引入正式 UI、资源、占领、完整连地、区域奖励、传送阵或 AI。

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞问题。
```

### 已确认

- `GameEntry` 的 R 键会遍历所有 `RuinComponent`。
- R 会跳过 `ruin.IsMainBaseRuin(mapData) == true` 的大本营废墟。
- R 会重建第一个普通可重建废墟。
- 如果没有普通可重建废墟，R 输出 `no rebuildable ruins`。
- `RuinComponent.CanUseAsRallyPoint(MapData)` 已新增，当前 main base ruin 返回 true。
- `GameEntry` 的 T 键会找到 main base ruin，并让存活 Player 士兵围绕该废墟巡逻。
- 多个士兵使用错开的巡逻角度。
- `kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍未提交。

## 残留注意事项

### T 仍是临时测试入口

T 目前直接扫描所有 Player `UnitCombat` 并设置巡逻点，这适合作为 Play Mode 验证入口，但不应视为正式派兵系统。下一步应先做“可连接未占领地点”的数据层，再做正式/临时派兵闭环。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.10 代码审查通过。

进入 MVP-03.11：大本营废墟连接未占领地点数据层。

用户规则继续保持：
- 最后的敌方大本营不能重建。
- 它可以作为聚兵点。
- 它可以连接别的未占领地点，后续可从这里派兵。

本轮目标：
- 不做正式 UI。
- 不做正式派兵系统。
- 不做资源、占领进度、升级、区域奖励、传送阵或 AI。
- 只给 main base ruin 增加“可连接哪些未占领地点”的最小数据查询能力。

允许：
- 在 RuinComponent 新增方法，例如 GetConnectableNeutralPlots(MapData mapData)。
- 该方法只在 CanUseAsRallyPoint(mapData) 为 true 时返回结果。
- 使用 MapData.GetNeighbors(sourcePlotId) 查找相邻 plot。
- 只返回 faction == Faction.Neutral 的邻居 plotId。
- 可以返回 IReadOnlyList<string> / List<string>，保持简单。
- 可以在 GameEntry 增加临时测试快捷键，例如 Y，打印 main base ruin 可连接的未占领地点列表。
- 保持 K / L / R / T 行为不变。

必须保持：
- 大本营废墟仍不能被 R 重建。
- T 仍能聚兵到大本营废墟。
- Y 只打印/验证连接数据，不派兵。
- Console 无明显错误。

禁止：
- 不做正式连接 UI。
- 不做正式派兵。
- 不改变 plot 归属。
- 不做资源、占领、升级、区域奖励、传送阵、AI。
- 不修改 ProjectSettings。
- 不提交 kingbattle/ProjectSettings/SceneTemplateSettings.json。

完成后更新 WORKLOG.md，说明修改文件、Y/R/T/K/L Play Mode 验证步骤、是否修改 ProjectSettings，并 commit / push。
```
