# NEXT_STEPS.md

## 当前阶段判断

项目目标：

```text
4 周左右完成一个完整 Unity 小游戏；本阶段不实际开发微信小程序、macOS、Android 移植版本，但继续保持后续移植边界。
```

当前进度判断：

- 技术底座：约 80%。
- 核心玩法闭环：约 72%。
- 完整游戏体验：约 55%-60%。

已经完成：

- 建筑摧毁、废墟、重建基础。
- 大本营废墟聚兵。
- 敌方大本营被击败后的敌方清场。
- 士兵生成、移动、巡逻、聚兵、派兵。
- Neutral plot 占领主路径。
- 占领需求 Small/Medium/Large。
- HUD 候选按钮派兵。
- O 与 HUD Dispatch 复用 command 服务。
- 敌方定时进攻压力。
- E debug 快捷键触发敌方进攻。
- PlayerVictory / PlayerDefeat 最小胜负状态。

## 当前缺失

- 没有最小资源/人口规则，单位会持续生成。
- Granary 没有玩法作用。
- HUD 还没有显示人口/建筑收益。
- 没有胜负界面，只有 HUD 文字。
- 缺少数值调优和回归清单。

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

当前进入第一步：人口/补给和建筑作用。

- Barracks = 生成士兵。
- Tower = 自动攻击敌方单位。
- Granary = 增加 supply cap。
- HUD 显示 units/cap 和建筑收益。

### Week 4：打磨与交付

- UI 文案与状态反馈整理。
- 音效/特效/颜色反馈最小打磨。
- Play Mode 回归清单。
- 清理测试快捷键或隐藏到 debug 模式。
- 打包前检查：不提交 `Library/`、`Logs/`、`UserSettings/`、非必要 `ProjectSettings`。

## 当前正式任务：MVP-04.2 游戏性最小系统第一步

目标：

```text
增加最小人口/补给规则，让 Granary 有明确作用，并在 HUD 显示双方单位和建筑收益状态。
```

实现方向：

```diff
+ 新增 GameRuleService / FactionStatsService 或等价规则统计服务
+ BarracksSpawner 接入 supply cap，达到上限停止产兵
+ Granary 增加 supply cap
+ HUD 显示 Player/Enemy units/cap 和 Granary/Tower 状态
- 不做复杂经济系统
- 不做金币/粮食库存
- 不做建筑升级
- 不做区域奖励
- 不修改 ProjectSettings
```

## MVP-04.2 后的建议顺序

### MVP-04.3 胜负界面和一局结束体验

- HUD 或临时结算面板显示 Victory / Defeat。
- 提供 restart / quit debug 按钮或提示。
- 清理胜负后的输入状态。

### MVP-04.4 数值调优和回归清单

- 出兵速度、人口上限、塔伤害、敌方进攻间隔。
- 完整 Play Mode 回归清单。
- 修复明显 UI/反馈问题。

### MVP-04.5 正式输入整理

- 如果时间允许，再做地图点击 / 目标选择高亮。
- 将 debug 快捷键集中到 debug-only 区域。

## 长期提醒

- 现在不做移植实现，但后续仍可能做微信小程序、macOS、Android，因此业务逻辑边界必须持续清楚。
- 平台能力不要散落在建筑、战斗、移动脚本中。
- 当前最重要的是“可玩闭环优先”，但不能用平台耦合换速度。
