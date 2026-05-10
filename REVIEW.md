# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
fd5a503 fix: release build unused local — move SetupBuildings tuple into ifdef
```

结论：**MVP-04.7 通过。**

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无 P1 / P2 阻塞问题。
```

已验证：

- `GameEntry` 中 `SetupBuildings` 返回值接收已限制在 `UNITY_EDITOR || DEVELOPMENT_BUILD`。
- release 分支直接调用 `SetupBuildings(mapData);`，避免 debug-only local 变量泄漏。
- `rg "hasHome|FindObjectsSortMode|FindFirstObjectByType|OverlapCircleNonAlloc" kingbattle/Assets/Scripts` 无结果。
- `tail -320 Editor.log` 未发现 `error CS`、`warning CS`、`hasHome`、目标 obsolete API 残留。
- `git show --check fd5a503` 无 whitespace 问题。
- 未修改 `ProjectSettings`、场景文件、`Library/`、`Logs/`、`UserSettings/`。

剩余非阻塞风险：

- 当前 `GameHud` 仍是 OnGUI debug 风格。
- HUD 每次绘制会查找 `PlayerInputController`，正式 UI 阶段应缓存引用或通过 `Initialize` 传入。
- 视觉表现仍偏开发调试，需要进入 MVP-05.0。

## 下一步 Review 建议

进入 **MVP-05.0：最小正式 UI**。

目标不是重做玩法，而是替换 debug OnGUI 表现层：

- 使用 Unity UI Canvas / uGUI。
- 运行时由 `GameEntry` 创建，不改 scene。
- UI 只读状态、调用 command service。
- 不让 UI 直接修改 map/building/unit 数据。
- 保持后续 macOS / Android / 微信小程序移植边界清楚。

## 给 Claude 的新任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

MVP-04.7 已通过。现在进入 MVP-05.0：最小正式 UI。

目标：
把当前 OnGUI debug HUD 改成运行时创建的最小正式 uGUI / Canvas UI。
不改玩法逻辑，不改 ProjectSettings，不改场景文件，不新增平台耦合。

任务 1：替换 GameHud 的 OnGUI 表现层
- 当前 GameHud.cs 使用 OnGUI/GUILayout，标题还是 Game Status (debug)。
- 请改成运行时创建 Canvas 的 uGUI UI。
- 可以继续使用 GameEntry 创建 GameHud，不要要求手动拖 scene prefab。
- 推荐结构：
  - 顶部状态栏：目标、敌方进攻倒计时、最近操作。
  - 左侧/右侧小面板：玩家/敌方单位数、人口上限、Granary/Tower 数。
  - 底部或侧边候选列表：最多显示 4 个 expansion candidates，每项有 Dispatch 按钮。
  - 选中提示：当前 selected source 和 highlighted targets 数量。
  - 胜负面板：Victory / Defeat、最终统计、Restart 按钮。
- 文案从 debug 口吻改成玩家可读口吻，不要出现 Game Status (debug)、Dsp 这类临时代码词。

任务 2：保持 UI/业务边界
- UI 只能读取：
  - GameStatusService
  - MatchResultService
  - FactionStatsService
  - StrategicConnectionService.GetExpansionPreviews
  - PlayerInputController selection state
- UI 触发派兵只能调用：
  - StrategicExpansionCommandService.DispatchCandidate(...)
- UI 触发 Restart 可以继续加载当前 scene。
- UI 不得直接修改 PlotData、MapData faction、building health、unit state。

任务 3：减少每帧查找
- 不要在每次 UI 刷新/绘制时 FindAnyObjectByType。
- 让 GameEntry 把 PlayerInputController 传给 GameHud.Initialize，或者 GameHud 初始化时缓存一次。
- 不要为此重构 PlayerInputController。

任务 4：响应式和移植边界
- Canvas 使用 Screen Space Overlay。
- 使用 CanvasScaler，适配 16:9 和较窄屏。
- UI 锚点清晰，不依赖固定像素绝对布局到处散落。
- 不使用平台 API。
- 不新增全局状态。

任务 5：更新 WORKLOG.md
- 记录修改文件。
- 记录 UI 边界。
- 记录验证结果。
- 记录没有修改 ProjectSettings、场景文件、Library/Logs/UserSettings。

验证：
- Unity Console 无 error CS。
- Play 初始显示正式 HUD，不再显示 OnGUI debug 框。
- 点击地图派兵仍正常。
- HUD Dispatch 按钮仍正常。
- O 快捷键与 HUD Dispatch / 点击派兵同路径。
- K/L/E/N 在 Editor Play Mode 仍可用。
- Victory/Defeat 后显示正式结束面板，Restart 可用。
- Victory/Defeat 后点击、HUD Dispatch、O/E 不再执行 gameplay command。

禁止：
- 不做新玩法。
- 不改战斗/占领/派兵 service 行为。
- 不改 ProjectSettings。
- 不提交 .claude、.idea、kingbattle.slnx、Library、Logs、UserSettings、要求.md 删除。

完成后 commit / push。
```
