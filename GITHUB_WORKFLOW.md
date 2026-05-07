# GITHUB_WORKFLOW.md

## 目标

用户要求：后续每一次修改都上传 GitHub，并编写详细工作文档。

本文件定义 Codex / Claude 的协作流程。当前先建立规则，在目标 GitHub 仓库未确认前不贸然推送。

## 当前 GitHub / Git 状态

- GitHub 插件：已安装并完成连接流程。
- `gh` CLI：当前不可用，`gh auth status` 返回 `command not found`。
- GitHub HTTPS 推送需要 Personal Access Token，不使用、不保存、不记录账号明文密码。
- Git 顶层目录：`/Users/jianghao/unity`
- 当前工作目录：`/Users/jianghao/unity`
- 当前项目目录：`/Users/jianghao/unity/kingbattle`
- 旧父级仓库 remote：

```text
origin https://git@github.com:hahaaaw/-.git
```

- 当前项目仓库 remote：

```text
origin https://github.com/12342023/unity.git
```

## 当前阻塞项

### 已处理：目标 GitHub 仓库已提供

用户已确认项目 Git 根目录使用 `/Users/jianghao/unity`，并已初始化独立 Git 仓库。

用户已提供新 GitHub 仓库地址，并已设置为当前项目仓库 `origin`。

在该问题确认前：

```diff
- 不允许 git add .
- 不允许批量提交整个工作树
- 不允许 push 到旧父级仓库 origin
+ 只允许查看 Git 状态
+ 只允许 stage 明确列出的项目文件
+ 推送前必须确认 staged diff 不包含生成目录
```

### 已处理：remote 目标已设置

当前 remote 指向 `hahaaaw/-.git`，但用户已确认需要上传到另一个 GitHub 账号。

当前项目 remote 已设置为：

```text
origin https://github.com/12342023/unity.git
```

### P0：缺少 GitHub Personal Access Token

当前仓库已切换到 `12342023`，但 GitHub HTTPS 推送需要 token。不要把账号密码写入 remote URL、Git 配置、脚本或工作文档。

安全做法：

```diff
- 不使用 GitHub 账号密码推送
- 不把密码写进命令行
- 不把密码写进 WORKLOG.md
+ 使用 GitHub Personal Access Token
+ token 只在 Git 凭据提示中输入
+ 如果 token 已泄露或误发，立即在 GitHub 中撤销并重新生成
```

## 每次修改后的标准流程

当仓库边界和 remote 被确认后，每次修改必须执行：

1. 更新工作文档：
   - `TASK.md`：当前任务和 Claude 指令
   - `REVIEW.md`：Review 结论和阻塞项
   - `NEXT_STEPS.md`：下一步计划
   - `WORKLOG.md`：本次实际操作记录
2. 检查变更范围：

```sh
git status --short -- <本次相关文件>
git diff -- <本次相关文件>
```

3. 只 stage 本次相关文件：

```sh
git add <明确文件列表>
```

4. 提交：

```sh
git commit -m "docs: update project coordination workflow"
```

5. 推送：

```sh
git push origin <当前分支>
```

6. 在 `WORKLOG.md` 记录：
   - 修改文件
   - 提交信息
   - commit hash
   - push 结果
   - 未完成事项

## Claude 工作要求

Claude 每次开始工作前必须先读：

- `AGENTS.md`
- `TASK.md`
- `REVIEW.md`
- `NEXT_STEPS.md`
- `GITHUB_WORKFLOW.md`
- `WORKLOG.md`

Claude 每次输出后必须说明：

- 修改了哪些文件
- 是否更新了工作文档
- 是否完成 commit
- 是否完成 push
- 如果没有 push，具体阻塞原因是什么

## Codex Review 要求

Codex 每次 Review Claude 输出时必须检查：

- 是否只改了任务范围内文件
- 是否没有批量重构
- 是否没有提交生成目录或无关文件
- 是否更新了工作文档
- 是否遵守 GitHub 上传规则
- 是否记录了无法上传的原因
