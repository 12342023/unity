# TASK.md

## 当前任务

正式发布 MVP-02.1 自动战争基础体验补强任务。

Codex 当前仍作为 Tech Lead / Reviewer 工作，不直接开发业务代码。Claude 是主要开发者。

## 项目定位

项目是 Unity 小游戏，当前主工程位于：

```text
kingbattle/
```

最新 `goal.md` 将游戏定位为：

```text
低操作、高战略、自动战争 RTS
```

玩家不直接操作单个单位，而是通过区域、道路、兵力、建筑和推进路线进行战略调度。

## 核心目标

最终体验应围绕：

- 自动战争
- 战线推进
- 区域争夺
- 多线调兵
- 中央大战场
- 偷袭切后方
- 前线拉锯战
- 军团推进感
- 战略区域控制

## 最新开发优先级

`goal.md` 明确阶段顺序如下：

### 第一阶段

- 地图
- 道路
- 单位移动
- 巡逻

### 第二阶段

- 自动出兵
- 自动战斗
- 锁敌
- 仇恨范围
- 脱战

### 第三阶段

- 摧毁与重建
- 建筑被击败后变成废墟
- 士兵可以围绕废墟转圈巡逻
- 建筑
- 升级
- 资源
- 连地系统

### 第四阶段

- 区域奖励
- 中央区域
- 传送阵
- AI

### 第五阶段

- UI
- 特效
- 音效
- 美术优化

## 当前实现状态

已完成 / 已存在：

- 固定地图
- 固定道路
- 单位沿道路移动
- 三种基础单位
- Tower / Barracks / Granary 原型
- Barracks 自动出兵
- Tower 自动攻击
- 单位基础战斗

当前工作区注意事项：

```text
goal.md                                            已由用户更新，作为最新需求来源
kingbattle/Assets/Scripts/Buildings/TowerAttack.cs 已有 Tower 只攻击 UnitCombat 的修复改动
kingbattle/ProjectSettings/SceneTemplateSettings.json 仍是未跟踪 Unity Editor 生成文件
```

Codex 不直接修改上述业务脚本。

## MVP-02 修复收尾状态

当前 Review 阻塞项已处理：

```diff
- Tower 可能攻击敌方建筑
+ Tower 只攻击带 UnitCombat 的敌方单位
+ 用户确认 Play Mode 验证 OK
+ WORKLOG.md 已记录修复详情
+ commit / push 已完成
```

已确认提交：

```text
48f7eb5 fix: tower targets units only, add missing meta files
```

## MVP-02.1 自动战争基础体验补强收尾

MVP-02.1 / MVP-02.1a 已完成核心收尾。

## 当前状态

### MVP-03.5 任务发布：废墟可重建判定数据层

Claude 最新提交：

```text
9bc149b feat: building identity data and ruin metadata for future reconstruction
```

Codex Review 结论：

```text
MVP-03.4 代码审查通过；允许进入 MVP-03.5
```

| 组件 | 状态 | 说明 |
|---|---|---|
| 巡逻系统 | ✅ 已修复 | 道路移动期间不再被 Tick 覆盖；脱战后 MoveTowards CurrentPatrolPosition 到 0.1 |
| 仇恨/锁敌 | ✅ | aggroRange=4, chaseRange=7, 自动 Chase → Attack |
| 脱战机制 | ✅ 已修复 | 回程目标改为 CurrentPatrolPosition，0.1 阈值后恢复 Tick；Pause/Resume 保留路线 |
| 集结点 | ✅ 已修复 | 先沿道路到达，到达后才转圈；无集结点时围绕 Barracks 转圈 |
| 波次推进 | ✅ | rallyThreshold=3, 够数后 road-based 推送 |
| Tower 仍只攻击单位 | ✅ | 不变 |

待处理：
- ⏳ MVP-03.5：为废墟增加可重建判定数据层
- ⏳ Claude 完成后，Codex 再 Review

已处理：
- ✅ UnitMovement.HasRemainingPath 为 true 时，不再允许 patrol.Tick 覆盖道路移动
- ✅ 无 rallyPoint 时也能出兵，并围绕所属 Barracks 转圈
- ✅ 旧版 Deaggro 一帧 MoveTowards 已删除，返回逻辑改为持续执行
- ✅ 脱战后 MoveTowards 到 `patrol.CurrentPatrolPosition`，0.1 阈值后才恢复 Tick
- ✅ 接敌时 `movement.Pause()` 保留 road path，脱战时 `movement.Resume()` 恢复路线
- ✅ 用户确认核心四项没问题
- ✅ HealthComponent 已记录原始颜色作为满血颜色，颜色修复方向正确
- ✅ Deaggro 清理残留 pushPath，解决击败敌方大本营后短暂折返
- ✅ 建筑死亡后已能生成可见废墟
- ✅ 非空目标场景下，普通单位死亡不再把死亡点传给 Deaggro
- ✅ target == null / chaseTarget == null 时只 Deaggro，不再切换 patrol center
- ✅ 一方大本营被击败后，该方建筑清场为废墟，士兵立即死亡
- ✅ K / L Play Mode 测试快捷键已添加，便于验证双方大本营清场
- ✅ `SpawnRuin` 已从 `GameEntry` 移入 `Buildings/BuildingDeathHandler.cs`
- ✅ `BuildingDeathHandler.cs.meta` 已提交
- ✅ 建筑身份数据 `BuildingIdentity` 已补充
- ✅ 废墟来源元数据 `sourcePlotId / sourceBuildingType / originalFaction` 已补充

