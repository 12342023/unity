# TASK.md

## 当前任务

MVP-05.0：最小正式 UI。

Codex 当前仍作为 Tech Lead 和 Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
fd5a503 fix: release build unused local — move SetupBuildings tuple into ifdef
```

Codex Review 结论：

```text
MVP-04.7 通过。
当前可进入 MVP-05.0：最小正式 UI。
```

## 目标

把当前 OnGUI debug HUD 改成运行时创建的最小正式 uGUI / Canvas UI。

范围边界：

- 不改玩法逻辑。
- 不改 `ProjectSettings`。
- 不改场景文件。
- 不新增平台耦合。
- UI 只读状态、调用 command service。
- 为后续 macOS / Android / 微信小程序移植保持清晰边界。

## 任务 A：替换 `GameHud` 的 OnGUI 表现层

当前：

- `GameHud.cs` 使用 `OnGUI` / `GUILayout`。
- 标题仍是 `Game Status (debug)`。
- 按钮文案有 `Dsp` 这类临时代码词。

要求：

- 改成运行时创建 Canvas 的 uGUI UI。
- 继续由 `GameEntry` 创建 `GameHud`，不要要求手动拖 scene prefab。
- 使用 `Screen Space Overlay`。
- 使用 `CanvasScaler`。
- 不改 `ProjectSettings`。

建议 UI 结构：

- 顶部状态栏：
  - 当前目标。
  - 敌方进攻倒计时。
  - 最近操作结果。
- 阵营状态面板：
  - 玩家/敌方单位数。
  - 人口上限。
  - Granary / Tower 数。
- 扩张候选列表：
  - 最多显示 4 个 candidates。
  - 每项显示 source、target、available / required。
  - 每项有清晰的 `Dispatch` 按钮。
- 选择提示：
  - 当前 selected source。
  - highlighted target 数量。
- 胜负面板：
  - Victory / Defeat。
  - 最终统计。
  - Restart 按钮。

## 任务 B：保持 UI / 业务边界

UI 只能读取：

- `GameStatusService`
- `MatchResultService`
- `FactionStatsService`
- `StrategicConnectionService.GetExpansionPreviews`
- `PlayerInputController` selection state

UI 触发派兵只能调用：

```csharp
StrategicExpansionCommandService.DispatchCandidate(...)
```

UI 触发 Restart 可以继续加载当前 scene。

禁止：

- UI 直接改 `PlotData.faction`。
- UI 直接改 building health。
- UI 直接改 unit state。
- UI 直接操作跨平台/平台 API。

## 任务 C：减少每帧查找

当前 `GameHud` 在绘制中调用：

```csharp
Object.FindAnyObjectByType<PlayerInputController>(FindObjectsInactive.Exclude)
```

要求：

- 不要在每次 UI 刷新/绘制时查找。
- 优先让 `GameEntry` 把 `PlayerInputController` 传给 `GameHud.Initialize`。
- 不要为此重构 `PlayerInputController`。

## 任务 D：响应式和移植边界

要求：

- UI 锚点清楚。
- 适配 16:9 和较窄屏。
- 不把固定像素布局散落在各处。
- 不新增全局状态。
- 不使用平台 API。

## 任务 E：更新 WORKLOG.md

记录：

- 修改文件。
- UI / 业务边界。
- 验证结果。
- 未修改 `ProjectSettings`、场景文件、`Library/`、`Logs/`、`UserSettings/`。

## 验证要求

- Unity Console 无 `error CS`。
- Play 初始显示正式 HUD，不再显示 OnGUI debug 框。
- 点击地图派兵仍正常。
- HUD `Dispatch` 按钮仍正常。
- `O` 快捷键与 HUD Dispatch / 点击派兵同路径。
- `K/L/E/N` 在 Editor Play Mode 仍可用。
- Victory/Defeat 后显示正式结束面板。
- Restart 按钮可用。
- Victory/Defeat 后点击、HUD Dispatch、`O/E` 不再执行 gameplay command。

## 禁止范围

- 不做新玩法系统。
- 不改战斗 / 占领 / 派兵 service 行为。
- 不修改 `ProjectSettings`。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。
- 不提交 `.claude/`、`kingbattle/.idea/`。
- 不提交 `kingbattle/kingbattle.slnx`。
- 不提交 `要求.md` 删除。

## 验收标准

- Console 无编译错误。
- 旧 OnGUI debug HUD 已被正式 Canvas UI 替代。
- UI 只读状态、只调用 command service。
- 点击、按钮、快捷键三条派兵路径都仍正常。
- 胜负与 Restart 正常。
- `WORKLOG.md` 已记录修改和验证。
