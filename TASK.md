# TASK.md

## 当前任务

发布 MVP-04.4：数值调优和回归清单。

Codex 当前仍作为 Tech Lead 和 Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
6c50000 feat: victory/defeat end panel, match-end command rejection, N restart
```

Codex Review 结论：

```text
MVP-04.3 通过；允许进入 MVP-04.4。
```

## 四周目标

用户当前四周目标：

```text
四周内先集中做出完整 Unity 小游戏；本阶段不实际开发微信小程序 / macOS / Android 移植版本。
```

长期目标仍包含后续移植。因此本阶段必须保持架构边界清楚，不能为了赶进度把平台、输入、UI 和核心玩法耦合在一起。

当前完成度粗估：

- 技术底座：约 85%。
- 核心玩法闭环：约 80%。
- 完整游戏体验：约 68%-72%。

## MVP-04.4 批量允许范围

本轮目标：

```text
梳理关键数值，执行 Play Mode 回归清单，只做必要的小范围调优，把“能跑通”推进到“能稳定试玩”。
```

任务 A：建立调优文档

- 新增 `BALANCE.md` 或 `GAMEPLAY_TUNING.md`。
- 记录当前关键数值：
  - supply cap base / Granary bonus。
  - Barracks spawn interval / rally threshold。
  - unit health / damage / speed。
  - Tower damage / range / interval。
  - Enemy pressure first attack / repeat interval。
  - Plot capture requirement Small / Medium / Large。
- 写清楚当前目标体验：3-5 分钟能打一局，玩家有扩张和防守压力。

任务 B：轻量配置收口

- 可以新增 `GameBalanceConfig` 或等价静态配置类。
- 只收口最明显的魔法数：
  - supply cap base / Granary bonus。
  - enemy pressure first/repeat interval。
  - 可选：tower damage/range/interval。
- 不要大规模重构所有数值。
- 不要为了配置化改动太多业务代码。

任务 C：执行 Play Mode 回归清单

- 按 `NEXT_STEPS.md` 的 Play Mode 回归清单逐项验证。
- 记录通过/失败项到 `WORKLOG.md`。
- 如果发现明显 bug，优先修 bug，而不是继续加功能。

任务 D：必要小范围调优

- 如果 Play Mode 观察到明显问题，可以小范围调整：
  - 敌方进攻过早/过晚。
  - 产兵过快/过慢。
  - supply cap 太高/太低。
  - Tower 过强/过弱。
- 每个调整数值都要在 `WORKLOG.md` 说明原因。

任务 E：交付风险清单

- 在 `NEXT_STEPS.md` 或 `WORKLOG.md` 增加“交付前剩余风险”：
  - 正式 UI 还没做。
  - debug 快捷键还没隐藏。
  - 移植还没开始。
  - ProjectSettings / 生成文件不能提交。

## 禁止范围

- 不做新玩法系统。
- 不做正式 UI 美术。
- 不做复杂菜单系统。
- 不做存档。
- 不引入第三方框架。
- 不做移动端/微信/macOS/Android 移植实现。
- 不重构 `UnitCombat`。
- 不重构 `PlotCaptureService`。
- 不修改 `ProjectSettings`。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `.claude/` 或 `kingbattle/.idea/`，除非用户明确要求。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。

## 验收标准

- `BALANCE.md` 或 `GAMEPLAY_TUNING.md` 已创建。
- 至少一轮 Play Mode 回归清单已执行并记录。
- 如调整数值，WORKLOG 写清楚原因。
- Console 无明显错误。
- 未修改或提交 `ProjectSettings`。
