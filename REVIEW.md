# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
a5f49ad fix: handle base defeat faction cleanup
```

结论：**MVP-03.1 / MVP-03.2 代码审查通过**。

说明：Claude 已修复 `target == null` / `chaseTarget == null` 被误当成建筑的问题，并实现“大本营被击败后，该阵营所有建筑变废墟、所有士兵立即死亡”的最小闭环。当前代码方向符合 `TASK.md` / `NEXT_STEPS.md` 的范围要求。

## CODEX PROJECT REVIEW

Gate: **PASS**

### 已通过：[P1] null target 不再切换 patrol center

File:

```text
kingbattle/Assets/Scripts/Combat/UnitCombat.cs
```

Review:

`UnitCombat` 新增 `GetDefeatedBuildingPos(HealthComponent defeatedTarget)`：

```csharp
if (defeatedTarget == null)
    return null;
if (defeatedTarget.GetComponent<UnitCombat>() != null)
    return null;
return defeatedTarget.transform.position;
```

这满足上一轮 review 要求：

```diff
+ target == null 或 chaseTarget == null 时，只 Deaggro()
+ 只有目标非空且无 UnitCombat 时，才 Deaggro(deathPos)
```

### 已通过：[P1] 大本营击败后触发阵营清场

Files:

```text
kingbattle/Assets/Scripts/Buildings/FactionDefeatHandler.cs
kingbattle/Assets/Scripts/Combat/HealthComponent.cs
kingbattle/Assets/Scripts/GameEntry.cs
```

Review:

`FactionDefeatHandler.OnMainBaseDefeated()` 会：

1. 遍历该阵营建筑列表。
2. 跳过 null 或 `IsDead` 的建筑，避免重复废墟。
3. 对仍存活建筑调用 `HealthComponent.Kill()`，触发原有 `OnDeath -> SpawnRuin -> Destroy`。
4. 通过 `FindObjectsByType<HealthComponent>(FindObjectsSortMode.None)` 找到该阵营所有带 `UnitCombat` 的存活单位并 `Kill()`。

`GameEntry` 将 `PlayerBase` / `EnemyBase` 的 Barracks 作为双方大本营，并把对应 `OnDeath` 绑定到阵营清场处理器。Unity 版本为 `2022.3.62f1`，`FindObjectsByType` API 可用。

### 已通过：[P2] HealthComponent.Kill() 复用 OnDeath 语义

File:

```text
kingbattle/Assets/Scripts/Combat/HealthComponent.cs
```

Review:

`Kill()` 与 `TakeDamage()` 的死亡路径保持一致：设置 `CurrentHealth = 0f`，触发 `OnDeath`，然后 `Destroy(gameObject)`。因此建筑清场能复用既有废墟生成逻辑，单位清场也能直接销毁。

## 残留注意事项

### Play Mode 仍需用户最终确认

Codex 本轮做了代码审查，没有在 Unity Editor 中亲自运行 Play Mode。Claude 的 `WORKLOG.md` 记录了 Play Mode 验证通过，但用户仍应在本机确认以下场景：

1. 蓝方击败 `EnemyBase` 后，敌方 `EnemyBase` 和 `EnemyOutpost` 都变成废墟。
2. 所有红方士兵立即死亡。
3. 蓝方士兵仍存活，并围绕废墟巡逻。
4. 红方击败 `PlayerBase` 时，蓝方建筑和士兵同样被清场。
5. Console 无明显错误。

### ProjectSettings 新增文件仍未处理

当前工作树仍有未跟踪文件：

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件由 Unity Editor 生成。由于项目规则要求不随意修改 `ProjectSettings`，继续保持未提交状态。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.1 / MVP-03.2 代码审查通过。

请先不要继续做新功能，等待用户 Play Mode 最终确认：
- 击败 EnemyBase 后，敌方所有建筑变成废墟。
- 所有敌方士兵立即死亡。
- 己方士兵仍存活，并围绕废墟巡逻。
- 反向验证 PlayerBase 被击败时蓝方被清场。
- Console 无明显错误。

如果用户确认 OK，再进入下一条小任务。
下一条建议任务先不要做完整资源/升级系统，而是整理建筑死亡/废墟职责边界：
- 将 SpawnRuin / 建筑死亡处理从 GameEntry 逐步下沉到 Buildings 下的小组件。
- GameEntry 只负责装配测试场景。
- 不做重建、占领进度、资源、升级、连地、区域奖励、传送阵、AI 或 UI。
```
