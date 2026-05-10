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

- 用户希望后续每轮多布置一些任务，加快进度；从本轮开始默认打包 2-3 个强相关小任务。
- 已完成：
  - 新增 `PlotCaptureRequirementService`。
  - Small / Medium / Large 返回 1 / 2 / 3。
  - P 快捷键打印各 plot 占领需求。
  - `ExpansionResult` / O 日志携带 dispatched vs required 预览。
  - 当前实际 capture 判定未改变。
- Codex review 通过，并发布 MVP-03.20。

### MVP-03.20 占领需求接入

- 已完成并通过：
  - U/O 传入 target plot required count。
  - `StrategicDispatchService` 用最终 `totalDispatched` 判定 capture requirement。
  - dispatched 不足时 blocked。
  - dispatched 足够时可占领。
  - handler self-cleanup 和 `captureConsidered` 一次性语义保留。
- Codex review 通过，并发布 MVP-03.21。

### MVP-03.21 占领反馈与结果状态

- 当前发布任务：为 U/O 派兵和占领结果补结构化状态，统一成功/失败日志。
- 批量目标：
  - `StrategicDispatchService` 增加 `DispatchResult` 或等价数据。
  - O/U 使用结构化 dispatch result。
  - 到达后成功/失败日志统一。
  - P/O/U 日志继续可验证需求和派兵数量。

GitHub 上传状态：

```text
09bb4dc docs: block mvp-03.20 capture count bug
```

推送结果：

```text
To https://github.com/12342023/unity.git
   1959d56..09bb4dc  main -> main
```

## 当前待处理状态

截至 MVP-03.21 任务发布前，工作区仍存在以下未提交/未跟踪变更：

```text
D  要求.md
?? kingbattle/ProjectSettings/SceneTemplateSettings.json
```

说明：

- `要求.md` 被删除需要单独确认，不应在本次压缩中顺手提交。
- `SceneTemplateSettings.json` 仍默认不提交。

## 最近关键提交

```text
681abb4 fix: use shared totalDispatched instead of per-iteration capturedCount
2c58c94 docs: record mvp-03.20 blocker push
09bb4dc docs: block mvp-03.20 capture count bug
1959d56 feat: capture requires minimum soldier count, U/O both enforce it
569c321 feat: show dispatched versus required capture count
404b86c feat: PlotCaptureRequirementService + P shortcut prints capture requirements
6002750 docs: record expanded mvp-03.19 batch
3fc6cb5 docs: expand mvp-03.19 task batch
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

1. Claude 执行 MVP-03.21。
2. 检查 `要求.md` 删除是否合理。
3. 继续保持 `SceneTemplateSettings.json` 未提交，除非用户明确要求处理。
4. Claude 完成后由 Codex review，再决定是否进入 MVP-03.22。

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

### MVP-03.19 任务 C：O 日志携带 dispatched vs required 预览

修改 1 个文件：
- `Assets/Scripts/Combat/StrategicExpansionService.cs`：
  - `ExpansionResult` 新增 `requiredCount` 字段
  - `ExpandNext()` 中查询 `PlotCaptureRequirementService.GetRequiredSoldierCount(targetPlot)`
  - O 日志从 `"Dispatched 3 soldiers from X to Y"` 变为 `"Dispatched 3/2 soldiers from X to Y"`
  - 不改变实际 capture 判定（到达即占领）

Play Mode 验证：
1. K → T → U(占 Crossroads) → 按 **O** → 日志显示 `Dispatched N/2 soldiers from Crossroads to Village`
2. 占领行为不变，到达即占领

### MVP-03.20 占领需求接入批量任务

操作人：Claude

**任务 A — DispatchToPlot 支持 capture requirement**
修改 `Assets/Scripts/Combat/StrategicDispatchService.cs`：
- `DispatchToPlot()` 新增 `int requiredSoldierCount = 1` 参数
- arrival capture handler 中 `capturedCount + 1 >= requiredSoldierCount` 时才调用 TryCapture
- 不足时 log "Capture blocked: dispatched N/M to X"
- handler self-cleanup 不变

**任务 B — O 接入需求判定**
修改 `Assets/Scripts/Combat/StrategicExpansionService.cs`：
- `ExpandNext()` 将 `required` 传给 `DispatchToPlot`
- `ExpansionResult` 新增 `hasEnoughDispatchedSoldiers`（computed）
- O 日志显示 `dispatched N/M (enough=True/False)`

**任务 C — U 接入需求判定**
修改 `Assets/Scripts/GameEntry.cs`：
- U 分支读取 `PlotCaptureRequirementService.GetRequiredSoldierCount(targetPlot)`
- 传给 `DispatchToPlot(..., required)`
- U 日志显示 `dispatched N/M`

**任务 D — 验证日志**
所有入口保持，P/I/U/O/Y/T/R/K/L 行为不变。

Play Mode 验证：
1. Play → T → 聚兵到 EnemyBase（如果不足 2 个蓝兵）
2. U → 派兵到 Crossroads（需 2）→ 日志显示 `dispatched 1/2` → 到达后 `Capture blocked`
3. 等更多蓝兵聚集 → 再次 U → dispatched 3/2 → 到达后占领成功，Crossroads 变蓝
4. O → 从 Crossroads 派兵到 Village（需 1）→ dispatched 1/1 → 到达后占领
5. P → 打印各 plot 需求不变
6. Console 无错误

修改文件（3 个）：
- `Assets/Scripts/Combat/StrategicDispatchService.cs` — requiredSoldierCount + capture gate
- `Assets/Scripts/Combat/StrategicExpansionService.cs` — 传递 required + hasEnoughDispatchedSoldiers
- `Assets/Scripts/GameEntry.cs` — U 传递 required

场景文件和 ProjectSettings：均未修改

手动 git 推送：
```sh
cd /Users/jianghao/unity
git add kingbattle/Assets/Scripts/Combat/StrategicDispatchService.cs \
        kingbattle/Assets/Scripts/Combat/StrategicExpansionService.cs \
        kingbattle/Assets/Scripts/GameEntry.cs \
        WORKLOG.md TASK.md
