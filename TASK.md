# TASK.md

## 当前任务

MVP-03.8 小修：补齐 `BuildingRebuildService` 的 `mapData` 空值防御。

Codex 当前仍作为 Tech Lead / Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 项目定位

这是一个 Unity 小游戏项目，当前主工程位于：

```text
kingbattle/
```

项目定位：

```text
低操作、高战略、自动战争 RTS
```

后续规划包括：

- 微信小程序移植
- macOS 移植
- Android 移植

当前阶段优先保持 Unity 工程结构清晰，不把未来平台差异散落在业务逻辑中。

## 最新 Review 结论

Claude 最新提交：

```text
6d517e6 feat: BuildingRebuildService - rebuild ruins via BuildingFactory
```

Codex Review 结论：

```text
MVP-03.8 暂未通过，需要补一个 mapData 空值保护
```

## 当前工作区注意事项

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件仍是未跟踪 Unity Editor 生成文件。除非用户明确批准，否则不要提交。

## 本次小修目标

`BuildingRebuildService.Rebuild(...)` 是未来重建系统的服务边界，错误入参必须安全返回。

当前问题：

```text
mapData == null 时，Rebuild 会继续调用 BuildingFactory.CreateBuilding(...)，
最终在 mapData.GetPlot(...) 处触发 NullReferenceException。
```

## 允许范围

- 修改 `kingbattle/Assets/Scripts/Buildings/BuildingRebuildService.cs`。
- 在调用 `BuildingFactory.CreateBuilding(...)` 前增加：

```csharp
if (mapData == null)
{
    Debug.LogWarning("[BuildingRebuildService] MapData is null.");
    return null;
}
```

- 可以小幅调整日志文字，但不要扩大行为。
- 更新 `WORKLOG.md`。

## 禁止范围

- 不改正式 UI。
- 不做资源、占领、升级、连地、区域奖励、传送阵或 AI。
- 不改 `GameEntry` 的 R 测试入口，除非只是同步日志说明。
- 不修改 `ProjectSettings`。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。

## 验收标准

- `BuildingRebuildService.Rebuild(null, mapData, faction)` 安全返回 `null`。
- `BuildingRebuildService.Rebuild(ruin, null, faction)` 安全返回 `null`。
- 正常 R 测试重建行为不变。
- Console 无明显错误。
- 仍未修改或提交 `ProjectSettings`。
