# NEXT_STEPS.md

## 当前阶段判断

项目目标：

```text
4 周左右完成一个完整 Unity 小游戏；当前项目已迁移到 Unity 6。
```

当前状态：

- Unity 6 迁移文件已收口。
- MVP-04.6 warning cleanup 已通过。
- 最新 `Editor.log` 显示 `Tundra build success`。
- 当前剩余一个非阻塞 warning：`UnitCombat.hasHome` 未使用。
- 下一步应先做交付前清理，再进入正式 UI。

## 当前正式任务：MVP-04.7 交付前清理

目标：

```text
不改玩法行为，只清理 warning、debug/test 边界、明显日志噪音。
```

任务：

- 清理 `UnitCombat.hasHome` unused warning。
- 用 `#if UNITY_EDITOR || DEVELOPMENT_BUILD` 限制 `TestUnitSpawner` 和 `DebugShortcutController` 创建。
- 收敛明显高频、低价值 runtime logs。
- 更新 `WORKLOG.md`。

## MVP-04.7 之后建议顺序

### MVP-05.0 最小正式 UI

- 用正式 UI 替换 debug OnGUI 的核心信息。
- 保留同一 command service，不让 UI 写业务状态。
- UI 只负责展示和触发 command。

### MVP-05.1 正式输入/反馈打磨

- 点击反馈。
- 派兵失败原因展示。
- 胜负面板文案与 restart。

### MVP-05.2 可玩性参数收口

- 士兵生成节奏。
- 敌方压力节奏。
- 占领人数要求。
- 建筑生命值/伤害。

## Play Mode 回归清单

- Unity Console 无 P1 编译错误。
- Unity Console 无目标 warning。
- Play 初始 HUD 显示目标、候选、人口、敌方压力。
- 鼠标点击 Player-owned source 后高亮为蓝色，valid target 高亮为黄色。
- 鼠标点击黄色 target 后派兵。
- 点击空白区域清空选择。
- HUD Dispatch 可派兵。
- `O` 与 HUD Dispatch / 点击派兵同路径。
- `E` 触发敌方进攻。
- `K` 触发 Victory。
- `L` 触发 Defeat。
- Victory/Defeat 后点击、HUD Dispatch、`O/E` 不再执行 gameplay command。
- `N` restart debug。

## 交付前剩余风险

- 正式 UI 还没做，目前仍是 debug OnGUI。
- Debug/test 入口需要本轮加 build 条件边界。
- Runtime logs 偏多，需要本轮收敛一部分。
- Unity Connect 401 属于外部服务/auth，暂不阻塞 gameplay。
- 移植还没开始，但边界需要持续保持。
- `ProjectSettings`、`.idea/`、`.claude/`、Unity 生成目录不能随便提交。
