# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
4aa6d9c fix: correct Unity 6 API signatures — remove FindObjectsSortMode, use FindAnyObjectByType
```

结论：**MVP-04.6 通过。**

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无 P1 / P2 阻塞问题。
```

已验证：

- `rg "FindObjectsSortMode|FindFirstObjectByType|OverlapCircleNonAlloc" kingbattle/Assets/Scripts` 无结果。
- `Editor.log` 显示 `Tundra build success`。
- 目标 `CS1503` 编译错误已消失。
- 目标 Unity 6 obsolete warnings 已消失。
- `git show --check 4aa6d9c` 无 whitespace 问题。

剩余非阻塞项：

- `Assets/Scripts/Combat/UnitCombat.cs(47,22)` 有 `CS0414`：`hasHome` 已赋值但未使用。
- Debug/test 入口仍默认创建：
  - `DebugShortcutController`
  - `TestUnitSpawner`
- Runtime logs 仍偏多，正式交付前需要收敛。

## 下一步 Review 建议

进入 **MVP-04.7 交付前清理**。

目标不是做新玩法，而是让当前 MVP 更像可交付版本：

- 清理 Unity warning。
- 明确 debug/test 边界。
- 降低 runtime log 噪音。
- 保持后续 macOS / Android / 微信小程序移植边界清楚。

## 给 Claude 的新任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

MVP-04.6 已通过。现在进入 MVP-04.7：交付前清理。

目标：
不改玩法行为，不改 ProjectSettings，不做正式 UI，只清理 debug/test 边界、剩余 warning、明显日志噪音。

任务 1：清理 UnitCombat unused warning
- 当前 Unity 6 只剩非阻塞 warning：
  Assets/Scripts/Combat/UnitCombat.cs(47,22): warning CS0414: UnitCombat.hasHome is assigned but never used
- 请检查 hasHome 是否真的没有逻辑用途。
- 如果没有用途，删除字段和赋值，保留 homePosition / SetHomePosition 行为不变。
- 不要重写 UnitCombat 状态机。

任务 2：明确 debug/test 入口边界
- GameEntry 现在总是创建 TestUnitSpawner 和 DebugShortcutController。
- 请把 TestUnitSpawner 和 DebugShortcutController 的创建限制在：
  #if UNITY_EDITOR || DEVELOPMENT_BUILD
  ...
  #endif
- Play Mode / development build 仍可用 debug keys。
- 普通正式 build 不应该自动启用 1-4 测试刷兵和 K/L/E/N/R/T/Y/U/I/O/P/Q debug 快捷键。
- GameHud 暂时保留，不在本轮隐藏。

任务 3：收敛明显 runtime log 噪音
- 不要全局删除日志。
- 只处理明显高频、低价值、每局会刷很多次的日志。
- 建议优先看：
  - UnitMovement reached destination
  - Barracks spawn / wave push 重复日志
  - TestUnitSpawner ready/spawn 日志
- 做法优先使用 #if UNITY_EDITOR || DEVELOPMENT_BUILD 包裹 debug-only logs。
- Warning/Error 不要隐藏。

任务 4：更新 WORKLOG.md
- 记录修改文件。
- 记录验证结果。
- 记录没有修改 ProjectSettings、场景文件、Library/Logs/UserSettings。

验证：
- Unity Console 无 error CS。
- Unity Console 无 UnitCombat.hasHome warning。
- rg "FindObjectsSortMode|FindFirstObjectByType|OverlapCircleNonAlloc" Assets/Scripts 无结果。
- Play smoke test：
  - 点击派兵。
  - HUD Dispatch。
  - O 派兵。
  - K/L/E/N 在 Editor Play Mode 仍可用。
  - Victory/Defeat 后 command 拒绝仍正常。
- 确认 release build 路径中不会自动创建 TestUnitSpawner / DebugShortcutController。

禁止：
- 不做新玩法。
- 不做正式 UI。
- 不重构 UnitCombat 状态机。
- 不改 ProjectSettings。
- 不提交 .claude、.idea、kingbattle.slnx、Library、Logs、UserSettings、要求.md 删除。

完成后 commit / push。
```
