# REVIEW.md

## Review 状态

Codex 已检查 Claude 当前 MVP-03.10 本地改动：

```text
RuinComponent.CanUseAsRallyPoint(MapData)
GameEntry T 聚兵测试快捷键
```

结论：**MVP-03.10 暂未通过，需要先修 R 测试入口**。

说明：T 聚兵方向基本符合任务，但用户反馈“R 键不能完成”。从 Unity 日志和代码看，原因是 `GameEntry` 的 R 键仍然只尝试 `ruins[0]`。当 `ruins[0]` 是大本营废墟时，`BuildingRebuildService` 会按规则拒绝重建，然后 R 直接失败，不会继续尝试后面的普通废墟。

## CODEX PROJECT REVIEW

Gate: **FAIL**

Findings:

### [P1] R 测试入口遇到 main base ruin 后不继续查找普通废墟

File:

```text
kingbattle/Assets/Scripts/GameEntry.cs:77
```

Problem:

当前 R 键逻辑：

```csharp
var ruins = FindObjectsByType<RuinComponent>(FindObjectsSortMode.None);
if (ruins.Length > 0)
{
    var result = BuildingRebuildService.Rebuild(ruins[0], mapData, Faction.Player);
    ...
}
```

这会依赖 Unity 返回顺序。现在 main base ruin 不能重建是正确规则，但 R 键如果先拿到 EnemyBase / PlayerBase 废墟，就会一直失败：

```text
[BuildingRebuildService] Main base ruin at EnemyBase cannot be rebuilt.
[GameEntry] Test shortcut R: rebuild failed (no valid ruin).
```

用户看到的就是“R 键不能完成”。

Fix:

R 键应遍历所有废墟，尝试重建第一个可重建的普通废墟：

```diff
- 只调用 BuildingRebuildService.Rebuild(ruins[0], ...)
+ foreach (var ruin in ruins)
+     if (ruin.IsMainBaseRuin(mapData)) continue;
+     var result = BuildingRebuildService.Rebuild(ruin, mapData, Faction.Player);
+     if (result != null) break;
+ 如果没有成功重建任何废墟，再输出 no rebuildable ruins
```

注意：

- 不要允许 main base ruin 被 R 重建。
- 不要销毁 main base ruin。
- 不要改正式 UI 或资源系统。

## 已通过部分

- `RuinComponent.CanUseAsRallyPoint(MapData)` 的方向符合 MVP-03.10。
- T 键聚兵方向符合任务边界。
- 没有看到新的 `ProjectSettings` 应提交内容。

## 需要 Claude 修复

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.10 暂未通过。

用户反馈：R 键不能完成。

原因：
- GameEntry 的 R 键当前只尝试 ruins[0]。
- 如果第一个废墟是 EnemyBase / PlayerBase 这种 main base ruin，BuildingRebuildService 会正确拒绝重建。
- 但 R 键没有继续尝试后面的普通废墟，所以用户看到 R 一直失败。

请修复：
- 修改 GameEntry 的 R 测试快捷键。
- R 应遍历所有 RuinComponent。
- 跳过 ruin.IsMainBaseRuin(mapData) == true 的废墟。
- 对第一个普通可重建废墟调用 BuildingRebuildService.Rebuild(...).
- 一旦重建成功就停止遍历并输出 rebuild OK。
- 如果没有任何普通可重建废墟，输出 no rebuildable ruins。

必须保持：
- EnemyBase / PlayerBase 大本营废墟仍不能被 R 重建。
- 普通非大本营废墟仍可被 R 重建。
- T 聚兵功能保持不变。
- K / L 清场行为不变。
- 不修改 ProjectSettings。
- 不提交 kingbattle/ProjectSettings/SceneTemplateSettings.json。

完成后更新 WORKLOG.md，说明 R/K/T/L Play Mode 验证，并 commit / push。
```
