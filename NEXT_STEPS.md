# NEXT_STEPS.md

## 当前阶段判断

项目目标：

```text
4 周左右完成一个完整 Unity 小游戏；本阶段不实际开发微信小程序、macOS、Android 移植版本，但继续保持后续移植边界。
```

当前进度判断：

- 技术底座：约 88%。
- 核心玩法闭环：约 84%。
- 完整游戏体验：约 74%-78%。

说明：MVP-04.5 功能方向正确，但当前提交有编译阻塞，需要先返修。

## 已完成方向

- 建筑摧毁、废墟、重建基础。
- 大本营废墟聚兵。
- 敌方大本营被击败后的敌方清场。
- 士兵生成、移动、巡逻、聚兵、派兵。
- Neutral plot 占领主路径。
- HUD 候选按钮派兵。
- O 与 HUD Dispatch 复用 command 服务。
- 敌方定时进攻压力。
- PlayerVictory / PlayerDefeat 最小胜负状态。
- supply cap：base 8，每个 Granary +4。
- Barracks / Tower / Granary 都已有最小作用。
- Victory/Defeat 结束面板 + 最终统计。
- match ended 后 gameplay command 统一拒绝。
- `BALANCE.md` 数值调优文档。
- `GameBalanceConfig` 轻量配置收口。
- 地图点击 source/target 派兵 + 高亮已实现，但需先修编译。
- Debug 快捷键集中到 `DebugShortcutController`，但需回归验证。

## 当前正式任务：MVP-04.5 返修

目标：

```text
修复 GameEntry 缺少 Units namespace 的编译问题，并重新验证 MVP-04.5。
```

实现方向：

```diff
+ GameEntry.cs 补 using Units;
+ Unity Console 编译错误清零
+ 重新跑点击派兵 / HUD / O / debug 快捷键回归
+ WORKLOG.md 记录修复和验证
- 不做新功能
- 不修改 ProjectSettings
```

## 返修后建议顺序

### MVP-04.6 交付前清理

- 清理或集中临时日志。
- 隐藏 debug 快捷键入口，保留 debug-only controller。
- 检查 `TestUnitSpawner` 是否还需要保留。
- 检查不提交 `Library/`、`Logs/`、`UserSettings/`、非必要 `ProjectSettings`。

### MVP-05.0 最小正式 UI

- 用正式 UI 替换 debug OnGUI 的核心信息。
- 保留同一 command service，不让 UI 写业务状态。

## Play Mode 回归清单

- Unity Console 无编译错误。
- Play 初始 HUD 显示目标、候选、人口、敌方压力。
- 鼠标点击 Player-owned source 后高亮为蓝色，valid target 高亮为黄色。
- 鼠标点击黄色 target 后派兵。
- 点击空白区域清空选择。
- HUD Dispatch 可派兵。
- O 与 HUD Dispatch / 点击派兵同路径。
- Q 只读预览。
- U main-base ruin debug 派兵。
- supply cap 达到上限后 Barracks 停止产兵。
- Granary 摧毁/重建会影响 cap。
- E 触发敌方进攻。
- K 触发 Victory。
- L 触发 Defeat。
- Victory/Defeat 后点击、HUD Dispatch、O/E 不再执行 gameplay command。
- N restart debug。
- Console 无明显错误。

## 交付前剩余风险

- 当前先修编译阻塞。
- 正式 UI 还没做，目前仍是 debug OnGUI。
- `TestUnitSpawner` 的 1-4 测试输入后续需要清理或标记 debug-only。
- 移植还没开始，但边界需要持续保持。
- `ProjectSettings`、`.idea/`、`.claude/`、Unity 生成目录不能随便提交。

## 长期提醒

- 现在不做移植实现，但后续仍可能做微信小程序、macOS、Android，因此业务逻辑边界必须持续清楚。
- 平台能力不要散落在建筑、战斗、移动脚本中。
- 输入层只负责发命令；未来触摸、鼠标、键盘应该能替换适配。
- 当前最重要的是“可玩闭环优先”，但不能用平台耦合换速度。
