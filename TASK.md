# TASK.md

## 当前任务

MVP-03.10 小修：修复 R 测试快捷键不能完成的问题。

Codex 当前仍作为 Tech Lead / Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 问题来源

用户反馈：

```text
现在出现错误了，R键不能完成
```

Codex 检查结论：

- `BuildingRebuildService` 拒绝重建 main base ruin 是正确规则。
- 但 `GameEntry` 的 R 键只尝试 `ruins[0]`。
- 当第一个废墟是 EnemyBase / PlayerBase 大本营废墟时，R 会直接失败，不会继续尝试后面的普通废墟。

## 当前工作区注意事项

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件仍是未跟踪 Unity Editor 生成文件。除非用户明确批准，否则不要提交。

## 允许范围

- 修改 `kingbattle/Assets/Scripts/GameEntry.cs` 的 R 测试快捷键逻辑。
- R 键遍历所有 `RuinComponent`。
- 跳过 `ruin.IsMainBaseRuin(mapData) == true`。
- 对第一个普通可重建废墟调用 `BuildingRebuildService.Rebuild(...)`。
- 成功后停止遍历并输出成功日志。
- 没有普通可重建废墟时输出清晰日志。
- 保持 T 聚兵逻辑不变。
- 更新 `WORKLOG.md`。

## 禁止范围

- 不允许让大本营废墟被 R 重建。
- 不做正式按钮或 UI。
- 不做连接未占领地正式系统。
- 不做正式派兵系统。
- 不做资源、占领进度、升级、区域奖励、传送阵、AI。
- 不修改 `ProjectSettings`。
- 不提交 `kingbattle/ProjectSettings/SceneTemplateSettings.json`。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。

## 验收标准

- 按 K 后出现 EnemyBase + EnemyOutpost 废墟。
- 按 R 时，EnemyBase 废墟被跳过，不被销毁、不被重建。
- EnemyOutpost 等普通废墟可以被 R 重建。
- 如果只剩大本营废墟，R 输出 no rebuildable ruins，而不是反复 rebuild failed。
- T 聚兵功能保持可用。
- K / L 清场行为不变。
- Console 无明显错误。
