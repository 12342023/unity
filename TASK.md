# TASK.md

## 当前任务

项目基线确认与开发协作启动。

Codex 当前只作为 Tech Lead / Reviewer 工作，不直接开发业务代码。Claude 是主要开发者，本轮任务是先确认项目事实、整理仓库边界和开发约束，再进入功能开发。

## 已确认事实

- 工作目录：`/Users/jianghao/unity`
- 项目目录：`kingbattle/`
- 当前实际结构更像 Unity 项目，而不是微信小程序项目：
  - `kingbattle/Assets`
  - `kingbattle/Packages`
  - `kingbattle/ProjectSettings`
  - `kingbattle/Library`
- Unity 版本：`2022.3.62f1`
- 当前依赖包含：
  - `com.unity.render-pipelines.universal` `14.0.12`
  - `com.unity.feature.2d` `2.0.1`
  - `com.unity.test-framework` `1.1.33`
  - `com.unity.ugui` `1.0.0`
  - `com.unity.visualscripting` `1.9.4`
- 当前 `Assets` 下只有场景和渲染设置，未发现 C# 业务脚本。
- 初始扫描时未发现已有 `README.md`、`TASK.md`、`REVIEW.md`、`NEXT_STEPS.md`；目前已由 Codex 创建协作文档。
- 初始 Git 顶层目录解析为 `/Users/jianghao`，后续用户已确认使用 `/Users/jianghao/unity` 作为项目 Git 根目录，且已初始化独立 Git 仓库。
- 已新增 `.gitignore`，排除 Unity 生成目录和本机 IDE 文件。

## 风险与不一致

1. `AGENTS.md` 说明项目是微信小程序，但当前目录实际是 Unity 工程。
2. 当前没有业务代码和产品说明，不能直接开始实现功能。
3. 新 GitHub 账号下的目标仓库地址尚未提供，暂不能上传。

## Claude 本轮任务

请 Claude 只做只读分析和文档输出，不要直接写业务代码。

### 目标 1：确认项目类型

Claude 需要确认以下问题：

- 当前要维护的项目到底是微信小程序，还是 `kingbattle` Unity 项目？
- 如果是微信小程序，请定位小程序根目录，例如是否存在 `app.json`、`project.config.json`、`pages/`。
- 如果是 Unity 项目，请指出 `AGENTS.md` 中“微信小程序项目”的描述需要更新。

### 目标 2：整理项目基线

Claude 输出一份简短项目清单，包含：

- 项目类型
- 主要目录
- 可编辑源码目录
- 生成目录 / 不应手动维护的目录
- 当前是否存在业务脚本
- 当前是否存在测试
- 当前是否有可运行入口

### 目标 3：提出仓库边界修正建议

Claude 需要给出建议，不直接执行大范围改动：

- 是否应把 Git 仓库根移动或初始化到 `/Users/jianghao/unity`
- 是否应给 Unity 项目增加 `.gitignore`
- 是否需要从版本控制中排除 `Library/`、`Logs/`、`UserSettings/`

修改建议请用 diff 风格示例表达，例如：

```diff
+ /kingbattle/Library/
+ /kingbattle/Logs/
+ /kingbattle/UserSettings/
+ /kingbattle/Temp/
+ /kingbattle/Obj/
+ /kingbattle/Build/
+ /kingbattle/Builds/
```

### 目标 4：不要做的事

Claude 本轮不要：

- 不要修改业务代码
- 不要新增全局状态
- 不要重构目录
- 不要改 Unity `ProjectSettings`，除非先给出理由并等待确认
- 不要假设这是微信小程序并创建 `app.json`
- 不要删除 `Library/`、`Logs/`、`UserSettings/`

### 目标 5：GitHub 与工作文档规范

用户要求后续“每一次修改都上传 GitHub，并编写详细工作文档”。Claude 需要先确认 GitHub / Git 状态，但不要在目标 remote 未确认前推送。

当前已知状态：

- GitHub 插件已完成安装/连接流程。
- 本机暂未发现 `gh` CLI：`gh auth status` 返回 `command not found`。
- 旧父级仓库 remote：

```text
origin https://git@github.com:hahaaaw/-.git
```

- 当前项目 Git 根目录已确认为 `/Users/jianghao/unity`。
- 当前项目仓库 remote 已设置为 `https://github.com/12342023/unity.git`。
- 用户已确认需要上传到另一个 GitHub 账号，因此当前 `origin` 不能视为目标仓库。

Claude 需要给出：

- 如何改为用户指定的新 GitHub 仓库
- 确认 staged diff 不包含 Unity 生成目录
- 后续每次修改的提交、推送、文档更新流程

后续每次修改必须遵守：

```diff
+ 更新 TASK.md / REVIEW.md / NEXT_STEPS.md / WORKLOG.md 中相关记录
+ 只 stage 本次任务相关文件
+ 提交信息说明本次变更目的
+ 推送到 GitHub
+ 在工作文档中记录 commit / push 结果
```

## 验收标准

Claude 完成本轮后，应提交：

- 项目类型确认结论
- 仓库边界风险说明
- 最小 `.gitignore` / 文档修正建议
- GitHub 上传流程建议
- 后续功能开发前需要用户确认的问题列表

Codex 将基于 Claude 的输出更新 `REVIEW.md` 和 `NEXT_STEPS.md`。
