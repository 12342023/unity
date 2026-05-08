# REVIEW.md

## Review 状态

Codex 已审查 Claude 提交的 MVP-02.1 巡逻修正：

```text
583b1e7 fix: change patrol to circle around building, not walk between plots
```

结论：**暂不批准 MVP-02.1 通过**。

原因：圆周巡逻方向正确，但当前实现可能覆盖单位前往集结点的道路移动。

## CODEX PROJECT REVIEW

Gate: **FAIL**

### [P1] 巡逻 Tick 会覆盖单位前往集结点的道路移动

File: `kingbattle/Assets/Scripts/Combat/UnitCombat.cs:149`

Problem:

`UnitCombat.UpdateIdle()` 在没有敌人和 pushPath 时，只要存在 `UnitPatrol` 就调用 `patrol.Tick(Time.deltaTime)`。刚生成的单位此时仍可能有 `UnitMovement.HasRemainingPath == true`，正在沿道路前往 rally point。`UnitPatrol.Tick()` 最终会直接设置 `transform.position = CurrentPatrolPosition`，覆盖道路移动。

Impact:

单位可能不会从 Barracks 沿道路走到集结点，而是被巡逻逻辑直接拉到建筑/集结点旁边转圈。这破坏了 `goal.md` 的道路推进核心，也会让 Play Mode 里看起来像“瞬移到巡逻圈”。

Fix:

请 Claude 确保圆周巡逻只在单位已经完成道路移动后才启动。最小修正方向：

```diff
+ if (movement != null && movement.HasRemainingPath)
+     return;

  if (patrol != null)
      patrol.Tick(Time.deltaTime);
```

或者让 `UnitPatrol` 默认 paused，等 `UnitMovement` 到达目的地后再 `Resume()`。修法任选其一，但必须满足：

- 新兵先沿道路到达 rally/building 附近
- 到达后才围绕建筑/集结点转圈
- 巡逻不能覆盖 pushPath 或 UnitMovement 的道路移动
- Play Mode 验证无瞬移

### [P2] 无集结点时 Barracks 可能无法生成单位

File: `kingbattle/Assets/Scripts/Buildings/BarracksSpawner.cs:82`

Problem:

`SpawnUnit()` 在 `rallyPlotId` 为空时把 `destPlotId` 设为 `currentPlotId`，随后要求 `pathIds.Count >= 2`，否则直接 `return`。如果 `RoadPathFinder.FindPath(mapData, currentPlotId, currentPlotId)` 返回单点路径或空路径，无集结点 Barracks 就不会出兵。

Impact:

这与“没有集结点时围绕所属建筑转圈巡逻”的要求冲突。当前 `GameEntry` 给测试 Barracks 都设置了 rallyPlotId，所以测试场景未必暴露，但组件行为不完整。

Fix:

请 Claude 支持无 rally 的情况：

```diff
+ rallyPlotId 为空时，生成单位后直接以当前 Barracks / 当前地块为 patrolCenter
+ 不要求 pathIds.Count >= 2
+ 无集结点时也能正常出兵并围绕所属建筑转圈
```

### 已处理：[P1] Tower 攻击建筑问题

File: `kingbattle/Assets/Scripts/Buildings/TowerAttack.cs:51`

Problem:

`TowerAttack.FindNearestEnemy()` 通过 `Physics2D.OverlapCircleNonAlloc()` 找到范围内所有带 `HealthComponent` 的敌对对象，但没有区分单位和建筑。`GameEntry` 给建筑也添加了 `BoxCollider2D` 和 `HealthComponent`，并且玩家 Tower 位于 `Crossroads`，敌方 Tower 位于 `EnemyOutpost`，两者距离约 3.2，小于 Tower 的 `attackRange = 3.5`。

Impact:

进入 Play Mode 后，Tower 可能先攻击敌方 Tower / Barracks / Granary 等建筑，而不是只攻击进入范围的单位。这违反 MVP-02 要求“Tower 自动攻击范围内敌方单位”，也会干扰 Barracks 出兵与单位战斗链路验证。

Fix:

`TowerAttack.cs` 已增加 `UnitCombat` 过滤：

```diff
+ var unitCombat = hitBuffer[i].GetComponent<UnitCombat>();
+ if (unitCombat == null)
+     continue;
```

用户已确认验证 OK，修复已提交并推送：

```text
48f7eb5 fix: tower targets units only, add missing meta files
```

### 已处理：[P1] 新增脚本的 Unity meta 文件漏提交

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

相关 `.meta` 文件已纳入版本管理。Claude 后续提交 Unity 新脚本时必须同时提交对应 `.meta` 文件。

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

## 新 goal.md 下的阶段判断

当前不应继续推进“摧毁与重建”或“资源 / 升级 / 连地”。这些属于新 `goal.md` 的第三阶段。

下一轮应发布为：

```text
MVP-02.1 自动战争基础体验补强
```

范围：

- 巡逻
- 仇恨范围
- 脱战
- 集结点
- 小波次推进

禁止：

- 摧毁与重建
- 建筑升级
- 粮食资源
- 人口系统
- 连地系统
- 区域奖励
- 中央区域
- 传送阵
- AI
- UI / 美术 / 音效

## Codex 当前判断

MVP-02 修复已通过。MVP-02.1 当前需要小修：

```text
先沿道路到达建筑/集结点，再围绕建筑转圈巡逻
```

允许范围：

- 巡逻：必须是建筑周边小半径转圈巡逻，不是地块之间来回走
- 仇恨范围
- 脱战
- 集结点
- 小波次推进

## 新需求澄清

用户已明确：“我要的巡逻是他在一个建筑那边转圈。”

因此后续 Review MVP-02.1 时，Codex 会把以下行为视为不符合需求：

```diff
- 单位只在 Village ↔ Crossroads 之类地块之间来回走
- 单位巡逻表现像道路行军
```

期望行为：

```diff
+ 单位在 Barracks / 所属建筑 / 集结点附近小半径环绕
+ 多个单位可错开角度围绕建筑待命
+ 遇敌后从巡逻切换到接敌
+ 脱战后回到建筑附近继续转圈
```

在以上问题修复并通过 Play Mode 验证前，不批准 MVP-02.1 通过，也不批准进入摧毁与重建、资源、升级、连地、区域奖励、传送阵或 AI。