## 新发布要求：MVP-03.5 废墟可重建判定数据层

```text
不做完整重建系统，只给废墟增加“是否可重建 / 可重建成什么”的最小判定能力。
```

## MVP-03.5 目标

- `RuinComponent` 能表达未来是否可重建。
- `RuinComponent` 能返回未来可重建的建筑类型。
- 当前玩法表现保持不变。

## MVP-03.5 允许范围

- 扩展 `RuinComponent`，新增简单方法，例如：
  - `CanRebuildFor(Faction faction)`
  - `GetRebuildBuildingType()`
- 规则保持最小：
  - 有 `sourcePlotId` 才可重建。
  - `sourceBuildingType` 决定可重建类型。
  - `originalFaction` 只作为来源记录，不在本任务里做复杂归属规则。
- 必要时新增小 enum / 小数据结构，但优先保持简单。
- 更新 `WORKLOG.md`。

## MVP-03.5 禁止范围

- 不做重建按钮。
- 不真正生成新建筑。
- 不做占领进度。
- 不做资源、升级、连地、区域奖励、传送阵、AI 或 UI。
- 不改变当前战斗、巡逻、清场表现。
- 不修改 `ProjectSettings`。
- 不提交 Unity 生成目录。

## MVP-03.5 验收标准

- `RuinComponent` 能判断是否具备重建所需的最小来源数据。
- `RuinComponent` 能返回来源建筑类型作为未来重建类型。
- 建筑死亡、K/L 清场、士兵围绕废墟巡逻行为不变。
- Console 无明显错误。

## 新发布要求：MVP-03.4 建筑身份数据与废墟元数据

```text
不做完整重建系统，只为未来重建准备最小必要数据。
```

## MVP-03.4 目标

- 每个建筑应有清晰身份数据：所属 plot、建筑类型、阵营。
- 建筑死亡后生成的废墟应保留来源信息：
  - `sourcePlotId`
  - `sourceBuildingType`
  - `originalFaction`
- 当前玩法表现保持不变。

## MVP-03.4 允许范围

- 新增 `Buildings/BuildingIdentity.cs` 或同等小组件。
- 在 `GameEntry.CreateBuilding()` 装配建筑时写入 `plotId`、`BuildingType`、`Faction`。
- 扩展 `RuinComponent` 记录来源建筑元数据。
- `BuildingDeathHandler` 生成 Ruin 时，把建筑身份数据传给 `RuinComponent`。
- 小范围调整 `GameEntry` 装配代码。
- 更新 `WORKLOG.md`。

## MVP-03.4 禁止范围

- 不做重建按钮。
- 不做占领进度。
- 不做资源、升级、连地、区域奖励、传送阵、AI 或 UI。
- 不改变当前战斗、巡逻、清场表现。
- 不修改 `ProjectSettings`。
- 不提交 Unity 生成目录。

## MVP-03.4 验收标准

- 建筑 GameObject 上存在清晰的身份组件或等价数据。
- Ruin 上能记录来源 plot / 原建筑类型 / 原阵营。
- 建筑死亡、K/L 清场、士兵围绕废墟巡逻行为不变。
- 单个建筑死亡仍只生成一个废墟。
- Console 无明显错误。
- 新增脚本必须提交对应 `.meta`。

## 新发布要求：MVP-03.3 建筑死亡 / 废墟职责边界整理

```text
不改变玩法表现，只整理建筑死亡和废墟生成的代码职责。
```

## MVP-03.3 目标

- 将 `SpawnRuin` / 建筑死亡处理从 `GameEntry` 中下沉到 `Buildings/` 下的小组件或小服务。
- `GameEntry` 保持测试场景装配职责，不继续承载建筑死亡业务逻辑。
- 保持当前所有玩法表现不变：
  - 建筑死亡后生成废墟。
  - 士兵击败建筑后围绕废墟巡逻。
  - 大本营被击败后，该阵营建筑清场为废墟，士兵立即死亡。
  - K / L 测试快捷键仍可用于 Play Mode 验证。

## MVP-03.3 允许范围

- 新增 `Buildings/BuildingDeathHandler.cs`、`Buildings/RuinSpawner.cs` 或同等小组件。
- 让建筑自己的死亡逻辑负责生成废墟。
- 让 `FactionDefeatHandler` 继续复用 `HealthComponent.Kill()` 和建筑死亡逻辑。
- 小范围调整 `GameEntry` 的装配代码。
- 更新 `WORKLOG.md`。

## MVP-03.3 禁止范围

