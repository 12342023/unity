# NEXT_STEPS.md

## 当前阶段判断

项目目标：

```text
4 周左右完成一个完整 Unity 小游戏；本阶段不实际开发微信小程序、macOS、Android 移植版本，但继续保持后续移植边界。
```

当前进度判断：

- 技术底座：约 85%。
- 核心玩法闭环：约 80%。
- 完整游戏体验：约 68%-72%。

已经完成：

- 建筑摧毁、废墟、重建基础。
- 大本营废墟聚兵。
- 敌方大本营被击败后的敌方清场。
- 士兵生成、移动、巡逻、聚兵、派兵。
- Neutral plot 占领主路径。
- HUD 候选按钮派兵。
- O 与 HUD Dispatch 复用 command 服务。
- 敌方定时进攻压力。
- E debug 快捷键触发敌方进攻。
- PlayerVictory / PlayerDefeat 最小胜负状态。
- supply cap：base 8，每个 Granary +4。
- Barracks / Tower / Granary 都已有最小作用。
- Victory/Defeat 结束面板 + 最终统计。
- match ended 后 gameplay command 统一拒绝。
- N debug 快捷键 / Restart 按钮重启。
- Play Mode 回归清单。

## 当前缺失

- 数值调优还没做。
- 调优文档还没做。
- 正式输入整理（地图点击 / 目标选择高亮）还没做。
- Debug 快捷键还没隐藏或集中。
- 正式 UI 还没做。
- 移植还没开始。

## 当前正式任务：MVP-04.4 数值调优和回归清单

目标：

```text
梳理关键数值，执行 Play Mode 回归清单，只做必要的小范围调优，把“能跑通”推进到“能稳定试玩”。
```

实现方向：

```diff
+ 新增 BALANCE.md 或 GAMEPLAY_TUNING.md
+ 可新增 GameBalanceConfig 收口少量关键数值
+ 执行 Play Mode 回归清单并记录结果
+ 必要时小范围调优
+ 增加交付前剩余风险清单
- 不做新玩法系统
- 不做正式 UI 美术
- 不做存档
- 不引入第三方框架
- 不修改 ProjectSettings
```

## MVP-04.4 后的建议顺序

### MVP-04.5 正式输入整理

- 如果时间允许，再做地图点击 / 目标选择高亮。
- 将 debug 快捷键集中到 debug-only 区域。

### MVP-04.6 交付前清理

- 清理临时日志。
- 隐藏或集中 debug 快捷键。
- 检查不提交 `Library/`、`Logs/`、`UserSettings/`、非必要 `ProjectSettings`。

## Play Mode 回归清单

- Play 初始 HUD 显示目标、候选、人口、敌方压力。
- HUD Dispatch 可派兵。
- O 与 HUD Dispatch 同路径。
- Q 只读预览。
- U main-base ruin debug 派兵。
- supply cap 达到上限后 Barracks 停止产兵。
- Granary 摧毁/重建会影响 cap。
- E 触发敌方进攻。
- K 触发 Victory。
- L 触发 Defeat。
- Victory/Defeat 后不再执行 gameplay command。
- N restart debug。
- Console 无明显错误。

## 交付前剩余风险

- 正式 UI 还没做，目前仍是 debug OnGUI。
- Debug 快捷键还没隐藏。
- 地图点击 / 目标选择高亮还没做。
- 移植还没开始，但边界需要持续保持。
- `ProjectSettings`、`.idea/`、`.claude/`、Unity 生成目录不能随便提交。

## 长期提醒

- 现在不做移植实现，但后续仍可能做微信小程序、macOS、Android，因此业务逻辑边界必须持续清楚。
- 平台能力不要散落在建筑、战斗、移动脚本中。
- 当前最重要的是“可玩闭环优先”，但不能用平台耦合换速度。
