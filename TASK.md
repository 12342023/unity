# TASK.md

## 当前任务

发布 MVP-03.14：派兵边界整理。

Codex 当前仍作为 Tech Lead / Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
ab78ee4 fix: uCaptureHandlers dict prevents stale handler on re-press U
```

Codex Review 结论：

```text
MVP-03.13 代码审查通过；允许进入 MVP-03.14
```

## 本轮目标

只做代码边界整理，不改变玩法。

目标：

```text
把 U 临时派兵、capture handler 管理、到达后占领触发从 GameEntry 下沉到小服务。
```

## 当前工作区注意事项

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件仍是未跟踪 Unity Editor 生成文件。除非用户明确批准，否则不要提交。

## 已完成基础能力

- Main base ruin 不能通过 R 重建。
- Main base ruin 可以作为 T 聚兵点。
- Y 可以查询 main base ruin 相邻 Neutral 地点。
- U 可以从 main base ruin 派兵去第一个相邻 Neutral plot。
- U 到达 Crossroads 后可将 Crossroads 从 Neutral 改为 Player。
- Crossroads 颜色会刷新为 Player 颜色。
- Crossroads 变 Player 后，再次 Y / U 不再把 Crossroads 当作 Neutral。

## MVP-03.14 允许范围

- 可以新增小服务类，例如：

```text
kingbattle/Assets/Scripts/Combat/StrategicDispatchService.cs
```

- 服务职责可以包括：
  - 管理 U 专用 capture handler 字典。
  - 给单位注册 one-shot capture handler。
  - 注册新 U handler 前移除该单位旧 U handler。
  - handler 触发后移除自身与字典记录。
  - 查找 main base ruin 附近的 Player 存活士兵。
  - 对这些士兵执行 `ClearPushPath()`、`UnitMovement.Stop()`、`SetPushPath(...)`。
  - 到达后调用 `PlotCaptureService.TryCapture(...)`。
- `GameEntry` 仍保留 K / L / R / T / Y / U 快捷键入口。
- `GameEntry` 的 U 分支应尽量只做：
  - 找 main base ruin。
  - 取第一个 connectable Neutral plot。
  - 用 `RoadPathFinder.FindPath(...)` 算路径。
  - 调用服务执行派兵。
- 新增 `.cs` 文件必须提交 `.meta`。
- 更新 `WORKLOG.md`。

## 禁止范围

- 不改变 K / L / R / T / Y / U 行为。
- 不做正式派兵 UI。
- 不做占领进度条。
- 不做敌方反夺。
- 不做资源、升级、区域奖励、传送阵、AI。
- 不改变 `MapData.CreateFixedMap()`。
- 不重构 `UnitCombat`。
- 不修改 `ProjectSettings`。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。

## 验收标准

- 新增 `StrategicDispatchService.cs` 或等价小服务。
- 新增脚本的 `.meta` 已提交。
- `GameEntry` 中 U 分支明显变薄。
- Play Mode：K -> T -> Y -> U。
- U 到达 Crossroads 后：
  - Crossroads 从 Neutral 变 Player。
  - Crossroads 颜色刷新为 Player 颜色。
  - Console 有清晰占领日志。
- 到达前重复按 U：
  - 不应留下旧 capture handler。
  - 到达后只触发当前 U 的 capture handler。
- Crossroads 被占领后再次 Y / U：
  - Y / U 不应再把 Crossroads 当作 Neutral。
  - U 应输出 no connectable neutral plots 或同等日志。
  - 不应再次派兵去 Crossroads。
- R 仍跳过大本营废墟。
- K / L 清场行为不变。
- Console 无明显错误。
- 仍未修改或提交 `ProjectSettings`。