git commit -m "feat: capture requires minimum soldier count, U/O both enforce it"
git push origin main
```

### MVP-03.20 阻塞修复：capturedCount + 1 → 共享 totalDispatched

操作人：Claude

Codex Review 指出的问题已修复。

**问题：** handler 使用 `capturedCount + 1`（创建 handler 时的迭代序号），不是最终总派兵数。dispatched 3/2 时第一名到达士兵的 `capturedCount + 1 = 1 < 2`，错误 blocked。

**修复：** `DispatchToPlot()` 中 handler 改用共享变量 `totalDispatched`。所有 lambda 捕获同一个 C# 局部变量引用，handler 触发时读到的是循环结束后的最终值。

```csharp
int totalDispatched = 0;
foreach (...)
{
    totalDispatched++;          // 递增在 handler 创建之前
    // handler 捕获 totalDispatched（by reference，不是 by value）
    localHandler = () => {
        if (totalDispatched >= requiredSoldierCount)   // ← 读到最终总数
            TryCapture(...);
    };
}
```

修改文件：仅 `Assets/Scripts/Combat/StrategicDispatchService.cs`

验证：
- dispatched 1/2 → handler 触发时 `totalDispatched=1 < 2` → blocked ✅
- dispatched 3/2 → handler 触发时 `totalDispatched=3 >= 2` → capture ✅

手动 git 推送：
```sh
cd /Users/jianghao/unity
git add kingbattle/Assets/Scripts/Combat/StrategicDispatchService.cs WORKLOG.md TASK.md
git commit -m "fix: use shared totalDispatched instead of per-iteration capturedCount"
git push origin main
```

### MVP-03.21 占领反馈与结果状态批量任务

操作人：Claude

**任务 A — DispatchResult 数据类型**
新增 `DispatchResult` 类（在 `StrategicDispatchService.cs` 顶部）：
- targetPlotId, dispatchedCount, requiredSoldierCount, hasEnoughSoldiers (computed), captureWillBeAttemptedOnArrival (computed), message
- `DispatchToPlot()` 返回 `DispatchResult` 替代 `int`

**任务 B — O/U 使用 DispatchResult**
- `StrategicExpansionService.ExpandNext()` 使用 `dispatchResult.message` 填充 `ExpansionResult.message`
-  GameEntry U 分支使用 `dispatchResult.message` 打印统一格式日志
-  O/U 日志一致：`dispatch 3/2 to Crossroads, willCapture=True`

**任务 C — 统一到达日志**
- capture handler 到达后允许时：`"Capture attempt allowed: dispatched 3/2 to Crossroads."`
- capture handler 到达后被 blocked：`"Capture blocked: dispatched 1/2 to Crossroads."`

**任务 D — 验证**
P/U/O 行为不变。不足人数 blocked，足够人数占领。

修改文件（3 个）：
- `Assets/Scripts/Combat/StrategicDispatchService.cs` — DispatchResult 类 + 返回类型改为 DispatchResult + 统一日志
- `Assets/Scripts/Combat/StrategicExpansionService.cs` — 使用 DispatchResult
- `Assets/Scripts/GameEntry.cs` — U 使用 DispatchResult 日志

场景文件和 ProjectSettings：均未修改

手动 git 推送：
```sh
cd /Users/jianghao/unity
git add kingbattle/Assets/Scripts/Combat/StrategicDispatchService.cs \
        kingbattle/Assets/Scripts/Combat/StrategicExpansionService.cs \
        kingbattle/Assets/Scripts/GameEntry.cs \
        WORKLOG.md TASK.md
