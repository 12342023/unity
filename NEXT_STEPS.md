# NEXT_STEPS.md

## 下一步计划

### 1. 让 Claude 执行基线确认

把 `TASK.md` 交给 Claude，要求 Claude 只做只读分析和文档输出。

Claude 需要先回答：

- 这个目录当前到底是 Unity 项目还是微信小程序项目？
- 如果目标是微信小程序，真正的小程序目录在哪里？
- 如果目标是 Unity，是否需要更新 `AGENTS.md` 的项目说明？
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
+ 修正 AGENTS.md 中项目类型描述
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

当前可以执行首次提交和推送；推送前仍需确认：

- staged 文件不包含 `Library/`、`Logs/`、`UserSettings/`、`.vscode/`、`.sln`
- 如遇认证失败，需要用户在 GitHub 或系统 Git 凭据中完成授权

### 5. 进入功能开发前的门禁

只有当以下条件满足后，才允许 Claude 开始业务开发：

- 项目类型已确认
- 仓库边界已确认：`/Users/jianghao/unity`
- 忽略规则已确认
- GitHub remote 已确认
- 每次修改上传规则已确认
- 目标功能已有明确需求
- Codex Review 无 P0 阻塞项

## 当前状态

下一步：提交并推送当前文档和项目基线到 `https://github.com/12342023/unity.git`。
