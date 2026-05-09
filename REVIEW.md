# REVIEW.md

## Review 状态

Codex 已审查 Claude 最新提交：

```text
852c70c fix: only buildings trigger patrol-center shift on death
```

结论：**MVP-03.1 暂不通过**。

说明：Claude 已修复“非空目标”场景下的建筑/单位区分，普通单位死亡时不会再传入单位死亡点。但当前实现仍把 `target == null` / `chaseTarget == null` 当成建筑处理，并传入 `transform.position`。在 Unity 中目标被其他士兵击杀并销毁后，引用可能在下一帧变成 null，这时当前士兵会把 patrol center 切到自己脚下。MVP-03.1 仍需再收一次边界。

## CODEX PROJECT REVIEW

Gate: **FAIL**

### 已通过：[P2] 非空目标的建筑/单位区分方向正确

File:

```text
kingbattle/Assets/Scripts/Combat/UnitCombat.cs
```

Review:

Claude 使用 `GetComponent<UnitCombat>() == null` 区分建筑，这符合当前项目结构：单位有 `UnitCombat`，建筑没有。对 `target.IsDead` 且 `target` 仍存在的情况，建筑会 `Deaggro(deathPos)`，普通单位会 `Deaggro()`。

这个方向正确。

### [P1] null 目标仍被当成建筑，导致巡逻中心切到自己脚下

File:

```text
kingbattle/Assets/Scripts/Combat/UnitCombat.cs:175
kingbattle/Assets/Scripts/Combat/UnitCombat.cs:219
```

Problem:

当前判断是：

```csharp
bool isBuilding = target == null || target.GetComponent<UnitCombat>() == null;
```

当 `target == null` 或 `chaseTarget == null` 时，`isBuilding` 会变成 true，随后代码会使用当前士兵位置作为 `deathPos`：

```csharp
Vector3 deathPos = target != null ? target.transform.position : transform.position;
Deaggro(deathPos);
```

这不符合“只有建筑死亡才切换 patrol center”。null 目标不能证明它是建筑；更安全的默认行为应该是 `Deaggro()`，不改变巡逻中心。

Impact:

多名士兵围攻同一个目标时，击杀者可能在本帧看到 `target.IsDead`，但其他正在追击/攻击同一目标的士兵可能在下一帧只看到目标引用变成 null。这些士兵会围绕自己当前位置巡逻，而不是保持原巡逻中心或明确围绕废墟。这会造成战场上多个小巡逻中心，视觉上像单位突然散开停留。

Fix:

请 Claude 做最小修复：

```diff
+ target == null 或 chaseTarget == null 时，只调用 Deaggro()
+ 只有 target/chaseTarget 非空，并且 GetComponent<UnitCombat>() == null 时，才 Deaggro(deathPos)
+ 不要用 target == null || ... 判断建筑
+ 可抽一个很小的 helper 减少三处分支重复，但不要重构战斗系统
```

### [P2] 废墟逻辑放在 GameEntry，后续应下沉到建筑组件

File:

```text
kingbattle/Assets/Scripts/GameEntry.cs
```

Problem:

`SpawnRuin()` 目前写在 `GameEntry` 中。MVP-03.1 可以接受这个原型，但 `GameEntry` 的职责应该保持“装配测试场景”，不应长期承载建筑死亡业务逻辑。

Fix:

本轮不要求大改。建议 Claude 如果能小范围处理，就把建筑死亡生成废墟下沉到 `Buildings/BuildingDeathHandler.cs` 或类似小组件；如果会扩大改动，可以先保留，并在 `WORKLOG.md` 标记为后续整理项。

## 残留注意事项

### ProjectSettings 新增文件仍未处理

当前工作树仍有未跟踪文件：

```text
kingbattle/ProjectSettings/SceneTemplateSettings.json
```

该文件由 Unity Editor 生成。由于项目规则要求不随意修改 `ProjectSettings`，继续保持未提交状态。后续需要单独确认是否提交或忽略。

## 给 Claude 的下一条任务

```text
请先阅读 AGENTS.md、TASK.md、REVIEW.md、NEXT_STEPS.md、WORKLOG.md。

Codex Review：MVP-03.1 仍暂不通过，但问题已经缩小。

问题：
你已经用 GetComponent<UnitCombat>() == null 区分建筑/单位，这个方向正确。
但现在代码把 target == null / chaseTarget == null 也当成建筑：

bool isBuilding = target == null || target.GetComponent<UnitCombat>() == null;

然后在 target 为 null 时使用 transform.position 调用 Deaggro(deathPos)。
这会导致目标被其他士兵击杀并销毁后，当前士兵把 patrol center 切到自己脚下。

请只修这个问题：
- target == null 或 chaseTarget == null 时，只 Deaggro()，不要传 defeatedPos。
- 只有 target/chaseTarget 非空，并且 GetComponent<UnitCombat>() == null 时，才 Deaggro(deathPos)。
- 不要用 target == null || ... 判断建筑。
- 可以抽一个很小 helper，例如 TryGetDefeatedBuildingPosition，但不要重构战斗系统。
- 保持建筑死亡生成废墟。
- 保持普通单位死亡不改变 patrol center。

新增用户要求：
对方大本营被攻占后，对方所有建筑成为废墟，所有兵立即死亡。

当前阶段还没有占领进度系统，所以请先按这个语义实现：
- 大本营血量归零 / 进入废墟 = 大本营被攻占。
- 当前 PlayerBase / EnemyBase 的 Barracks 视为双方大本营。
- 任一方大本营被击败后，该阵营所有仍存活建筑立即转为废墟。
- 该阵营所有仍存活士兵立即死亡 / 销毁。
- 已经生成废墟的建筑不要重复生成第二个废墟。
- 废墟仍无攻击、无出兵、无 HealthComponent。
- 可以新增小组件，例如 Buildings/FactionDefeatHandler.cs；GameEntry 只负责装配，不要继续堆业务逻辑。

不要实现重建、占领进度、资源、升级、连地、区域奖励、传送阵、AI 或 UI。

完成后更新 WORKLOG.md，说明修改文件、Play Mode 验证步骤、是否修改场景/ProjectSettings，并 commit / push。
```
