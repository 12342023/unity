# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
88455d4 fix: preserve unit original color, prevent one-frame chase to dead target
```

结论：**MVP-02.1a 暂不通过**。

说明：颜色问题的代码方向正确；但用户确认“打败敌人大本营后返回时短暂折返”仍然存在。Claude 本次修复只处理了击杀后一帧可能追逐 dead target 的情况，没有处理 `pushPath` / push order 在目标建筑死亡后的残留。

## CODEX PROJECT REVIEW

Gate: **FAIL**

### 已修复：[P2] 士兵颜色被白色覆盖

File:

```text
kingbattle/Assets/Scripts/Combat/HealthComponent.cs:31
```

Review:

Claude 已在 `Start()` 中捕获 `SpriteRenderer.color`：

```csharp
if (targetRenderer != null)
    originalColor = targetRenderer.color;
```

受伤时改为：

```csharp
targetRenderer.color = Color.Lerp(lowHealthColor, originalColor, t);
```

这符合要求：满血/未受伤时保留单位原色，受伤时从原色向低血量色过渡，不再把 Soldier 原色覆盖成白色。

### [P1] 击败敌人大本营后仍短暂折返

Files:

```text
kingbattle/Assets/Scripts/Combat/UnitCombat.cs:122
kingbattle/Assets/Scripts/Combat/UnitCombat.cs:223
kingbattle/Assets/Scripts/Combat/UnitCombat.cs:242
```

Problem:

Claude 本次只在 `TakeDamage()` 后增加了：

```csharp
if (target.IsDead)
{
    Deaggro();
    return;
}
```

这可以避免“击杀后一帧继续追 dead target”，但没有清理旧的 `pushPath`。当前 `UpdateIdle()` 的执行顺序是：

```text
1. 扫描敌人
2. 如果 pushPath != null，则继续沿 pushPath 移动
3. pushPath == null 时才返回巡逻圆
```

所以单位在打死敌人大本营后，如果 `pushPath` 仍然指向敌方大本营/旧推进终点，`Deaggro()` 回到 Idle 后仍会先执行旧 pushPath，表现就是用户看到的“回来时折返一下”。

Impact:

用户已经在 Play Mode 中确认折返仍存在。这个问题会让波次推进收尾显得不稳定，也会影响第三阶段“建筑废墟 / 围绕废墟巡逻”的基础。

Fix:

请 Claude 继续小修，不要进入第三阶段。重点不是 dead target chase，而是 push order 残留：

```diff
+ 当攻击目标建筑死亡，并且该目标属于当前 push 目标 / 敌方大本营时，清理 pushPath
+ 清理 chaseTarget / target 后，单位应直接进入返回 rally/patrol 的单一路径
+ Deaggro 后不要再执行指向已摧毁建筑的旧 pushPath
+ 可以在 UnitCombat 中增加明确方法，例如 ClearPushPathOnTargetDeath
+ 或由 BarracksSpawner 在 enemyTarget 死亡时通知已派出的单位 ClearPushPath
- 不要实现废墟、占领、重建、资源或 AI
```

验收标准：

- 打败敌人大本营后，单位不再朝已摧毁目标点短暂折返。
- 单位稳定返回 rally/patrol。
- 颜色保持本次修复效果。
- Console 无明显错误。

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

用户再次 Play Mode 验证：颜色问题看起来已经按方向修了，但“打败敌人大本营后回来时短暂折返”仍然存在。

Codex Review 判断：你上次只修了 dead target chase 的一帧问题，但没有处理 pushPath / push order 残留。单位打死敌方大本营后，Deaggro 回到 Idle，仍可能继续执行旧 pushPath，先朝已摧毁的大本营/旧推进终点走一下，再返回 rally/patrol。

请只修这个问题：
- 当攻击目标建筑死亡，并且它是当前 push 目标 / 敌方大本营时，清理 pushPath。
- 清理 target / chaseTarget / pushPath 后，单位应直接稳定返回 rally/patrol。
- 不要让单位在目标死亡后继续执行指向已摧毁建筑的旧 pushPath。

不要实现建筑废墟、重建、资源、升级、连地、区域奖励、传送阵、AI 或 UI。

完成后更新 WORKLOG.md，说明修改文件、Play Mode 验证步骤、是否修改场景/ProjectSettings，并 commit / push。
```
