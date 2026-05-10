# WORKLOG.md

> 压缩版工作日志。旧版逐条详细记录已保存在 Git 历史中，可查看压缩前提交 `a3bdb55`。

## 项目协作规则

- Codex 角色：Tech Lead / Reviewer，不直接大规模开发业务代码。
- Claude 角色：主要开发者。
- 协作文档：
  - `TASK.md`：当前任务和边界。
  - `REVIEW.md`：Codex review 结论与给 Claude 的命令。
  - `NEXT_STEPS.md`：阶段规划。
  - `WORKLOG.md`：压缩后的阶段记录。
- GitHub 目标仓库：`https://github.com/12342023/unity.git`
- Git 根目录：`/Users/jianghao/unity`
- 主 Unity 工程：`kingbattle/`
- 每次有效修改后需要 commit / push，并记录上传状态。

## 长期约束

- 不随意修改 `ProjectSettings`。
- 不提交 Unity 生成目录：`Library/`、`Logs/`、`UserSettings/`。
- `kingbattle/ProjectSettings/SceneTemplateSettings.json` 一直是 Unity Editor 生成/设置类未跟踪文件，默认不提交。
- 为微信小程序、macOS、Android 移植保留边界：平台能力要封装，不散落在业务脚本里。
- 当前阶段继续让 `GameEntry` 变薄，把规则能力下沉到清晰的小服务。

## 仓库与 GitHub 初始化

- 2026-05-07 确认 `/Users/jianghao/unity` 为项目 Git 根目录。
- 上传内容包含协作文档与 `kingbattle/`。
- 设置远程仓库为 `https://github.com/12342023/unity.git`。
- 完成首次提交与推送。
- 后续所有文档与代码阶段都按“review / 任务发布 / 上传记录”流转。

## 阶段摘要

### MVP-01 固定地图与道路移动

- 完成固定地图 `MapData`、道路数据、地图渲染、道路寻路和基础单位移动。
- Review 中删除了不适合跨平台维护的 `MapData.Instance` 全局状态。
- 结论：通过，进入建筑与战斗阶段。

### MVP-02 建筑、出兵与基础战斗

- 完成建筑创建、兵营出兵、单位战斗、Tower 只攻击单位等基础玩法。
- 根据新版 `goal.md` 调整方向，明确当前是 Unity 小游戏，未来计划微信小程序、macOS、Android 移植。
- 结论：通过，进入巡逻/集结/推进阶段。

### MVP-02.1 巡逻、仇恨、集结、小波次推进

- 明确“巡逻”含义：士兵围绕建筑或废墟转圈，而不是在地块间来回走。
- 修复过的问题：
  - 无集结点不应出兵。
  - 巡逻不能瞬移。
  - 脱战后不能折返或 snap。
  - 集合/推进时中途变色问题。
- 结论：核心行为通过，进入废墟与阵营清场阶段。

### MVP-03.1 建筑废墟与废墟巡逻

- 建筑死亡后生成废墟。
- 士兵击败建筑后可围绕废墟巡逻。
- 修复过的问题：
  - 普通单位死亡不应成为巡逻中心。
  - null target 不能被当作建筑位置。
- 结论：通过。

### MVP-03.2 大本营击败后的阵营清场

- 击败 `EnemyBase` 后：
  - 敌方所有建筑变成废墟。
  - 敌方所有士兵立即死亡。
  - 己方士兵存活并围绕废墟巡逻。
- 反向验证 `PlayerBase` 被击败时蓝方被清场。
- 新增测试快捷键：
  - `K`：击败 EnemyBase。
  - `L`：击败 PlayerBase。
- 结论：通过。

### MVP-03.3 建筑死亡 / 废墟职责边界

- 整理建筑死亡和废墟生成职责。
- 补齐新增脚本 `.meta` 后 review 通过。
- 结论：通过并进入建筑身份数据阶段。

### MVP-03.4 建筑身份数据与废墟元数据

- 新增或完善建筑身份数据：
  - plotId
  - BuildingType
  - Faction
- 废墟记录来源建筑信息，便于后续重建和特殊规则。
- 结论：通过。

### MVP-03.5 废墟可重建判定数据层

- 增加废墟可重建判定所需数据。
- 不真正生成新建筑，不做 UI。
- 结论：通过。

### MVP-03.6 建筑创建逻辑下沉

- 建筑创建逻辑下沉到 `BuildingFactory`。
- 目标是减少 `GameEntry` 中的业务创建细节。
- 结论：通过。

### MVP-03.7 建筑运行时注册表

- 新增 `BuildingRegistry`。
- 支持运行时查询建筑，用于阵营清场和后续规则服务。
- 结论：通过。

### MVP-03.8 废墟重建最小服务

- 新增 `BuildingRebuildService`。
- 追加 `mapData == null` 防御后 review 通过。
- 结论：通过。

### MVP-03.9 大本营废墟特殊规则

