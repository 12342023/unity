# NEXT_STEPS.md

## 当前阶段判断

项目目标：

```text
4 周左右完成一个完整 Unity 小游戏；当前项目已迁移到 Unity 6。
```

当前状态：

- Unity 6 迁移文件已收口。
- 最新 Editor log 尾部有 P1 编译错误 `CS1503`。
- MVP-04.6 warning cleanup 必须先返修，因为当前 API 替换已导致 Unity 6 编译失败。

## 当前正式任务：MVP-04.6 warning cleanup 返修

目标：

```text
彻底移除 Unity 6 obsolete API 残留，不改变玩法行为。
同时修复当前 `FindObjectsByType` 参数顺序导致的 `CS1503` 编译错误。
```

实现方向：

```diff
- Object.FindObjectsByType<T>(FindObjectsSortMode.None, FindObjectsInactive.Exclude)
+ Object.FindObjectsByType<T>(FindObjectsInactive.Exclude)

- Object.FindFirstObjectByType<PlayerInputController>(FindObjectsInactive.Exclude)
+ Object.FindAnyObjectByType<PlayerInputController>(FindObjectsInactive.Exclude)

- TowerAttack 注释中的 OverlapCircleNonAlloc
+ TowerAttack 注释改为 OverlapCircle + reusable buffer
```

## 返修后建议顺序

### MVP-04.7 交付前清理

- 检查 debug 快捷键是否要隐藏或标记 debug-only。
- 检查 `TestUnitSpawner` 是否保留。
- 清理过多 runtime log。

### MVP-05.0 最小正式 UI

- 用正式 UI 替换 debug OnGUI 的核心信息。
- 保留同一 command service，不让 UI 写业务状态。

## Play Mode 回归清单

- Unity Console 无 P1 编译错误，尤其不能再有 `CS1503`。
- Unity Console 无本轮目标 obsolete warnings。
- Play 初始 HUD 显示目标、候选、人口、敌方压力。
- 鼠标点击 Player-owned source 后高亮为蓝色，valid target 高亮为黄色。
- 鼠标点击黄色 target 后派兵。
- 点击空白区域清空选择。
- HUD Dispatch 可派兵。
- O 与 HUD Dispatch / 点击派兵同路径。
- E 触发敌方进攻。
- K 触发 Victory。
- L 触发 Defeat。
- Victory/Defeat 后点击、HUD Dispatch、O/E 不再执行 gameplay command。
- N restart debug。

## 交付前剩余风险

- 正式 UI 还没做，目前仍是 debug OnGUI。
- `TestUnitSpawner` 的 1-4 测试输入后续需要清理或标记 debug-only。
- Runtime logs 偏多，交付前需要收敛。
- Unity Connect 401 属于外部服务/auth，暂不阻塞 gameplay。
- 移植还没开始，但边界需要持续保持。
- `ProjectSettings`、`.idea/`、`.claude/`、Unity 生成目录不能随便提交。
