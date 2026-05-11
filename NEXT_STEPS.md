# NEXT_STEPS.md

## 当前阶段判断

项目目标：

```text
4 周左右完成一个完整 Unity 小游戏；当前项目已迁移到 Unity 6。
```

当前状态：

- MVP-04.7 已通过。
- MVP-05.0 最小正式 UI 已进入返修阶段。
- `CS0815` 编译错误已修复。
- Play Mode 发现新的 P1：`GameHud` 使用 Unity 6 已失效 builtin font，HUD 初始化失败。

## 当前正式任务：MVP-05.0 二次返修

目标：

```text
让 Canvas/uGUI HUD 在 Unity 6 Play Mode 正常初始化和显示，并完成完整交互回归。
```

任务：

- `GameHud` builtin font 从 `Arial.ttf` 改为 `LegacyRuntime.ttf`。
- 修复 / 防御 HUD 初始化失败后的 `NullReferenceException`。
- HUD 初始化完成后首帧刷新 candidate previews。
- 保持 candidate rows 触发式刷新，不回到每帧 Destroy/Recreate。
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

- Unity Console 无 `error CS`。
- Play 后无 `ArgumentException` / `NullReferenceException`。
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

- 正式 UI 尚未通过 Play Mode runtime 验证。
- Unity Connect 网络/auth 错误属于外部服务问题，暂不阻塞 gameplay，但不要混同为脚本错误。
- UI 视觉仍需至少一轮调参。
- 移植还没开始，但边界需要持续保持。
- `ProjectSettings`、`.idea/`、`.claude/`、Unity 生成目录不能随便提交。
