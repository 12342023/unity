# REVIEW.md

## Review 状态

Codex 已完成 Claude 的 MVP-01 固定地图与道路移动原型静态审查。

结论：**暂不批准进入 MVP-02**。

原因不是功能方向错误，而是 MVP-01 还有两个必须收口的问题：

1. 代码中出现了不必要的全局状态入口。
2. Play Mode / Console Error 验证尚未完成。

## CODEX PROJECT REVIEW

Gate: **FAIL**

### [P1] MVP-01 缺少 Play Mode 验证

File: `WORKLOG.md`

Problem:

`NEXT_STEPS.md` 的 MVP-01 验收标准要求 Play Mode 无明显 Console Error。Claude 的记录说明其未执行 commit / push，但也没有提供 Unity Play Mode 验证结果。Codex 尝试使用 Unity batchmode 验证，第一次被 Unity License Client IPC 超时阻断，第二次被 `kingbattle/Temp/UnityLockfile` 阻断，提示已有 Unity 实例打开项目。

Impact:

当前只能确认脚本结构和代码意图，不能确认 Unity 是否完成脚本导入、编译，以及按键 1-4 是否能在 Play Mode 中正常生成并移动单位。

Fix:

Claude 或用户需要在 Unity Editor 中打开 `kingbattle/`，进入 Play Mode，验证：

- Console 没有编译错误或运行时错误
- 进入 Play Mode 后自动生成地图
- Key 1 / 2 / 3 可分别生成 Samurai / Elf Archer / Soldier
- 单位沿道路移动到 EnemyBase
- Key 4 可生成 Samurai 并移动到 Crossroads

验证完成后，把结果写入 `WORKLOG.md`。

### [P2] 移除未使用的全局 MapData.Instance

File: `kingbattle/Assets/Scripts/Map/MapData.cs:14`

Problem:

`MapData` 新增了 `public static MapData Instance { get; private set; }`，并在 `CreateFixedMap()` 里赋值。当前代码已经通过 `GameEntry` 显式把 `mapData` 传给 `MapRenderer` 和 `TestUnitSpawner`，这个静态入口没有实际必要。

Impact:

项目要求明确避免随意新增全局状态。现在保留 `MapData.Instance` 会给后续建筑、战斗、AI、平台适配留下隐式依赖入口，容易让系统逐渐绕过清晰的模块边界。

Fix:

请 Claude 删除：

```diff
- public static MapData Instance { get; private set; }
```

以及：

```diff
- Instance = map;
```

继续使用显式依赖传递。

## 通过项

- MVP-01 范围控制良好，没有提前实现战斗、AI、占领、建筑、资源或英雄系统。
- `Scripts/Core`、`Scripts/Map`、`Scripts/Units` 目录边界符合要求。
- 固定地图包含 6 个地块和 7 条道路，满足 5 到 7 个地块的要求。
- 地块大小、阵营、主基地标记和建筑槽位数据标记已覆盖。
- BFS 道路寻路方向合理，单位移动入口使用道路路径生成 waypoint，没有直接穿越空白地图的公开接口。
- 三个兵种速度差异符合 `goal.md`：Samurai 最慢，Elf Archer 中等，Soldier 最快。
- 未修改 `ProjectSettings`。
- 当前未发现 Unity 生成目录被 stage。

## 风格建议

本次新增脚本中有较多非 ASCII 注释符号，例如箭头和线框字符。功能上不是阻塞项，但后续建议 Claude 使用普通 ASCII 注释，保持代码文件风格稳定。

## Codex 当前判断

MVP-01 方向正确，但需要先让 Claude 做一次小修：

```text
移除 MapData.Instance
完成 Unity Play Mode 验证
更新 WORKLOG.md
再提交并推送
```

在这两项完成前，不批准进入 MVP-02。
