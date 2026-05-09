# NEXT_STEPS.md

## 当前阶段判断

项目正在第三阶段早期：摧毁与重建。

已经完成：

- 建筑被击败后变成废墟。
- 士兵可以围绕废墟巡逻。
- 大本营被击败后，该阵营建筑变废墟，士兵立即死亡。
- 废墟携带来源 plot / 建筑类型 / 原阵营。
- 废墟具备最小可重建判定。
- 建筑创建逻辑已从 `GameEntry` 下沉到 `BuildingFactory`。
- 运行时建筑已通过 `BuildingRegistry` 按阵营登记和查询。
- `BuildingRebuildService` 已新增，并已补齐 `mapData` 空值防御。

## 新增用户规则

```text
最后的敌方大本营不能重建，但是可以聚兵，也可以连接别的未占领的地方可以派兵。
```

Codex 拆分：

- 大本营废墟不再是普通可重建建筑。
- 大本营废墟未来是战略据点。
- 战略据点可以承载聚兵、连接未占领地点、派兵。
- 本轮先做规则边界，不做完整 UI / 资源 / 占领 / 派兵系统。

## 当前正式任务：MVP-03.9 大本营废墟特殊规则

目标：

```text
MainBase ruin 不允许通过普通 BuildingRebuildService 重建成建筑。
```

实现方向：

```diff
+ BuildingRebuildService 使用 mapData.GetPlot(ruin.sourcePlotId).isMainBase 判断
+ mainBase ruin 时 return null，不销毁废墟
+ R 测试不能把 EnemyBase / PlayerBase 废墟重建成 Barracks
+ 普通非大本营废墟仍可重建
+ 可新增 RuinComponent 数据层方法表达“可聚兵 / 可派兵战略据点”
- 不做正式重建按钮
- 不做 UI / 资源 / 占领 / 完整连地 / AI
- 不修改 ProjectSettings
```

## MVP-03.9 后的建议顺序

### MVP-03.10 大本营废墟聚兵点最小原型

在大本营不可重建规则稳定后，再做“聚兵点”最小原型：

- 大本营废墟可作为 rally center。
- 士兵可以围绕大本营废墟聚集 / 巡逻。
- 不做正式 UI，只用测试入口或固定逻辑验证。

### MVP-03.11 未占领地点连接与派兵测试入口

再往后才处理“连接别的未占领地点并派兵”：

- 读取 `MapData.GetNeighbors(plotId)`。
- 只允许派兵到相邻未占领地点。
- 先做测试入口，不做正式 UI。

资源、升级、区域奖励、传送阵、AI 继续后置。

## 长期提醒

- 微信小程序、macOS、Android 移植会要求业务逻辑边界干净。
- 平台能力不要散落在建筑、战斗、移动脚本中。
- 当前阶段继续优先保持 `GameEntry` 变薄，业务能力下沉到清晰的小组件 / 小服务。