git commit -m "feat: DispatchResult structured data, unified capture logs"
git push origin main
```

### MVP-03.22 扩张预览与可派兵统计批量任务

操作人：Claude

已完成 5 项要求：

**1. 只读可派兵统计**
`StrategicConnectionService` 中新增 `CountSoldiersNear(Vector3, float)` — 统计某个位置附近空闲 Player 士兵数量

**2. ExpansionPreview / GetExpansionPreviews**
- `ExpansionPreview` 类：sourcePlotId, targetPlotId, requiredCount, availableCount, hasEnough (computed)
- `GetExpansionPreviews(MapData, gatherRadius=5)` — 每个 candidate 附带实时可派兵数

**3. Q 快捷键**
GameEntry 新增 Q 键，打印全部扩张候选预览：
```
Crossroads → Village: avail=3, req=1, enough=True
Crossroads → Farmland: avail=3, req=1, enough=True
```

**4. 修正过期注释**
`StrategicExpansionService.cs` 中 `"preview only — does not affect capture logic"` → `"used to gate DispatchToPlot"`

**5. WORKLOG 已更新**

修改文件（3 个）：
- `Assets/Scripts/Combat/StrategicConnectionService.cs` — ExpansionPreview + CountSoldiersNear + GetExpansionPreviews
- `Assets/Scripts/Combat/StrategicExpansionService.cs` — 修正注释
- `Assets/Scripts/GameEntry.cs` — 新增 Q 快捷键

Play Mode 验证：
1. **P** — plot 需求打印不变
2. **Q** — 显示每个扩张候选的 available / required / enough
3. **U** — dispatched N/M 日志，不足 blocked / 足够占领
4. **O** — 同上，从 frontier plot 派兵
5. Console 无错误

场景文件和 ProjectSettings：均未修改

手动 git 推送：
```sh
cd /Users/jianghao/unity
git add kingbattle/Assets/Scripts/Combat/StrategicConnectionService.cs \
        kingbattle/Assets/Scripts/Combat/StrategicExpansionService.cs \
        kingbattle/Assets/Scripts/GameEntry.cs \
        WORKLOG.md TASK.md
git commit -m "feat: expansion previews with live soldier count, Q shortcut"
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

### MVP-03.19 任务批量加速调整

操作人：Codex

用户要求：后续每轮多布置一些任务，加快进度。

调整：

- 从本轮开始默认给 Claude 布置 2-3 个强相关小任务。
- MVP-03.19 扩展为：
  - 占领需求服务。
  - P 快捷键日志验证。
  - O / `ExpansionResult` 携带 dispatched vs required 预览数据。
- 仍不改变当前实际 capture 判定。

GitHub 上传状态：

```text
3fc6cb5 docs: expand mvp-03.19 task batch
```

推送结果：

```text
To https://github.com/12342023/unity.git
   404b86c..3fc6cb5  main -> main
```

### MVP-03.19 Codex Review：通过并发布 MVP-03.20

操作人：Codex

审查提交：

```text
404b86c feat: PlotCaptureRequirementService + P shortcut prints capture requirements
569c321 feat: show dispatched versus required capture count
```

结论：MVP-03.19 代码审查通过。

新发布任务：MVP-03.20 占领需求接入批量任务。

GitHub 上传状态：

```text
b4ca674 docs: review mvp-03.19 and release mvp-03.20
```

推送结果：

```text
To https://github.com/12342023/unity.git
   569c321..b4ca674  main -> main
```

### MVP-03.20 Codex Review：通过并发布 MVP-03.21

操作人：Codex

审查提交：

```text
681abb4 fix: use shared totalDispatched instead of per-iteration capturedCount
```

结论：MVP-03.20 修复通过。

新发布任务：MVP-03.21 占领反馈与结果状态批量任务。

GitHub 上传状态：

```text
501e2e1 docs: review mvp-03.20 and release mvp-03.21
```

推送结果：

```text
To https://github.com/12342023/unity.git
   681abb4..501e2e1  main -> main
```

### MVP-03.21 Claude 实现审查：通过并发布 MVP-03.22

操作人：Codex

审查提交：

```text
ac2af48 feat: DispatchResult structured data, unified capture logs
```

结论：MVP-03.21 通过。

已确认：

- `StrategicDispatchService.DispatchToPlot(...)` 返回 `DispatchResult`。
- U/O 日志已统一为 dispatched / required / willCapture。
- 到达后的 allowed / blocked 日志已统一。
- 未提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。

新发布任务：MVP-03.22 扩张预览与可派兵统计批量任务。

任务范围：

- 增加只读可派兵统计。
- 增加 `ExpansionPreview` / `GetExpansionPreviews`。
- 新增 Q 快捷键打印全部候选预览。
- 修正 `StrategicExpansionService` 过期注释。

GitHub 上传状态：

```text
bfc6271 docs: review mvp-03.21 and release mvp-03.22
```

推送结果：

```text
To https://github.com/12342023/unity.git
   ac2af48..bfc6271  main -> main
```
