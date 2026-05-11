# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
d85d121 fix: Unity 6 LegacyRuntime.ttf, first-frame RefreshData, Update NRE guard
```

结论：**MVP-05.0 二次返修通过。**

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞问题。
```

已验证：

- `GameHud.Initialize(...)` 已从 `Arial.ttf` 改为 `LegacyRuntime.ttf`。
- `CreateCanvas()` 后立即 `RefreshData()`，首帧 candidate previews 可刷新。
- `GameHud.Update()` 已加入 root null guard。
- Play Mode 进入成功，Console 出现 `[GameHud] uGUI HUD initialized.`。
- Editor log 最近 500 行无 `error CS`、`ArgumentException`、`NullReferenceException`。
- 游戏循环继续运行，Barracks spawn、EnemyPressureController、单位移动日志正常。
- UI 派兵仍走 `StrategicExpansionCommandService.DispatchCandidate(...)`。
- 鼠标点击派兵入口 `PlayerInputController` 在 match ended 后直接 return。
- `StrategicExpansionCommandService` 在 match ended 后拒绝 dispatch。
- `EnemyAttackCommandService` 在 match ended 后拒绝 enemy attack。
- 未发现 UI 直接修改 `MapData` / `PlotData.faction` / building health / unit state。

残余风险：

- Computer Use 对 Unity 坐标点击不稳定，本轮未完整自动化证明 HUD `Dispatch` 按钮点击路径；代码路径已确认仍调用同一 command service。
- 当前 HUD 视觉仍偏紧凑，下一轮应优先做可读性和布局打磨。
- Unity Connect 网络/auth 错误属于外部服务问题，不计入 gameplay blocker。

## 给 Claude 的下一轮任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

MVP-05.0 二次返修已通过。进入 MVP-05.1：正式 UI 视觉与交互反馈打磨。

任务 1：改善 HUD 可读性与布局
- 调整 Canvas/HUD panel 尺寸、字体、间距，让左上 HUD 文字清晰可读。
- 保持 runtime uGUI 创建方式，不引入 scene prefab。
- 不要遮挡地图核心操作区过多。

任务 2：改善 candidate rows 展示
- Candidate row 最多显示 4 条即可。
- 保持 Dispatch 文案。
- source -> target available/required 要清晰可读。
- enough/short 可以改成更玩家化短文案。
- Candidate rows 仍不能每帧 Destroy/Recreate。

任务 3：改善反馈文案
- HUD 层可以短化 LastActionResult。
- 不改 command service 的结构化结果语义。
- 鼠标点击选择、HUD Dispatch、O 快捷键继续走 StrategicExpansionCommandService。

任务 4：完整 Play 验证并更新 WORKLOG
- Console 无 error CS。
- Play 后无 ArgumentException / NullReferenceException。
- Canvas/uGUI HUD 显示清晰，不显示旧 OnGUI debug 框。
- HUD Dispatch、地图点击派兵、O 快捷键都正常。
- K/L/E/N 正常。
- Victory/Defeat 面板与 Restart 正常。
- Victory/Defeat 后点击、HUD Dispatch、O/E 不再执行 gameplay command。

禁止：
- 不重写整个 UI。
- 不改战斗/占领/派兵 service 行为。
- 不改 ProjectSettings。
- 不提交 .claude、.idea、kingbattle.slnx、Library、Logs、UserSettings、要求.md 删除。

完成后 commit / push。
```
