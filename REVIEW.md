# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
3df2ced feat: building ruins with patrol around ruins after defeat
```

结论：**MVP-03.1 暂不通过**。

说明：废墟生成方向基本正确，建筑死亡后会生成可见 Ruin；但 `UnitCombat` 当前把“任何死亡目标”的位置都当成废墟巡逻中心。这样士兵打死普通敌方单位时，也可能围着单位死亡点转圈，违反 MVP-03.1 验收标准“普通单位死亡仍直接销毁，不变成废墟”。

## CODEX PROJECT REVIEW

Gate: **FAIL**

### 已通过：[P2] 建筑死亡后生成可见废墟

Files:

```text
kingbattle/Assets/Scripts/GameEntry.cs
kingbattle/Assets/Scripts/Buildings/RuinComponent.cs
```

Review:

`CreateBuilding()` 通过 `HealthComponent.OnDeath` 注册 `SpawnRuin()`，建筑死亡后会在原位置生成 Ruin GameObject。Ruin 只有 `SpriteRenderer`、`BoxCollider2D`、`RuinComponent`，没有 `TowerAttack`、`BarracksSpawner`、`HealthComponent`，因此不会继续攻击、出兵或被当成战斗目标。

这一点符合 MVP-03.1 的最小目标。

### [P1] 普通单位死亡也会把巡逻中心切到死亡点

File:

```text
kingbattle/Assets/Scripts/Combat/UnitCombat.cs:205
```

Problem:

`UpdateAttack()` 当前在任何 `target.IsDead` 情况下都会传入死亡位置：

```csharp
Vector3 deathPos = target.transform.position;
Deaggro(deathPos);
```

`Deaggro(defeatedPos)` 又会无条件把 patrol center 改成这个位置：

```csharp
if (defeatedPos.HasValue && patrol != null)
{
    Vector3 center = new Vector3(defeatedPos.Value.x, defeatedPos.Value.y, -0.2f);
    homePosition = center;
    patrol.Setup(center, 0.9f, 0f);
}
```

但 `target` 可能是敌方单位，也可能是敌方建筑。普通单位没有废墟，也不应该成为新的巡逻中心。

Impact:

士兵在路上打死敌方 Soldier 后，可能开始围绕那个单位死亡点转圈，而不是继续原本 rally/patrol 逻辑。这会破坏波次推进和废墟巡逻的语义：只有建筑被击败后才应该产生废墟巡逻。

Fix:

请 Claude 做最小修复：

```diff
+ 只有被击败目标是建筑时，才把 patrol center 切到 defeatedPos
+ 判断方式可用 target.GetComponent<UnitCombat>() == null 作为当前项目里的建筑/单位区分
+ 或新增清晰的小标记组件，但不要重构整体战斗系统
+ 普通单位死亡时只 Deaggro，不传 defeatedPos，不改变 patrol center
+ 保持 Ruin 生成逻辑只对建筑生效
```

### [P2] 废墟逻辑放在 GameEntry，后续应下沉到建筑组件

File:

```text
kingbattle/Assets/Scripts/GameEntry.cs
```

Problem:

`SpawnRuin()` 目前写在 `GameEntry` 中。MVP-03.1 可以接受这个原型，但 `GameEntry` 的职责应该保持“装配测试场景”，不应长期承载建筑死亡业务逻辑。

Fix:

本轮不要求大改。建议 Claude 如果能小范围处理，就把建筑死亡生成废墟下沉到 `Buildings/BuildingDeathHandler.cs` 或类似小组件；如果会扩大改动，可以先保留，并在 `WORKLOG.md` 标记为后续整理项。

## 残留注意事项

### ProjectSettings 新增文件仍未处理

当前工作树仍有未跟踪文件：

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件由 Unity Editor 生成。由于项目规则要求不随意修改 `ProjectSettings`，继续保持未提交状态。后续需要单独确认是否提交或忽略。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.1 暂不通过。

问题：
你现在在 UnitCombat 里对任何 target.IsDead 都调用 Deaggro(deathPos)，然后 Deaggro 会把 patrol center 切到 defeatedPos。这样士兵打死普通敌方单位时，也会围着普通单位死亡点转圈。

请只修这个问题：
- 只有被击败目标是建筑时，才把 patrol center 切到目标死亡位置 / 废墟位置。
- 普通单位死亡时，只 Deaggro，不改变 patrol center。
- 当前项目里可以用 target.GetComponent<UnitCombat>() == null 判断建筑，或用一个很小的标记组件，但不要重构战斗系统。
- 保持建筑死亡后生成废墟。
- 保持废墟无攻击、无出兵、无 HealthComponent。

不要实现重建、占领进度、资源、升级、连地、区域奖励、传送阵、AI 或 UI。

完成后更新 WORKLOG.md，说明修改文件、Play Mode 验证步骤、是否修改场景/ProjectSettings，并 commit / push。
```
