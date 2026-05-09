# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
774a48e refactor: extract BuildingFactory from GameEntry for rebuild reuse
```

结论：**MVP-03.6 代码审查通过**。

说明：本轮只把建筑创建细节从 `GameEntry` 下沉到 `Buildings/BuildingFactory.cs`，没有引入 UI、资源、占领、实际重建或平台相关逻辑，符合任务范围。

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞问题。
```

### 已确认

- `kingbattle/Assets/Scripts/Buildings/BuildingFactory.cs` 已新增。
- `kingbattle/Assets/Scripts/Buildings/BuildingFactory.cs.meta` 已提交。
- `GameEntry.CreateBuilding()` 现在只委托 `BuildingFactory.CreateBuilding(...)`。
- `GameEntry` 仍负责测试场景里创建哪些建筑、设置 rally / push target、装配 `FactionDefeatHandler`、保留 K / L 测试快捷键。
- 建筑位置、颜色、缩放、血量、`TowerAttack` / `BarracksSpawner` 装配逻辑与原实现一致。
- 未提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。

## 残留注意事项

### ProjectSettings 新增文件仍未处理

当前工作树仍有未跟踪文件：

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件由 Unity Editor 生成。由于项目规则要求不随意修改 `ProjectSettings`，继续保持未提交状态。

### 下一步不要直接做完整重建 UI

`BuildingFactory` 已经可以复用创建建筑，但当前 `FactionDefeatHandler` 仍依赖初始建筑列表。未来如果重建出新建筑，而清场逻辑不知道这些新建筑，就会出现“新建筑不参与阵营清场”的架构问题。

因此下一步先补一个小的建筑运行时注册边界，再做真正重建。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.6 代码审查通过。

进入 MVP-03.7：建筑运行时注册表，为后续重建接入清场逻辑做准备。

目标：
- 不做实际重建。
- 不做 UI。
- 不做资源、占领、升级、连地或 AI。
- 只新增一个很小的 BuildingRegistry 或同等组件，让运行时建筑可以按阵营被登记和查询。

允许：
- 新增 Buildings/BuildingRegistry.cs 或同等小类。
- GameEntry 在创建初始建筑后，把 Player / Enemy 建筑注册进去。
- FactionDefeatHandler 可以从 BuildingRegistry 查询当前阵营建筑，而不是只依赖一次性的初始 List。
- 查询时要过滤 null / 已死亡建筑，避免重复生成废墟。
- 当前初始建筑清场行为必须保持不变。
- 新增脚本必须提交 .meta。

必须保持：
- 当前场景建筑位置、颜色、大小、血量和行为不变。
- Barracks 正常出兵，Tower 正常攻击单位。
- 建筑死亡后仍生成废墟。
- 按 K 击败 EnemyBase 后：敌方所有建筑变废墟，敌兵立即死亡，己方士兵存活并可围绕废墟巡逻。
- 按 L 击败 PlayerBase 后：蓝方建筑变废墟，蓝方士兵立即死亡。
- Console 无明显错误。

禁止：
- 不实现重建按钮。
- 不真正把废墟重建成新建筑。
- 不做选择废墟、资源消耗、进度条、占领、升级、连地、区域奖励、传送阵、AI 或 UI。
- 不修改 ProjectSettings。
- 不提交 kingbattle/ProjectSettings/SceneTemplateSettings.json。
- 不提交 Library、Logs、UserSettings。

完成后更新 WORKLOG.md，说明：
- 修改文件
- BuildingRegistry 的职责边界
- K / L Play Mode 验证步骤
- 是否修改场景 / ProjectSettings
- commit / push 结果
```
