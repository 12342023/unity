# NEXT_STEPS.md

## 当前阶段判断

项目正在第三阶段早期：摧毁与重建 / 战略据点。

已经完成：

- 建筑被击败后变成废墟。
- 士兵可以围绕废墟巡逻。
- 大本营废墟不能走普通重建。
- 大本营废墟可以作为聚兵点。
- R 已能跳过大本营废墟，重建普通废墟。
- Y 已能查询 main base ruin 相邻 Neutral 地点。

## 当前正式任务：MVP-03.12 临时派兵测试入口

目标：

```text
从 main base ruin 派附近 Player 士兵前往第一个可连接 Neutral plot。
```

实现方向：

```diff
+ GameEntry 增加 U 临时测试快捷键
+ U 找 main base ruin
+ U 取 GetConnectableNeutralPlots(mapData) 的第一个目标
+ U 找靠近 ruin 的 Player 士兵
+ U 使用 RoadPathFinder.FindPath 生成路径
+ U 调用 UnitCombat.SetPushPath 派兵
- 不做正式 UI
- 不改变 plot 归属
- 不做占领进度
- 不修改 ProjectSettings
```

## MVP-03.12 后的建议顺序

### MVP-03.13 占领与归属最小规则

派兵测试稳定后，再考虑最小占领规则：

- 单位到达未占领地点后是否改变归属。
- 是否需要停留时间。
- 是否允许敌方反夺。

### MVP-03.14 派兵边界整理

如果 U 测试入口变复杂，应把派兵逻辑从 `GameEntry` 下沉到小服务，例如 `StrategicDispatchService`。

资源、升级、区域奖励、传送阵、AI 继续后置。

## 长期提醒

- 微信小程序、macOS、Android 移植会要求业务逻辑边界干净。
- 平台能力不要散落在建筑、战斗、移动脚本中。
- 当前阶段继续优先保持 `GameEntry` 变薄，业务能力下沉到清晰的小组件 / 小服务。
