# NEXT_STEPS.md

## 当前阶段判断

项目目标：

```text
4 周左右完成一个完整 Unity 小游戏；本阶段不实际开发微信小程序、macOS、Android 移植版本，但继续保持后续移植边界。
```

当前进度判断：

- 技术底座：约 82%。
- 核心玩法闭环：约 76%。
- 完整游戏体验：约 62%-65%。

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

## 当前缺失

- 胜利/失败后仍需要更明确的一局结束界面。
- match ended 后 gameplay command 还需要统一拒绝。
- 还缺 restart debug。
- 还缺完整 Play Mode 回归清单。
- 还缺数值调优。

## 四周冲刺路线

### 全程架构边界

- 核心玩法和平台能力分离。
- 输入 / UI 只发命令和读状态，不直接改核心数据。
- 后续触摸输入、移动端 HUD、平台存档可以替换当前键盘 / 临时 HUD。
- 不把平台判断散落到建筑、战斗、移动、地图逻辑中。
- 不为赶进度提交 Unity 生成目录或非必要 `ProjectSettings`。

### Week 1：核心可玩闭环

状态：基本完成。

### Week 2：玩家正式操作与敌方压力

状态：基本完成。

### Week 3：游戏性系统最小版

状态：人口/补给与建筑作用已完成。

### Week 4：打磨与交付

当前进入一局结束体验。

- 胜负结束面板。
- match ended 后输入收口。
- restart debug。
- Play Mode 回归清单。
- 数值调优。

## 当前正式任务：MVP-04.3 胜负界面和一局结束体验

目标：

```text
收口一局结束体验：胜负显示更明确，match ended 后阻止继续 gameplay command，并提供最小 restart debug 能力。
```

实现方向：

```diff
+ gameplay command 入口统一检查 match ended
+ Victory / Defeat 时 HUD 显示明显结束面板
+ N debug 快捷键 restart
+ Play Mode 回归清单雏形
- 不做正式 UI 美术
- 不做复杂菜单系统
- 不做存档
- 不修改 ProjectSettings
```

## MVP-04.3 后的建议顺序

### MVP-04.4 数值调优和回归清单

- 出兵速度、人口上限、塔伤害、敌方进攻间隔。
- 完整 Play Mode 回归清单。
- 修复明显 UI/反馈问题。

### MVP-04.5 正式输入整理

- 如果时间允许，再做地图点击 / 目标选择高亮。
- 将 debug 快捷键集中到 debug-only 区域。

### MVP-04.6 交付前清理

- 清理临时日志。
- 隐藏或集中 debug 快捷键。
- 检查不提交 `Library/`、`Logs/`、`UserSettings/`、非必要 `ProjectSettings`。

## Play Mode 回归清单雏形

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

## 长期提醒

- 现在不做移植实现，但后续仍可能做微信小程序、macOS、Android，因此业务逻辑边界必须持续清楚。
- 平台能力不要散落在建筑、战斗、移动脚本中。
- 当前最重要的是“可玩闭环优先”，但不能用平台耦合换速度。
