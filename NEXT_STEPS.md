# NEXT_STEPS.md

## 当前阶段判断

项目正在第三阶段早期：摧毁与重建。

已经完成：

- 建筑被击败后变成废墟。
- 士兵可以围绕废墟巡逻。
- 大本营被击败后，该阵营建筑变废墟，士兵立即死亡。
- 废墟携带来源 plot / 建筑类型 / 原阵营。
- 废墟具备最小可重建判定。
- 建筑创建逻辑已从 `GameEntry` 下沉到 `BuildingFactory`。

下一步不要直接做完整 UI 或资源系统，应继续用小步补齐“重建前的运行时边界”。

## 当前正式任务：MVP-03.7 建筑运行时注册表

目标：

```text
让运行时建筑可以按阵营被登记和查询，为后续重建出的建筑接入清场逻辑做准备。
```

为什么先做这个：

- 未来重建会创建新建筑。
- 新建筑如果不注册进运行时建筑集合，`FactionDefeatHandler` 就不知道它存在。
- 阵营被击败时，未注册的新建筑可能不会变成废墟。

## 给 Claude 的实现方向

```diff
+ 新增 Buildings/BuildingRegistry.cs 或同等小类
+ GameEntry 创建初始建筑后注册进去
+ FactionDefeatHandler 从 registry 查询当前阵营建筑
+ 查询时过滤 null / dead building
+ 保持 K / L 清场行为完全不变
- 不做实际重建
- 不做 UI / 资源 / 占领 / 升级 / 连地 / AI
- 不修改 ProjectSettings
```

## MVP-03.7 后的建议顺序

### MVP-03.8 废墟重建最小服务

在注册表稳定后，再新增 `BuildingRebuildService` 或同等小服务：

- 输入 `RuinComponent`、`MapData`、目标 `Faction`
- 使用 `RuinComponent.GetRebuildBuildingType()`
- 通过 `BuildingFactory.CreateBuilding(...)` 创建建筑
- 创建成功后注册到 `BuildingRegistry`
- 再销毁废墟
- 仍不做正式 UI / 资源

### MVP-03.9 调试入口验证重建闭环

仅在服务边界稳定后，再考虑一个临时调试快捷键验证闭环。正式 UI、资源消耗和占领规则继续后置。

## 长期提醒

- 微信小程序、macOS、Android 移植会要求业务逻辑边界干净。
- 平台能力不要散落在建筑、战斗、移动脚本中。
- 当前阶段继续优先保持 `GameEntry` 变薄，业务能力下沉到清晰的小组件 / 小服务。
