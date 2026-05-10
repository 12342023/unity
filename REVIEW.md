# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
6232f7a fix: GameStatusService.cs.meta GUID length 34 -> 32 hex chars
```

结论：**Unity 6 P1 编译阻塞已通过，允许进入 Unity 6 迁移文件收口任务。**

说明：`GameStatusService.cs.meta` 已修成 32 位 GUID；最新 Editor log 尾部没有 `GameStatusService does not exist` 或 ShaderGraph `GUID could not be found` 编译错误，已经进入 Play/战斗/Victory 日志。

## CODEX PROJECT REVIEW

Gate: **PASS**

Findings:

```text
无阻塞编译问题。
```

### 已确认

- `Assets/Scripts/Combat/GameStatusService.cs.meta` 当前 GUID 为：

```text
bcdef234567890123456789012345678
```

- GUID 长度为 32 位十六进制。
- Codex 用脚本扫描 `Assets/Scripts/**/*.meta`，未发现其他非 32 位 GUID。
- `git show --check HEAD` 未发现 whitespace 问题。
- 最新 Editor log 尾部未发现：
  - `GameStatusService does not exist`
  - `does not have a valid GUID`
  - `GUID could not be found`
  - `error CS`
- Editor log 尾部已出现正常 gameplay 日志：Barracks spawn、Enemy attack、FactionDefeat、PlayerVictory、ruin spawned。

### 当前残余风险

工作区仍有 Unity 6 自动改出的未提交文件：

```text
kingbattle/Assets/Settings/Renderer2D.asset
kingbattle/Assets/Settings/UniversalRP.asset
kingbattle/Assets/UniversalRenderPipelineGlobalSettings.asset
kingbattle/Packages/manifest.json
kingbattle/Packages/packages-lock.json
kingbattle/ProjectSettings/GraphicsSettings.asset
kingbattle/ProjectSettings/ProjectVersion.txt
kingbattle/ProjectSettings/ShaderGraphSettings.asset
kingbattle/ProjectSettings/URPProjectSettings.asset
kingbattle/Assets/DefaultVolumeProfile.asset
kingbattle/Assets/DefaultVolumeProfile.asset.meta
kingbattle/ProjectSettings/MultiplayerManager.asset
kingbattle/ProjectSettings/SceneTemplateSettings.json
kingbattle/kingbattle.slnx
```

这些包含 `ProjectSettings` 和 Unity 生成/迁移文件。不能盲目全部提交。

### 当前判断

- 既然用户当前明确使用 Unity 6，这些文件里有一部分可能必须纳入仓库，例如：
  - `Packages/manifest.json`
  - `Packages/packages-lock.json`
  - `ProjectSettings/ProjectVersion.txt`
  - URP/Graphics/ShaderGraph 相关迁移资产
- 但 `.idea/`、`.claude/`、`.slnx`、`SceneTemplateSettings.json`、`MultiplayerManager.asset` 是否需要提交，需要单独判断。

下一步不做玩法功能，先做 **Unity 6 迁移文件收口**。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：Unity 6 P1 编译阻塞已通过。

进入下一步：Unity 6 迁移文件收口。

当前情况：
- GameStatusService.cs.meta 已修复。
- 最新 Editor.log 尾部没有 GameStatusService / ShaderGraph / CS 编译错误。
- 现在工作区有大量 Unity 6 自动改出的 Packages / URP / ProjectSettings / asset 文件。
- 不允许盲目全部提交。

任务 A：列出并分类当前未提交文件
- 用 `git status --short` 和 `git diff --stat`。
- 分类为：
  1. Unity 6 必须迁移文件。
  2. 可能需要但要说明原因的 URP/Graphics 资产。
  3. 不应提交的 IDE/临时/生成文件。
  4. 需要用户/Codex 决定的文件。

任务 B：给出提交建议
- 明确建议哪些文件应该提交：
  - Packages/manifest.json
  - Packages/packages-lock.json
  - ProjectSettings/ProjectVersion.txt
  - 以及确实由 Unity 6/URP 迁移必需的 Assets/Settings 或 ProjectSettings 文件。
- 明确建议哪些文件不提交：
  - .claude/
  - kingbattle/.idea/
  - kingbattle/kingbattle.slnx
  - Unity 生成目录
  - 任何无关文件

任务 C：不要马上提交有争议的 ProjectSettings
- 如果你认为 ProjectSettings/GraphicsSettings.asset、URPProjectSettings.asset、ShaderGraphSettings.asset 必须提交，请在 WORKLOG.md 解释原因。
- 如果不确定，先不要提交，等待 Codex review。

任务 D：记录 Unity 6 warnings
- 继续记录但本轮不大范围修：
  - Physics2D.OverlapCircleNonAlloc obsolete。
  - FindObjectsByType<T>(FindObjectsSortMode) obsolete。
  - FindFirstObjectByType obsolete。

任务 E：验证
- Unity Console 无编译错误。
- Play 能进入场景。
- 做一次 smoke test：点击派兵 / HUD Dispatch / O / K/L/E/N。

输出：
- 更新 WORKLOG.md，写清楚每个文件的提交/不提交建议。
- 不做新玩法功能。
- 不重构。
- 如果要 commit，请只 commit 你能明确解释的 Unity 6 迁移文件和 WORKLOG.md。
- 不提交 .claude、.idea、kingbattle.slnx、Library、Logs、UserSettings。
```
