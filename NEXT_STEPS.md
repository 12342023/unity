# NEXT_STEPS.md

## 当前阶段判断

项目正在第三阶段早期：摧毁与重建。

已经完成：

- 建筑被击败后变成废墟。
- 士兵可以围绕废墟巡逻。
- 大本营被击败后，该阵营建筑变废墟，士兵立即死亡。
- 废墟携带来源 plot / 建筑类型 / 原阵营。
- 废墟具备最小可重建判定。
- 建筑创建逻辑已从 `GameEntry` 下沉到 `BuildingFactory`。
- 运行时建筑已通过 `BuildingRegistry` 按阵营登记和查询。
- `BuildingRebuildService` 已初步新增，但需要补一个空值防御。

## 当前正式任务：MVP-03.8 小修

目标：

```text
补齐 BuildingRebuildService.Rebuild(...) 的 mapData 空值保护。
```

这是服务边界问题，不是玩法扩展。修完后再继续做重建闭环验证。

## 给 Claude 的实现方向

```diff
+ 在 BuildingRebuildService.Rebuild(...) 中检查 mapData == null
+ 为空时 Debug.LogWarning 并 return null
+ 保持现有 R 测试入口不变
+ 更新 WORKLOG.md 并 commit / push
- 不做正式 UI
- 不做资源 / 占领 / 升级 / 连地 / AI
- 不修改 ProjectSettings
```

## 小修通过后的建议顺序

### MVP-03.9 临时调试入口整理与验证重建闭环

当前已经有 R 测试入口。小修通过后，下一轮建议不要立刻做正式 UI，而是整理并验证临时调试闭环：

- K 生成敌方废墟。
- R 重建第一个废墟为 Player 建筑。
- 验证旧废墟销毁、新建筑注册到 `BuildingRegistry`。
- 再通过 K / L 或额外测试确认重建建筑参与阵营清场。
- 明确 R 是临时测试入口，不是正式 UI。

### 后续再做重建规则

等调试闭环稳定后，再考虑最小规则：

- 哪个阵营允许重建。
- 重建后归属谁。
- 是否只能重建特定 plot。

资源、升级、连地、区域奖励继续后置。

## 长期提醒

- 微信小程序、macOS、Android 移植会要求业务逻辑边界干净。
- 平台能力不要散落在建筑、战斗、移动脚本中。
- 当前阶段继续优先保持 `GameEntry` 变薄，业务能力下沉到清晰的小组件 / 小服务。
