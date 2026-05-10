# NEXT_STEPS.md

## 当前阶段判断

项目目标：

```text
4 周左右完成一个完整 Unity 小游戏；本阶段不实际开发微信小程序、macOS、Android 移植版本，但继续保持后续移植边界。
```

当前进度判断：

- 技术底座：约 87%。
- 核心玩法闭环：约 83%。
- 完整游戏体验：约 72%-75%。

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
- `BALANCE.md` 数值调优文档。
- `GameBalanceConfig` 轻量配置收口。

## 当前缺失

- 地图点击 source/target 派兵还没做。
- 目标选择高亮还没做。
- Debug 快捷键还没集中。
- `GameEntry.Update()` 仍承担太多 debug 输入。
- `TestUnitSpawner` 仍保留 1-4 测试输入。
- 正式 UI 还没做。
- 移植还没开始。

## 当前正式任务：MVP-04.5 正式输入整理

目标：

```text
把玩家操作从 debug/HUD-only 推进到可点击地图的试玩输入，同时集中 debug 快捷键，为后续移动端/微信输入适配留边界。
```

实现方向：

```diff
+ 新增 PlayerInputController 或等价组件
+ 地图点击选择 source plot
+ 高亮 selected source 和 valid neutral target
+ 点击 target 后调用 StrategicExpansionCommandService
+ HUD Dispatch / O / 点击派兵保持同一命令路径
+ 新增 DebugShortcutController 集中 K/L/E/N/R/T/Y/U/I/O/P/Q
- 不做正式 UI 美术
- 不引入新 Input System package
- 不复制派兵/占领逻辑
- 不修改 ProjectSettings
```

## MVP-04.5 后的建议顺序

### MVP-04.6 交付前清理

- 清理或集中临时日志。
- 隐藏 debug 快捷键入口，保留 debug-only controller。
- 检查 `TestUnitSpawner` 是否还需要保留。
- 检查不提交 `Library/`、`Logs/`、`UserSettings/`、非必要 `ProjectSettings`。

### MVP-05.0 最小正式 UI

- 用正式 UI 替换 debug OnGUI 的核心信息。
- 保留同一 command service，不让 UI 写业务状态。

## Play Mode 回归清单

- Play 初始 HUD 显示目标、候选、人口、敌方压力。
- 鼠标点击 Player-owned source 后出现 target 高亮。
- 鼠标点击高亮 Neutral target 后派兵。
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

- 正式 UI 还没做，目前仍是 debug OnGUI。
- Debug 快捷键需要集中到 debug-only controller。
- `TestUnitSpawner` 的 1-4 测试输入后续需要清理或标记 debug-only。
- 移植还没开始，但边界需要持续保持。
- `ProjectSettings`、`.idea/`、`.claude/`、Unity 生成目录不能随便提交。

## 长期提醒

- 现在不做移植实现，但后续仍可能做微信小程序、macOS、Android，因此业务逻辑边界必须持续清楚。
- 平台能力不要散落在建筑、战斗、移动脚本中。
- 输入层只负责发命令；未来触摸、鼠标、键盘应该能替换适配。
- 当前最重要的是“可玩闭环优先”，但不能用平台耦合换速度。
