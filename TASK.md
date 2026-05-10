# TASK.md

## 当前任务

MVP-04.7：交付前清理。

Codex 当前仍作为 Tech Lead 和 Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
4aa6d9c fix: correct Unity 6 API signatures — remove FindObjectsSortMode, use FindAnyObjectByType
```

Codex Review 结论：

```text
MVP-04.6 通过。
Unity 6 编译成功。
目标 obsolete API 残留已清理。
```

当前可以进入 **MVP-04.7 交付前清理**。

## 目标

让当前 MVP 更接近可交付状态，但不改变玩法行为：

- 清理剩余 Unity warning。
- 明确 debug/test 边界。
- 收敛明显 runtime log 噪音。
- 保持后续 macOS / Android / 微信小程序移植边界清楚。

## 任务 A：清理 `UnitCombat.hasHome` warning

当前 Unity 6 只剩非阻塞 warning：

```text
Assets/Scripts/Combat/UnitCombat.cs(47,22): warning CS0414:
The field 'UnitCombat.hasHome' is assigned but its value is never used
```

要求：

- 检查 `hasHome` 是否真的无逻辑用途。
- 如果无用途，删除字段和赋值。
- 保留 `homePosition` / `SetHomePosition` 行为不变。
- 不重写 `UnitCombat` 状态机。

## 任务 B：明确 debug/test 入口边界

当前 `GameEntry` 默认创建：

- `TestUnitSpawner`
- `DebugShortcutController`

要求：

- 用以下条件限制这两个 debug/test 入口：

```csharp
#if UNITY_EDITOR || DEVELOPMENT_BUILD
// create TestUnitSpawner / DebugShortcutController
#endif
```

- Editor Play Mode 和 development build 仍能用 debug keys。
- 普通正式 build 不应自动启用 1-4 测试刷兵和 debug 快捷键。
- `GameHud` 本轮暂时保留，不隐藏。

## 任务 C：收敛明显 runtime log 噪音

要求：

- 不全局删除日志。
- 不隐藏 Warning / Error。
- 只处理明显高频、低价值、每局会刷很多次的 `Debug.Log`。
- 优先使用：

```csharp
#if UNITY_EDITOR || DEVELOPMENT_BUILD
Debug.Log(...);
#endif
```

建议优先检查：

- `UnitMovement reached destination`
- `BarracksSpawner` spawn / wave push 重复日志
- `TestUnitSpawner` ready / spawn 日志

## 任务 D：更新 WORKLOG.md

记录：

- 修改文件。
- 验证结果。
- 未修改 `ProjectSettings`、场景文件、`Library/`、`Logs/`、`UserSettings/`。

## 验证要求

- Unity Console 无 `error CS`。
- Unity Console 无 `UnitCombat.hasHome` warning。
- `rg "FindObjectsSortMode|FindFirstObjectByType|OverlapCircleNonAlloc" Assets/Scripts` 无结果。
- Play smoke test：
  - 点击派兵。
  - HUD Dispatch。
  - `O` 派兵。
  - `K/L/E/N` 在 Editor Play Mode 仍可用。
  - Victory/Defeat 后 command 拒绝仍正常。
- 确认 release build 路径中不会自动创建 `TestUnitSpawner` / `DebugShortcutController`。

## 禁止范围

- 不做新玩法系统。
- 不做正式 UI。
- 不重构 `UnitCombat` 状态机。
- 不修改 `ProjectSettings`。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。
- 不提交 `.claude/`、`kingbattle/.idea/`。
- 不提交 `kingbattle/kingbattle.slnx`。
- 不提交 `要求.md` 删除。

## 验收标准

- Console 无编译错误。
- Console 无 `UnitCombat.hasHome` warning。
- Debug/test 入口有明确 build 条件边界。
- 高频低价值日志已收敛，Warning/Error 保留。
- `WORKLOG.md` 已记录修改和验证。
