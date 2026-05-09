# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
8dbbbd7 fix: clear stale pushPath on deaggro to prevent one-frame折返 toward dead building
```

结论：**MVP-02.1a 通过，允许进入第三阶段第一个小任务**。

说明：Claude 已在 `Deaggro()` 中清理残留 `pushPath`，解决击败敌方大本营后仍沿旧 push 路线短暂折返的问题。用户已表示“好了进行下一步”，因此 Codex 允许发布第三阶段的第一个小范围任务：建筑变废墟 + 围绕废墟巡逻。

## CODEX PROJECT REVIEW

Gate: **PASS**

### 已修复：[P1] 目标死亡后旧 pushPath 残留导致折返

File:

```text
kingbattle/Assets/Scripts/Combat/UnitCombat.cs:242
```

Review:

`Deaggro()` 已增加：

```csharp
ClearPushPath();
```

修复效果：

```diff
+ 清理 target / chaseTarget 后同步清理旧 pushPath
+ 单位不再继续执行指向已摧毁建筑的旧推进路径
+ 击败敌方大本营后可稳定返回 rally/patrol
```

### 已修复：[P2] 士兵颜色被白色覆盖

File:

```text
kingbattle/Assets/Scripts/Combat/HealthComponent.cs:31
```

Review:

`HealthComponent` 已记录 `SpriteRenderer.color` 作为满血颜色，受伤时从原始颜色过渡到低血量色，不再把 Soldier 原色覆盖成白色。

## 残留注意事项

### pushPath 清理策略后续需观察

当前 `Deaggro()` 会在任何脱战场景下清理 `pushPath`。这解决了大本营死亡后的折返问题，但后续如果希望“波次部队打完路上小兵后继续推进”，可能需要更细的 push order 恢复策略。当前不阻塞 MVP-03.1。

### ProjectSettings 新增文件仍未处理

当前工作树仍有未跟踪文件：

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件由 Unity Editor 生成。由于项目规则要求不随意修改 `ProjectSettings`，继续保持未提交状态。后续需要单独确认是否提交或忽略。

## 下一条任务：MVP-03.1 建筑废墟与废墟巡逻

请 Claude 先阅读：

```text
AGENTS.md
TASK.md
REVIEW.md
NEXT_STEPS.md
WORKLOG.md
```

然后只做以下任务：

```text
建筑被击败后进入废墟状态；士兵可以围绕废墟转圈巡逻。
```

### 允许范围

- 建筑死亡后生成或转换为 Ruin / 废墟状态。
- 废墟应保留原建筑位置，作为新的巡逻中心。
- 击败建筑的士兵，或参与攻击该建筑的士兵，在目标死亡后可以围绕废墟转圈巡逻。
- 废墟使用简单视觉即可，例如灰色/暗色方块或破损样式。
- Tower / Barracks / Granary 进入废墟后应停止原功能：Tower 不再攻击，Barracks 不再出兵。
- 建筑废墟逻辑应放在 `Buildings/` 或清晰的组件中，不要塞进大型 `GameManager`。
- 可使用 `HealthComponent.OnDeath` 或新增小组件处理建筑死亡，但不要破坏单位死亡逻辑。

### 禁止范围

- 不做重建。
- 不做占领进度。
- 不做资源产出。
- 不做建筑升级。
- 不做连地系统。
- 不做区域奖励。
- 不做传送阵。
- 不做 AI 决策。
- 不做 UI / 美术大改 / 音效。

### 验收标准

- EnemyBase / 敌方建筑血量归零后，不是简单消失，而是留下可见废墟。
- 废墟不再执行原建筑功能。
- 士兵打败建筑后不会折返旧目标点，而是围绕废墟转圈巡逻。
- 普通单位死亡仍按原逻辑销毁，不变成废墟。
- Console 无明显错误。
- 不修改 `ProjectSettings`，除非先说明理由并等待确认。
- 不提交 Unity 生成目录。
- 更新 `WORKLOG.md`，并 commit / push。
