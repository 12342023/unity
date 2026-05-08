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

## 新发布要求：MVP-02.1 自动战争基础体验补强

Claude 下一阶段正式进入 MVP-02.1。

## 当前状态

### MVP-02.1 Codex Review：暂不通过

Claude 最新提交：

```text
91d2bfb fix: prevent patrol overwriting road movement, handle missing rally point
```

Codex Review 结论：

```text
暂不通过
```

| 组件 | 状态 | 说明 |
|---|---|---|
| 巡逻系统 | ⏳ 需继续修复 | 道路移动期间不再被 Tick 覆盖，但脱战后仍可能被 Tick 直接拉回巡逻圆 |
| 仇恨/锁敌 | ✅ | aggroRange=4, chaseRange=7, 自动 Chase → Attack |
| 脱战机制 | ❌ 阻塞 | Deaggro 只移动一帧，下一帧 Idle patrol.Tick 可能造成瞬移 |
| 集结点 | ✅ 已修复 | 先沿道路到达，到达后才转圈；无集结点时围绕 Barracks 转圈 |
| 波次推进 | ✅ | rallyThreshold=3, 够数后 road-based 推送 |
| Tower 仍只攻击单位 | ✅ | 不变 |

待处理：
- ⏳ 脱战后必须可见地走回 patrol circle / home，不能下一帧 snap 回巡逻圆
- ⏳ 前往 rallyPoint 途中接敌时，不能让 `movement.Stop()` 清掉路线后没有恢复策略
- ⏳ Play Mode 验证：追敌远离建筑后脱战，单位走回去再转圈
- ⏳ 更新 WORKLOG.md
- ⏳ commit / push
- ⏳ 等待 Codex Review 通过后进入第三阶段

已处理：
- ✅ UnitMovement.HasRemainingPath 为 true 时，不再允许 patrol.Tick 覆盖道路移动
- ✅ 无 rallyPoint 时也能出兵，并围绕所属 Barracks 转圈

主题：

```text
巡逻 + 仇恨范围 + 脱战 + 集结点 + 小波次推进
```

用户对“巡逻”的最新定义：

```text
单位在一个建筑旁边转圈巡逻
```

因此 Claude 需要把巡逻理解为建筑周边小半径环绕，而不是在多个地块之间来回走。

目标：

让已有出兵、移动、战斗系统更接近 `goal.md` 的自动战争体验，而不是继续堆新系统。

具体要求见：

```text
NEXT_STEPS.md
```

## 禁止提前开发

MVP-02.1 禁止：

- 摧毁与重建
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
