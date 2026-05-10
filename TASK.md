# TASK.md

## 当前任务

MVP-04.6 返修：正确清理 Unity 6 obsolete API warnings。

Codex 当前仍作为 Tech Lead 和 Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
12bbb01 fix: Unity 6 obsolete API warnings — OverlapCircle, FindObjectsByType, FindFirstObjectByType
```

Codex Review 结论：

```text
MVP-04.6 暂不通过；FindObjectsByType / FindFirstObjectByType 替换仍需返修。
当前 Unity 6 编译失败，Editor log 已出现 CS1503。
```

## 阻塞问题

### 问题 A：FindObjectsByType 参数顺序和 API 选择错误

当前写法：

```csharp
Object.FindObjectsByType<T>(FindObjectsSortMode.None, FindObjectsInactive.Exclude)
```

问题：

- Unity 6 两参数 overload 顺序是 `FindObjectsInactive, FindObjectsSortMode`。
- 继续使用 `FindObjectsSortMode` 本身也不能消除 obsolete warning。
- 当前已经触发 `CS1503`：
  - `Argument 1: cannot convert from 'UnityEngine.FindObjectsSortMode' to 'UnityEngine.FindObjectsInactive'`
  - `Argument 2: cannot convert from 'UnityEngine.FindObjectsInactive' to 'UnityEngine.FindObjectsSortMode'`

本轮要求改为：

```csharp
Object.FindObjectsByType<T>(FindObjectsInactive.Exclude)
```

涉及：

- `DebugShortcutController.cs`
- `StrategicConnectionService.cs`
- `StrategicDispatchService.cs`
- `FactionStatsService.cs`
- `EnemyAttackCommandService.cs`
- `FactionDefeatHandler.cs`

### 问题 B：GameHud 仍使用 FindFirstObjectByType

当前写法：

```csharp
Object.FindFirstObjectByType<PlayerInputController>(FindObjectsInactive.Exclude)
```

本轮要求改为：

```csharp
Object.FindAnyObjectByType<PlayerInputController>(FindObjectsInactive.Exclude)
```

或做轻量缓存引用，但不要重构 HUD。

### 问题 C：TowerAttack 注释仍写旧 API

注释中不能再写 `OverlapCircleNonAlloc`。

## 验证要求

- `rg "FindObjectsSortMode|FindFirstObjectByType|OverlapCircleNonAlloc" Assets/Scripts` 无结果。
- Unity Console 无 `error CS`，尤其不能再有 `CS1503`。
- Unity Console 无上述 obsolete warnings。
- Play smoke test：
  - 点击派兵。
  - HUD Dispatch。
  - O。
  - K/L/E/N。
  - Victory/Defeat 后 command 拒绝仍正常。

## 非阻塞日志

当前 Editor log 里有 Unity Connect / Project ID 401：

```text
Project ID request failed ... HTTP error code 401
```

这是 Unity services/auth 问题，不是游戏编译阻塞。本轮只记录，不处理。

## 禁止范围

- 不做新玩法系统。
- 不做正式 UI。
- 不重构战斗/占领系统。
- 不修改 ProjectSettings。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。
- 不提交 `.claude/`、`kingbattle/.idea/`。
- 不提交 `kingbattle/kingbattle.slnx`。
- 不提交 `要求.md` 删除。

## 验收标准

- 上述 API 搜索无残留。
- Console 无编译错误，尤其不能再有 `CS1503`。
- Console 无本轮目标 obsolete warnings。
- Play smoke test 通过。
- `WORKLOG.md` 已记录返修和验证。