- 不改变当前玩法数值。
- 不新增完整重建系统。
- 不新增占领进度。
- 不新增资源、升级、连地、区域奖励、传送阵、AI 或 UI。
- 不扩大 K / L 测试快捷键为正式功能。
- 不修改 `ProjectSettings`。
- 不提交 Unity 生成目录。

## MVP-03.3 验收标准

- `GameEntry` 中不再包含 `SpawnRuin` 的具体实现。
- 废墟生成逻辑位于 `Buildings/` 下清晰命名的小组件/小服务中。
- 单个建筑死亡仍只生成一个废墟。
- 阵营清场仍能让失败方所有存活建筑变废墟、士兵立即死亡。
- K / L 测试快捷键仍可触发双方大本营清场。
- Console 无明显错误。
- 不修改 `ProjectSettings`，除非先说明理由并等待确认。
- 不提交 Unity 生成目录。

## 历史要求：MVP-03.1 建筑废墟与废墟巡逻

```text
建筑被击败后进入废墟状态；士兵可以围绕废墟转圈巡逻。
```

## MVP-03.1 目标

- 建筑死亡后留下可见废墟，而不是完全消失。
- 废墟成为新的巡逻中心。
- 士兵打败建筑后围绕废墟巡逻。
- 废墟不再执行原建筑功能。

## MVP-03.1 允许范围

- 建筑死亡后生成或转换为 Ruin / 废墟状态
- 简单废墟视觉，例如灰色/暗色方块
- 禁用废墟上的 TowerAttack / BarracksSpawner 等建筑功能
- 攻击建筑的士兵在目标死亡后围绕废墟转圈
- 必要时新增小组件，例如 `BuildingRuin` / `BuildingDeathHandler`

## MVP-03.1 禁止范围

- 重建
- 占领进度
- 资源产出
- 建筑升级
- 连地系统
- 区域奖励
- 传送阵
- AI 决策
- UI / 美术大改 / 音效

## MVP-03.1 验收标准

- EnemyBase / 敌方建筑血量归零后留下废墟
- 废墟不再出兵、不再攻击
- 士兵打败建筑后围绕废墟转圈
- 普通单位死亡仍直接销毁
- Console 无明显错误
- 不修改 `ProjectSettings`，除非先说明理由并等待确认
- 不提交 Unity 生成目录

## 新增要求：MVP-03.2 大本营击败后的阵营清场

用户新增规则：

```text
对方大本营被攻占后，对方所有建筑成为废墟，所有兵立即死亡。
```

当前阶段尚未实现“占领进度”系统，因此本任务暂按以下语义执行：

```text
大本营被击败 / 血量归零 / 进入废墟 = 大本营被攻占
```

MVP-03.2 目标：

- 当前阶段以 `PlayerBase` / `EnemyBase` 对应的 Barracks 作为双方大本营。
- 任一方大本营被击败后，判定该阵营失败。
- 失败阵营所有仍存活建筑立即转为废墟。
- 失败阵营所有仍存活单位立即死亡 / 销毁。
- 废墟仍不攻击、不出兵、无 `HealthComponent`。
- 该逻辑应小范围实现，不引入完整占领系统、胜负 UI、资源、AI 或重建。

实现建议：

- 优先新增小组件或小服务，例如 `Buildings/FactionDefeatHandler.cs`。
- `GameEntry` 可以负责装配引用，但不要继续堆大型业务逻辑。
- 需要避免重复生成同一个建筑的废墟。
- 需要避免清场时触发连锁异常，例如遍历时对象被销毁。

具体要求见：

```text
NEXT_STEPS.md
```

## 历史主题：MVP-02.1

```text
巡逻 + 仇恨范围 + 脱战 + 集结点 + 小波次推进
```

## 当前仍需保持

MVP-03.1 仍然禁止：

- 建筑升级
- 粮食资源
- 人口系统
- 连地系统
- 区域奖励
- 中央区域奖励
- 传送阵
- AI 决策
- 地形信息 UI
- 英雄
- 科技树
- 装备
- 随机地图
- 联机
- 平台适配
- 大规模重构

## GitHub 与工作文档规范

每次修改必须遵守：

```diff
+ 更新 TASK.md / REVIEW.md / NEXT_STEPS.md / WORKLOG.md 中相关记录
+ 只 stage 本次任务相关文件
+ 提交信息说明本次变更目的
+ 推送到 GitHub
+ 在 WORKLOG.md 记录 commit / push 结果
```

仍然禁止：

```diff
- git add .
- 提交 Library/、Logs/、UserSettings/、Temp/ 等 Unity 生成目录
- 混入未确认的 ProjectSettings 修改
- 使用或记录 GitHub 明文密码 / token
```

## Claude 输出要求

Claude 完成当前修复或 MVP-02.1 后必须说明：

- 新增或修改了哪些文件
- 如何在 Unity 中运行验证
- 如何观察单位围绕建筑转圈巡逻
- 哪些逻辑是临时测试入口
- 哪些 `goal.md` 内容仍未实现
- 是否修改了场景文件
- 是否修改了 `ProjectSettings`
- 是否完成 commit / push

Codex 将基于 Claude 输出更新 `REVIEW.md`，并决定是否允许进入后续第三阶段系统。
