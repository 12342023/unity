# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
d5b46ed feat: patrol after capture, victory/defeat state, and game HUD
```

结论：**MVP-03.23 通过，允许进入 MVP-04.0**。

说明：占领后士兵行为、最小胜负状态、临时 HUD 均已完成。未发现阻塞问题。

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞问题。
```

### 已确认

- `StrategicDispatchService` 在派出士兵到达目标后，将该士兵 patrol center / home position 切到目标 plot。
- 足够兵力时仍走 `PlotCaptureService.TryCapture(...)`，不足兵力时 blocked。
- blocked 后士兵也停留在目标附近巡逻，不回旧来源点。
- `MatchResultService` 使用 `TryDeclareVictory()` / `TryDeclareDefeat()` 防止重复结算。
- `FactionDefeatHandler` 在清场后声明 PlayerVictory / PlayerDefeat。
- `GameHud` 是临时 `OnGUI` HUD，显示目标、候选数量、最近结果、胜负状态。
- 新增 `.meta` 已随代码提交。
- `git show --check HEAD` 未发现 whitespace 或 patch 问题。
- `kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍未提交。

### 观察

`GameStatusService` 当前是轻量共享状态，短期可接受。后续如果状态变多，应该收口成只读 `GameState` / `HudViewModel`，避免 UI 状态散落到核心逻辑中。

### 当前完成度判断

- 技术底座：约 75%。
- 核心玩法闭环：约 65%。
- 完整游戏体验：约 45%-50%。

现在已经能从测试入口推进到胜负状态，但还缺少正式玩家操作、敌方压力、简单经济/人口、打磨反馈。

### 说明

工作区仍有非本轮项：

```text
D 要求.md
?? .claude/
?? kingbattle/ProjectSettings/SceneTemplateSettings.json
```

这些不应随本轮提交，除非用户明确确认。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.23 通过。

进入 MVP-04.0：玩家正式操作第一步，候选按钮 + gameplay command 边界。

项目目标：
- 四周内先完成完整 Unity 小游戏。
- 后续仍可能做微信小程序、macOS、Android 移植。
- 所以本轮必须把“UI/输入”和“核心玩法命令”分开。

本轮目标：
- 不再只靠 O 自动派第一个候选。
- 临时 HUD 显示扩张候选列表。
- 玩家可以通过 HUD 按钮选择某个候选并派兵。
- O 快捷键改为复用同一个 command 服务，不能继续重复写路径/派兵逻辑。

任务 A：新增扩张命令服务
- 新增 `StrategicExpansionCommandService` 或等价小服务。
- 提供类似：
  - `DispatchExpansionCandidate(MapData mapData, MapRenderer mapRenderer, string sourcePlotId, string targetPlotId)`
  - 或 `DispatchExpansionCandidate(..., StrategicConnectionService.ExpansionPreview preview)`
- 该服务负责：
  - 验证 source/target 仍是合法 expansion candidate。
  - 查找 road path。
  - 计算 target required count。
  - 调用 `StrategicDispatchService.DispatchToPlot(...)`。
  - 返回 `ExpansionResult` 或等价结构化结果。
- 该服务不能依赖键盘、鼠标、OnGUI 或平台 API。

任务 B：HUD 候选按钮
- 扩展临时 `GameHud`。
- 显示当前 expansion previews 列表，至少显示前 4 个候选。
- 每项显示：
  - source -> target
  - available / required
  - enough / not enough
  - Dispatch 按钮
- 点击按钮调用任务 A 的 command 服务派兵。
- HUD 不直接修改 map/building/unit，只调用 command 服务并显示返回结果。

任务 C：O 快捷键复用 command 服务
- `GameEntry` 的 O 分支改为：
  - 获取第一个 expansion candidate / preview。
  - 调用同一个 command 服务。
- 不再在 `StrategicExpansionService` 和 `GameEntry` 中保留两套路径/派兵编排逻辑。
- U 可以暂时保留为 main-base ruin debug 快捷键。

任务 D：保持移植边界
- HUD 仍标记为 temporary/debug。
- 本轮不做正式美术 UI、不做世界点击 raycast、不做触摸输入实现。
- 但结构要保证后续可替换为手机触摸按钮。

任务 E：文档和验证
- 更新 WORKLOG.md。
- Play Mode 验证：
  - HUD 能列出候选。
  - 点击候选 Dispatch 能派兵。
  - O 与 HUD 按钮使用同一条 command 路径。
  - Q 仍只读。
  - U/P/K/L 仍可验证。
  - Console 无明显错误。
- 记录是否新增 .meta，是否修改 ProjectSettings。

禁止：
- 不做正式 UI 美术。
- 不做敌方 AI。
- 不做资源/升级/区域奖励。
- 不做移动端/微信/macOS/Android 移植实现。
- 不修改 ProjectSettings。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。

完成后 commit / push。
```
