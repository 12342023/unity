# NEXT_STEPS.md

## 下一步计划

### 1. 让 Claude 执行基线确认

把 `TASK.md` 交给 Claude，要求 Claude 只做只读分析和文档输出。

Claude 需要先回答：

- 当前 Unity 小游戏工程的最小可维护结构是什么？
- 未来微信小程序、macOS、Android 移植需要哪些边界？
- 哪些代码应保持平台无关，哪些能力需要平台适配层？
- Git 仓库根已调整为 `/Users/jianghao/unity`
- 当前 GitHub remote 已设置为 `https://github.com/12342023/unity.git`
- 每次修改上传 GitHub 的执行流程如何落地？

### 2. Codex Review Claude 输出

Claude 输出后，Codex 更新 `REVIEW.md`：

- 标出阻塞问题
- 标出可以接受的建议
- 对不合理建议给出 diff 风格修正
- 明确是否允许进入下一阶段开发

### 3. 确认仓库卫生

在用户确认后，再考虑以下动作：

```diff
+ 已新增 .gitignore
+ 已明确 Unity 工程根目录
+ 明确文档职责边界
+ 已修正 AGENTS.md 中项目类型描述
+ 已确认 GitHub remote
+ 建立每次修改后的 commit / push 规则
```

### 4. GitHub 上传门禁

后续每次修改完成后，原则上必须：

```diff
+ 更新工作文档
+ 只 stage 本次相关文件
+ commit
+ push 到确认后的 GitHub 仓库
+ 在 WORKLOG.md 记录提交和推送结果
```

GitHub 上传已验证成功，当前远端为 `https://github.com/12342023/unity.git`。

### 5. 进入功能开发前的门禁

只有当以下条件满足后，才允许 Claude 开始业务开发：

- 项目类型已确认：Unity 小游戏
- 仓库边界已确认：`/Users/jianghao/unity`
- 忽略规则已确认
- GitHub remote 已确认
- 每次修改上传规则已确认
- 目标功能已有明确需求
- Codex Review 无 P0 阻塞项

## 当前状态

下一步：

```diff
+ 重新配置 GitHub Personal Access Token 或官方授权
+ 推送本地未上传提交
+ 让 Claude 按 TASK.md 做项目基线确认
+ Codex 根据 Claude 输出更新 REVIEW.md
+ 后续每次修改后继续 commit / push / 记录 WORKLOG.md
```

当前未上传提交：

```text
14f7789 docs: update project platform roadmap
```
