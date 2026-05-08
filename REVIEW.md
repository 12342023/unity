# REVIEW.md

## Review 状态

Codex 已完成 Claude 的 MVP-02 建筑、出兵与基础战斗原型静态审查。

结论：**暂不批准进入 MVP-03**。

原因：MVP-02 主体方向正确，但 Tower 当前会攻击敌方建筑，不只攻击敌方单位；同时新增脚本的 Unity `.meta` 文件漏提交，需要补齐仓库状态。

## CODEX PROJECT REVIEW

Gate: **FAIL**

### [P1] Tower 会攻击敌方建筑，导致开局建筑互打

File: `kingbattle/Assets/Scripts/Buildings/TowerAttack.cs:51`

Problem:

`TowerAttack.FindNearestEnemy()` 通过 `Physics2D.OverlapCircleNonAlloc()` 找到范围内所有带 `HealthComponent` 的敌对对象，但没有区分单位和建筑。`GameEntry` 给建筑也添加了 `BoxCollider2D` 和 `HealthComponent`，并且玩家 Tower 位于 `Crossroads`，敌方 Tower 位于 `EnemyOutpost`，两者距离约 3.2，小于 Tower 的 `attackRange = 3.5`。

Impact:

进入 Play Mode 后，Tower 可能先攻击敌方 Tower / Barracks / Granary 等建筑，而不是只攻击进入范围的单位。这违反 MVP-02 要求“Tower 自动攻击范围内敌方单位”，也会干扰 Barracks 出兵与单位战斗链路验证。

Fix:

请 Claude 让 Tower 只选择单位目标。最小修法可以在 `FindNearestEnemy()` 中过滤掉没有 `UnitCombat` 的对象：

```diff
+ var unitCombat = hitBuffer[i].GetComponent<UnitCombat>();
+ if (unitCombat == null)
+     continue;
```

需要同时添加对应 namespace：

```diff
+ using Combat;
```

`TowerAttack.cs` 已经引用 `Combat`，因此只需要在筛选逻辑里加单位判定即可。修完后重新 Play Mode 验证：Tower 不应攻击建筑，只攻击进入范围的敌方 Soldier。

### [P1] 新增脚本的 Unity meta 文件漏提交

File: `kingbattle/Assets/Scripts/Buildings.meta`

Problem:

MVP-02 提交 `fc1a6bc` 已包含新增 C# 脚本，但当前工作树仍有以下未跟踪 `.meta` 文件：

```text
kingbattle/Assets/Scripts/Buildings.meta
kingbattle/Assets/Scripts/Buildings/BarracksSpawner.cs.meta
kingbattle/Assets/Scripts/Buildings/TowerAttack.cs.meta
kingbattle/Assets/Scripts/Combat.meta
kingbattle/Assets/Scripts/Combat/HealthComponent.cs.meta
kingbattle/Assets/Scripts/Combat/UnitCombat.cs.meta
kingbattle/Assets/Scripts/Core/BuildingType.cs.meta
```

Impact:

Unity 项目应提交源码对应 `.meta` 文件，保持 GUID 稳定。漏提交会让其他机器重新生成 meta，后续引用、Prefab、Scene 或 Inspector 绑定可能出现 GUID 漂移。

Fix:

Codex 本轮会把这些 `.meta` 文件作为仓库卫生补提交。Claude 后续提交 Unity 新脚本时必须同时提交对应 `.meta` 文件。

## 通过项

- MVP-02 范围基本受控，没有提前实现 AI、占领、粮食资源、建造 UI、英雄、随机地图或联机。
- 已新增 `Buildings/` 与 `Combat/` 边界，未出现大型 `GameManager`。
- Barracks 出兵逻辑独立在 `BarracksSpawner`，没有堆进 `GameEntry`。
- UnitMovement 增加 Pause / Resume 支持，单位进入战斗时暂停移动的方向合理。
- HealthComponent 统一处理 health、damage、death，满足 MVP-02 最小战斗需求。
- `ProjectSettings` 未纳入本次业务提交。

## 残留注意事项

### ProjectSettings 新增文件仍未处理

当前工作树仍有未跟踪文件：

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件由 Unity Editor 生成。由于项目规则要求不随意修改 `ProjectSettings`，继续保持未提交状态。后续需要单独确认是否提交或忽略。

### Health 颜色反馈会覆盖阵营颜色

`HealthComponent` 默认用白色作为满血颜色，受到伤害后会把建筑 / 单位颜色往白色和红色之间插值。功能上不阻塞 MVP-02，但后续如果颜色用于阵营识别，应让 `HealthComponent` 在 `Start()` 记录原始 `SpriteRenderer.color` 作为满血颜色。

## Codex 当前判断

MVP-02 需要小修后复审：

```text
Tower 只攻击单位，不攻击建筑
确认新增脚本 meta 文件已提交
Play Mode 重新验证 Tower 攻击行为
更新 WORKLOG.md
再 commit / push
```

在上述问题修复前，不批准进入 MVP-03。
