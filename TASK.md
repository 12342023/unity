# TASK.md

## 当前任务

Unity 6 迁移文件收口。

Codex 当前仍作为 Tech Lead 和 Reviewer 工作，不直接大规模开发业务代码。Claude 是主要开发者。

## 最新 Review 结论

Claude 最新提交：

```text
6232f7a fix: GameStatusService.cs.meta GUID length 34 -> 32 hex chars
```

Codex Review 结论：

```text
Unity 6 P1 编译阻塞已通过；进入 Unity 6 迁移文件收口。
```

## 当前情况

当前 Unity 版本：

```text
Unity 6000.4.6f1
```

已确认：

- `GameStatusService.cs.meta` 已修为 32 位 GUID。
- `Assets/Scripts/**/*.meta` 未发现其他非 32 位 GUID。
- Editor log 尾部没有 P1 编译错误。
- Play 已能跑到实际 gameplay / Victory 日志。

当前工作区仍有 Unity 6 自动改动，包含：

- `Packages/manifest.json`
- `Packages/packages-lock.json`
- `ProjectSettings/ProjectVersion.txt`
- URP / Graphics / ShaderGraph 相关设置
- `Assets/DefaultVolumeProfile.asset`
- `ProjectSettings/MultiplayerManager.asset`
- `kingbattle/kingbattle.slnx`

这些文件不能盲目全部提交。

## 允许范围

任务 A：文件分类

- 列出当前 `git status --short`。
- 把未提交文件分成：
  - 必须提交的 Unity 6 迁移文件。
  - 可能需要提交但必须解释原因的 URP/Graphics/ShaderGraph 文件。
  - 不应提交的 IDE/生成/临时文件。
  - 需要 Codex/用户决定的文件。

任务 B：提交建议

- 对每个文件给出一行建议：
  - 提交。
  - 不提交。
  - 暂缓，等待确认。
- 对 `ProjectSettings` 类文件必须写清楚理由。

任务 C：必要时小范围提交

- 如果文件确实是 Unity 6 打开项目所必需，可以提交：
  - `Packages/manifest.json`
  - `Packages/packages-lock.json`
  - `ProjectSettings/ProjectVersion.txt`
  - 必要的 URP/Graphics/ShaderGraph 迁移资产
- 不提交 `.claude/`、`kingbattle/.idea/`、`kingbattle/kingbattle.slnx`、`Library/`、`Logs/`、`UserSettings/`。
- `ProjectSettings/SceneTemplateSettings.json` 默认不提交，除非明确说明必要性。
- `ProjectSettings/MultiplayerManager.asset` 默认不提交，除非明确说明必要性。

任务 D：验证

- Unity Console 无编译错误。
- Play 能进入场景。
- smoke test：
  - 点击派兵。
  - HUD Dispatch。
  - O。
  - K/L/E/N。

任务 E：文档

- 更新 `WORKLOG.md`。
- 记录：
  - Unity 6 编译状态。
  - 文件分类。
  - 提交/不提交理由。
  - 剩余 warnings。

## 禁止范围

- 不做新玩法系统。
- 不做 UI 美术。
- 不重构战斗/占领系统。
- 不提交 `Library/`、`Logs/`、`UserSettings/`。
- 不提交 `.claude/`、`kingbattle/.idea/`。
- 不提交 `kingbattle/kingbattle.slnx`。
- 不盲目提交所有 `ProjectSettings`。
- 不手改并提交 `Library/PackageCache`。

## 验收标准

- 当前 Unity 6 迁移文件已有明确分类。
- 如有提交，提交范围清楚且理由写入 `WORKLOG.md`。
- Console 无 P1 编译错误。
- Play smoke test 通过。
- 不应提交文件仍未提交。
