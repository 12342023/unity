# NEXT_STEPS.md

## 当前阶段判断

项目正在第三阶段早期：摧毁与重建 / 战略据点。

当前应先修复 R 测试入口，再继续推进聚兵点或派兵系统。

## 当前正式任务：MVP-03.10 R 测试入口小修

目标：

```text
R 键跳过大本营废墟，重建第一个普通可重建废墟。
```

实现方向：

```diff
+ R 遍历所有 RuinComponent
+ 跳过 main base ruin
+ 重建第一个普通可重建废墟
+ 没有普通可重建废墟时输出 no rebuildable ruins
- 不允许 R 重建 EnemyBase / PlayerBase 废墟
- 不做正式 UI
- 不做派兵系统
- 不修改 ProjectSettings
```

## 修复通过后的建议顺序

### MVP-03.10 Review

R 小修完成后，Codex 再 Review 当前 MVP-03.10：

- `CanUseAsRallyPoint`
- T 聚兵
- R/K/L 是否仍正常

### MVP-03.11 未占领地点连接数据层

聚兵点稳定后，再处理“连接别的未占领地点”：

- 使用 `MapData.GetNeighbors(plotId)`。
- 判断邻居 plot 是否 `Faction.Neutral`。
- 在 main base ruin 上提供可连接目标查询方法。
- 不做正式 UI，只先做数据层。

## 长期提醒

- 微信小程序、macOS、Android 移植会要求业务逻辑边界干净。
- 平台能力不要散落在建筑、战斗、移动脚本中。
- 当前阶段继续优先保持 `GameEntry` 变薄，业务能力下沉到清晰的小组件 / 小服务。
