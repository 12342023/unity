# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
9bc149b feat: building identity data and ruin metadata for future reconstruction
```

结论：**MVP-03.4 代码审查通过**。

说明：建筑身份数据和废墟来源元数据已补齐，`BuildingIdentity.cs.meta` 已提交。当前实现没有引入重建、资源、UI 或占领进度，符合 MVP-03.4 的范围。

## CODEX PROJECT REVIEW

Gate: **PASS**

### 已通过：[P1] 建筑身份数据已装配

Files:

```text
kingbattle/Assets/Scripts/Buildings/BuildingIdentity.cs
kingbattle/Assets/Scripts/GameEntry.cs
```

Review:

`GameEntry.CreateBuilding()` 已为每个建筑写入：

```csharp
identity.plotId = plotId;
identity.buildingType = type;
identity.faction = faction;
```

`BuildingIdentity.cs.meta` 已提交，Unity GUID 稳定。

### 已通过：[P1] 废墟保存来源元数据

Files:

```text
kingbattle/Assets/Scripts/Buildings/RuinComponent.cs
kingbattle/Assets/Scripts/Buildings/BuildingDeathHandler.cs
```

Review:

`RuinComponent` 现在保存：

```csharp
public string sourcePlotId;
public BuildingType sourceBuildingType;
public Faction originalFaction;
```

`BuildingDeathHandler` 在生成 Ruin 时从 `BuildingIdentity` 传入这些字段。这样后续重建系统可以知道废墟来自哪个 plot、原建筑类型和原阵营。

## 残留注意事项

### ProjectSettings 新增文件仍未处理

当前工作树仍有未跟踪文件：

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件由 Unity Editor 生成。由于项目规则要求不随意修改 `ProjectSettings`，继续保持未提交状态。

### 下一步仍不做完整 UI / 资源

下一步可以进入“废墟可重建判定”前置层，但仍不要做完整重建 UI、资源消耗、升级、连地或 AI。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.4 代码审查通过。

进入 MVP-03.5：废墟可重建判定数据层。

目标：
- 不做完整重建系统。
- 不做 UI。
- 只给 Ruin 增加“是否可重建 / 可重建成什么”的最小判定数据和方法。

允许：
- 扩展 RuinComponent，新增只读或简单方法，例如 CanRebuildFor(Faction faction)、GetRebuildBuildingType()。
- 规则先保持最小：
  - 有 sourcePlotId 才可重建。
  - sourceBuildingType 决定可重建类型。
  - originalFaction 只作为来源记录，不在本任务里做复杂归属规则。
- 可以新增一个小的数据结构 / enum，但优先保持简单。
- 更新 WORKLOG.md。

必须保持：
- 当前玩法表现不变。
- 建筑死亡后仍生成废墟。
- 士兵击败建筑后仍围绕废墟巡逻。
- 大本营被击败后，该阵营所有存活建筑变废墟，士兵立即死亡。
- K / L 测试快捷键仍可验证双方大本营清场。
- 单个建筑死亡只生成一个废墟。

禁止：
- 不做重建按钮。
- 不真正生成新建筑。
- 不做占领进度。
- 不做资源、升级、连地、区域奖励、传送阵、AI 或 UI。
- 不修改 ProjectSettings。
- 不提交 kingbattle/ProjectSettings/SceneTemplateSettings.json。
- 不提交 Library、Logs、UserSettings。

完成后更新 WORKLOG.md，说明修改文件、Play Mode 验证步骤、是否修改场景/ProjectSettings，并 commit / push。
```
