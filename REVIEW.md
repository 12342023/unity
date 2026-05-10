# REVIEW.md

## Review 状态

Codex 已读取 Unity 6 Editor log：

```text
/Users/jianghao/Library/Logs/Unity/Editor.log
```

当前 Unity 版本：

```text
Unity 6000.4.6f1
```

结论：**当前不是玩法逻辑问题，而是 Unity 6 导入/编译阻塞，需要先修编译环境和 meta 文件。**

## CODEX PROJECT REVIEW

Gate: **FAIL**

Findings:

```text
[P1] GameStatusService.cs.meta GUID 无效，Unity 忽略 GameStatusService.cs，导致大量 GameStatusService 找不到。
[P1] ShaderGraph package 在 Unity 6 下报 GUID 类型缺失，疑似 package cache/package version 不匹配。
[P2] 多处 Unity 6 obsolete warning，需要后续清理，但不是当前阻塞。
```

### [P1] `GameStatusService.cs.meta` GUID 无效

File: `kingbattle/Assets/Scripts/Combat/GameStatusService.cs.meta:2`

当前内容：

```text
guid: bcdef23456789012345678901234567890
```

问题：

- Unity `.meta` GUID 必须是 32 位十六进制。
- 当前 GUID 长度为 34。
- Unity 6 日志明确提示：

```text
The GUID inside 'Assets/Scripts/Combat/GameStatusService.cs.meta' cannot be extracted by the YAML Parser.
The .meta file Assets/Scripts/Combat/GameStatusService.cs.meta does not have a valid GUID and its corresponding Asset file will be ignored.
```

影响：

- Unity 忽略 `GameStatusService.cs`。
- 所有引用 `GameStatusService` 的代码都会报 `CS0103`。

代表性连锁错误：

```text
Assets/Scripts/Input/PlayerInputController.cs(79,17): error CS0103: The name 'GameStatusService' does not exist in the current context
Assets/Scripts/Combat/StrategicExpansionService.cs(32,17): error CS0103: The name 'GameStatusService' does not exist in the current context
Assets/Scripts/Buildings/FactionDefeatHandler.cs(58,21): error CS0103: The name 'GameStatusService' does not exist in the current context
Assets/Scripts/Combat/StrategicExpansionCommandService.cs(99,13): error CS0103: The name 'GameStatusService' does not exist in the current context
Assets/Scripts/Combat/EnemyAttackCommandService.cs(96,17): error CS0103: The name 'GameStatusService' does not exist in the current context
Assets/Scripts/Combat/EnemyPressureController.cs(31,13): error CS0103: The name 'GameStatusService' does not exist in the current context
Assets/Scripts/Debug/DebugShortcutController.cs(171,17): error CS0103: The name 'GameStatusService' does not exist in the current context
Assets/Scripts/GameEntry.cs(41,9): error CS0103: The name 'GameStatusService' does not exist in the current context
Assets/Scripts/UI/GameHud.cs(69,29): error CS0103: The name 'GameStatusService' does not exist in the current context
Assets/Scripts/Combat/StrategicDispatchService.cs(123,29): error CS0103: The name 'GameStatusService' does not exist in the current context
```

Smallest fix:

```diff
-guid: bcdef23456789012345678901234567890
+guid: bcdef23456789012345678901234567
```

要求：

- 使用唯一、32 位、纯十六进制 GUID。
- 不改 `GameStatusService.cs` 业务代码。
- 修复后让 Unity 重新导入并确认 `GameStatusService` 连锁错误消失。

### [P1] ShaderGraph package 在 Unity 6 下报 `GUID` 类型缺失

日志：

```text
Library/PackageCache/com.unity.shadergraph@04a6ef07359e/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Targets/BuiltInCanvasSubTarget.cs(10,25): error CS0246: The type or namespace name 'GUID' could not be found
Library/PackageCache/com.unity.shadergraph@04a6ef07359e/Editor/Generation/Contexts/TargetSetupContext.cs(62,40): error CS0246: The type or namespace name 'GUID' could not be found
```

判断：

- 这是 `Library/PackageCache` 里的 Unity package 编译错误，不是我们 `Assets/Scripts` 业务代码。
- 可能原因是 Unity 6 升级后 package cache 损坏，或 package/lock 版本和 Unity 6000.4.6f1 不匹配。
- 当前 `manifest.json` 使用：

