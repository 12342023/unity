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
- 运行时建筑已通过 `BuildingRegistry` 按阵营登记和查询。

下一步开始做“重建闭环”的最小服务，但仍然不做正式 UI、资源、占领或升级。

## 当前正式任务：MVP-03.8 废墟重建最小服务

目标：

```text
让代码层面具备“从废墟重建建筑”的单一服务入口。
```

为什么现在做：

- `RuinComponent` 已经知道自己来自哪个 plot、哪个建筑类型。
- `BuildingFactory` 已经能创建建筑。
- `BuildingRegistry` 已经能让新建筑进入阵营清场体系。
- 现在可以用一个小服务把三者串起来，而不用把重建逻辑塞回 `GameEntry`。

## 给 Claude 的实现方向

```diff
+ 新增 Buildings/BuildingRebuildService.cs 或同等小类
+ 输入 RuinComponent / MapData / Faction
+ 使用 CanRebuildFor 和 GetRebuildBuildingType
+ 通过 BuildingFactory.CreateBuilding 创建新建筑
+ 创建成功后销毁旧废墟
+ 新建筑依靠 BuildingFactory 自动注册到 BuildingRegistry
+ 新增脚本必须提交 .meta
- 不做正式重建按钮
- 不做 UI / 资源 / 占领 / 升级 / 连地 / AI
- 不修改 ProjectSettings
```

## MVP-03.8 后的建议顺序

### MVP-03.9 临时调试入口验证重建闭环

服务边界稳定后，再加一个临时调试入口，例如仅用于 Play Mode 的快捷键或小测试脚本：

- 找到一个可重建废墟
- 调用 `BuildingRebuildService`
- 验证新建筑生成、旧废墟销毁、新建筑参与 K / L 清场

仍不做正式 UI、资源消耗或占领进度。

### MVP-03.10 重建规则最小化

在服务和调试闭环都稳定后，再考虑最小规则：

- 哪个阵营允许重建
- 是否只能重建特定 plot
- 重建后归属谁

资源、升级、连地、区域奖励继续后置。

## 长期提醒

- 微信小程序、macOS、Android 移植会要求业务逻辑边界干净。
- 平台能力不要散落在建筑、战斗、移动脚本中。
- 当前阶段继续优先保持 `GameEntry` 变薄，业务能力下沉到清晰的小组件 / 小服务。
