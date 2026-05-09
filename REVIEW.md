# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
162df1a fix: MoveTowards patrol circle until 0.1, Pause/Resume road path
```

结论：**MVP-02.1 核心行为通过，进入 MVP-02.1a 小修**。

说明：用户已在 Play Mode 中确认主要行为没问题，但发现两个可见体验问题：士兵集合/前进途中会变颜色；打败敌人大本营后返回时会短暂折返。进入第三阶段前，先让 Claude 做一个小修补丁。

## CODEX PROJECT REVIEW

Gate: **PASS CORE / FIX P2 POLISH**

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

## Play Mode 用户反馈

用户确认以下核心行为没有问题：

```text
这四个没问题。
```

剩余问题：

```text
兵在集合前进的时候中途会变颜色。
打败敌人大本营后回来的时候会折返一下。
```

### [P2] 士兵集合/前进途中会变颜色

File:

```text
kingbattle/Assets/Scripts/Combat/HealthComponent.cs:18
```

Problem:

`HealthComponent` 当前默认：

```csharp
private Color fullHealthColor = Color.white;
private Color lowHealthColor = Color.red;
```

受到伤害后会执行：

```csharp
targetRenderer.color = Color.Lerp(lowHealthColor, fullHealthColor, t);
```

这会把 Soldier 原本的蓝色阵营/单位颜色往白色、红色方向覆盖。用户在集合/前进时看到士兵中途变颜色，和这个逻辑高度吻合。

Fix:

请 Claude 做最小修复：

```diff
+ HealthComponent.Start() 中记录 targetRenderer.color 作为原始满血颜色
+ 受伤反馈从 originalColor lerp 到 lowHealthColor
+ 满血或未受伤时保持 Soldier / 阵营原色
+ 不要引入血条 UI 或复杂特效
```

### [P2] 打败敌人大本营后返回时会短暂折返

Files:

```text
kingbattle/Assets/Scripts/Combat/UnitCombat.cs
kingbattle/Assets/Scripts/Buildings/BarracksSpawner.cs
```

Problem:

用户观察到打败敌人大本营后，士兵回来的时候会折返一下。当前阶段还没有废墟/占领系统，因此击败目标后的合理行为应是稳定返回 rally/patrol，不应出现来回切换方向的短暂抖动。

Fix:

请 Claude 先复现并定位，不要直接做第三阶段废墟。最小修复方向：

```diff
+ 目标建筑死亡后，清理 chase / attack / push 的残留意图
+ 确保单位只选择一个返回意图：继续回 rally/patrol
+ 不要在同一段返回中又短暂朝已死亡目标或旧 push 方向移动
+ 不实现废墟、占领、重建或资源
```

## 残留注意事项

### ProjectSettings 新增文件仍未处理

当前工作树仍有未跟踪文件：

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件由 Unity Editor 生成。由于项目规则要求不随意修改 `ProjectSettings`，继续保持未提交状态。后续需要单独确认是否提交或忽略。

## 下一步

让 Claude 做 MVP-02.1a 小修，不要继续开发新功能。

给 Claude 的任务：

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

用户 Play Mode 反馈：核心四项没问题，但有两个体验问题：
1. 兵在集合前进的时候中途会变颜色。
2. 打败敌人大本营后回来的时候会折返一下。

请只做 MVP-02.1a 小修：
- HealthComponent 保留原始 SpriteRenderer.color 作为满血颜色，受伤时从原色向 lowHealthColor 过渡，不要把 Soldier 原色覆盖成白色。
- 复现并修复打败敌人大本营后返回时短暂折返的问题。目标死亡后应稳定返回 rally/patrol，不要短暂朝已死亡目标或旧 push 方向移动。

不要实现建筑废墟、重建、资源、升级、连地、区域奖励、传送阵、AI 或 UI。

完成后更新 WORKLOG.md，说明修改文件、Play Mode 验证步骤、是否修改场景/ProjectSettings，并 commit / push。
```

两个问题修复后，再允许进入第三阶段任务拆分。
