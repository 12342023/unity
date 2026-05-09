# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
6d517e6 feat: BuildingRebuildService - rebuild ruins via BuildingFactory
```

结论：**MVP-03.8 代码审查未通过，需要小修**。

说明：`BuildingRebuildService` 的方向正确，已经复用 `RuinComponent`、`BuildingFactory`、`BuildingRegistry`，也提交了 `.meta`。但当前服务方法没有防御 `mapData == null`，不满足本轮“服务失败时应安全返回，不抛出明显空引用错误”的验收标准。

## CODEX PROJECT REVIEW

Gate: **FAIL**

Findings:

### [P1] `BuildingRebuildService.Rebuild` 缺少 `mapData` 空值保护

File:

```text
kingbattle/Assets/Scripts/Buildings/BuildingRebuildService.cs:27
```

Problem:

当前只检查了 `ruin == null` 和 `ruin.CanRebuildFor(faction)`。如果调用方传入 `mapData == null`，代码会继续执行到：

```text
BuildingFactory.CreateBuilding(plotId, mapData, faction, buildingType)
```

而 `BuildingFactory.CreateBuilding(...)` 里面会直接调用 `mapData.GetPlot(plotId)`，导致空引用异常。

本轮 `TASK.md` 的验收标准明确要求：

```text
服务方法失败时应安全返回，不抛出明显空引用错误。
```

Fix:

在读取 plot / 创建建筑前增加空值保护：

```diff
+ if (mapData == null)
+ {
+     Debug.LogWarning("[BuildingRebuildService] MapData is null.");
+     return null;
+ }
```

建议顺手确认 `sourcePlotId` 是否为空或无效时仍安全返回。当前 `CanRebuildFor` 已覆盖空 `sourcePlotId`，`BuildingFactory.CreateBuilding` 对不存在 plot 会返回 `null`，这部分可以接受。

## 已通过部分

- `BuildingRebuildService.cs` 已新增。
- `BuildingRebuildService.cs.meta` 已提交。
- 重建服务读取 `RuinComponent.sourcePlotId` 和 `GetRebuildBuildingType()`。
- 创建建筑通过 `BuildingFactory.CreateBuilding(...)`，因此新建筑会自动注册进 `BuildingRegistry`。
- 创建成功后销毁旧废墟。
- 本轮没有做正式 UI、资源、占领、升级、连地、区域奖励、传送阵或 AI。
- `kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍未提交。

## 需要 Claude 修复

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.8 暂未通过，只需要小修。

阻塞问题：
- BuildingRebuildService.Rebuild(...) 缺少 mapData == null 防御。
- 这会让服务方法在错误入参下抛 NullReferenceException，不满足“失败时安全返回”的验收标准。

请修改：
- 在 BuildingRebuildService.Rebuild(...) 中调用 BuildingFactory.CreateBuilding(...) 前增加 mapData 空值检查。
- mapData 为空时 Debug.LogWarning 并 return null。
- 保持现有 R 测试快捷键不变。
- 不新增 UI、资源、占领、升级、连地、区域奖励、传送阵、AI。
- 不修改 ProjectSettings。
- 不提交 kingbattle/ProjectSettings/SceneTemplateSettings.json。

修复后请更新 WORKLOG.md，说明小修内容、Play Mode / Console 验证，并 commit / push。
```
