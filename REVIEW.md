# REVIEW.md

## Review 状态

Codex 已完成 Claude 的 MVP-01 固定地图与道路移动原型复审。

结论：**MVP-01 通过，允许进入 MVP-02。**

## CODEX PROJECT REVIEW

Gate: **PASS**

## 复审结论

### 已修复：移除 MapData.Instance

Claude 已按 Review 要求移除 `MapData.Instance`：

- `kingbattle/Assets/Scripts/Map/MapData.cs` 中不再存在 `public static MapData Instance`
- `CreateFixedMap()` 中不再存在 `Instance = map`
- 全局搜索 `kingbattle/Assets/Scripts` 未发现 `MapData.Instance` 引用
- `MapRenderer` 和 `TestUnitSpawner` 仍通过 `Initialize(MapData)` 显式传入地图数据

该项通过。

### 已验证：Unity Play Mode

根据 `WORKLOG.md` 记录，用户已在 Unity Editor 中手动验证：

- Console 无编译错误、无运行时错误
- Key 1 生成 Samurai 并移动到 EnemyBase
- Key 2 生成 Elf Archer 并移动到 EnemyBase
- Key 3 生成 Soldier 并移动到 EnemyBase
- Key 4 生成 Samurai 并移动到 Crossroads
- 地图渲染、道路、建筑槽位标记、主基地边框均可见

该项通过。

### 已确认：MVP-01 范围控制

本轮实现仍保持在 MVP-01 范围：

- 固定地图
- 地块数据
- 固定道路
- BFS 道路寻路
- 单位沿道路移动
- 临时键盘测试入口

未提前实现：

- 战斗
- AI
- 占领进度
- 建筑建造 / 重建 / 拆除
- 粮食资源
- 英雄
- 联机
- 随机地图
- 复杂 UI

该项通过。

## 残留注意事项

### ProjectSettings 新增文件未纳入本次提交

当前工作树出现未跟踪文件：

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件是 Unity Editor 生成的 ProjectSettings 文件。由于项目规则要求不随意修改 `ProjectSettings`，本次不纳入提交。后续需要单独决定：

- 是否确认为 Unity 项目必要设置并提交
- 或是否作为本机 / 编辑器生成文件忽略

在明确前不要把它和业务代码一起提交。

### 注释风格

本次脚本中仍存在部分非 ASCII 注释符号。功能上不阻塞，但后续 Claude 写新脚本时应优先使用普通 ASCII 注释，减少跨编辑器显示差异。

## Codex 当前判断

MVP-01 已满足目标，可以进入 MVP-02。

MVP-02 应继续保持小步推进，只做：

```text
建筑数据
兵营出兵
基础战斗
```

暂不加入 AI、占领进度、粮食资源、建筑重建 / 拆除、英雄和复杂 UI。
