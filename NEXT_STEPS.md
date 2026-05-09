# NEXT_STEPS.md

## 当前阶段判断

项目正在第三阶段中期：摧毁、废墟、战略据点、连地派兵。

已经完成：

- 建筑被击败后变成废墟。
- 士兵可以围绕废墟巡逻。
- 大本营废墟不能走普通重建。
- 大本营废墟可以作为聚兵点。
- R 已能跳过大本营废墟，重建普通废墟。
- Y 已能查询 main base ruin 相邻 Neutral 地点。
- U 已能从 main base ruin 向第一个可连接 Neutral plot 临时派兵。

## 当前正式任务：MVP-03.13 Neutral plot 最小占领与颜色刷新

目标：

```text
U 派出的 Player 士兵到达 Neutral plot 后，占领该 plot，并刷新地图颜色。
```

实现方向：

```diff
+ 新增小型 PlotCaptureService / StrategicCaptureService
+ 只允许 Neutral -> Player
+ GameEntry 在 U 派兵到达后触发一次占领
+ MapRenderer 增加按 plotId 刷新颜色的最小方法
- 不做正式 UI
- 不做占领进度条
- 不允许占领 Enemy plot
- 不做资源、升级、区域奖励、传送阵、AI
- 不修改 ProjectSettings
```

## MVP-03.13 后的建议顺序

### MVP-03.14 派兵边界整理

如果 U / 占领逻辑继续变复杂，应把临时派兵流程从 `GameEntry` 下沉到小服务，例如 `StrategicDispatchService`。

### MVP-03.15 占领规则增强

最小占领闭环稳定后，再考虑：

- 是否需要停留时间。
- 是否需要多士兵加速。
- 是否允许敌方反夺。
- 是否要显示占领进度。

资源、升级、区域奖励、传送阵、AI 继续后置。

## 长期提醒

- 微信小程序、macOS、Android 移植会要求业务逻辑边界干净。
- 平台能力不要散落在建筑、战斗、移动脚本中。
- 当前阶段继续优先保持 `GameEntry` 变薄，业务能力下沉到清晰的小组件 / 小服务。
