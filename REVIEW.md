# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
bb7ea8b refactor: move ruin spawning from GameEntry to BuildingDeathHandler
```

结论：**MVP-03.3 代码审查通过**。

说明：Claude 已补齐 `BuildingDeathHandler.cs.meta`，并将废墟生成从 `GameEntry` 下沉到 `Buildings/BuildingDeathHandler.cs`。`GameEntry` 不再包含 `SpawnRuin()` 具体实现，符合 MVP-03.3 的职责边界要求。

## CODEX PROJECT REVIEW

Gate: **PASS**

### 已通过：[P1] BuildingDeathHandler.cs.meta 已提交

Files:

```text
kingbattle/Assets/Scripts/Buildings/BuildingDeathHandler.cs
kingbattle/Assets/Scripts/Buildings/BuildingDeathHandler.cs.meta
```

Review:

新增 Unity 脚本和对应 `.meta` 已在提交中出现：

```text
guid: 84ea71435da7147b19c0145efee15c8c
```

上一轮阻塞项已解除。

### 已通过：[P1] SpawnRuin 已离开 GameEntry

Files:

```text
kingbattle/Assets/Scripts/Buildings/BuildingDeathHandler.cs
kingbattle/Assets/Scripts/GameEntry.cs
```

Review:

`GameEntry.CreateBuilding()` 现在只装配：

```csharp
go.AddComponent<BuildingDeathHandler>();
```

`BuildingDeathHandler` 自己订阅 `HealthComponent.OnDeath` 并生成 Ruin。`GameEntry` 不再保留 `SpawnRuin()` 方法，职责边界比上一版清楚。

## 残留注意事项

### ProjectSettings 新增文件仍未处理

当前工作树仍有未跟踪文件：

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件由 Unity Editor 生成。由于项目规则要求不随意修改 `ProjectSettings`，继续保持未提交状态。

### 下一步不直接做完整重建

当前第三阶段已经有“建筑死亡 -> 废墟 -> 阵营清场”的基础闭环。下一步建议先给建筑和废墟补身份数据，为未来重建打基础，但不要直接做重建 UI / 资源 / 占领进度。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.3 代码审查通过。

进入 MVP-03.4：建筑身份数据与废墟元数据。

目标：
- 不做完整重建系统。
- 只为未来重建准备最小必要数据。
- 建筑死亡后生成的 Ruin 应知道自己来自哪个 plot、哪个建筑类型、哪个阵营。

允许：
- 新增 Buildings/BuildingIdentity.cs 或同等小组件。
- 在 GameEntry.CreateBuilding() 装配建筑时写入 plotId、BuildingType、Faction。
- 扩展 RuinComponent，让废墟记录 sourcePlotId、sourceBuildingType、originalFaction。
- BuildingDeathHandler 生成 Ruin 时，把建筑身份数据传给 RuinComponent。
- 小范围调整 GameEntry 的装配代码。

必须保持：
- 当前玩法表现不变。
- 建筑死亡后仍生成废墟。
- 士兵击败建筑后仍围绕废墟巡逻。
- 大本营被击败后，该阵营所有存活建筑变废墟，士兵立即死亡。
- K / L 测试快捷键仍可验证双方大本营清场。
- 单个建筑死亡只生成一个废墟。

禁止：
- 不做重建按钮。
- 不做占领进度。
- 不做资源、升级、连地、区域奖励、传送阵、AI 或 UI。
- 不修改 ProjectSettings。
- 不提交 kingbattle/ProjectSettings/SceneTemplateSettings.json。
- 不提交 Library、Logs、UserSettings。

完成后更新 WORKLOG.md，说明修改文件、Play Mode 验证步骤、是否修改场景/ProjectSettings，并 commit / push。
```
