# TASK.md

## 当前任务

MVP-04.6：Unity 6 obsolete warning cleanup。

Codex 当前仍作为 Tech Lead 和 Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
31da520 chore: Unity 6 migration — packages, URP, project settings
```

Codex Review 结论：

```text
Unity 6 迁移文件收口通过；允许进入 Unity 6 obsolete warning cleanup。
```

## 本轮目标

```text
清理 Unity 6 API 过时警告，只做兼容性小改，不改变玩法行为。
```

## 允许范围

任务 A：TowerAttack 物理查询 API

- 文件：`Assets/Scripts/Buildings/TowerAttack.cs`
- 当前问题：`Physics2D.OverlapCircleNonAlloc` obsolete。
- 改为 Unity 6 推荐 API。
- 保持 Tower 行为不变：
  - 按 interval 扫描。
  - 只攻击敌方 `UnitCombat`。
  - 不攻击建筑。

任务 B：FindObjectsByType overload

- 清理 `FindObjectsByType<T>(FindObjectsSortMode.None)`。
- 涉及文件：
  - `DebugShortcutController.cs`
  - `StrategicConnectionService.cs`
  - `StrategicDispatchService.cs`
  - `FactionStatsService.cs`
  - `EnemyAttackCommandService.cs`
  - `FactionDefeatHandler.cs`
- 改成 Unity 6 推荐 overload。
- 保持语义：只查询当前 active runtime objects。

任务 C：GameHud 查找输入控制器

- 文件：`Assets/Scripts/UI/GameHud.cs`
- 当前问题：`FindFirstObjectByType<PlayerInputController>()` obsolete。
- 改为 `FindAnyObjectByType<PlayerInputController>()` 或轻量缓存引用。
- 不重构 HUD。

任务 D：验证

- Unity Console 无上述 obsolete warnings。
- Unity Console 无 `error CS`。
- Play smoke test：
  - 点击派兵。
  - HUD Dispatch。
  - O。
  - K/L/E/N。
  - Victory/Defeat 后 command 拒绝仍正常。

任务 E：文档

- 更新 `WORKLOG.md`。
- 记录每个替换点和验证结果。

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

- Unity 6 obsolete warnings 中上述三类已清理。
- Console 无编译错误。
- Play smoke test 通过。
- `WORKLOG.md` 已记录变更和验证。
- 不应提交文件仍未提交。
