# REVIEW.md

## Review 状态

当前尚未收到 Claude 的实现或分析输出，因此本文件先记录基线 Review 结论和后续审查标准。

## 基线 Review 结论

### P0：项目说明与实际结构不一致

`AGENTS.md` 声明这是微信小程序项目，但当前目录 `kingbattle/` 是 Unity 工程结构，且未发现 `app.json`、`project.config.json`、`pages/` 等小程序入口。

在确认项目类型前，不应开始功能开发。

### 已处理：Git 仓库边界疑似错误

`git rev-parse --show-toplevel` 返回 `/Users/jianghao`。这说明当前 Git 仓库可能覆盖整个用户目录，导致 `git status` 出现大量无关文件。

用户已确认项目根目录使用 `/Users/jianghao/unity`，并已初始化独立 Git 仓库。后续 Git 操作应在 `/Users/jianghao/unity` 内执行。

仍不建议执行 `git add .`，应只 stage 本次任务相关文件。

### 已处理：GitHub 上传目标已设置

用户要求后续每一次修改都上传 GitHub。当前已完成 GitHub 插件安装/连接流程，但本机未发现 `gh` CLI。

当前项目 Git remote 已设置为：

```text
origin https://github.com/12342023/unity.git
```

已完成本地首次提交。本地仓库 Git 作者与 remote 已切到 `12342023`，推送不再使用 `hahaaaw`。当前剩余阻塞是本机缺少 `12342023` 的 GitHub HTTPS Token，导致无法读取密码完成 push。

注意：不应使用、保存或记录 GitHub 明文密码。需要改用 Personal Access Token。

已使用 token 尝试推送，GitHub 识别账号为 `12342023`，但返回 403。当前 token 缺少 `12342023/unity.git` 写权限，需要重新生成带 `Contents: Read and write` 的 fine-grained token，且 repository access 必须包含 `unity`。

### P1：Unity 生成目录需要版本控制策略

当前项目包含 `Library/`、`Logs/`、`UserSettings/` 等 Unity 生成或本机状态目录。后续如果进入 Unity 开发，需要先确认 `.gitignore` 策略，避免提交大量机器生成文件。

## Claude 输出后的 Review 检查项

- 是否明确区分“事实”和“假设”
- 是否确认项目类型后再规划开发
- 是否避免改动业务代码
- 是否没有随意修改 `ProjectSettings`
- 是否给出最小可执行的下一步
- 是否用 diff 风格表达修改建议
- 是否遵守“每次修改都有文档、提交、推送记录”的流程

## 当前结论

暂不批准进入功能开发。当前基线已本地提交，但 GitHub 上传被认证权限阻塞；修复认证后再推送。
