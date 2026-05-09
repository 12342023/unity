# TASK.md

## 当前任务

发布 MVP-03.10：大本营废墟聚兵点最小原型。

Codex 当前仍作为 Tech Lead / Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 项目定位

这是一个 Unity 小游戏项目，当前主工程位于：

```text
kingbattle/
```

项目定位：

```text
低操作、高战略、自动战争 RTS
```

后续规划包括：

- 微信小程序移植
- macOS 移植
- Android 移植

当前阶段优先保持 Unity 工程结构清晰，不把未来平台差异散落在业务逻辑中。

## 最新 Review 结论

MVP-03.9 最新修复提交：

```text
a2472bb fix: add Map namespace for main base ruin rule
```

Codex Review 结论：

```text
MVP-03.9 代码审查通过；允许进入 MVP-03.10
```

## 用户规则

用户要求：

```text
最后的敌方大本营不能重建，但是可以聚兵，也可以连接别的未占领的地方可以派兵。
```

已经完成：

- 大本营废墟不能通过普通 `BuildingRebuildService` 重建。

本轮只处理：

- 大本营废墟可以作为聚兵点。

暂不处理：

- 连接别的未占领地。
- 正式派兵系统。

## 当前工作区注意事项

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件仍是未跟踪 Unity Editor 生成文件。除非用户明确批准，否则不要提交。

## MVP-03.10 目标

让大本营废墟具备最小“聚兵点”能力，并能在 Play Mode 通过临时测试入口观察到。

```text
MainBase ruin != rebuildable building
MainBase ruin == rally point for troops
```

## MVP-03.10 允许范围

- 修改 `RuinComponent.cs`，新增最小数据层方法，例如：

```csharp
public bool CanUseAsRallyPoint(MapData mapData)
{
    return IsMainBaseRuin(mapData);
}
```

- 在 `GameEntry` 增加临时测试快捷键，例如 `T`。
- `T` 的行为：
  - 找到第一个 main base ruin。
  - 找到当前存活的 Player 士兵。
  - 让这些士兵围绕该废墟巡逻。
- 聚兵实现可以复用：
  - `UnitPatrol.Setup(center, radius, staggerAngle)`
  - `UnitMovement.Stop()`
  - `UnitCombat.ClearPushPath()`
- 多个士兵使用不同 `staggerAngle`，避免重叠。
- 保持 `K` / `L` / `R` 测试快捷键现有行为。
- 更新 `WORKLOG.md`。

## MVP-03.10 禁止范围

- 不做正式按钮或 UI。
- 不做连接未占领地的正式系统。
- 不做正式派兵系统。
- 不做资源、占领进度、升级、区域奖励、传送阵、AI。
- 不修改 `ProjectSettings`。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。

## MVP-03.10 验收标准

- 按 K 击败 EnemyBase 后，EnemyBase 废墟仍不能被 R 重建。
- 按 T 后，当前 Player 士兵会聚到 EnemyBase 废墟周围巡逻。
- 多个士兵不会完全重叠在同一个点。
- 普通非大本营废墟仍可按当前测试逻辑重建。
- K / L 清场行为不变。
- Console 无明显错误。
- 仍未修改或提交 `ProjectSettings`。
