# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
10abfb2 fix: smooth deaggro return to patrol circle, no teleport
```

结论：**暂不批准 MVP-02.1 通过**。

原因：Claude 已尝试修复脱战返回，但当前实现仍不是“走回巡逻圆后再转圈”。它只让单位走回 `patrol.Center` 附近，一旦进入较大的阈值范围，就继续调用 `patrol.Tick()`；而 `UnitPatrol.Tick()` 仍然直接设置 `transform.position = CurrentPatrolPosition`，所以仍可能产生位置跳变。前往 rallyPoint 途中接敌后 `movement.Stop()` 清空路线的问题也未处理。

## CODEX PROJECT REVIEW

Gate: **FAIL**

### 已修复：[P1] Deaggro 不再只移动一帧

Files:

```text
kingbattle/Assets/Scripts/Combat/UnitCombat.cs:145
kingbattle/Assets/Scripts/Combat/UnitCombat.cs:232
```

Review:

`Deaggro()` 已删除旧的一帧 `MoveTowards`，改为 `patrol?.Resume()`，并把返回逻辑放进 `UpdateIdle()` 持续执行。这比上一版更接近目标。

### [P1] 进入 center 阈值后仍可能被 patrol.Tick() snap 到巡逻圆

Files:

```text
kingbattle/Assets/Scripts/Combat/UnitCombat.cs:145
kingbattle/Assets/Scripts/Units/UnitPatrol.cs:57
kingbattle/Assets/Scripts/Units/UnitPatrol.cs:89
```

Problem:

当前返回逻辑是：

```csharp
float distToCenter = Vector3.Distance(transform.position, patrol.Center);
float returnThreshold = patrol.Radius * 1.5f + 0.5f;
if (distToCenter > returnThreshold)
{
    transform.position = Vector3.MoveTowards(transform.position, patrol.Center, step);
    return;
}
```

`radius = 0.9` 时，阈值约为 `1.85`。这意味着单位只要进入中心点 1.85 范围内，就会停止“返回”逻辑，开始执行：

```csharp
patrol.Tick(Time.deltaTime);
```

但 `UnitPatrol.Tick()` 最终仍然直接：

```csharp
transform.position = CurrentPatrolPosition;
```

如果单位在距离中心 1.8 的位置，而当前巡逻圆目标点在另一侧，下一帧仍可能被拉动很大距离。即使单位走到 `Center`，恢复 Tick 时也会从中心跳到半径 `0.9` 的圆周点。这不是可见的平滑返回。

Impact:

Play Mode 里仍可能看到单位追敌脱战后靠近建筑/集结点时突然跳到圆周位置。用户要求的是“回到建筑附近继续转圈”，不是“靠近一点后被吸到巡逻圆上”。

Fix:

请 Claude 做更小但更精确的修复：

```diff
+ 返回目标应是 patrol.CurrentPatrolPosition，或当前角度对应的圆周切入点
+ 只有距离该目标足够近，例如 <= 0.05 或 <= 0.1，才允许 patrol.Tick()
+ 返回期间不要调用 patrol.Tick()
+ 或者把 UnitPatrol.Tick() 改成 MoveTowards 到 CurrentPatrolPosition，再进入圆周推进
```

不要新增复杂 AI，也不要做废墟系统。

### [P2] 前往 rally 途中接敌后，原道路移动仍会被 Stop 清掉

Files:

```text
kingbattle/Assets/Scripts/Combat/UnitCombat.cs:112
kingbattle/Assets/Scripts/Units/UnitMovement.cs:63
```

Problem:

`UpdateIdle()` 扫描到敌人时仍然执行：

```csharp
movement?.Stop();
```

`UnitMovement.Stop()` 会清空：

```csharp
hasPath = false;
waypoints = null;
```

如果单位正在前往 rallyPoint，这条路线会丢失。Claude 本次提交没有修改 `UnitMovement`，也没有在 `UnitCombat` 中保存或恢复被打断的路线。

Impact:

单位在前往集结点途中接敌，脱战后不一定能继续到 rallyPoint。后续“多线调兵”和“小波次推进”会依赖这个行为，因此现在应该至少给出明确策略：恢复路线，或明确转入 return-to-rally/home。

Fix:

最小修复方向：

```diff
+ 不要用 Stop() 清掉 road movement 路线；可改为 Pause()/Resume()
+ 如果必须 Stop()，则保存 interrupted route 或 rally/home 返回目标
+ 脱战后恢复原路线，或明确走回 rally/home，再进入巡逻
```

### 通过项

- 本次提交没有实现建筑变废墟，也没有抢跑第三阶段。
- 未修改 `ProjectSettings`。
- `UnitPatrol` 只新增 `Radius` 只读属性，范围较小。
- `GameEntry` 未继续膨胀。

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

请 Claude 继续小修 MVP-02.1，不要进入第三阶段：

```text
Codex Review 仍不通过。

问题 1：
你现在是走回 patrol.Center 附近，进入 returnThreshold 后就调用 patrol.Tick()。但 patrol.Tick() 仍然会直接 transform.position = CurrentPatrolPosition，所以单位仍可能从 center 附近或阈值边缘跳到圆周点。

请改成：脱战后持续 MoveTowards 到 patrol.CurrentPatrolPosition 或一个明确的圆周切入点。只有距离该点 <= 0.05~0.1 时，才允许恢复 patrol.Tick()。返回期间禁止 Tick。

问题 2：
前往 rallyPoint 途中接敌时，movement.Stop() 仍会清掉 UnitMovement 路线。请改为 Pause/Resume，或保存被打断的路线，或脱战后明确返回 rally/home。不要留下“路线丢了但状态不知道”的情况。

只做以上两点最小修复。不要实现建筑废墟、重建、资源、升级、连地、区域奖励、传送阵、AI 或 UI。

完成后更新 WORKLOG.md，说明修改文件、Play Mode 验证步骤、是否修改场景/ProjectSettings，并 commit / push。
```

在以上问题修复并通过 Play Mode 验证前，不批准 MVP-02.1 通过，也不批准进入第三阶段系统。
