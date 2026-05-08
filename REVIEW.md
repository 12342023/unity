# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
162df1a fix: MoveTowards patrol circle until 0.1, Pause/Resume road path
```

结论：**MVP-02.1 代码审查通过，等待 Play Mode 最终确认**。

说明：本次提交已针对上一轮两个阻塞点做了最小修复。Codex 通过代码审查确认方向正确，但没有在 Unity Editor 中亲自运行 Play Mode，因此最终进入第三阶段前仍需要用户手动确认运行表现。

## CODEX PROJECT REVIEW

Gate: **PASS WITH MANUAL QA**

### 已修复：[P1] 脱战返回巡逻圆仍可能 snap

Files:

```text
kingbattle/Assets/Scripts/Combat/UnitCombat.cs:145
kingbattle/Assets/Scripts/Combat/UnitCombat.cs:160
```

Review:

Claude 已把脱战后的回程目标从 `patrol.Center` 改为 `patrol.CurrentPatrolPosition`：

```csharp
float distToCircle = Vector3.Distance(transform.position, patrol.CurrentPatrolPosition);
if (distToCircle > 0.1f)
{
    var step = movement.speed * Time.deltaTime;
    transform.position = Vector3.MoveTowards(
        transform.position, patrol.CurrentPatrolPosition, step);
    return;
}
```

只有当单位距离圆周切入点小于等于 `0.1` 时，才允许执行：

```csharp
patrol.Tick(Time.deltaTime);
```

这满足上一轮要求：

```diff
+ 返回期间不 Tick
+ 走到圆周切入点附近后再恢复转圈
+ 不再从 center 阈值边缘跳到圆周点
```

### 已修复：[P2] 前往 rallyPoint 途中接敌后路线被 Stop 清掉

Files:

```text
kingbattle/Assets/Scripts/Combat/UnitCombat.cs:112
kingbattle/Assets/Scripts/Combat/UnitCombat.cs:233
```

Review:

Claude 已将接敌时的：

```csharp
movement?.Stop();
```

改为：

```csharp
movement?.Pause();
```

脱战时调用：

```csharp
movement?.Resume();
```

并且返回巡逻圆逻辑增加了 `!movement.HasRemainingPath` 守卫：

```csharp
if (patrol != null && pushPath == null && (movement == null || !movement.HasRemainingPath))
```

因此单位如果在去 rallyPoint 的道路上接敌，脱战后会优先恢复道路路线，而不是丢失路径后直接进入巡逻。

### 通过项

- 没有实现建筑废墟、重建、资源、升级、连地、区域奖励、传送阵、AI 或 UI。
- `GameEntry` 未继续膨胀。
- `ProjectSettings` 未纳入提交。
- 修改范围集中在 `UnitCombat.cs`，符合最小修复要求。
- `WORKLOG.md` 已记录 Claude 的修改说明与验证步骤。

## Play Mode 最终确认

请用户在 Unity 中手动确认：

1. 巡逻单位接敌后追出建筑/集结点，目标死亡或超出追击范围后，单位可见地走回巡逻圆切入点。
2. 单位接近圆周点后恢复转圈，没有明显瞬移。
3. 单位在前往 rallyPoint 途中接敌，脱战后继续沿原路线前往 rallyPoint。
4. 到达 rallyPoint 后围绕建筑/集结点转圈。
5. Console 无明显错误。

如果以上 5 项都通过，Codex 批准 MVP-02.1 收尾，并允许开始拆分第三阶段任务。

## 残留注意事项

### ProjectSettings 新增文件仍未处理

当前工作树仍有未跟踪文件：

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件由 Unity Editor 生成。由于项目规则要求不随意修改 `ProjectSettings`，继续保持未提交状态。后续需要单独确认是否提交或忽略。

### Health 颜色反馈会覆盖阵营颜色

`HealthComponent` 默认用白色作为满血颜色，受到伤害后会把建筑 / 单位颜色往白色和红色之间插值。功能上不阻塞 MVP-02.1，但后续如果颜色用于阵营识别，应让 `HealthComponent` 在 `Start()` 记录原始 `SpriteRenderer.color` 作为满血颜色。

## 下一步

不要让 Claude 继续开发新功能，先让用户完成 Play Mode 验证。

验证通过后，再发布第三阶段任务拆分：

```text
建筑被击败后进入废墟状态；士兵可以围绕废墟转圈巡逻。
```

第三阶段仍需拆小，不要一次性混入资源、升级、连地、区域奖励、传送阵或 AI。
