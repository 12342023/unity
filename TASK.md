# TASK.md

## 当前任务

Unity 6 编译返修：修复 `GameStatusService.cs.meta` 无效 GUID，并排查 ShaderGraph package 错误。

Codex 当前仍作为 Tech Lead 和 Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 当前 Unity 环境

```text
Unity 6000.4.6f1
```

Codex 已读取：

```text
/Users/jianghao/Library/Logs/Unity/Editor.log
```

## 阻塞错误 A：GameStatusService meta 无效

Unity 日志：

```text
The GUID inside 'Assets/Scripts/Combat/GameStatusService.cs.meta' cannot be extracted by the YAML Parser.
The .meta file Assets/Scripts/Combat/GameStatusService.cs.meta does not have a valid GUID and its corresponding Asset file will be ignored.
```

当前文件：

```text
Assets/Scripts/Combat/GameStatusService.cs.meta
guid: bcdef23456789012345678901234567890
```

问题：

- Unity `.meta` GUID 必须是 32 位十六进制。
- 当前 GUID 长度为 34。
- Unity 忽略 `GameStatusService.cs` 后，所有使用 `GameStatusService` 的脚本都会报错。

连锁错误包括：

```text
PlayerInputController.cs: GameStatusService does not exist
StrategicExpansionService.cs: GameStatusService does not exist
FactionDefeatHandler.cs: GameStatusService does not exist
StrategicExpansionCommandService.cs: GameStatusService does not exist
EnemyAttackCommandService.cs: GameStatusService does not exist
EnemyPressureController.cs: GameStatusService does not exist
DebugShortcutController.cs: GameStatusService does not exist
GameEntry.cs: GameStatusService does not exist
GameHud.cs: GameStatusService does not exist
StrategicDispatchService.cs: GameStatusService does not exist
```

## 阻塞错误 B：ShaderGraph package `GUID` 类型缺失

Unity 日志：

```text
Library/PackageCache/com.unity.shadergraph@04a6ef07359e/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Targets/BuiltInCanvasSubTarget.cs(10,25): error CS0246: The type or namespace name 'GUID' could not be found
Library/PackageCache/com.unity.shadergraph@04a6ef07359e/Editor/Generation/Contexts/TargetSetupContext.cs(62,40): error CS0246: The type or namespace name 'GUID' could not be found
```

判断：

- 这是 package cache/package version 问题，不是业务代码。
- 先修 meta，再看这个错误是否仍存在。

## 允许范围

任务 A：修复 meta

- 修复 `Assets/Scripts/Combat/GameStatusService.cs.meta`。
- 将 guid 改成唯一、32 位、纯十六进制字符串。
- 不修改 `GameStatusService.cs` 业务逻辑。
- 不删除 `GameStatusService.cs`。

任务 B：重新编译

- 让 Unity 重新导入/编译。
- 确认 `GameStatusService does not exist` 全部消失。

任务 C：处理 ShaderGraph package

- 如果修完 meta 后 ShaderGraph `GUID could not be found` 仍存在：
  - 不要改 `Library/PackageCache` 并提交。
  - 优先关闭 Unity 后让 package cache 重建。
  - 或通过 Package Manager 重新解析/更新 URP/ShaderGraph 到 Unity 6000.4.6f1 兼容版本。
  - 如果必须改 `Packages/manifest.json` / `Packages/packages-lock.json`，需要在 `WORKLOG.md` 写清楚原因。

任务 D：记录 Unity 6 warnings

- 记录但本轮不大范围修：
  - `Physics2D.OverlapCircleNonAlloc` obsolete。
  - `FindObjectsByType<T>(FindObjectsSortMode)` obsolete。
  - `FindFirstObjectByType<T>()` obsolete。

## 禁止范围

- 不做新玩法系统。
- 不做 UI 美术。
- 不重构战斗/占领系统。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。
- 不提交 `.claude/`、`kingbattle/.idea/`。
- 不提交 `ProjectSettings`，除非 Codex/用户明确确认 Unity 6 迁移文件可以入仓。
- 不手改并提交 `Library/PackageCache`。

## 验收标准

- Unity Console 无 P1 编译错误。
- `GameStatusService.cs` 能被 Unity 正常导入。
- `GameStatusService does not exist` 连锁错误消失。
- 如果 ShaderGraph package 错误仍存在，已记录当前状态和下一步。
- Play 能进入场景。
- 点击派兵 / HUD Dispatch / O / debug keys 做 smoke test。
- `WORKLOG.md` 记录修复、验证、剩余 warnings。
