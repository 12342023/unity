# TASK.md

## 当前任务

MVP-04.7 返修：交付前清理补验证。

Codex 当前仍作为 Tech Lead 和 Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
2bfb30b cleanup: remove unused hasHome field, guard debug/test entry points, reduce log noise
```

Codex Review 结论：

```text
MVP-04.7 基本正确，但需要小返修 / 补验证。
```

## 已通过部分

- `UnitCombat.hasHome` 已删除。
- `TestUnitSpawner` / `DebugShortcutController` 已限制在 `UNITY_EDITOR || DEVELOPMENT_BUILD`。
- 明显高频 runtime logs 已收敛。
- 目标 obsolete API 残留为 0。

## 返修 A：`GameEntry` release-only warning 风险

当前 `GameEntry` 写法：

```csharp
var (playerBaseHp, enemyBaseHp) = SetupBuildings(mapData);
```

这两个变量只在 debug/development build 区块中使用：

```csharp
#if UNITY_EDITOR || DEVELOPMENT_BUILD
debugCtrl.Initialize(mapData, mapRenderer, enemyController, playerBaseHp, enemyBaseHp);
#endif
```

正式 build 预处理后可能留下 unused local warning。

要求改为：

```csharp
#if UNITY_EDITOR || DEVELOPMENT_BUILD
var (playerBaseHp, enemyBaseHp) = SetupBuildings(mapData);
#else
SetupBuildings(mapData);
#endif
```

要求：

- 不改 `SetupBuildings` 行为。
- 不重构 `GameEntry`。
- 不改玩法。

## 返修 B：补 Unity 编译验证

当前 `Editor.log` 尾部还停留在旧编译记录，仍显示修改前的 `UnitCombat.hasHome` warning。代码已经删除字段，但必须触发 Unity 刷新/重新编译后确认。

验证要求：

- Unity Console 无 `error CS`。
- Unity Console 无 `UnitCombat.hasHome` warning。
- `rg "hasHome|FindObjectsSortMode|FindFirstObjectByType|OverlapCircleNonAlloc" Assets/Scripts` 无结果。
- Play smoke test：
  - 点击派兵。
  - HUD Dispatch。
  - `O` 派兵。
  - `K/L/E/N` 在 Editor Play Mode 仍可用。
  - Victory/Defeat 后 command 拒绝仍正常。

## 返修 C：更新 WORKLOG.md

记录：

- 小返修文件。
- Unity 编译验证结果。
- 未修改 `ProjectSettings`、场景文件、`Library/`、`Logs/`、`UserSettings/`。

## 禁止范围

- 不做新玩法系统。
- 不做正式 UI。
- 不重构 `GameEntry`。
- 不修改 `ProjectSettings`。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。
- 不提交 `.claude/`、`kingbattle/.idea/`。
- 不提交 `kingbattle/kingbattle.slnx`。
- 不提交 `要求.md` 删除。

## 验收标准

- Console 无编译错误。
- Console 无 `UnitCombat.hasHome` warning。
- Debug/test 入口仍有明确 build 条件边界。
- `WORKLOG.md` 已记录返修和验证。
