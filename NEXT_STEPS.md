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
- O 的扩张编排已下沉到 `StrategicExpansionService`。
- 扩张 source/target 已整理成 `ExpansionCandidate` 候选数据。
- 占领需求数据层已完成，P 可打印 Small/Medium/Large 对应需求，O 日志可显示 dispatched / required。

## 当前正式任务：MVP-03.20 占领需求判定修复

目标：

```text
修复 capture gate 使用局部序号导致足够派兵仍可能不占领的问题。
```

实现方向：

```diff
+ StrategicDispatchService 使用最终 totalDispatched 判定需求
+ handler 清理和 captureConsidered 逻辑保持正确
+ blocked 日志显示最终 dispatched/required
+ P/O/U 日志继续可验证需求和派兵数量
- 不改变 K/L/R/T/Y/U/I/O 行为
- 不做正式 UI
- 不做自动扩张
- 不做占领进度条
- 不做资源、升级、区域奖励、传送阵、AI
- 不修改 ProjectSettings
```

## MVP-03.20 后的建议顺序

### MVP-03.21 占领反馈批量任务

占领需求接入稳定后，再打包做：

- 占领失败时士兵巡逻/停留行为整理。
- 占领成功/失败日志统一。
- 为未来 UI 准备只读状态数据。

### 后续

资源、升级、区域奖励、传送阵、AI 继续后置。

## 长期提醒

- 微信小程序、macOS、Android 移植会要求业务逻辑边界干净。
- 平台能力不要散落在建筑、战斗、移动脚本中。
- 当前阶段继续优先保持 `GameEntry` 变薄，业务能力下沉到清晰的小组件 / 小服务。
