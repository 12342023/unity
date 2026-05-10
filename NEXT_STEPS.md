# NEXT_STEPS.md

## 当前阶段判断

项目目标：

```text
4 周左右完成一个完整 Unity 小游戏；当前项目已迁移到 Unity 6。
```

当前状态：

- MVP-04.7 已通过。
- MVP-05.0 已开始，但当前 `GameHud.cs` 编译失败。
- 必须先修复 MVP-05.0 编译错误，再继续 UI polish。

## 当前正式任务：MVP-05.0 UI 返修

目标：

```text
修复 GameHud.cs 编译错误，收敛 candidate rows 刷新方式，并修正按钮文案。
```

任务：

- 修复 `CreateSeparator` 返回 `void` 却赋值给 `var` 的 `CS0815`。
- 避免 candidate rows 每帧 Destroy/Recreate。
- `Dsp` 改为 `Dispatch`。
- 更新 `WORKLOG.md`。

## 返修后建议顺序

### MVP-05.1 UI 视觉与反馈打磨

- 检查 Canvas 在 16:9 和较窄屏下是否遮挡地图核心区域。
- 调整字体大小、按钮宽度、面板透明度。
- 派兵失败原因展示更短、更玩家化。
- 当前选中和目标提示更明确。

### MVP-05.2 可玩性参数收口

- 士兵生成节奏。
- 敌方压力节奏。
- 占领人数要求。
- 建筑生命值 / 伤害。

### MVP-06.0 最小音效 / 反馈

- 点击音效。
- 派兵音效。
- 胜负提示音。

## Play Mode 回归清单

- Unity Console 无 P1 编译错误。
- Play 初始显示 Canvas/uGUI HUD。
- 不再出现旧 OnGUI debug 框。
- 鼠标点击 Player-owned source 后高亮为蓝色，valid target 高亮为黄色。
- 鼠标点击黄色 target 后派兵。
- 点击空白区域清空选择。
- HUD Dispatch 可派兵。
- `O` 与 HUD Dispatch / 点击派兵同路径。
- `E` 触发敌方进攻。
- `K` 触发 Victory。
- `L` 触发 Defeat。
- Victory/Defeat 后点击、HUD Dispatch、`O/E` 不再执行 gameplay command。
- Restart 按钮和 `N` restart debug 可用。

## 交付前剩余风险

- 正式 UI 正在实现中，尚未通过编译。
- UI 视觉仍需至少一轮调参。
- Unity Connect 401 属于外部服务/auth，暂不阻塞 gameplay。
- 移植还没开始，但边界需要持续保持。
- `ProjectSettings`、`.idea/`、`.claude/`、Unity 生成目录不能随便提交。
