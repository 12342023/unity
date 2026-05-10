# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
6c50000 feat: victory/defeat end panel, match-end command rejection, N restart
```

结论：**MVP-04.3 通过，允许进入 MVP-04.4**。

说明：胜负结束面板、match ended 后 command 拒绝、N/Restart 重启、回归清单均已完成。未发现阻塞问题。

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞问题。
```

### 已确认

- `StrategicExpansionCommandService.DispatchCandidate(...)` 在 match ended 后拒绝执行。
- `EnemyAttackCommandService.DispatchAttack(...)` 和 `DispatchAttackToBestTarget(...)` 在 match ended 后拒绝执行。
- Victory / Defeat 后 HUD 切换到结束面板，不再显示 Dispatch 按钮。
- O / E 会自然走 command/controller 的拒绝路径，不再真正派兵。
- N 快捷键和 Restart 按钮提供重启路径。
- `NEXT_STEPS.md` 和 `WORKLOG.md` 已包含 Play Mode 回归清单。
- `git show --check HEAD` 未发现 whitespace 或 patch 问题。
- `kingbattle/ProjectSettings/SceneTemplateSettings.json` 仍未提交。

### 残余风险

- Restart 依赖当前 scene name reload。如果当前场景未配置为可加载，Unity 可能需要后续单独处理；当前实现已有空 scene name 日志提示。
- HUD 仍是临时 `OnGUI`，适合当前阶段，但不是最终 UI。
- 数值还没有系统调优，当前只是能玩。

### 当前完成度判断

- 技术底座：约 85%。
- 核心玩法闭环：约 80%。
- 完整游戏体验：约 68%-72%。

下一步应做 MVP-04.4：数值调优和回归清单，把“能跑通”推进到“能稳定试玩”。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-04.3 通过。

进入 MVP-04.4：数值调优和回归清单。

项目目标：
- 四周内先完成完整 Unity 小游戏。
- 后续仍可能做微信小程序、macOS、Android 移植。
- 本轮是打磨/稳定轮，不是新系统开发轮。

本轮目标：
- 梳理当前所有关键数值。
- 执行 Play Mode 回归清单。
- 只做必要的小范围数值调整。
- 记录调优依据，方便后续继续打磨。

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
- 记录通过/失败项到 WORKLOG.md。
- 如果发现明显 bug，优先修 bug，而不是继续加功能。

任务 D：必要小范围调优
- 如果 Play Mode 观察到明显问题，可以小范围调整：
  - 敌方进攻过早/过晚。
  - 产兵过快/过慢。
  - supply cap 太高/太低。
  - Tower 过强/过弱。
- 每个调整数值都要在 WORKLOG.md 说明原因。

任务 E：交付风险清单
- 在 NEXT_STEPS.md 或 WORKLOG.md 增加“交付前剩余风险”：
  - 正式 UI 还没做。
  - debug 快捷键还没隐藏。
  - 移植还没开始。
  - ProjectSettings / 生成文件不能提交。

验证：
- Play Mode 回归清单至少跑一遍。
- Console 无明显错误。
- 如新增 `.meta`，随代码提交。
- 不修改 ProjectSettings。

禁止：
- 不做新玩法系统。
- 不做正式 UI 美术。
- 不做存档。
- 不引入第三方框架。
- 不做移动端/微信/macOS/Android 移植实现。
- 不修改 ProjectSettings。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `.claude/`、`kingbattle/.idea/`。

完成后 commit / push。
```
