# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
a5bfe8a feat: click-to-dispatch map input, plot highlighting, debug shortcut consolidation
```

结论：**MVP-04.5 暂不通过，需要返修后再进入 MVP-04.6**。

说明：点击派兵、高亮、debug 快捷键集中方向正确，但当前提交存在 Unity 编译阻塞风险。

## CODEX PROJECT REVIEW

Gate: **FAIL**

Findings:

```text
[P1] GameEntry 缺少 Units namespace，TestUnitSpawner 无法解析。
```

### [P1] `GameEntry` 引用 `TestUnitSpawner` 但未导入 `Units`

File: `kingbattle/Assets/Scripts/GameEntry.cs:53`

Problem:

`GameEntry.Start()` 中仍然执行：

```csharp
var spawner = spawnerObj.AddComponent<TestUnitSpawner>();
```

但 `TestUnitSpawner` 定义在：

```text
kingbattle/Assets/Scripts/Units/TestUnitSpawner.cs
namespace Units
```

当前 `GameEntry.cs` 顶部只有：

```csharp
using Buildings;
using Combat;
using Core;
using Map;
using UnityEngine;
```

缺少：

```csharp
using Units;
```

这会导致 Unity 编译错误：

```text
The type or namespace name 'TestUnitSpawner' could not be found
```

Smallest fix:

```diff
 using Map;
+using Units;
 using UnityEngine;
```

或改为：

```csharp
var spawner = spawnerObj.AddComponent<Units.TestUnitSpawner>();
```

建议优先使用 `using Units;`，保持和旧版 `GameEntry` 一致。

### 已确认的非阻塞部分

- `PlayerInputController` 职责边界基本正确：点击输入只翻译为 command，不直接写 map/building/unit 状态。
- `MapRenderer` 高亮只改 SpriteRenderer 颜色，不改 `PlotData.faction`。
- HUD Dispatch、O debug、点击派兵都指向 `StrategicExpansionCommandService.DispatchCandidate(...)`。
- `DebugShortcutController` 已集中 K/L/E/N/R/T/Y/U/I/O/P/Q。
- `git show --check HEAD` 未发现 whitespace 问题。
- 本轮未提交 `ProjectSettings`。

### 返修后必须重新验证

- Unity Console 无编译错误。
- Play 初始能进入场景。
- 点击 Player-owned source 后出现 target 高亮。
- 点击 target 后派兵。
- HUD Dispatch 仍可派兵。
- O 仍可派兵且同路径。
- K/L/E/N/R/T/Y/U/I/O/P/Q 行为无回归。

## 给 Claude 的返修任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-04.5 暂不通过，需要返修。

阻塞问题：
- `GameEntry.cs` 创建 `TestUnitSpawner`，但 `TestUnitSpawner` 位于 `namespace Units`。
- 当前 `GameEntry.cs` 缺少 `using Units;`，Unity 会编译失败。

请执行：
1. 在 `GameEntry.cs` 顶部补上 `using Units;`。
2. 不要改 ProjectSettings。
3. 不要顺手做新功能。
4. 重新打开/等待 Unity 编译，确认 Console 无编译错误。
5. 跑一轮 MVP-04.5 回归：
   - Play 初始能进入场景。
   - 点击 Player-owned source 后出现 target 高亮。
   - 点击 target 后派兵。
   - HUD Dispatch 仍可派兵。
   - O 仍可派兵且同路径。
   - Victory/Defeat 后点击、HUD、O/E 不再派兵。
   - K/L/E/N/R/T/Y/U/I/O/P/Q 行为无回归。
6. 更新 WORKLOG.md，记录修复和验证。
7. commit / push。

完成后我再 review，确认通过后再发布 MVP-04.6。
```
