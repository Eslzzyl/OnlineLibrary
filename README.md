# 网上图书馆

## 基于 ASP.NET Core 和 Vue.js 的网上图书馆项目

本项目是合肥工业大学宣城校区 2023-2024 学年秋季学期计算机科学与技术专业 20 级开设的《实习与实训》课程的项目。

本项目是一个完整的前后端分离 Web 项目，但是功能可能比较简陋，因为时间太仓促了😇

> 该项目处于开发当中，代码可能会快速变化。开发结束后，我会移除这条信息。

## 后端

ASP.NET Core Web API，基于 .NET 8.0

数据库采用 SQLite

数据库连接采用 EntityFramework Core 框架

## 前端

使用 Vuetify 3 框架，基于 Vue 3，组合式 API

能用 TypeScript 的地方都用了，如果遇到一些比较棘手的类型问题，会在局部改用 JavaScript

## 如何运行本项目

### 后端

确保已经安装了 ASP.NET 8.0 SDK

推荐使用的开发工具是：
- Microsoft Visual Studio 2022（为了支持 .NET 8.0，需要 17.8 或更新的版本），或者
- JetBrains Rider（为了支持 .NET 8.0，需要 2023.3 或更新的版本）

本项目的大部分代码是在 Rider 上编写的。

克隆项目后，使用 IDE 打开项目根目录下的 `OnlineLibrary.sln` 解决方案文件。

【数据库准备部分有点麻烦，待补】

数据库准备完成后，启动解决方案的 http 配置即可。

> 前端默认向后端的 http 端口发请求，因此测试运行时应该启动 http 配置，而非 https 配置。如果希望修改目标端口，可以到 `Client/src/plugins/util/axiosInstance.ts` 文件中修改。

### 前端

推荐使用的开发工具是：
- Microsoft VSCode，带有以下扩展：
  - TypeScript Vue Plugin（Volar）：`Vue.vscode-typescript-vue-plugin`
  - Vue Language Features（Volar）：`Vue.volar`
- JetBrains WebStorm

Rider 也通过官方插件对 Vue 和 TypeScript 有较好的支持，因此也可以用 Rider 做前端。

克隆项目后，打开 `Client` 目录即可。推荐使用的包管理器是 `pnpm`。安装依赖：
```shell
pnpm install
```

启动开发服务器（构建工具是 Vite）：
```shell
pnpm dev
```

服务器启动后，将自动显示前端的 URL。

## 设计文档

见 [此处](./OnlineLibrary/Doc/设计文档.md)