```text
com.unity.render-pipelines.universal: 17.4.0
```

建议处理顺序：

1. 先修 `GameStatusService.cs.meta`。
2. 让 Unity 重新编译。
3. 如果 ShaderGraph 的 `GUID` 错误仍存在，再处理 package：
   - 关闭 Unity。
   - 删除/刷新 `Library/PackageCache`，让 Unity 重建。
   - 或在 Package Manager 中重新解析/更新 URP 和 ShaderGraph 到 Unity 6000.4.6f1 兼容版本。
4. 如果必须改 `Packages/manifest.json` / `Packages/packages-lock.json`，要单独说明原因并提交这两个文件。
5. 不要提交 `Library/`。

### [P2] Unity 6 obsolete warnings

这些不是当前编译阻塞，但后续需要清理：

```text
TowerAttack.cs: Physics2D.OverlapCircleNonAlloc obsolete，建议改 Physics2D.OverlapCircle。
FindObjectsByType<T>(FindObjectsSortMode) obsolete，建议改 Unity 6 新 overload。
FindFirstObjectByType<T>() obsolete，建议改 FindAnyObjectByType<T>()。
```

涉及文件包括：

- `Assets/Scripts/Buildings/TowerAttack.cs`
- `Assets/Scripts/Combat/FactionStatsService.cs`
- `Assets/Scripts/Combat/StrategicConnectionService.cs`
- `Assets/Scripts/Buildings/FactionDefeatHandler.cs`
- `Assets/Scripts/Combat/EnemyAttackCommandService.cs`
- `Assets/Scripts/Debug/DebugShortcutController.cs`
- `Assets/Scripts/Combat/StrategicDispatchService.cs`
- `Assets/Scripts/UI/GameHud.cs`

当前任务先修 P1。P2 放到后续 Unity 6 cleanup。

## 给 Claude 的修复任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

当前引擎是 Unity 6：6000.4.6f1。

Codex 从 Editor.log 里整理出当前阻塞：

P1-1：GameStatusService.cs.meta GUID 无效
- 文件：Assets/Scripts/Combat/GameStatusService.cs.meta
- 当前 guid 长度 34，不是 Unity 要求的 32 位 hex。
- Unity 因此忽略 GameStatusService.cs。
- 导致大量 CS0103：GameStatusService does not exist。

请先修：
1. 把 GameStatusService.cs.meta 的 guid 改成唯一的 32 位十六进制字符串。
2. 不修改 GameStatusService.cs 业务逻辑。
3. 让 Unity 重新导入/编译。
4. 确认所有 GameStatusService does not exist 错误消失。

P1-2：ShaderGraph package 报 GUID 类型缺失
- 如果修完 meta 后仍报：
  Library/PackageCache/com.unity.shadergraph... GUID could not be found
- 不要改 Library。
- 优先关闭 Unity 后让 PackageCache 重建，或用 Package Manager 重新解析/更新 URP/ShaderGraph 到 Unity 6000.4.6f1 兼容版本。
- 如果必须修改 Packages/manifest.json 和 packages-lock.json，要在 WORKLOG 写清楚原因。

P2：Unity 6 obsolete warnings
- FindObjectsByType(...FindObjectsSortMode) obsolete
- FindFirstObjectByType obsolete
- Physics2D.OverlapCircleNonAlloc obsolete
本轮先不要大范围改这些，只记录到 WORKLOG，等 P1 编译通过后再安排 cleanup。

验证：
1. Unity Console 无 P1 编译错误。
2. GameStatusService.cs 能被 Unity 正常导入。
3. Play 能进入场景。
4. 点击派兵 / HUD Dispatch / O / K/L/E/N/R/T/Y/U/I/O/P/Q 至少做快速 smoke test。
5. WORKLOG.md 记录：修复了哪个 meta、是否还剩 ShaderGraph/package 问题、是否还有 obsolete warnings。

提交：
- 提交 GameStatusService.cs.meta 修复。
- 如果修改 Packages/manifest.json 和 packages-lock.json，单独说明并提交。
- 不提交 Library、Logs、UserSettings、.idea、.claude。
- 不提交 ProjectSettings，除非 Codex/用户明确确认 Unity 6 迁移文件可以进仓。
```