- 普通重建不能重建大本营废墟。
- 修复 Unity 编译问题：补充 `MapData` namespace。
- 结论：通过。

### MVP-03.10 大本营废墟聚兵点

- 大本营废墟可以作为聚兵点。
- `T`：蓝兵聚到第一个大本营废墟。
- `R`：遍历所有废墟，跳过大本营废墟，只重建普通废墟。
- 结论：通过。

### MVP-03.11 大本营废墟连接未占领地点

- 大本营废墟可查询相邻 Neutral plot。
- `Y`：打印 main-base ruin 可连接 Neutral。
- 结论：通过。

### MVP-03.12 大本营废墟临时派兵

- `U`：从 main-base ruin 派附近 Player 士兵去第一个相邻 Neutral plot。
- 修复编译和派兵路径问题。
- 结论：通过。

### MVP-03.13 Neutral plot 最小占领

- 新增 `PlotCaptureService`。
- 支持 Player-only、Neutral-only、no-main-base 的最小占领。
- 到达后刷新地块颜色。
- 多轮修复后收口：
  - `Reset()` 清状态。
  - 防止重复占领。
  - 修复重复按 U 时 stale handler 风险。
- 结论：通过。

### MVP-03.14 派兵边界整理

- 新增 `StrategicDispatchService`。
- 将 U 派兵、附近士兵筛选、capture handler 管理从 `GameEntry` 下沉到服务。
- `GameEntry` 的 U 分支只保留高层流程。
- 结论：通过。

### MVP-03.15 占领后的下一层连接查询

- 新增 `StrategicConnectionService`。
- `I`：查询 Player-owned non-main-base frontier 的相邻 Neutral。
- 文档更正：当前 `MapData.CreateFixedMap()` 中 `Village` 与 `Farmland` 都是 Neutral；Crossroads 被占领后，I 合理输出包含 `Village, Farmland`。
- 结论：通过。

### MVP-03.16 从已占领 plot 派兵到相邻 Neutral

- `O`：从第一个 Player-owned frontier plot 派兵到第一个相邻 Neutral。
- 复用：
  - `StrategicConnectionService`
  - `RoadPathFinder`
  - `StrategicDispatchService`
  - `PlotCaptureService`
- 到达后目标 Neutral 变 Player 并刷新颜色。
- Codex review 通过，并发布 MVP-03.17。

### MVP-03.17 扩张编排服务

- 目标：将 O 的 source/target/path/dispatch 编排从 `GameEntry` 下沉到 `StrategicExpansionService`。
- 新增 `kingbattle/Assets/Scripts/Combat/StrategicExpansionService.cs` 与对应 `.meta`。
- `GameEntry` 的 O 分支简化为调用 `StrategicExpansionService.ExpandNext(mapData, mapRenderer)` 并打印结果。
- O 外部行为保持不变。
- Codex review 通过，并发布 MVP-03.18。

### MVP-03.18 连地网络规则整理

- 新增 `ExpansionCandidate` 数据模型。
- 新增 `StrategicConnectionService.GetExpansionCandidates(mapData)`。
- `StrategicExpansionService` 改为使用第一个 candidate 派兵。
- `ExpansionResult` 补充 `sourcePlotId`、`targetPlotId`、`dispatchedCount`。
- O/I/K/L/R/T/Y/U 行为不变。
- Codex review 通过，并发布 MVP-03.19。

### MVP-03.19 占领需求数据层

- 当前发布任务：按 `PlotSize` 查询占领所需兵力。
- 用户希望后续每轮多布置一些任务，加快进度；从本轮开始默认打包 2-3 个强相关小任务。
- 本轮批量目标：
  - 新增 `PlotCaptureRequirementService` 或等价服务。
  - Small / Medium / Large 返回 1 / 2 / 3 或等价清晰规则。
  - 增加 P 临时日志快捷键打印各 plot 占领需求。
  - `ExpansionResult` / O 日志携带 dispatched vs required 预览。
  - 暂不接入实际 capture 判定，当前到达即占领行为不变。

## 当前待处理状态

截至 MVP-03.19 任务发布前，工作区仍存在以下未提交/未跟踪变更：

```text
D  要求.md
?? kingbattle/ProjectSettings/SceneTemplateSettings.json
```

说明：

- `要求.md` 被删除需要单独确认，不应在本次压缩中顺手提交。
- `SceneTemplateSettings.json` 仍默认不提交。

## 最近关键提交

```text
692c4a7 refactor: add ExpansionCandidate data model and GetExpansionCandidates query
c995889 docs: record mvp-03.18 task push
e8d5d5a docs: review mvp-03.17 and release mvp-03.18
9979941 refactor: extract O expansion orchestration to StrategicExpansionService
cc96991 docs: compress worklog
a3bdb55 docs: record mvp-03.17 task push
914a3ae docs: review mvp-03.16 and release mvp-03.17
d76e602 feat: O shortcut dispatches from frontier plot to adjacent neutral
2ffb481 docs: record mvp-03.16 task push
3a30f68 docs: review mvp-03.15 and release mvp-03.16
966a408 feat: StrategicConnectionService + I shortcut for frontier neutral plots
d29b28b docs: review mvp-03.14 and release mvp-03.15
712cd39 refactor: extract dispatch + capture handler to StrategicDispatchService
ab78ee4 fix: uCaptureHandlers dict prevents stale handler on re-press U
```

