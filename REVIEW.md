# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
12bbb01 fix: Unity 6 obsolete API warnings — OverlapCircle, FindObjectsByType, FindFirstObjectByType
```

结论：**MVP-04.6 暂不通过，需要返修。**

说明：Editor log 尾部目前没有 `error CS`，但最新代码的 Unity 6 API 替换仍有静态问题，下一次脚本编译/检查仍可能失败或继续产生 obsolete warnings。

## CODEX PROJECT REVIEW

Gate: **FAIL**

Findings:

```text
[P1] FindObjectsByType 参数顺序错误，并且仍使用 FindObjectsSortMode。
[P2] GameHud 仍使用 FindFirstObjectByType，未按任务要求改为 FindAnyObjectByType 或缓存。
[P3] TowerAttack 注释仍写 OverlapCircleNonAlloc。
```

### [P1] `FindObjectsByType` 参数顺序错误，且仍使用 obsolete enum

问题写法：

```csharp
Object.FindObjectsByType<UnitCombat>(FindObjectsSortMode.None, FindObjectsInactive.Exclude)
```

本机 Unity 6 XML API 显示重载顺序是：

```text
Object.FindObjectsByType<T>(FindObjectsInactive, FindObjectsSortMode)
Object.FindObjectsByType<T>(FindObjectsInactive)
Object.FindObjectsByType<T>()
```

也就是说如果要传两个参数，顺序应是：

```csharp
Object.FindObjectsByType<UnitCombat>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
```

但更重要的是：之前 Unity 6 warning 明确说 `FindObjectsSortMode` 也 obsolete，建议使用不带 `FindObjectsSortMode` 的 overload。因此本轮应该改成：

```csharp
Object.FindObjectsByType<UnitCombat>(FindObjectsInactive.Exclude)
```

需要修复的文件：

- `Assets/Scripts/Debug/DebugShortcutController.cs`
- `Assets/Scripts/Combat/StrategicConnectionService.cs`
- `Assets/Scripts/Combat/StrategicDispatchService.cs`
- `Assets/Scripts/Combat/FactionStatsService.cs`
- `Assets/Scripts/Combat/EnemyAttackCommandService.cs`
- `Assets/Scripts/Buildings/FactionDefeatHandler.cs`

### [P2] `GameHud` 仍使用 `FindFirstObjectByType`

当前代码：

```csharp
Object.FindFirstObjectByType<PlayerInputController>(FindObjectsInactive.Exclude)
```

之前 Unity 6 warning 指出 `FindFirstObjectByType` 本身已 deprecated，因为依赖 instance ID ordering。任务要求是：

```text
改成 FindAnyObjectByType<PlayerInputController>() 或更好的引用缓存。
```

建议最小修复：

```csharp
Object.FindAnyObjectByType<PlayerInputController>(FindObjectsInactive.Exclude)
```

### [P3] `TowerAttack` 注释仍写旧 API

当前注释：

```csharp
/// Uses Physics2D.OverlapCircleNonAlloc for efficient enemy scanning.
```

但代码已经改为：

```csharp
Physics2D.OverlapCircle(...)
```

建议把注释改成：

```csharp
/// Uses Physics2D.OverlapCircle with a reusable buffer for enemy scanning.
```

### 当前 Editor log 状态

最新 log 尾部没有发现：

- `error CS`
- `NullReferenceException`
- `MissingReferenceException`
- `GameStatusService does not exist`
- `GUID could not be found`

发现的非阻塞外部问题：

```text
Unity Connect / Project ID request failed: HTTP 401
```

这属于 Unity services/auth，不是当前游戏编译阻塞。

## 给 Claude 的返修任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-04.6 暂不通过，需要返修。

问题 1：FindObjectsByType 替换不正确
- 当前用了：
  Object.FindObjectsByType<T>(FindObjectsSortMode.None, FindObjectsInactive.Exclude)
- Unity 6 的参数顺序是 FindObjectsInactive 在前。
- 但为了真正消除 obsolete warning，本轮不要再使用 FindObjectsSortMode。
- 请改为：
  Object.FindObjectsByType<T>(FindObjectsInactive.Exclude)

涉及文件：
- DebugShortcutController.cs
- StrategicConnectionService.cs
- StrategicDispatchService.cs
- FactionStatsService.cs
- EnemyAttackCommandService.cs
- FactionDefeatHandler.cs

问题 2：GameHud 仍使用 FindFirstObjectByType
- 当前：
  Object.FindFirstObjectByType<PlayerInputController>(FindObjectsInactive.Exclude)
- 请改为：
  Object.FindAnyObjectByType<PlayerInputController>(FindObjectsInactive.Exclude)
- 或者轻量缓存引用，但不要重构 HUD。

问题 3：TowerAttack 注释仍写 OverlapCircleNonAlloc
- 改成描述 OverlapCircle + reusable buffer。

验证：
- rg 搜索不能再出现：
  - FindObjectsSortMode
  - FindFirstObjectByType
  - OverlapCircleNonAlloc
- Unity Console 无 error CS。
- Unity Console 无上述 obsolete warnings。
- Play smoke test：点击派兵、HUD Dispatch、O、K/L/E/N、Victory 后不能继续派兵。
- 记录 Unity Connect 401 是外部服务/auth 问题，不作为游戏阻塞。
- 更新 WORKLOG.md。

禁止：
- 不做新玩法。
- 不改 ProjectSettings。
- 不提交 .claude、.idea、kingbattle.slnx、Library、Logs、UserSettings、要求.md 删除。

完成后 commit / push。
```
