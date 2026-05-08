# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
91d2bfb fix: prevent patrol overwriting road movement, handle missing rally point
```

结论：**暂不批准 MVP-02.1 通过**。

原因：上一轮两个阻塞点已修复，但脱战返回巡逻仍存在瞬移风险。单位从追击/攻击脱战后，`Deaggro()` 只移动一小步，下一帧 `UpdateIdle()` 可能直接调用 `patrol.Tick()`，而 `UnitPatrol.Tick()` 会直接设置 `transform.position` 到巡逻圆位置。

## CODEX PROJECT REVIEW

Gate: **FAIL**

### 已修复：[P1] 巡逻 Tick 覆盖前往集结点的道路移动

Files:

```text
kingbattle/Assets/Scripts/Combat/UnitCombat.cs:145
kingbattle/Assets/Scripts/Units/UnitPatrol.cs:27
```

Review:

Claude 已在 `UpdateIdle()` 中增加守卫：

```text
patrol != null && pushPath == null && movement != null && !movement.HasRemainingPath
```

同时 `UnitPatrol.Setup()` 不再初始化时直接 snap 到巡逻圆。该方向符合要求：

```diff
+ 新兵沿道路移动期间不会被 patrol.Tick() 覆盖
+ 到达 rally/building 后才开始圆周巡逻
```

### 已修复：[P2] 无 rallyPoint 时 Barracks 可能无法出兵

File:

```text
kingbattle/Assets/Scripts/Buildings/BarracksSpawner.cs:90
```

Review:

`SpawnUnit()` 已改为先创建单位，再判断是否需要道路移动：

```text
needRoadMove = destPlotId != currentPlotId
```

无 rallyPoint 时不再要求 `pathIds.Count >= 2`，单位可以围绕所属 Barracks 本地巡逻。该问题通过代码审查。

### [P1] 脱战后下一帧仍可能被 patrol.Tick() 拉回巡逻圆

Files:

```text
kingbattle/Assets/Scripts/Combat/UnitCombat.cs:218
kingbattle/Assets/Scripts/Combat/UnitCombat.cs:145
kingbattle/Assets/Scripts/Units/UnitPatrol.cs:57
kingbattle/Assets/Scripts/Units/UnitPatrol.cs:86
```

Problem:

`Deaggro()` 当前逻辑：

```text
1. state = Idle
2. patrol.Resume()
3. MoveTowards(transform.position, patrol.CurrentPatrolPosition, one frame step)
```

但下一帧进入 `UpdateIdle()` 后，如果没有敌人、没有 pushPath、没有 remaining road path，就会调用 `patrol.Tick(Time.deltaTime)`。`UnitPatrol.Tick()` 最终调用 `ApplyCirclePosition()`，直接执行：

```csharp
transform.position = CurrentPatrolPosition;
```

如果单位追敌离开了建筑/集结点较远，下一帧会被直接拉回巡逻圆，看起来像瞬移。

Impact:

这违反用户对巡逻的明确要求：

```text
遇敌后从巡逻切换到接敌，脱战后回到建筑附近继续转圈
```

这里的“回到”应该是可观察的移动返回，而不是突然跳回巡逻圆。该问题会破坏自动战争的空间可信度，也会影响后续 macOS / Android / 微信小程序移植时的表现一致性。

Fix:

请 Claude 做最小修复，不要重构整套状态机。可选方向：

```diff
+ 增加 ReturningToPatrol / ReturningHome 状态
+ 脱战后先 MoveTowards 回到 patrol.CurrentPatrolPosition 或 patrol.Center 附近
+ 距离小于阈值后才 Resume 并允许 patrol.Tick()
+ UnitPatrol.Tick() 不应在单位离巡逻圆很远时直接 snap
```

验收条件：

- 单位追敌离开建筑/集结点后，目标死亡或超出 chaseRange，会走回巡逻圆附近。
- 返回过程中不能瞬移。
- 回到巡逻圆附近后才继续围绕建筑/集结点转圈。
- Console 无明显错误。
- 不引入全局单例，不扩展第三阶段系统。

### [P2] 前往 rally 途中接敌后，原道路移动会被 Stop 清掉

File:

```text
kingbattle/Assets/Scripts/Combat/UnitCombat.cs:112
```

Problem:

`UpdateIdle()` 在扫描到敌人时会执行：

```csharp
movement?.Stop();
```

如果单位此时还在沿道路前往 rallyPoint，`Stop()` 会清空 `UnitMovement` 的剩余路径。战斗结束后，单位不再知道自己原本还要继续走到 rallyPoint，只能进入当前的巡逻/回家逻辑。

Impact:

这不一定立刻阻塞 MVP-02.1 的主演示，但会造成边界行为不稳定：单位在前往集结点途中接敌，脱战后可能不恢复原路线。后续“多线调兵”和“小波次推进”会更容易暴露这个问题。

Fix:

请 Claude 与 P1 一起小修：

```diff
+ 如果单位是在 road movement 中接敌，脱战后应继续原路线，或明确转入返回 rally/home 的移动状态
+ 不要在业务逻辑中留下“路径被清掉但状态不知道”的半完成状态
```

## 通过项

- 当前提交没有提前实现摧毁与重建、资源、升级、连地、区域奖励、传送阵或 AI。
- `Buildings/`、`Combat/`、`Units/` 边界仍然清晰。
- `GameEntry` 没有被继续塞入巡逻/战斗核心逻辑。
- 无 rallyPoint 出兵路径已补齐。
- `ProjectSettings` 未纳入本次提交。

## 残留注意事项

### ProjectSettings 新增文件仍未处理

当前工作树仍有未跟踪文件：

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件由 Unity Editor 生成。由于项目规则要求不随意修改 `ProjectSettings`，继续保持未提交状态。后续需要单独确认是否提交或忽略。

### Health 颜色反馈会覆盖阵营颜色

`HealthComponent` 默认用白色作为满血颜色，受到伤害后会把建筑 / 单位颜色往白色和红色之间插值。功能上不阻塞 MVP-02.1，但后续如果颜色用于阵营识别，应让 `HealthComponent` 在 `Start()` 记录原始 `SpriteRenderer.color` 作为满血颜色。

## 给 Claude 的下一条任务

请 Claude 继续修复 MVP-02.1，不要进入新系统：

```text
修复脱战返回巡逻：单位追敌离开建筑/集结点后，目标死亡或超出 chaseRange 时，必须可见地走回巡逻圆附近，然后再恢复围绕建筑/集结点转圈。禁止下一帧 patrol.Tick 直接把单位 snap 回 CurrentPatrolPosition。

同时处理前往 rallyPoint 途中接敌的边界：不要让 movement.Stop() 清掉路线后没有恢复策略。可以选择恢复原道路路线，或脱战后明确返回 rally/home，再恢复巡逻。

只做最小修改；不要开发摧毁与重建、资源、升级、连地、区域奖励、传送阵、AI 或 UI。

完成后请更新 WORKLOG.md，说明修改文件、Play Mode 验证步骤、是否修改场景/ProjectSettings，并 commit / push。
```

在以上问题修复并通过 Play Mode 验证前，不批准 MVP-02.1 通过，也不批准进入第三阶段系统。
