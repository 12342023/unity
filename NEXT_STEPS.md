# NEXT_STEPS.md

## 当前阶段判断

项目目标：

```text
4 周左右完成一个完整 Unity 小游戏；当前用户使用 Unity 6。
```

当前优先级：

```text
先把 Unity 6 迁移文件边界收口，再继续玩法/交付清理。
```

## 当前状态

- `GameStatusService.cs.meta` GUID 已修复。
- Unity 6 P1 编译阻塞已解除。
- Editor log 尾部已进入正常 gameplay / Victory 日志。
- 工作区仍有 Unity 6 自动改出的 Packages / URP / ProjectSettings / asset 文件。

## 当前正式任务：Unity 6 迁移文件收口

目标：

```text
判断哪些 Unity 6 迁移文件必须提交，哪些必须排除，避免把 IDE/生成文件或无关 ProjectSettings 混入仓库。
```

实现方向：

```diff
+ 分类当前未提交 Unity 6 文件
+ 解释 Packages / URP / ProjectSettings 迁移原因
+ 只提交必要文件
+ 不提交 .claude / .idea / slnx / Library / Logs / UserSettings
- 不做新玩法
- 不盲目提交所有 ProjectSettings
```

## 收口后建议顺序

### MVP-04.5 Review 恢复

- 重新 review 点击派兵、高亮、debug 快捷键集中。
- 确认 Unity 6 下 smoke test 通过。

### MVP-04.6 Unity 6 cleanup

- 清理 Unity 6 obsolete warnings：
  - `FindObjectsByType<T>(FindObjectsSortMode)`。
  - `FindFirstObjectByType<T>()`。
  - `Physics2D.OverlapCircleNonAlloc`。
- 仍要保持小步提交，不做玩法重构。

### MVP-05.0 最小正式 UI

- 用正式 UI 替换 debug OnGUI 的核心信息。
- 保留同一 command service，不让 UI 写业务状态。

## Play Mode 回归清单

- Unity Console 无 P1 编译错误。
- Play 初始 HUD 显示目标、候选、人口、敌方压力。
- 鼠标点击 Player-owned source 后高亮为蓝色，valid target 高亮为黄色。
- 鼠标点击黄色 target 后派兵。
- 点击空白区域清空选择。
- HUD Dispatch 可派兵。
- O 与 HUD Dispatch / 点击派兵同路径。
- Q 只读预览。
- U main-base ruin debug 派兵。
- supply cap 达到上限后 Barracks 停止产兵。
- Granary 摧毁/重建会影响 cap。
- E 触发敌方进攻。
- K 触发 Victory。
- L 触发 Defeat。
- Victory/Defeat 后点击、HUD Dispatch、O/E 不再执行 gameplay command。
- N restart debug。
- Console 无明显错误。

## 交付前剩余风险

- Unity 6 migration 文件尚未决定提交范围。
- 正式 UI 还没做，目前仍是 debug OnGUI。
- `TestUnitSpawner` 的 1-4 测试输入后续需要清理或标记 debug-only。
- Unity 6 obsolete warnings 尚未清理。
- 移植还没开始，但边界需要持续保持。
- `ProjectSettings`、`.idea/`、`.claude/`、Unity 生成目录不能随便提交。

## 长期提醒

- 现在不做移植实现，但后续仍可能做微信小程序、macOS、Android，因此业务逻辑边界必须持续清楚。
- 平台能力不要散落在建筑、战斗、移动脚本中。
- 输入层只负责发命令；未来触摸、鼠标、键盘应该能替换适配。
