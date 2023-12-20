# 网上图书馆

## 基于 ASP.NET Core 和 Vue.js 的网上图书馆项目

本项目是合肥工业大学宣城校区 2023-2024 学年秋季学期计算机科学与技术专业 20 级开设的《实习与实训》课程的项目。

本项目是一个完整的前后端分离 Web 项目，但是功能可能比较简陋，因为时间不是很充裕，而且我也不太想在一个 Web 项目上浪费太多时间。

> 本项目的开发已经基本完成。在课程设计结束之后，我会移除这条信息。

## 功能

分为用户部分和管理员部分两大块

### 登录和注册

没什么可说的。注册之后自动获得用户身份。新管理员账号必须手动在数据库中插入。

### 用户部分

- 用户主页，可以查看一些统计信息，并通过图表的方式查看借阅记录统计。
- 以表格的形式查看所有书籍
  - 本项目的所有表格都带有分页、内容搜索和按列排序功能。
  - 对于每本书，可以进行评论，也可以回复已有的评论。
- 以表格的形式查看当前借阅情况
- 以表格的形式查看历史借阅情况
- 以表格的形式荐购书籍，可以查看已有的荐购记录，也可以新增荐购。
- 修改账号信息（修改联系方式、密码，更换头像等）

### 管理员部分

- 和用户类似的主页功能
- 以表格的形式管理所有书籍
- 以表格的形式查看当前借阅情况（所有用户）
- 以表格的形式查看历史借阅情况（所有用户）
- 以表格的形式处理荐购书籍，可以回复说明处理结果
- 以表格的形式管理用户，可以删除用户
- 以表格的形式查看系统日志，支持清空系统日志
- 调整系统设置，目前包括如下设置项：
  - 用户借阅册数的上限
  - 用户借阅时长的上限
- 修改账号信息（修改联系方式、密码，更换头像等）

### 杂项

