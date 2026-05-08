# TASK.md

## 当前任务

### 状态：MVP-02 已审查，需要 Claude 小修

Claude 已完成 MVP-02 的脚本实现：
- 新增 `Buildings/` 和 `Combat/` 目录
- 三种建筑（Tower / Barracks / Granary）的最小数据和表现
- Barracks 每 5 秒自动生成 Soldier，沿道路前往敌方目标
- Tower 自动攻击范围内敌方单位
- 单位进入范围后攻击敌方建筑
- 血量归零后对象自动销毁
- UnitMovement 新增 Pause / Resume
- 建筑和单位均添加 Collider2D 支持 Tower 的 Physics2D 检测
- Key 1-4 测试入口保持可用

Codex 已完成静态审查，结论见 `REVIEW.md`：

- 暂不批准进入 MVP-03
- Tower 当前会攻击敌方建筑，需要改为只攻击敌方单位
- 新增脚本 `.meta` 文件漏提交，Codex 本轮会补齐
- 修复后需要重新 Play Mode 验证 Tower 不攻击建筑，只攻击敌方 Soldier

- `MapData.Instance` 已移除
- `kingbattle/Assets/Scripts` 下未发现 `MapData.Instance` 引用
- Unity Play Mode 手动验证通过
- MVP-01 范围控制通过
- 允许进入 MVP-02

## 已确认事实

- 工作目录：`/Users/jianghao/unity`
- 项目目录：`kingbattle/`
- 当前项目是 Unity 小游戏项目。
- Unity 版本：`2022.3.62f1`
- Git 根目录：`/Users/jianghao/unity`
- GitHub remote：`https://github.com/12342023/unity.git`
- 后续规划包括：
  - 微信小程序移植
  - macOS 移植
  - Android 移植

## MVP-01 已完成内容

- 固定地图：6 个地块，7 条双向道路
- 地块大小：Small / Medium / Large
- 地块归属：Player / Enemy / Neutral
- 玩家主基地与敌方主基地
- 建筑槽位数据标记
- BFS 道路寻路
- Samurai / Elf Archer / Soldier 三种单位移动速度差异
- Key 1-4 临时测试入口
- 单位沿道路移动验证

## 当前仓库注意事项

Unity Editor 生成了未跟踪文件：

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件暂不纳入本次提交。后续需要单独决定是否提交或忽略，不要和业务代码混在一起处理。

## Claude 下一轮任务

Claude 下一轮任务见 `NEXT_STEPS.md`，主题为：

```text
MVP-02 建筑、出兵与基础战斗
```

允许做：

- 新增 `Scripts/Buildings/`
- 新增 `Scripts/Combat/`
- Tower / Barracks / Granary 的最小数据和表现
- Barracks 定时生成 Soldier
- 单位沿道路前往敌方目标
- 单位基础攻击
- Tower 自动攻击范围内敌方单位
- health / damage / attackRange / attackInterval 等最小战斗数据

禁止做：

- AI 决策
- 占领进度
- 粮食资源
- 建造 / 拆除 / 重建 UI
- 英雄系统
- 技能树、装备、科技升级
- 随机地图
- 联机
- 复杂 UI
- 平台适配
- 未说明理由就修改 `ProjectSettings`

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
- 混入未确认的 ProjectSettings 修改
- 使用或记录 GitHub 明文密码 / token
```

## MVP-02 验收标准

Claude 完成 MVP-02 后，应提交：

- 清晰的 `Buildings/` 和 `Combat/` 目录边界
- Barracks 自动出兵验证
- Tower 自动攻击验证
- 单位攻击建筑或单位验证
- 血量归零移除对象验证
- Play Mode / Console Error 验证说明
- 未实现内容清单
- 工作文档更新
- commit / push 记录

Codex 将基于 Claude 的修复输出更新 `REVIEW.md`，并决定是否允许进入 MVP-03：占领、资源、基础 AI 或 UI。
