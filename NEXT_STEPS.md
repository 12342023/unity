# NEXT_STEPS.md

## 当前阶段判断

项目正在第三阶段早期：摧毁与重建 / 战略据点。

已经完成：

- 建筑被击败后变成废墟。
- 士兵可以围绕废墟巡逻。
- 大本营被击败后，该阵营建筑变废墟，士兵立即死亡。
- 废墟携带来源 plot / 建筑类型 / 原阵营。
- 废墟具备最小可重建判定。
- 建筑创建逻辑已从 `GameEntry` 下沉到 `BuildingFactory`。
- 运行时建筑已通过 `BuildingRegistry` 按阵营登记和查询。
- `BuildingRebuildService` 已新增，并已补齐 `mapData` 空值防御。
- 大本营废墟不能通过普通重建服务重建。

## 当前正式任务：MVP-03.10 大本营废墟聚兵点最小原型

目标：

```text
让 main base ruin 成为可聚兵的临时战略点。
```

实现方向：

```diff
+ RuinComponent 增加 CanUseAsRallyPoint(MapData)
+ GameEntry 增加 T 临时测试快捷键
+ T 找到 main base ruin
+ T 将当前 Player 士兵聚到该废墟周围巡逻
+ 复用 UnitPatrol / UnitMovement / UnitCombat 现有能力
- 不做正式 UI
- 不做连接未占领地正式系统
- 不做正式派兵系统
- 不修改 ProjectSettings
```

## MVP-03.10 后的建议顺序

### MVP-03.11 未占领地点连接数据层

聚兵点稳定后，再处理“连接别的未占领地点”：

- 使用 `MapData.GetNeighbors(plotId)`。
- 判断邻居 plot 是否 `Faction.Neutral`。
- 在 main base ruin 上提供可连接目标查询方法。
- 不做正式 UI，只先做数据层。

### MVP-03.12 临时派兵测试入口

连接数据稳定后，再做临时派兵验证：

- 从 main base ruin 聚集点选择一个相邻未占领地点。
- 派当前聚集的 Player 士兵沿道路前往目标。
- 仍不做正式 UI、资源或 AI。

## 长期提醒

- 微信小程序、macOS、Android 移植会要求业务逻辑边界干净。
- 平台能力不要散落在建筑、战斗、移动脚本中。
- 当前阶段继续优先保持 `GameEntry` 变薄，业务能力下沉到清晰的小组件 / 小服务。
