# NEXT_STEPS.md

## 当前阶段判断

项目正在第三阶段中期：摧毁、废墟、战略据点、连地派兵、最小占领。

已经完成：

- 建筑被击败后变成废墟。
- 士兵可以围绕废墟巡逻。
- 大本营废墟不能走普通重建。
- 大本营废墟可以作为聚兵点。
- R 已能跳过大本营废墟，重建普通废墟。
- Y 已能查询 main base ruin 相邻 Neutral 地点。
- U 已能从 main base ruin 向第一个可连接 Neutral plot 临时派兵。
- Neutral -> Player 最小占领主路径已实现。
- U capture handler 生命周期已收口。
- U 派兵边界已下沉到 `StrategicDispatchService`。
- I 已能查询 Player-owned frontier 的相邻 Neutral。
- O 已能从第一个 Player-owned frontier plot 派兵到第一个相邻 Neutral，并触发占领。

## 当前正式任务：MVP-03.17 扩张编排服务收口

目标：

```text
把 O 的扩张编排从 GameEntry 下沉到 StrategicExpansionService 或等价小服务。
```

实现方向：

```diff
+ 新增 StrategicExpansionService 或等价小服务
+ 服务复用 StrategicConnectionService / RoadPathFinder / StrategicDispatchService
+ GameEntry 的 O 分支只负责按键和日志
+ O 外部行为保持不变
- 不改变 K/L/R/T/Y/U/I/O 行为
- 不做正式 UI
- 不做自动扩张
- 不做资源、升级、区域奖励、传送阵、AI
- 不修改 ProjectSettings
```

## MVP-03.17 后的建议顺序

### MVP-03.18 连地网络规则整理

扩张编排服务稳定后，再整理：

- 哪些 Player-owned plot 可以作为出兵源。
- main base ruin 与已占领 plot 如何共同形成连接网络。
- 多个 frontier 时如何选择源和目标。

### MVP-03.19 占领规则增强

再考虑：

- 是否需要停留时间。
- 是否需要多士兵加速。
- 是否允许敌方反夺。
- 是否要显示占领进度。

资源、升级、区域奖励、传送阵、AI 继续后置。

## 长期提醒

- 微信小程序、macOS、Android 移植会要求业务逻辑边界干净。
- 平台能力不要散落在建筑、战斗、移动脚本中。
- 当前阶段继续优先保持 `GameEntry` 变薄，业务能力下沉到清晰的小组件 / 小服务。
