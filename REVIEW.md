# REVIEW.md

## Review 状态

Codex 已审查 Claude 当前未提交输出：

```text
MVP-03.3 建筑死亡 / 废墟职责边界整理
```

结论：**MVP-03.3 暂不通过**。

说明：实现方向正确，`SpawnRuin` 已从 `GameEntry` 移到 `Buildings/BuildingDeathHandler.cs`，`GameEntry` 不再包含具体废墟生成方法。但当前交付不完整：新增脚本缺少 `.meta` 文件，且代码还没有 commit / push。Unity 项目中新增脚本必须提交对应 `.meta`，否则后续 GUID 会漂移。

## CODEX PROJECT REVIEW

Gate: **FAIL**

### 已通过：[P2] 废墟生成职责已下沉到 Buildings

Files:

```text
kingbattle/Assets/Scripts/Buildings/BuildingDeathHandler.cs
kingbattle/Assets/Scripts/GameEntry.cs
```

Review:

`GameEntry.CreateBuilding()` 已删除旧的 `health.OnDeath += SpawnRuin(...)` 逻辑，改为：

```csharp
go.AddComponent<BuildingDeathHandler>();
```

`BuildingDeathHandler` 自己订阅 `HealthComponent.OnDeath` 并生成废墟。这个方向符合 MVP-03.3。

### [P1] 新脚本缺少 BuildingDeathHandler.cs.meta

File:

```text
kingbattle/Assets/Scripts/Buildings/BuildingDeathHandler.cs
```

Problem:

当前工作区有新增脚本：

```text
kingbattle/Assets/Scripts/Buildings/BuildingDeathHandler.cs
```

但没有对应：

```text
kingbattle/Assets/Scripts/Buildings/BuildingDeathHandler.cs.meta
```

Impact:

Unity 会为脚本生成 `.meta` 并分配 GUID。如果不提交 `.meta`，不同机器或后续重新导入时 GUID 可能变化，场景、Prefab、脚本引用会有长期风险。项目此前已要求新增 Unity 脚本时提交对应 `.meta`。

Fix:

请 Claude 在 Unity Editor 中让 Unity 生成该 `.meta`，或确认文件已生成后提交：

```diff
+ kingbattle/Assets/Scripts/Buildings/BuildingDeathHandler.cs
+ kingbattle/Assets/Scripts/Buildings/BuildingDeathHandler.cs.meta
```

### [P1] 代码尚未 commit / push

Files:

```text
kingbattle/Assets/Scripts/Buildings/BuildingDeathHandler.cs
kingbattle/Assets/Scripts/GameEntry.cs
WORKLOG.md
```

Problem:

当前 `git status` 仍显示 MVP-03.3 代码是本地未提交改动。`WORKLOG.md` 写了手动提交命令，但仓库最新提交仍是：

```text
d445c2b docs: record mvp-03.3 task push
```

Fix:

Claude 需要补齐 `.meta` 后再 commit / push。

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

Codex Review：MVP-03.3 暂不通过，但实现方向正确。

你已经把 SpawnRuin 从 GameEntry 移到 Buildings/BuildingDeathHandler.cs，这个方向可以。

现在请只补齐交付问题：

1. 生成并提交脚本 meta 文件
- 当前新增了 kingbattle/Assets/Scripts/Buildings/BuildingDeathHandler.cs
- 但缺少 kingbattle/Assets/Scripts/Buildings/BuildingDeathHandler.cs.meta
- 请打开 Unity 或让 Unity 导入脚本后生成该 .meta
- commit 时必须包含 .cs 和 .cs.meta

2. 保持当前玩法不变
- 建筑死亡后仍生成废墟。
- 士兵击败建筑后仍围绕废墟巡逻。
- 大本营被击败后，该阵营所有存活建筑变废墟，士兵立即死亡。
- K / L 测试快捷键仍可验证双方大本营清场。
- 单个建筑死亡只生成一个废墟。

3. 不要扩大范围
- 不做重建。
- 不做占领进度。
- 不做资源、升级、连地、区域奖励、传送阵、AI 或 UI。
- 不修改 ProjectSettings。
- 不提交 kingbattle/ProjectSettings/SceneTemplateSettings.json。
- 不提交 Library、Logs、UserSettings。

4. 完成后更新 WORKLOG.md 并 commit / push
建议提交：
git add kingbattle/Assets/Scripts/Buildings/BuildingDeathHandler.cs \
        kingbattle/Assets/Scripts/Buildings/BuildingDeathHandler.cs.meta \
        kingbattle/Assets/Scripts/GameEntry.cs \
        WORKLOG.md TASK.md
git commit -m "refactor: move ruin spawning from GameEntry to BuildingDeathHandler"
git push origin main
```
