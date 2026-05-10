# TASK.md

## 当前任务

MVP-04.5 返修：修复 `GameEntry` 编译阻塞并重新验证点击输入。

Codex 当前仍作为 Tech Lead 和 Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
a5bfe8a feat: click-to-dispatch map input, plot highlighting, debug shortcut consolidation
```

Codex Review 结论：

```text
MVP-04.5 暂不通过；必须先修编译问题，再重新验证。
```

## 阻塞问题

`GameEntry.cs` 中仍然创建 `TestUnitSpawner`：

```csharp
var spawner = spawnerObj.AddComponent<TestUnitSpawner>();
```

但 `TestUnitSpawner` 定义在：

```text
namespace Units
```

当前 `GameEntry.cs` 缺少：

```csharp
using Units;
```

## 允许范围

任务 A：修复编译

- 在 `GameEntry.cs` 顶部补 `using Units;`。
- 或使用 `Units.TestUnitSpawner` 全限定名。
- 优先使用 `using Units;`，和旧代码保持一致。

任务 B：重新验证 MVP-04.5

- Unity Console 无编译错误。
- Play 初始能进入场景。
- 点击 Player-owned source 后出现 target 高亮。
- 点击高亮 target 后派兵。
- HUD Dispatch 仍可派兵。
- O 仍可派兵且同路径。
- Victory/Defeat 后点击、HUD Dispatch、O/E 不再派兵。
- K/L/E/N/R/T/Y/U/I/O/P/Q 行为无回归。

任务 C：更新文档

- 更新 `WORKLOG.md`，记录修复内容和验证结果。
- 不需要发布新功能。
- 不要把 `NEXT_STEPS.md` 推进到 MVP-04.6，除非本轮修复和验证已经完成。

## 禁止范围

- 不做新玩法系统。
- 不做正式 UI 美术。
- 不做存档。
- 不引入第三方框架。
- 不引入新 Input System package。
- 不做移动端/微信/macOS/Android 移植实现。
- 不重构 `UnitCombat`。
- 不重构 `PlotCaptureService`。
- 不修改 `ProjectSettings`。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `.claude/` 或 `kingbattle/.idea/`，除非用户明确要求。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。

## 验收标准

- Unity 编译通过。
- Console 无明显错误。
- MVP-04.5 点击派兵和 debug 快捷键回归通过。
- `WORKLOG.md` 已记录修复和验证。
- 未修改或提交 `ProjectSettings`。
