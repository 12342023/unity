# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
04adcc7 refactor: BuildingRegistry runtime registry, removes manual building lists
```

结论：**MVP-03.7 代码审查通过**。

说明：本轮新增 `BuildingRegistry`，并让 `FactionDefeatHandler` 从注册表查询阵营建筑，解决了后续“重建出的建筑也必须被阵营清场逻辑管理”的边界问题。没有实现 UI、资源、占领、升级、连地、AI 或真正重建。

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞问题。
```

### 已确认

- `kingbattle/Assets/Scripts/Buildings/BuildingRegistry.cs` 已新增。
- `kingbattle/Assets/Scripts/Buildings/BuildingRegistry.cs.meta` 已提交。
- `BuildingFactory.CreateBuilding(...)` 会在建筑装配完成后调用 `BuildingRegistry.Register(go, faction)`。
- `FactionDefeatHandler` 不再依赖 `GameEntry` 传入的一次性建筑列表。
- `BuildingRegistry.GetBuildings(faction)` 会过滤 `null` 和已死亡建筑，降低重复清场 / 重复废墟风险。
- `GameEntry` 删除了手动维护 `playerBuildings` / `enemyBuildings` 的逻辑，继续只负责场景装配、rally / push target、K / L 快捷键。
- 未提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。

## 残留注意事项

### Play Mode 未由 Codex 本地亲跑

Claude 在 `WORKLOG.md` 记录了 K / L Play Mode 验证通过。Codex 本轮做的是代码审查与文档审查，没有亲自打开 Unity 运行 Play Mode。

### 轻微文档措辞

`BuildingRegistry.cs` 注释中写了 `BuildingFactory.Register()`，实际调用是 `BuildingRegistry.Register(...)`。这是注释措辞问题，不影响编译和玩法，不阻塞本轮。

### ProjectSettings 新增文件仍未处理

当前工作树仍有未跟踪文件：

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件由 Unity Editor 生成。由于项目规则要求不随意修改 `ProjectSettings`，继续保持未提交状态。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.7 代码审查通过。

进入 MVP-03.8：废墟重建最小服务。

目标：
- 不做正式 UI。
- 不做资源、占领、升级、连地、区域奖励、传送阵或 AI。
- 新增一个很小的 BuildingRebuildService 或同等小类，提供“从 RuinComponent 重建建筑”的服务方法。
- 该服务应复用 RuinComponent 的来源数据、BuildingFactory、BuildingRegistry。

允许：
- 新增 Buildings/BuildingRebuildService.cs 或同等小类。
- 提供类似 TryRebuild(RuinComponent ruin, MapData mapData, Faction faction, out GameObject building) 的方法。
- 检查 ruin 不为空，CanRebuildFor(faction) 为 true，sourcePlotId 不为空。
- 使用 ruin.GetRebuildBuildingType() 决定建筑类型。
- 调用 BuildingFactory.CreateBuilding(sourcePlotId, mapData, faction, type) 创建建筑。
- 创建成功后销毁废墟 GameObject。
- 因为 BuildingFactory 已自动注册，重建出的建筑也应自动进入 BuildingRegistry。
- 可以保留服务暂时未接入正式输入；本轮重点是服务边界。
- 新增脚本必须提交 .meta。

必须保持：
- 当前开局建筑、出兵、Tower 攻击、建筑死亡生成废墟、大本营清场、K / L 快捷键行为不变。
- 单个建筑死亡仍只生成一个废墟。
- Console 无明显错误。

禁止：
- 不做正式重建按钮。
- 不做选择废墟 UI。
- 不做资源消耗、进度条、占领、升级、连地、区域奖励、传送阵、AI。
- 不修改 ProjectSettings。
- 不提交 kingbattle/ProjectSettings/SceneTemplateSettings.json。
- 不提交 Library、Logs、UserSettings。

完成后更新 WORKLOG.md，说明：
- 修改文件
- BuildingRebuildService 的职责边界
- 是否接入任何临时入口
- Play Mode 验证步骤
- 是否修改场景 / ProjectSettings
- commit / push 结果
```
