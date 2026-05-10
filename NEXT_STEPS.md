# NEXT_STEPS.md

## 当前阶段判断

项目目标：

```text
4 周左右完成一个完整 Unity 小游戏；当前项目已迁移到 Unity 6。
```

当前状态：

- MVP-04.6 Unity 6 API cleanup 已通过。
- MVP-04.7 交付前清理已通过。
- 当前主要短板是 UI 仍为 OnGUI debug 风格。
- 下一步进入 MVP-05.0：最小正式 UI。

## 当前正式任务：MVP-05.0 最小正式 UI

目标：

```text
用运行时创建的 uGUI / Canvas UI 替换 OnGUI debug HUD，不改玩法逻辑。
```

任务：

- 替换 `GameHud` 的 OnGUI 表现层。
- 使用 Canvas / CanvasScaler / 锚点布局。
- 保留同一 command service。
- 缓存 `PlayerInputController` 引用，避免每帧查找。
- 更新 `WORKLOG.md`。

## MVP-05.0 之后建议顺序

### MVP-05.1 正式输入/反馈打磨

- 点击反馈更清晰。
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
- Play 初始显示正式 HUD。
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

- 正式 UI 尚未完成。
- UI 视觉仍需至少一轮调参。
- Unity Connect 401 属于外部服务/auth，暂不阻塞 gameplay。
- 移植还没开始，但边界需要持续保持。
- `ProjectSettings`、`.idea/`、`.claude/`、Unity 生成目录不能随便提交。
