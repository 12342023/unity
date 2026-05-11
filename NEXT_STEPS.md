# NEXT_STEPS.md

## 当前阶段判断

项目目标：

```text
4 周左右完成一个完整 Unity 小游戏；当前项目已迁移到 Unity 6。
```

当前状态：

- MVP-04.7 已通过。
- MVP-05.0 最小正式 UI 已通过二次返修。
- Canvas/uGUI HUD runtime 初始化问题已修复。
- MVP-05.1 正在三次返修：脚本无错误，HUD 面板可见，但 Text 内容仍不可见。
- Claude 当前停在未提交 WIP：`GameHud.cs` 已加 Text 探针，Codex 已完成 Play 验证，尚未删除测试探针、尚未修正式 HUD、尚未提交。

## 当前正式任务：MVP-05.1 Text 渲染定位与修复

目标：

```text
先用固定 RectTransform 的 Text 探针定位 Canvas/Text 渲染链路，再修正式 HUD 面板内文字显示。
```

任务：

- 删除 `TestProbe` / `NULL FONT TEST` 探针。
- 记录探针结论：绿色 `HUD TEXT TEST` 可见，红色 `NULL FONT TEST` 未看到，问题在 HUD panel/layout 子树。
- 检查 `CreateText(...)` / `CreateLinkedText(...)` 的 RectTransform 与 LayoutElement。
- 给主要 Text 和 candidate row 设置合理高度。
- 避免同一对象重复添加 `LayoutElement`。
- 确保 Objective、stats、candidate rows、Dispatch 按钮可读。
- 可先用固定 anchoredPosition/sizeDelta 的简单 HUD layout 恢复可读性。
- 保留已做的 LastAction / selection / enemy timer 短文案。
- 保持 candidate rows 触发式刷新，不回到每帧 Destroy/Recreate。
- 保持 UI 只读状态并调用 command service。
- Claude push 前必须检查 `pwd`、`git status -sb`、`git remote -v`，避免旧 remote。
- Claude commit/push 前必须确认 `rg "TestProbe|HUD TEXT TEST|NULL FONT TEST|TEMP" kingbattle/Assets/Scripts/UI/GameHud.cs` 无输出。
- 更新 `WORKLOG.md`。

## 后续建议顺序

### MVP-05.2 可玩性参数收口

- 士兵生成节奏。
- 敌方压力节奏。
- 占领人数要求。
- 建筑生命值 / 伤害。

### MVP-06.0 最小音效 / 反馈

- 点击音效。
- 派兵音效。
- 胜负提示音。

### MVP-07.0 打包前整理

- 进一步收敛 debug/development boundary。
- 准备 macOS / Android / 微信小程序移植边界清单。
- 盘点 platform service 接口候选点。

## Play Mode 回归清单

- Unity Console 无 `error CS`。
- Play 后无 `ArgumentException` / `NullReferenceException`。
- Play 初始显示 Canvas/uGUI HUD。
- 不再出现旧 OnGUI debug 框。
- HUD 面板内文字在当前 Game view 下清晰可读。
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

- UI Text 可见性仍需返修。
- Claude 偶发在错误目录 / 旧 remote push，需要每次 push 前检查 `git remote -v`。
- Unity Connect 网络/auth 错误属于外部服务问题，暂不阻塞 gameplay。
- 移植还没开始，但边界需要持续保持。
- `ProjectSettings`、`.idea/`、`.claude/`、Unity 生成目录不能随便提交。
