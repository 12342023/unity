# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
31da520 chore: Unity 6 migration — packages, URP, project settings
```

结论：**Unity 6 迁移文件收口通过，允许进入 Unity 6 obsolete warning cleanup。**

说明：提交范围合理，包含 Unity 6 必需的 Packages、URP、Graphics、ShaderGraph、ProjectVersion 和默认 Volume 资源；未提交 `.claude/`、`.idea/`、`kingbattle.slnx`、`SceneTemplateSettings.json`、`MultiplayerManager.asset`、`要求.md`。

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞问题。
```

### 已确认

- `Packages/manifest.json` / `packages-lock.json` 已迁移到 Unity 6 / URP 17 对应包版本。
- `ProjectSettings/ProjectVersion.txt` 已记录 `6000.4.6f1`。
- URP/Graphics/ShaderGraph 相关 asset 属于 Unity 6 自动序列化迁移范围。
- `Assets/DefaultVolumeProfile.asset` 和 `.meta` 是 URP 自动生成的默认 Volume 资源，随 URP 迁移提交合理。
- 最新 Editor log 尾部未发现 P1 编译错误。
- Play 已跑到实际 gameplay / Victory 日志。

### Codex 小修

`git show --check HEAD` 发现 Unity 生成 YAML 里有 6 处行尾空格。Codex 已做纯格式修复，涉及：

- `kingbattle/Assets/DefaultVolumeProfile.asset.meta`
- `kingbattle/Assets/Settings/UniversalRP.asset`
- `kingbattle/Assets/UniversalRenderPipelineGlobalSettings.asset`

修复后 `git diff --check` 通过。

### 当前仍未提交且应继续排除

```text
要求.md deletion
.claude/
kingbattle/.idea/
kingbattle/ProjectSettings/MultiplayerManager.asset
kingbattle/ProjectSettings/SceneTemplateSettings.json
kingbattle/kingbattle.slnx
```

这些不属于本轮需要入仓的 Unity 6 migration 文件。

### 下一步

进入 **MVP-04.6：Unity 6 obsolete warning cleanup**。

目标：只清理 Unity 6 API 过时警告，不做玩法改动。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：Unity 6 迁移文件收口通过。

进入 MVP-04.6：Unity 6 obsolete warning cleanup。

本轮只清理 Unity 6 API obsolete warnings，不做新玩法，不重构业务系统。

需要处理：
1. Physics2D.OverlapCircleNonAlloc obsolete
   - 文件：Assets/Scripts/Buildings/TowerAttack.cs
   - 改成 Unity 6 推荐的 Physics2D.OverlapCircle / 等价新 API。
   - 保持 Tower 行为不变：扫描敌方 UnitCombat，按 interval 攻击。

2. FindObjectsByType<T>(FindObjectsSortMode.None) obsolete
   - 涉及：
     - DebugShortcutController.cs
     - StrategicConnectionService.cs
     - StrategicDispatchService.cs
     - FactionStatsService.cs
     - EnemyAttackCommandService.cs
     - FactionDefeatHandler.cs
   - 改成 Unity 6 推荐 overload，例如 FindObjectsByType<T>(FindObjectsInactive.Exclude) 或项目中最合适的新 API。
   - 不改变查询语义：仍只找当前 active scene 中的运行时对象。

3. FindFirstObjectByType<T>() obsolete
   - 文件：GameHud.cs
   - 改成 FindAnyObjectByType<PlayerInputController>() 或更好的引用缓存。
   - 优先小改，避免重构 HUD。

验证：
- Unity Console 无 obsolete warnings 中上述三类。
- Unity Console 无 error CS。
- Play smoke test：
  - 点击 source/target 派兵。
  - HUD Dispatch。
  - O。
  - K/L/E/N。
  - Victory 后不能继续派兵。
- 更新 WORKLOG.md，记录每个 API 替换点和验证结果。

禁止：
- 不做新玩法。
- 不做正式 UI。
- 不改 ProjectSettings。
- 不提交 .claude、.idea、kingbattle.slnx、Library、Logs、UserSettings。
- 不提交 要求.md 删除。

完成后 commit / push。
```
