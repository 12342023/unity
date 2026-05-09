# TASK.md

## 当前任务

发布 MVP-03.7：建筑运行时注册表。

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

- 固定地图
- 固定道路
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

## 最新 Review 结论

Claude 最新提交：

```text
774a48e refactor: extract BuildingFactory from GameEntry for rebuild reuse
```

Codex Review 结论：

```text
MVP-03.6 代码审查通过；允许进入 MVP-03.7
```

## 当前工作区注意事项

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件仍是未跟踪 Unity Editor 生成文件。除非用户明确批准，否则不要提交。

## MVP-03.7 目标

为后续废墟重建准备建筑运行时注册边界。

当前 `BuildingFactory` 已能创建建筑，但未来重建出的建筑也必须进入阵营清场逻辑。否则后续会出现：

```text
重建出的建筑没有被 FactionDefeatHandler 管理，阵营被击败时无法正确变成废墟。
```

本轮只补注册表，不做真正重建。

## MVP-03.7 允许范围

- 新增 `Buildings/BuildingRegistry.cs` 或同等小类。
- 记录运行时建筑与阵营关系。
- 提供按阵营查询当前建筑的方法。
- 查询时过滤 `null` / 已死亡建筑，避免重复清场或重复生成废墟。
- `GameEntry` 创建初始建筑后注册建筑。
- `FactionDefeatHandler` 可以改为通过 registry 获取当前阵营建筑。
- 保持 K / L 清场行为不变。
- 新增脚本必须提交 `.meta`。

## MVP-03.7 禁止范围

- 不做实际重建。
- 不做重建按钮。
- 不做废墟选择交互。
- 不做资源、进度条、占领、升级、连地、区域奖励、传送阵、AI 或 UI。
- 不改变建筑位置、颜色、大小、血量或当前行为。
- 不修改 `ProjectSettings`。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。

## MVP-03.7 验收标准

- 初始建筑都被注册到 `BuildingRegistry` 或同等结构中。
- `FactionDefeatHandler` 清场仍能正确处理当前阵营所有存活建筑。
- K：击败 EnemyBase 后，敌方所有建筑变废墟，敌兵立即死亡，己方士兵仍存活。
- L：击败 PlayerBase 后，蓝方建筑变废墟，蓝方士兵立即死亡。
- 单个建筑死亡仍只生成一个废墟。
- Console 无明显错误。