登录/注册页的动态 3D 背景是用 [Vanta.js](https://www.vantajs.com/) 实现的，用了它的“GLOBE”效果。我的[另一个项目](https://github.com/Eslzzyl/graduate-info-frontend)使用了 Vanta.js 的“CLOUDS”效果。

Vuetify 框架可以较容易地切换全局主题，因此本项目也能支持主题的自动（根据用户操作系统的颜色主题确定）和手动切换。目前只有两种主题：深色和浅色。

项目中与头像有关的实现引用了外部图像，这些图像是保存在我自己架设的图床中的，关于图床的实现，可见[此处](https://github.com/Eslzzyl/imagebed)。我不确定这个图床能维护多久，因此如果图床挂了，你可能需要修改相关的代码来改用其他方案。

## 截图

贴一些图，同样托管在我的图床中。

<details>
  <summary>展开</summary>
  
  深色主题的登录页：

  <img src="https://img.eslzzyl.eu.org/67be3278e5430f5fe60a9c759960718a.jpg" alt="深色主题的登录页" width="50%" height="50%">

  用户主页：

  <img src="https://img.eslzzyl.eu.org/3e0886f232f9e1b058b3afb9e9faad64.jpg" alt="用户主页" width="50%" height="50%">

  深色主题的用户主页：

  <img src="https://img.eslzzyl.eu.org/6b41806972a92f9e92bc01cea6121d5b.jpg" alt="深色主题的用户主页" width="50%" height="50%">

  查看书籍：

  <img src="https://img.eslzzyl.eu.org/8ae8c6163d8dd0db335d96754cbeb09.jpg" alt="查看书籍" width="50%" height="50%">

  查看书籍评论：

  <img src="https://img.eslzzyl.eu.org/b6dd38bac02a43c24f4d0abf0135260d.jpg" alt="查看书籍评论" width="50%" height="50%">

  当前借阅列表：

  <img src="https://img.eslzzyl.eu.org/86a9937b181c3ed58507a5d697544422.jpg" alt="当前借阅列表" width="50%" height="50%">

  历史借阅列表：

  <img src="https://img.eslzzyl.eu.org/406048ae223de58e854f7e0fbde05ed0.jpg" alt="历史借阅列表" width="50%" height="50%">

  荐购详情（图示的记录已被处理）：

  <img src="https://img.eslzzyl.eu.org/c700a55d451440fced1b39e15546af52.jpg" alt="历史借阅列表" width="50%" height="50%">

  个人信息：

  <img src="https://img.eslzzyl.eu.org/6f3c401c75267bf23800c76a451c6b4e.jpg" alt="历史借阅列表" width="50%" height="50%">

  管理员主页：

  <img src="https://img.eslzzyl.eu.org/5efcc6a7599c93ef41af5d112a907e9d.jpg" alt="历史借阅列表" width="50%" height="50%">

  管理员修改书籍信息（新增书籍样式一样）：

  <img src="https://img.eslzzyl.eu.org/c11d5e28c471fde55584e69828fe345a.jpg" alt="历史借阅列表" width="50%" height="50%">

  用户管理：

  <img src="https://img.eslzzyl.eu.org/c11d5e28c471fde55584e69828fe345a.jpg" alt="历史借阅列表" width="50%" height="50%">

  日志管理：

  <img src="https://img.eslzzyl.eu.org/2e65a25e0c27946f58fd16449d4fc2e5.jpg" alt="历史借阅列表" width="50%" height="50%">

  系统设置：

  <img src="https://img.eslzzyl.eu.org/6a78e8679465e2388ec2290a989023e1.jpg" alt="历史借阅列表" width="50%" height="50%">

</details>

## 后端技术实现

ASP.NET Core Web API，基于 .NET 8.0

数据库采用 SQLite

数据库连接采用 EntityFramework Core 框架

日志采用 Serilog 库实现，是直接抄的现成代码，日志写进一个 SQLite 数据库

## 前端技术实现

使用 Vuetify 3 框架，基于 Vue 3，组合式 API

能用 TypeScript 的地方都用了，如果遇到一些比较棘手的类型问题，会在局部改用 JavaScript

图表用了 [vue-chartjs](https://github.com/apertureless/vue-chartjs) 这个库，底层实际上是 [Chart.js](http://chartjs.cn/docs/latest/)。

## 如何运行本项目

### 后端

确保已经安装了 .NET 8 SDK。在 Windows 系统上，你需要在 Visual Studio Installer 中勾选“ASP.NET 和 Web 开发”工作负载；在其他系统上，从命令行安装 .NET Core SDK 即可。见 [此处](https://dotnet.microsoft.com/zh-cn/learn/aspnet/blazor-tutorial/install)

推荐使用的开发工具是：
- Microsoft Visual Studio 2022（为了支持 .NET 8.0，需要 17.8 或更新的版本），或者
- JetBrains Rider（为了支持 .NET 8.0，需要 2023.3 或更新的版本）

本项目的大部分代码是在 Rider 上编写的。

克隆项目后，使用 `dotnet` 命令行工具恢复依赖：

```shell
dotnet restore
```

然后使用 IDE 打开项目根目录下的 `OnlineLibrary.sln` 解决方案文件。

接下来需要初始化（Seed）数据库。

1. 暂时注释掉 `OnlineLibrary/Controller/SeedController.cs` 中 `SeedController` 类前面的 `[Authorize(Roles = RoleNames.Admin)]` 标记（从而跳过执行时的用户身份验证，因为用户数据在数据库里，而数据库还不存在）
2. 在命令行中切换到 OnlineLibrary 目录（即后端项目的根目录），执行

    ```shell
    dotnet ef database update --context ApplicationDbContext
    ```

    这会自动在 `./Data/` 目录下创建 `OnlineLibrary.db` 数据库并按顺序执行已有的一系列 Migrations。

    > 本项目引入了两个 `DbContext`，其中 `ApplicationDbContext` 为主数据库，`LogsDbContext` 为日志数据库。因此，在执行 `database update` 时必须明确指定针对哪个数据库进行操作。我们这里只创建主数据库。你可以在 `OnlineLibrary/Model/DatabaseContext` 目录找到本项目的两个 `DbContext`。

3. 在 `OnlineLibrary` 目录中执行 `dotnet run` 启动后端项目，你应该可以看到内容为“OnlineLibrary”的 ASCII Art。然后在 Swagger 页面依次执行 `/Seed/AuthData`、`/Seed/BookData` 和 `/Seed/SettingsDate` 3个 EndPoint，等待数据库导入数据。其中导入书籍数据的时间可能稍长，导入用户数据和导入设置数据应当很快完成。导入完毕后，你应该能够在 Response 中看到导入的数据量。

    > 这是我的第一个 .NET 项目，此前我完全没写过 C#。因此你可能会看到一些关于空值的警告。抱歉！

4. 按 Ctrl+C 中止后端运行，然后将 `SeedController` 类前面的 `[Authorize(Roles = RoleNames.Admin)]` 标记取消注释，正常启动项目即可。

数据库准备完成后，启动解决方案的 http 配置即可。看到”OnlineLibrary“的 ASCII Art 后，你可以访问 [http://localhost:5057/swagger/index.html](http://localhost:5057/swagger/index.html) 来打开 Swagger 页面。

`./Data/Logs.db` 文件可以随意删除，删除之后会重新创建。如果你觉得它有点大了，直接删除即可。当然旧的日志也就丢掉了。

> 前端默认向后端的 http 端口发请求，因此测试运行时应该启动 http 配置，而非 https 配置。如果希望修改前端的目标端口，可以到 `Client/src/plugins/util/axiosInstance.ts` 文件中修改。

### 前端

推荐使用的开发工具是：
- Microsoft VSCode，带有以下扩展：
  - TypeScript Vue Plugin（Volar）：`Vue.vscode-typescript-vue-plugin`
  - Vue Language Features（Volar）：`Vue.volar`
- JetBrains WebStorm

Rider 也通过官方插件对 Vue 和 TypeScript 有较好的支持，因此也可以用 Rider 做前端。

本项目前端的几乎全部代码都是在 VSCode 上编写的。

克隆项目后，打开 `Client` 目录即可。推荐使用的包管理器是 `pnpm`。安装依赖：
```shell
pnpm install
```

启动开发服务器（构建工具是 Vite）：
```shell
pnpm dev
```

服务器启动后，将自动显示前端的 URL。

如果你通过 `/Seed/AuthData/` 导入了用户数据，那么数据库中默认存在 4 个用户：`TestUser`、`TestModerator`、`TestAdmin` 和 `DeletedUser`。它们的密码都是 `123456`。前三种用户的权限依次提高，而 `DeletdUser` 是普通用户权限且不可登录。由于时间仓促，我没有为 `Moderator` 用户组添加前端功能，因此实际可用的用户只有 `User` 和 `Admin` 两类。

## 设计文档

见 [此处](./OnlineLibrary/Doc/设计文档.md)。在开发中后期，我没有再更新过这个文档，因此它的内容可能已经不准确，而且有缺失。实际功能请以 [功能](#功能) 这一节为准。

## 代码量统计

Commit [260f914](https://github.com/Eslzzyl/OnlineLibrary/commit/260f914c5facfebf904d6c2e6b57b9b185719934) 的代码量统计，统计工具是 [tokei](https://tokei.rs/)

### 前端

```
===============================================================================
 Language            Files        Lines         Code     Comments       Blanks
===============================================================================
 CSS                     1           12           11            0            1
 HTML                    1           16           13            0            3
 JSON                    3           69           69            0            0
 Markdown                1           60            0           33           27
 SVG                     1            6            6            0            0
 TypeScript             11          423          328           42           53
 YAML                    1         1124          974            0          150
-------------------------------------------------------------------------------
 Vue                    22          810          756            0           54
 |- CSS                  5           87           77            0           10
 |- HTML                22          838          817           10           11
 |- JavaScript          22         2394         2089           16          289
 (Total)                           4129         3739           26          364
===============================================================================
 Total                  41         2520         2157           75          288
===============================================================================
```

### 后端

排除了 `Migrations` 目录，因为该目录下的所有代码都是 EF Core 自动生成的。

```
===============================================================================
 Language            Files        Lines         Code     Comments       Blanks
===============================================================================
 C#                     38         2511         2176           35          300
 JSON                    3           71           71            0            0
 MSBuild                 1           33           29            0            4
-------------------------------------------------------------------------------
 Markdown                1          252            0          151          101
 |- JSON                 1           30           27            0            3
 (Total)                            282           27          151          104
===============================================================================
 Total                  43         2867         2276          186          405
===============================================================================
```

## 附录

本项目的图书数据（见 [./OnlineLibrary/Data/Books.csv](./OnlineLibrary/Data/Books.csv)）爬取自合肥工业大学图书馆的“热门推荐”页面，只爬取了“热门借阅”这个分类。这个页面应该只在校园网内部可用。

本项目在开发过程中得到了 ChatGPT 和 GitHub Copilot (Chat) 的极大帮助。
