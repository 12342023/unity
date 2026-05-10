# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
2bfb30b cleanup: remove unused hasHome field, guard debug/test entry points, reduce log noise
```

结论：**MVP-04.7 基本正确，但需要小返修 / 补验证。**

## CODEX PROJECT REVIEW

Gate: **CONDITIONAL PASS**

Findings:

```text
[P2] GameEntry 正式 build 路径可能出现 release-only unused local warning。
[P3] WORKLOG 缺少 Unity 重新编译后的验证结果。
```

已确认通过：

- `hasHome` 残留搜索为 0。
- `FindObjectsSortMode / FindFirstObjectByType / OverlapCircleNonAlloc` 残留搜索为 0。
- `git show --check 2bfb30b` 无 whitespace 问题。
- `TestUnitSpawner` 和 `DebugShortcutController` 的创建已经包进 `#if UNITY_EDITOR || DEVELOPMENT_BUILD`。
- 高频低价值日志已用 debug/development build 条件限制。
- 未修改 `ProjectSettings`、场景文件、`Library/`、`Logs/`、`UserSettings/`。

### [P2] `GameEntry` 正式 build 路径可能有 unused local warning

当前写法：

```csharp
var (playerBaseHp, enemyBaseHp) = SetupBuildings(mapData);
```

这两个变量只在后面的 debug-only 区块里传给 `DebugShortcutController`：

```csharp
#if UNITY_EDITOR || DEVELOPMENT_BUILD
debugCtrl.Initialize(mapData, mapRenderer, enemyController, playerBaseHp, enemyBaseHp);
#endif
```

正式 build 预处理后，`playerBaseHp` / `enemyBaseHp` 可能变成“赋值但未使用”的局部变量 warning。建议最小修复：

```csharp
#if UNITY_EDITOR || DEVELOPMENT_BUILD
var (playerBaseHp, enemyBaseHp) = SetupBuildings(mapData);
#else
SetupBuildings(mapData);
#endif
```

这能避免 release-only warning，同时不改变玩法。

### [P3] 缺少新提交后的 Unity 编译验证记录

当前 `Editor.log` 尾部仍是旧编译记录，包含修改前的 `UnitCombat.hasHome` warning。代码已经删除该字段，但还需要在 Unity 中触发刷新/编译后确认：

- Unity Console 无 `error CS`。
- Unity Console 无 `UnitCombat.hasHome` warning。

## 给 Claude 的返修任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

MVP-04.7 基本正确，但需要小返修和补验证。

任务 1：修正 GameEntry 的 release-only unused local 风险
- 当前：
  var (playerBaseHp, enemyBaseHp) = SetupBuildings(mapData);
- 这两个变量只在 #if UNITY_EDITOR || DEVELOPMENT_BUILD 内给 DebugShortcutController 用。
- 请改成最小条件编译结构：

  #if UNITY_EDITOR || DEVELOPMENT_BUILD
  var (playerBaseHp, enemyBaseHp) = SetupBuildings(mapData);
  #else
  SetupBuildings(mapData);
  #endif

- 不要改 SetupBuildings 行为。
- 不要改玩法。

任务 2：补 Unity 验证
- 在 Unity 里触发一次刷新/重新编译。
- 确认 Console 无 error CS。
- 确认 Console 无 UnitCombat.hasHome warning。
- 确认 rg "hasHome|FindObjectsSortMode|FindFirstObjectByType|OverlapCircleNonAlloc" Assets/Scripts 无结果。

任务 3：更新 WORKLOG.md
- 记录小返修。
- 记录 Unity 编译验证结果。
- 记录没有修改 ProjectSettings、场景文件、Library/Logs/UserSettings。

禁止：
- 不做新玩法。
- 不做正式 UI。
- 不重构 GameEntry。
- 不改 ProjectSettings。
- 不提交 .claude、.idea、kingbattle.slnx、Library、Logs、UserSettings、要求.md 删除。

完成后 commit / push。
```
