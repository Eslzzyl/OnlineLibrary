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

克隆项目后，使用 NuGet 恢复依赖：

```shell
nuget restore .\OnlineLibrary.sln
```

然后使用 IDE 打开项目根目录下的 `OnlineLibrary.sln` 解决方案文件。

接下来需要初始化（Seed）数据库。

1. 移除 `OnlineLibrary/Controller/SeedController.cs` 中 `SeedController` 类前面的 `[Authorize(Roles = RoleNames.Admin)]` 标记（从而跳过执行时的用户身份验证，因为用户数据在数据库里，而数据库还不存在）
2. 在命令行中切换到 OnlineLibrary 目录（即后端项目的根目录），执行

    ```shell
    dotnet ef database update
    ```

    这会自动在 `./Data/` 目录下创建 `OnlineLibrary.db` 数据库并按顺序执行已有的一系列 Migrations。
3. 按照下面的步骤启动后端项目，然后在 Swagger 页面依次执行 `/Seed/AuthData`、`/Seed/BookData` 和 `/Seed/SettingsDate` 3个路由，等待数据库导入数据。其中导入书籍数据的时间可能稍长，导入用户数据和导入设置数据应当很快完成。导入完毕后，你应该能够在 Response 中看到导入的数据量。
4. 将 `SeedController` 类前面的 `[Authorize(Roles = RoleNames.Admin)]` 标记添加回去，正常启动项目即可。

数据库准备完成后，启动解决方案的 http 配置即可。看到”OnlineLibrary“的 ASCII Art 后，你可以访问 [http://localhost:5057/swagger/index.html](http://localhost:5057/swagger/index.html) 来打开 Swagger 页面。

`./Data/Logs.db` 文件可以随意删除，删除之后会重新创建。如果你觉得它有点大了，直接删除即可。当然旧的日志也就丢掉了。

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

如果你通过 `/Seed/AuthData/` 导入了用户数据，那么数据库中默认存在三个用户：`TestUser`、`TestModerator` 和 `TestAdmin`。它们的密码都是 `123456`。三种用户的权限依次提高。由于时间仓促，我没有为 `Moderator` 用户组添加前端功能，因此实际可用的用户只有 `User` 和 `Admin` 两类。

## 设计文档

见 [此处](./OnlineLibrary/Doc/设计文档.md)