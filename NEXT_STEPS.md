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

## 当前正式任务：MVP-03.14 派兵边界整理

目标：

```text
把 U 派兵与 capture handler 管理从 GameEntry 下沉到小服务，保持玩法不变。
```

实现方向：

```diff
+ 新增 StrategicDispatchService 或等价小服务
+ 服务管理 U capture handler 字典
+ 服务负责附近 Player 士兵筛选与 SetPushPath
+ 服务负责到达后调用 PlotCaptureService.TryCapture
+ GameEntry 只保留快捷键入口与高层流程
- 不改变玩法表现
- 不做正式 UI
- 不做占领进度条
- 不做资源、升级、区域奖励、传送阵、AI
- 不修改 ProjectSettings
```

## MVP-03.14 后的建议顺序

### MVP-03.15 连地扩展策略

派兵边界整理稳定后，再考虑占领后的下一层战略规则：

- 从已占领 Crossroads 继续连接 Village / Farmland。
- 是否允许从 main base ruin + 已占领 connected plot 共同形成连接网络。
- 是否需要正式选择目标入口。

### MVP-03.16 占领规则增强

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
