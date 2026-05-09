# NEXT_STEPS.md

## 当前阶段判断

项目正在第三阶段早期：摧毁与重建 / 战略据点。

已经完成：

- 建筑被击败后变成废墟。
- 士兵可以围绕废墟巡逻。
- 大本营废墟不能走普通重建。
- 大本营废墟可以作为聚兵点。
- R 已能跳过大本营废墟，重建普通废墟。

## 当前正式任务：MVP-03.11 连接未占领地点数据层

目标：

```text
main base ruin 能查询相邻且未占领的地点。
```

实现方向：

```diff
+ RuinComponent 增加 GetConnectableNeutralPlots(MapData)
+ 使用 MapData.GetNeighbors(sourcePlotId)
+ 只返回 Faction.Neutral 的邻居 plotId
+ GameEntry 增加 Y 临时测试快捷键打印结果
- 不做正式连接 UI
- 不做正式派兵
- 不改变 plot 归属
- 不修改 ProjectSettings
```

## MVP-03.11 后的建议顺序

### MVP-03.12 临时派兵测试入口

连接数据稳定后，再做临时派兵验证：

- 从 main base ruin 获取第一个可连接 neutral plot。
- 将聚集在 main base ruin 附近的 Player 士兵沿道路派往该 plot。
- 仍不做正式 UI、资源或 AI。

### MVP-03.13 占领与归属最小规则

派兵测试稳定后，再考虑最小占领规则：

- 单位到达未占领地点后是否改变归属。
- 是否需要停留时间。
- 是否允许敌方反夺。

资源、升级、区域奖励、传送阵、AI 继续后置。

## 长期提醒

- 微信小程序、macOS、Android 移植会要求业务逻辑边界干净。
- 平台能力不要散落在建筑、战斗、移动脚本中。
- 当前阶段继续优先保持 `GameEntry` 变薄，业务能力下沉到清晰的小组件 / 小服务。
