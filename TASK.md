# TASK.md

## 当前任务

### 状态：MVP-01 实现已审查，需要 Claude 小修

Claude 已完成 MVP-01 的脚本实现，新增 12 个 C# 文件，覆盖：
- 固定地图（6 地块 + 7 道路）
- 地块大小 / 阵营 / 主基地 / 建筑槽位标记
- 基于 BFS 的道路寻路
- 三种单位移动速度差异
- 键盘触发的测试生成入口（Key 1-4）

Codex 已完成静态审查，结论见 `REVIEW.md`：

- 暂不批准进入 MVP-02
- 需要移除 `MapData.Instance` 这个未使用的全局状态入口
- 需要完成 Unity Play Mode / Console Error 验证
- 验证结果需要写入 `WORKLOG.md`

## 已确认事实

- 工作目录：`/Users/jianghao/unity`
- 项目目录：`kingbattle/`
- 当前项目是 Unity 小游戏项目。
- Unity 版本：`2022.3.62f1`
- 当前依赖包含：
  - `com.unity.render-pipelines.universal` `14.0.12`
  - `com.unity.feature.2d` `2.0.1`
  - `com.unity.test-framework` `1.1.33`
  - `com.unity.ugui` `1.0.0`
  - `com.unity.visualscripting` `1.9.4`
- 当前 `Assets` 下只有场景和渲染设置，未发现 C# 业务脚本。
- Git 根目录：`/Users/jianghao/unity`
- GitHub remote：`https://github.com/12342023/unity.git`
- 当前仓库已可推送到 GitHub。
- 后续规划包括：
  - 微信小程序移植
  - macOS 移植
  - Android 移植

## goal.md 摘要

项目目标是一个轻量 RTS + 区块占领 + 塔防 + 自动出兵 + 道路进攻小游戏。

核心体验：

```text
占领土地
→ 建造建筑
→ 获得资源
→ 出兵
→ 攻打敌方
→ 扩张领地
```

第一版本必须控制范围。按 `goal.md` 的开发优先级，第一阶段先做：

1. 地图
2. 地块
3. 道路
4. 单位移动

## Claude 下一轮任务

Claude 下一轮任务见 `NEXT_STEPS.md`，主题为：

```text
MVP-01 固定地图与道路移动原型
```

允许做：

- 创建最小脚本目录：`Core/`、`Map/`、`Units/`
- 固定地图数据或场景对象
- 5 到 7 个地块
- Small / Medium / Large 地块大小
- Player / Enemy / Neutral 阵营归属
- 固定道路连接
- 单位只能沿道路移动
- Samurai / Elf Archer / Soldier 三种单位移动速度配置
- 一个临时测试入口，用于生成单位并移动到目标地块

禁止做：

- 战斗系统
- AI
- 占领进度
- 建筑建造 / 重建 / 拆除
- 粮食资源
- 英雄系统
- 复杂 UI
- 随机地图
- 联机
- 修改 `ProjectSettings`，除非先说明理由并等待确认

## 架构要求

Claude 必须避免：

- 超长脚本
- 所有逻辑写进 `GameManager`
- 全局状态膨胀
- 平台差异散落在玩法逻辑中
- 过早为微信小程序、macOS、Android 创建适配工程

推荐边界：

```text
Scripts/
├── Core/
├── Map/
└── Units/
```

后续系统目录只有在对应阶段开始时再新增。

## GitHub 与工作文档规范

后续每次修改必须遵守：

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
- 使用或记录 GitHub 明文密码 / token
```

## 验收标准

Claude 完成 MVP-01 后，应提交：

- 清晰的脚本目录边界
- 固定地块和道路结构
- 单位沿道路移动的 Play Mode 验证说明
- 三个兵种移动速度差异
- 未实现内容清单
- 工作文档更新
- commit / push 记录

Codex 将基于 Claude 的输出更新 `REVIEW.md`，并决定是否允许进入 MVP-02：建筑、出兵、基础战斗。
