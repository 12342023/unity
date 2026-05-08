# REVIEW.md

## Review 状态

当前尚未收到 Claude 的 MVP-01 实现输出。本文件记录 Codex 基于 `goal.md` 给出的阶段门禁和后续审查标准。

## 当前 Review 结论

### 已批准：进入 MVP-01 原型开发

`goal.md` 已明确第一阶段优先级：

```text
地图
地块
道路
单位移动
```

因此允许 Claude 开始最小业务原型开发，但只批准 `NEXT_STEPS.md` 中定义的固定地图与道路移动范围。

### 不批准：提前开发完整玩法系统

以下内容仍不允许在 MVP-01 中实现：

- 战斗
- AI
- 占领进度
- 建筑建造 / 重建 / 拆除
- 粮食资源
- 英雄
- 联机
- 随机地图
- 复杂 UI

原因：这些内容依赖地图、道路和单位移动是否成立。先把路线移动原型打通，后续再分阶段叠加系统。

### 仍需保持：Unity 工程与仓库边界

当前项目根目录已确认为 `/Users/jianghao/unity`，Unity 主工程位于 `kingbattle/`。

后续仍不允许提交：

```diff
- kingbattle/Library/
- kingbattle/Logs/
- kingbattle/UserSettings/
- kingbattle/Temp/
- kingbattle/Obj/
- kingbattle/Build/
- kingbattle/Builds/
```

仍不建议使用 `git add .`，应只 stage 本次任务相关文件。

### 仍需保持：多平台移植边界

微信小程序、macOS、Android 是后续移植目标，不是 MVP-01 的实现目标。

Claude 如果遇到输入、存储、分享、支付、构建发布等平台能力，应先提出接口边界，不要把平台判断写进玩法逻辑。

## Claude 输出后的 Review 检查项

- 是否符合 `goal.md` 第一阶段范围
- 是否只实现固定地图、地块、道路、单位移动
- 是否没有提前实现战斗、AI、占领、建筑、资源、英雄
- 是否创建了清晰的 `Scripts/Core`、`Scripts/Map`、`Scripts/Units` 边界
- 是否避免超大 `GameManager`
- 道路和地块关系是否清晰
- 单位移动是否被限制在道路上
- 三个兵种是否只有必要的移动配置差异
- 是否未修改 `ProjectSettings`，或已提前说明理由
- 是否没有提交 Unity 生成目录
- 是否更新工作文档
- 是否完成 commit / push 并记录结果

## Codex 当前判断

可以进入 MVP-01，但不能越界到完整游戏系统。

下一次 Review 的重点不是“功能多不多”，而是：

```text
地图结构是否清楚
道路移动是否成立
架构边界是否干净
后续系统是否容易接上
```
