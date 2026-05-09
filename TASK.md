# TASK.md

## 当前任务

发布 MVP-03.8：废墟重建最小服务。

Codex 当前仍作为 Tech Lead / Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 项目定位

这是一个 Unity 小游戏项目，当前主工程位于：

```text
kingbattle/
```

最新 `goal.md` 将游戏定位为：

```text
低操作、高战略、自动战争 RTS
```

后续规划包括：

- 微信小程序移植
- macOS 移植
- Android 移植

当前阶段优先保持 Unity 工程结构清晰，不把未来平台差异散落在业务逻辑中。

## 当前实现状态

已完成 / 已存在：

- 固定地图和道路
- 单位沿道路移动
- Barracks 自动出兵
- Tower 自动攻击单位
- 单位基础战斗、仇恨、脱战、巡逻
- 建筑死亡后生成废墟
- 士兵可围绕废墟巡逻
- 一方大本营被击败后，该方建筑清场为废墟，士兵立即死亡
- K / L Play Mode 测试快捷键
- `BuildingDeathHandler` 负责生成废墟
- `BuildingIdentity` 记录建筑来源数据
- `RuinComponent` 记录 `sourcePlotId / sourceBuildingType / originalFaction`
- `RuinComponent.CanRebuildFor(...)` / `GetRebuildBuildingType()`
- `BuildingFactory` 负责创建并装配建筑
- `BuildingRegistry` 负责运行时登记和查询阵营建筑
- `FactionDefeatHandler` 通过 `BuildingRegistry` 查询当前阵营建筑

## 最新 Review 结论

Claude 最新提交：

```text
04adcc7 refactor: BuildingRegistry runtime registry, removes manual building lists
```

Codex Review 结论：

```text
MVP-03.7 代码审查通过；允许进入 MVP-03.8
```

## 当前工作区注意事项

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件仍是未跟踪 Unity Editor 生成文件。除非用户明确批准，否则不要提交。

## MVP-03.8 目标

新增废墟重建的最小服务边界，但不做正式 UI、不做资源、不做占领。

当前已经具备：

- 废墟知道来源 plot 和建筑类型。
- `BuildingFactory` 能创建建筑。
- `BuildingRegistry` 能记录运行时建筑。

本轮目标是把这些能力串成一个可复用服务：

```text
RuinComponent -> BuildingRebuildService -> BuildingFactory -> BuildingRegistry
```

## MVP-03.8 允许范围

- 新增 `Buildings/BuildingRebuildService.cs` 或同等小类。
- 提供一个清晰的服务方法，例如：

```csharp
public static bool TryRebuild(
    RuinComponent ruin,
    MapData mapData,
    Faction faction,
    out GameObject building)
```

- 方法内检查：
  - `ruin != null`
  - `mapData != null`
  - `ruin.CanRebuildFor(faction)`
  - `sourcePlotId` 有效
- 使用 `ruin.GetRebuildBuildingType()` 决定新建筑类型。
- 调用 `BuildingFactory.CreateBuilding(...)` 创建建筑。
- 创建成功后销毁废墟 GameObject。
- 重建出的建筑通过 `BuildingFactory` 自动进入 `BuildingRegistry`。
- 可以让服务暂时不接入正式输入；本轮重点是服务边界。
- 新增脚本必须提交 `.meta`。

## MVP-03.8 禁止范围

- 不做正式重建按钮。
- 不做选择废墟 UI。
- 不做资源消耗。
- 不做重建进度条。
- 不做占领、升级、连地、区域奖励、传送阵、AI。
- 不改变当前建筑、战斗、巡逻、清场行为。
- 不修改 `ProjectSettings`。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。

## MVP-03.8 验收标准

- 存在清晰命名的重建服务类。
- 服务复用 `RuinComponent`、`BuildingFactory`、`BuildingRegistry` 的现有边界。
- 服务方法失败时应安全返回，不抛出明显空引用错误。
- 如果服务创建建筑成功，旧废墟会被销毁，新建筑会自动注册到 `BuildingRegistry`。
- 当前 K / L 清场、建筑死亡生成废墟、士兵围绕废墟巡逻行为不变。
- Console 无明显错误。
