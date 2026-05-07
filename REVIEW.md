# REVIEW.md

## Review 状态

当前尚未收到 Claude 的实现或分析输出，因此本文件先记录基线 Review 结论和后续审查标准。

## 基线 Review 结论

### 已处理：项目说明与实际结构不一致

`AGENTS.md` 原先声明这是微信小程序项目，但当前目录 `kingbattle/` 是 Unity 工程结构。

用户已确认：当前项目是 Unity 小游戏，后续规划微信小程序、macOS、Android 移植。`AGENTS.md` 已更新。

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

已完成本地首次提交。本地仓库 Git 作者与 remote 已切到 `12342023`，推送不再使用 `hahaaaw`。

注意：不应使用、保存或记录 GitHub 明文密码。需要改用 Personal Access Token。

当前项目基线已成功推送到 `https://github.com/12342023/unity.git`。仍建议撤销聊天中暴露过的 token。

### P1：Unity 生成目录需要版本控制策略

当前项目包含 `Library/`、`Logs/`、`UserSettings/` 等 Unity 生成或本机状态目录。后续如果进入 Unity 开发，需要先确认 `.gitignore` 策略，避免提交大量机器生成文件。

### P1：多平台移植边界需要提前设计

项目后续计划移植到微信小程序、macOS、Android。核心玩法和数据逻辑应尽量保持平台无关；输入、存储、登录、支付、分享、构建发布等平台能力需要通过清晰接口隔离。

## Claude 输出后的 Review 检查项

- 是否明确区分“事实”和“假设”
- 是否基于 Unity 小游戏主工程规划开发
- 是否为微信小程序、macOS、Android 移植保留边界
- 是否避免改动业务代码
- 是否没有随意修改 `ProjectSettings`
- 是否给出最小可执行的下一步
- 是否用 diff 风格表达修改建议
- 是否遵守“每次修改都有文档、提交、推送记录”的流程

## 当前结论

暂不批准进入业务功能开发。当前项目说明、仓库边界和 GitHub 上传流程已明确；下一步请 Claude 先完成 Unity 项目基线确认和多平台边界建议。
