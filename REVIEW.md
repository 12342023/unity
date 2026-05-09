# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
ca20f62 feat: ruins rebuild-predicate data layer (CanRebuildFor / GetRebuildBuildingType)
```

结论：**MVP-03.5 代码审查通过**。

说明：`RuinComponent` 已具备最小可重建判定数据层，没有实现 UI、资源、占领进度或实际重建，符合任务范围。

## CODEX PROJECT REVIEW

Gate: **PASS**

### 已通过：[P2] RuinComponent 提供最小重建判定 API

File:

```text
kingbattle/Assets/Scripts/Buildings/RuinComponent.cs
```

Review:

新增方法：

```csharp
public bool CanRebuildFor(Faction faction)
{
    return !string.IsNullOrEmpty(sourcePlotId);
}

public BuildingType GetRebuildBuildingType()
{
    return sourceBuildingType;
}
```

这符合 MVP-03.5 的最小规则：

```diff
+ 有 sourcePlotId 才可重建
+ sourceBuildingType 决定可重建类型
+ originalFaction 只作为来源记录
- 不真正生成新建筑
- 不做 UI / 资源 / 占领进度
```

`CanRebuildFor(Faction faction)` 当前没有使用 `faction` 参数，这是刻意保留的未来扩展点；本轮不阻塞。

## 残留注意事项

### ProjectSettings 新增文件仍未处理

当前工作树仍有未跟踪文件：

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件由 Unity Editor 生成。由于项目规则要求不随意修改 `ProjectSettings`，继续保持未提交状态。

### 下一步建议先抽建筑创建边界

现在废墟已经知道“能不能重建 / 重建成什么”。真正重建前，需要先避免未来重建逻辑依赖 `GameEntry.CreateBuilding()`。下一步建议把建筑创建逻辑抽到 `Buildings/BuildingFactory`，但不改变玩法表现。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.5 代码审查通过。

进入 MVP-03.6：建筑创建逻辑边界整理。

目标：
- 不做实际重建。
- 不做 UI。
- 只把当前 GameEntry.CreateBuilding 的建筑创建逻辑下沉到 Buildings/ 下的小组件 / 小服务，为后续重建复用。

允许：
- 新增 Buildings/BuildingFactory.cs 或同等小类。
- 将创建建筑 GameObject、SpriteRenderer、HealthComponent、BuildingIdentity、BuildingDeathHandler、Collider、TowerAttack / BarracksSpawner 的装配逻辑移入 BuildingFactory。
- GameEntry 仍负责决定测试场景里创建哪些建筑、设置 rally / push target、装配 FactionDefeatHandler、保留 K / L 测试快捷键。
- 行为保持不变。
- 新增脚本必须提交 .meta。

必须保持：
- 当前建筑出现位置、颜色、大小、血量、Tower/Barracks 行为不变。
- 建筑死亡后仍生成废墟。
- 大本营被击败后，该阵营所有存活建筑变废墟，士兵立即死亡。
- K / L 测试快捷键仍可验证双方大本营清场。
- Console 无明显错误。

禁止：
- 不做重建按钮。
- 不真正生成“重建后的新建筑”流程。
- 不做占领进度。
- 不做资源、升级、连地、区域奖励、传送阵、AI 或 UI。
- 不修改 ProjectSettings。
- 不提交 kingbattle/ProjectSettings/SceneTemplateSettings.json。
- 不提交 Library、Logs、UserSettings。

完成后更新 WORKLOG.md，说明修改文件、Play Mode 验证步骤、是否修改场景/ProjectSettings，并 commit / push。
```
