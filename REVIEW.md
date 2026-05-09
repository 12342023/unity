# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
eb62289 feat: add K/L test shortcuts for faction defeat
```

结论：**测试快捷键代码审查通过，允许进入 MVP-03.3**。

说明：K / L 仅用于 Play Mode 快速验证双方大本营清场，改动范围小，没有改变正式战斗逻辑。当前下一步应整理 `GameEntry` 中过多的建筑死亡 / 废墟生成职责。

## CODEX PROJECT REVIEW

Gate: **PASS**

### 已通过：[P2] K / L 测试快捷键范围可接受

File:

```text
kingbattle/Assets/Scripts/GameEntry.cs
```

Review:

`GameEntry.Update()` 中新增：

```csharp
if (Input.GetKeyDown(KeyCode.K) && enemyBaseHealth != null && !enemyBaseHealth.IsDead)
    enemyBaseHealth.TakeDamage(enemyBaseHealth.CurrentHealth);

if (Input.GetKeyDown(KeyCode.L) && playerBaseHealth != null && !playerBaseHealth.IsDead)
    playerBaseHealth.TakeDamage(playerBaseHealth.CurrentHealth);
```

这会走正常 `TakeDamage -> OnDeath -> FactionDefeatHandler` 路径，适合验证 MVP-03.2。`IsDead` 守卫可以避免重复触发已死亡大本营。

### 残留：[P2] GameEntry 继续承载过多业务职责

File:

```text
kingbattle/Assets/Scripts/GameEntry.cs
```

Problem:

`GameEntry` 当前仍包含：

- 建筑创建和装配。
- 大本营清场处理器装配。
- K / L 测试快捷键。
- `SpawnRuin()` 具体实现。
- 建筑 `OnDeath` 到废墟生成的绑定逻辑。

这已经超过“薄启动脚本”的长期职责。短期可运行，但后续会让建筑死亡、废墟、清场、测试入口混在一起。

Fix:

发布下一条小任务：`MVP-03.3 建筑死亡 / 废墟职责边界整理`。

## 残留注意事项

### ProjectSettings 新增文件仍未处理

当前工作树仍有未跟踪文件：

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件由 Unity Editor 生成。由于项目规则要求不随意修改 `ProjectSettings`，继续保持未提交状态。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：K / L 测试快捷键通过。现在进入 MVP-03.3。

任务：建筑死亡 / 废墟职责边界整理。

目标：
- 不改变当前玩法表现。
- 将 SpawnRuin / 建筑死亡处理从 GameEntry 下沉到 Buildings/ 下的小组件或小服务。
- GameEntry 只负责装配测试场景。

允许：
- 新增 Buildings/BuildingDeathHandler.cs、Buildings/RuinSpawner.cs 或同等小组件。
- 让建筑自己的死亡逻辑负责生成废墟。
- FactionDefeatHandler 继续通过 HealthComponent.Kill() 触发建筑死亡逻辑。
- 小范围调整 GameEntry 装配代码。

必须保持：
- 建筑死亡后生成废墟。
- 士兵击败建筑后围绕废墟巡逻。
- 大本营被击败后，该阵营所有存活建筑变废墟，士兵立即死亡。
- K / L 测试快捷键仍可验证双方大本营清场。
- 单个建筑死亡只生成一个废墟。

禁止：
- 不做重建。
- 不做占领进度。
- 不做资源、升级、连地、区域奖励、传送阵、AI 或 UI。
- 不扩大 K / L 测试快捷键为正式功能。
- 不修改 ProjectSettings。
- 不提交 Unity 生成目录。
- 不提交 kingbattle/ProjectSettings/SceneTemplateSettings.json。

完成后更新 WORKLOG.md，说明修改文件、Play Mode 验证步骤、是否修改场景/ProjectSettings，并 commit / push。
```