## 当前测试快捷键

- `K`：击败 EnemyBase。
- `L`：击败 PlayerBase。
- `R`：重建第一个普通废墟，跳过大本营废墟。
- `T`：聚兵到第一个 main-base ruin。
- `Y`：打印 main-base ruin 可连接 Neutral。
- `U`：从 main-base ruin 派兵到第一个可连接 Neutral。
- `I`：打印 Player-owned frontier 可连接 Neutral。
- `O`：从第一个 Player-owned frontier 派兵到第一个相邻 Neutral。

## 当前验证主链路

```text
Play
K
Y
T
U
等待 Crossroads 变 Player
I
O
等待目标 Neutral 变 Player
再次 I / O 验证继续扩张
R
L
确认 Console 无明显错误
```

## 下一步

1. Claude 执行 MVP-03.19。
2. 检查 `要求.md` 删除是否合理。
3. 继续保持 `SceneTemplateSettings.json` 未提交，除非用户明确要求处理。
4. Claude 完成后由 Codex review，再决定是否进入 MVP-03.20。

## 最近上传记录

```text
e8d5d5a docs: review mvp-03.17 and release mvp-03.18
```

推送结果：

```text
To https://github.com/12342023/unity.git
   9979941..e8d5d5a  main -> main
```

### MVP-03.18 连地网络规则整理

操作人：Claude

已完成目标：新增 ExpansionCandidate 数据模型 + GetExpansionCandidates 查询。

修改 2 个文件：
- `Assets/Scripts/Combat/StrategicConnectionService.cs` — 新增 ExpansionCandidate + GetExpansionCandidates
- `Assets/Scripts/Combat/StrategicExpansionService.cs` — 改用 candidates[0]，补充结构化字段

### MVP-03.19 占领需求数据层

操作人：Claude

新增 1 个文件：
- `Assets/Scripts/Combat/PlotCaptureRequirementService.cs` — 占领需求查询服务
  - `GetRequiredSoldierCount(PlotData)` — Small=1, Medium=2, Large=3, null=0
  - `GetRequiredSoldierCount(PlotSize)` — 同上

修改 1 个文件：
- `Assets/Scripts/GameEntry.cs` — 新增 P 快捷键打印所有 plot 的 capture requirement

Play Mode 验证：
1. Play → 按 **P** → Console 显示每个 plot 的 size 与需求兵力
2. O / I / U / Y / T / R / K / L 行为不变
3. Console 无错误

场景文件和 ProjectSettings：均未修改

手动 git 推送：
```sh
cd /Users/jianghao/unity
git add kingbattle/Assets/Scripts/Combat/PlotCaptureRequirementService.cs \
        kingbattle/Assets/Scripts/Combat/PlotCaptureRequirementService.cs.meta \
        kingbattle/Assets/Scripts/GameEntry.cs \
        WORKLOG.md TASK.md
git commit -m "feat: PlotCaptureRequirementService + P shortcut prints capture requirements"
git push origin main
```

修改 2 个文件：
- `Assets/Scripts/Combat/StrategicConnectionService.cs` — 新增：
  - `ExpansionCandidate` 类（sourcePlotId, targetPlotId）
  - `GetExpansionCandidates(MapData)` — 返回所有候选
  - 规则：source 必须 Player-owned、非 main base；target 必须相邻 Neutral
- `Assets/Scripts/Combat/StrategicExpansionService.cs` — 改为使用 candidates[0]
  - `ExpansionResult` 补充 sourcePlotId, targetPlotId, dispatchedCount

O/I/K/L/R/T/Y/U 行为不变。场景文件和 ProjectSettings 均未修改。

手动 git 推送：
```sh
cd /Users/jianghao/unity
git add kingbattle/Assets/Scripts/Combat/StrategicConnectionService.cs \
        kingbattle/Assets/Scripts/Combat/StrategicExpansionService.cs \
        WORKLOG.md TASK.md
git commit -m "refactor: add ExpansionCandidate data model and GetExpansionCandidates query"
git push origin main
```

### MVP-03.18 Codex Review：通过并发布 MVP-03.19

操作人：Codex

审查提交：

```text
692c4a7 refactor: add ExpansionCandidate data model and GetExpansionCandidates query
```

结论：MVP-03.18 代码审查通过。

新发布任务：MVP-03.19 占领需求数据层。

GitHub 上传状态：

```text
612e932 docs: review mvp-03.18 and release mvp-03.19
```

推送结果：

```text
To https://github.com/12342023/unity.git
   692c4a7..612e932  main -> main
```
