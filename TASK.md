# TASK.md

## 当前任务

发布 MVP-03.15：占领后的下一层可连接 Neutral 查询。

Codex 当前仍作为 Tech Lead / Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
712cd39 refactor: extract dispatch + capture handler to StrategicDispatchService
```

Codex Review 结论：

```text
MVP-03.14 代码审查通过；允许进入 MVP-03.15
```

## 本轮目标

只做数据层/临时验证，不做正式 UI 和复杂派兵。

目标：

```text
已占领的 Player plot 能查询相邻 Neutral plot，为后续从 Crossroads 继续扩张做准备。
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
- U 派兵与 capture handler 管理已下沉到 `StrategicDispatchService`。

## MVP-03.15 允许范围

- 可以新增小服务类，例如：

```text
kingbattle/Assets/Scripts/Map/StrategicConnectionService.cs
```

- 服务职责：
  - 给定 `MapData`、`sourcePlotId`、`Faction`。
  - 检查 source plot 是否存在。
  - 检查 source plot 是否属于传入 faction。
  - 只返回相邻 `Faction.Neutral` plotId。
  - 不改变任何 plot 归属。
- 可以在 `GameEntry` 增加临时测试快捷键，例如 `I`。
- `I` 的行为：
  - 找到第一个 Player-owned 且非 main base 的 plot。
  - 打印它可连接的 Neutral 邻居。
  - 不派兵，不占领。
- 如果 Crossroads 还没被占领，I 可以输出 no owned frontier plot。
- 新增 `.cs` 文件必须提交 `.meta`。
- 更新 `WORKLOG.md`。

## 禁止范围

- 不改变 K / L / R / T / Y / U 行为。
- 不做正式派兵 UI。
- 不做多点派兵。
- 不做自动扩张。
- 不做占领进度条。
- 不做敌方反夺。
- 不做资源、升级、区域奖励、传送阵、AI。
- 不改变 `MapData.CreateFixedMap()`。
- 不重构 `UnitCombat`。
- 不修改 `ProjectSettings`。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。

## 验收标准

- 新增 `StrategicConnectionService.cs` 或等价小服务。
- 新增脚本的 `.meta` 已提交。
- Play Mode：
  - K -> T -> Y -> U。
  - U 到达 Crossroads 后，Crossroads 变 Player。
  - 再按 I，Console 能打印 Crossroads 可连接的 Neutral 邻居，例如 Village / Farmland。
- I 不改变任何 plot 归属。
- I 不派兵。
- Y / U 原有行为不变。
- R 仍跳过大本营废墟。
- K / L 清场行为不变。
- Console 无明显错误。
- 仍未修改或提交 `ProjectSettings`。
