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

首次提交已完成；推送失败，原因是当前本机 GitHub 凭据没有目标仓库权限：

```text
remote: Permission to 12342023/unity.git denied to hahaaaw.
fatal: unable to access 'https://github.com/12342023/unity.git/': The requested URL returned error: 403
```

需要二选一处理：

```diff
+ 切换本机 GitHub 凭据到 12342023
+ 或在 GitHub 仓库中把 hahaaaw 添加为 collaborator 并授予写权限
```

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

下一步：修复 GitHub 认证权限后，重新执行 `git push -u origin main`。

当前本地 Git 配置已切到 `12342023`，再次 push 的错误已变为：

```text
fatal: could not read Password for 'https://12342023@github.com': Device not configured
```

这表示旧账号问题已解除，剩余问题是需要给本机配置 `12342023` 的 GitHub HTTPS Token。

不要使用或保存 GitHub 明文密码。下一步需要在 GitHub 创建 Personal Access Token，然后在本机执行 push 时把 token 当作密码输入。

已尝试使用 token 推送，但 GitHub 返回：

```text
remote: Permission to 12342023/unity.git denied to 12342023.
fatal: unable to access 'https://github.com/12342023/unity.git/': The requested URL returned error: 403
```

这说明账号已正确识别为 `12342023`，但 token 没有该仓库写权限。需要重新生成 token，并确保 `unity` 仓库的 `Contents` 权限是 `Read and write`。
