# NEXT_STEPS.md

## 下一步要求：MVP-02 建筑、出兵与基础战斗

依据 `goal.md` 和 MVP-01 复审结果，下一步进入 MVP-02。

MVP-02 目标不是做完整游戏，而是在已有固定地图和道路移动基础上，验证最小战斗链路：

```text
兵营生成单位
单位沿道路前进
进入范围后攻击
目标血量归零
```

## 给 Claude 的任务

Claude 作为主要开发者，本轮可以继续开发业务原型，但必须严格限制范围。

## 1. 建筑数据与表现

新增最小建筑系统，建议目录：

```text
kingbattle/Assets/Scripts/Buildings/
```

本轮只实现三种建筑的数据和基础表现：

- Tower
- Barracks
- Granary

要求：

- 建筑必须挂在已有地块上
- 每个地块先只放一个测试建筑即可
- 建筑有 faction、health、maxHealth
- Tower / Barracks / Granary 先用简单颜色或形状区分
- Granary 本轮只作为数据和可视化存在，不产粮

禁止做：

- 不做建造菜单
- 不做拆除
- 不做废墟
- 不做重建
- 不做建筑升级

## 2. 兵营出兵

Barracks 本轮需要能定时生成单位。

要求：

- 玩家兵营可以按固定间隔生成 Soldier
- 敌方兵营可以按固定间隔生成 Soldier
- 生成出的单位使用 MVP-01 的道路移动能力
- 单位目标可以先固定为敌方主基地或最近敌方建筑
- 出兵逻辑不要写进 `GameEntry`

暂时不要做：

- 兵种选择 UI
- 粮食消耗
- 出兵队列
- 多兵种自动组合

## 3. 基础战斗

新增最小战斗系统，建议目录：

```text
kingbattle/Assets/Scripts/Combat/
```

本轮只做固定伤害。

要求：

- 单位有 health、damage、attackRange、attackInterval
- 建筑有 health
- 单位进入攻击范围后停止移动并攻击敌方建筑或敌方单位
- Tower 可以自动攻击范围内敌方单位
- 血量归零后对象从场景移除

暂时不要做：

- 防御力
- 暴击
- 技能
- 复杂寻敌权重
- 投射物美术
- 死亡动画

## 4. 测试入口

保留临时测试入口即可。

Claude 需要提供一种简单方式验证：

- 玩家 Barracks 自动出 Soldier
- 敌方 Barracks 或 Tower 存在
- Soldier 沿道路移动到目标附近
- Soldier 能攻击敌方建筑
- Tower 能攻击进入范围的敌方单位
- Console 无明显错误

可以继续使用键盘触发或 Play Mode 自动启动，但必须在 `WORKLOG.md` 写清楚测试步骤。

## 架构边界

MVP-02 推荐结构：

```text
Scripts/
├── Core/
├── Map/
├── Units/
├── Buildings/
└── Combat/
```

要求：

- `GameEntry` 仍保持薄启动脚本
- 不新增大型 `GameManager`
- 不引入全局单例
- 不把平台判断写入玩法逻辑
- 不改 `ProjectSettings`，除非先说明理由并等待确认

## 本轮禁止事项

Claude 本轮不要做：

- 不做 AI 决策
- 不做占领进度
- 不做粮食资源
- 不做建造 / 拆除 / 重建 UI
- 不做英雄系统
- 不做技能树、装备、科技升级
- 不做随机地图
- 不做联机
- 不做复杂 UI
- 不做平台适配

## 验收标准

Claude 完成 MVP-02 后，必须满足：

- 已新增清晰的 `Buildings/` 和 `Combat/` 边界
- Barracks 能自动生成至少一种单位
- 单位能沿道路移动到敌方目标附近
- 单位能攻击敌方单位或建筑
- Tower 能自动攻击范围内敌方单位
- 血量归零后对象会被移除
- Play Mode 验证通过，Console 无明显错误
- 未修改 `ProjectSettings`，或已提前说明理由
- 未提交 Unity 生成目录
- 已更新 `TASK.md`、`REVIEW.md`、`NEXT_STEPS.md`、`WORKLOG.md`
- 已 commit / push

## Claude 输出要求

Claude 提交后必须说明：

- 新增或修改了哪些文件
- 如何在 Unity 中运行 MVP-02 测试
- 哪些逻辑是临时测试入口
- 哪些 `goal.md` 内容仍未实现
- 是否修改了场景文件
- 是否修改了 `ProjectSettings`
- 是否完成 commit / push

## Codex Review 重点

Claude 输出后，Codex 重点 Review：

- 是否保持 MVP-02 范围，没有提前做 AI、占领、资源和复杂 UI
- 建筑、单位、战斗之间是否有清晰边界
- 是否出现全局单例或超大管理器
- Tower / Barracks 的行为是否能在 Play Mode 验证
- 单位是否仍沿道路移动，而不是直接穿越地图
- 对象销毁是否简单可靠
- 是否没有提交 Unity 生成目录或未确认的 `ProjectSettings`

## 当前状态

```diff
+ MVP-01 已通过 Codex 复审
+ MVP-02 脚本实现已完成，待 Codex 审查
+ 新增 Buildings/ 和 Combat/ 目录
+ 三种建筑数据 + Barracks 自动出兵 + Tower 自动攻击 + 单位基础战斗
- Codex 静态审查发现 Tower 会攻击建筑，需要修复
- 暂不批准进入 MVP-03
- 暂不批准 AI、占领、粮食资源、建造 UI、拆除、重建、英雄系统
```

## 下一步：MVP-02 修复

Claude 下一步先修复 MVP-02 Review 项：

```diff
- Tower 当前会攻击敌方建筑
+ Tower 只攻击带 UnitCombat 的敌方单位
+ 重新 Play Mode 验证 Tower 不攻击建筑
+ 更新 WORKLOG.md
+ commit / push
```

修复完成并通过 Codex 复审后，MVP-03 才预计包含：

- 占领系统
- 粮食资源
- 基础 AI
- 简单 UI
