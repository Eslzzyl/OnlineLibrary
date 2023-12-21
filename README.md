# 网上图书馆

## 基于 ASP.NET Core 和 Vue.js 的网上图书馆项目

本项目是合肥工业大学宣城校区 2023-2024 学年秋季学期计算机科学与技术专业 20 级开设的《实习与实训》课程的项目。

本项目是一个完整的前后端分离 Web 项目，但是功能可能比较简陋，因为时间不是很充裕，而且我也不太想在一个 Web 项目上浪费太多时间。

> 本项目的开发已经基本完成。在课程设计结束之后，我会移除这条信息。

- [网上图书馆](#网上图书馆)
  - [基于 ASP.NET Core 和 Vue.js 的网上图书馆项目](#基于-aspnet-core-和-vuejs-的网上图书馆项目)
  - [功能](#功能)
    - [登录和注册](#登录和注册)
    - [用户部分](#用户部分)
    - [管理员部分](#管理员部分)
    - [杂项](#杂项)
  - [截图](#截图)
  - [后端技术实现](#后端技术实现)
  - [前端技术实现](#前端技术实现)
  - [如何运行本项目](#如何运行本项目)
    - [后端](#后端)
    - [前端](#前端)
  - [部署](#部署)
    - [前端](#前端-1)
      - [修改请求 URL](#修改请求-url)
      - [编译项目](#编译项目)
      - [配置 Nginx](#配置-nginx)
    - [后端](#后端-1)
      - [发布项目](#发布项目)
      - [在服务器安装 ASP.NET 运行时](#在服务器安装-aspnet-运行时)
      - [运行后端](#运行后端)
      - [Nginx 反向代理](#nginx-反向代理)
  - [设计文档](#设计文档)
  - [代码量统计](#代码量统计)
    - [前端](#前端-2)
    - [后端](#后端-2)
  - [附录](#附录)


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

4. 按 Ctrl+C 中止后端运行，然后将 `SeedController` 类前面的 `[Authorize(Roles = RoleNames.Admin)]` 标记取消注释，之后正常启动项目即可。

数据库准备完成后，启动解决方案的 http 配置。看到”OnlineLibrary“的 ASCII Art 后，你可以访问 [http://localhost:5057/swagger/index.html](http://localhost:5057/swagger/index.html) 来打开 Swagger 页面。

`./Data/Logs.db` 文件可以随意删除，删除之后会重新创建。如果你觉得它有点大了，可以直接删除。当然旧的日志也就丢掉了。

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

## 部署

本节介绍如何将项目部署到一台 Ubuntu 22.04 服务器上，使用 Nginx 作为 Web 服务器和反向代理服务器。

本项目是前后端分离项目，前后端可以独立部署。为简单起见，下面的说明中假设前端部署在 `example.com/library`，后端部署在 `example.com/library/api`。

### 前端

#### 修改请求 URL

你需要首先确定后端 API 的 URL。在本例中，这个 URL 是 `https://example.com/library/api`。然后修改 `Client/src/plugins/util/axiosInstance.ts` 文件，将以下代码段

```typescript
//使用axios下面的create([config])方法创建axios实例，其中config参数为axios最基本的配置信息。
const axiosInstance = axios.create({
  baseURL: 'http://localhost:5057', //请求后端数据的基本地址，自定义
  timeout: 5000                     //请求超时设置，单位ms
})
```

改成对应的 URL，如

```typescript
//使用axios下面的create([config])方法创建axios实例，其中config参数为axios最基本的配置信息。
const axiosInstance = axios.create({
  baseURL: 'https://example.com/library/api', //请求后端数据的基本地址，自定义
  timeout: 5000                     //请求超时设置，单位ms
})
```

然后保存。

> 强烈建议使用 https 访问后端 API，详见 [后文](#nginx-反向代理)。

#### 编译项目

进入 `Client` 目录，执行

```shell
pnpm build
```

这会编译整个前端项目，生成完全静态的页面，并在 `Client` 目录中生成一个 `dist` 目录。将该目录整体上传到服务器的某个位置，比如 `/home/eslzzyl/WorkSpace/OnlineLibrary/`。

#### 配置 Nginx

然后，在服务器 Nginx 的根 `server` 块（`example.com` 块）中进行如下配置：

```nginx
location /library {
  alias /home/eslzzyl/WorkSpace/OnlineLibrary/dist/;
  index index.html;
  try_files $uri $uri/ =404;
}
```

这会把对 `example.com/library` 的请求映射到 `/home/eslzzyl/WorkSpace/OnlineLibrary/dist/`。由于编译好的前端是完全静态的，因此直接使用 Nginx 作为 Web 服务器即可。

SSL 相关的配置与本项目关系不大，此处不再详述。

配置完成后，重载 Nginx 配置：

```bash
sudo nginx -s reload
```

然后访问前端，查看网页是否正常显示。如果没有配置后端，会登录不上账号，这是正常的。

### 后端

#### 发布项目

进入 `OnlineLibrary` 目录，执行

```shell
dotnet publish --no-self-contained --ucr -a x64 --os linux
```

> `--no-self-contained` 表示发布文件不带运行时，`--ucr` 表示以开发的 SDK 版本作为运行时版本（在本项目中，是 .NET 8.0），`-a` 指定发布目标的体系结构，`--os` 指定发布目标的操作系统。

你可以在 `OnlineLibrary/bin/Release/net8.0/linux-x64` 中找到发布文件。将 `linux-x64` 目录整体上传到服务器的某个位置，比如 `/home/eslzzyl/WorkSpace/OnlineLibrary/`（和前端放一起）。建议压缩后上传到服务器再解压。

#### 在服务器安装 ASP.NET 运行时

然后，你需要在服务器安装 ASP.NET 8.0 运行时。Ubuntu 有多个 .NET 运行时软件源，我推荐使用由微软提供的那个。因为目前只有微软源支持在 Ubuntu 22.04 上安装 .NET 8.0 运行时。关于添加微软 .NET 软件源，请参考 [此处](https://learn.microsoft.com/zh-cn/dotnet/core/install/linux-ubuntu#register-the-microsoft-package-repository)。添加后请参考 [此处](https://learn.microsoft.com/zh-cn/dotnet/core/install/linux-ubuntu-2204) 来安装 ASP.NET 运行时。

#### 运行后端

安装好运行时后，就可以运行后端了。进入 `linux-x64/publish` 目录，先为 `OnlineLibrary` 可执行文件添加可执行权限：

```bash
chmod +x ./OnlineLibrary
```

然后，你需要使用某种终端复用工具，如 `screen` 或 `tmux`，来复用终端。这样，当你断开 SSH 连接后，后端项目会保持正常运行。

以 `tmux` 为例，执行

```bash
tmux new -s library
```

来创建一个名为 `library` 的新 `session`。然后使用

```bash
tmux a -t library
```

来进入这个 `session`。在该 `session` 中，执行

```bash
./OnlineLibrary
```

来运行后端项目。你可能会发现程序异常终止，因为无法在 SQLite 数据库中找到 Book 表。这是正常的，因为我们还没有加入数据库。

将本地初始化好的数据库文件 `OnlineLibrary.db` 上传到服务器的 `linux-x64/publish/Data` 目录中。`Data` 目录应该会在程序第一次执行并异常终止之后自动生成。

再次运行后端项目。如果一切正常，你可以很快看到书籍数量统计和“OnlineLibrary”的 ASCII Art。

你可以通过 `tmux` 的 `detach` 快捷键来离开 `library` `session`，同时保持其中的程序继续执行。这个快捷键默认是 `C-b d`（先按下 `Ctrl+B`，松开，再按下 `D`）。

为了在外网访问后端，需要在 Nginx 中配置反向代理。尽管 ASP.NET Core 自带的 Web 服务器 `Kestrel` 也可以处理外网请求（需要额外的配置），但仍然推荐用反向代理。

#### Nginx 反向代理

在服务器 Nginx 的根 `server` 块（`example.com` 块）中进行如下配置：

```nginx
location /library/api {
  proxy_ssl_server_name on;
  rewrite ^/library/api(.*)$ $1 break;
  proxy_pass http://127.0.0.1:5000/;
  proxy_redirect off;
  proxy_set_header Host $host;
  proxy_set_header X-Real-IP $remote_addr;
  proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
  proxy_set_header X-Forwarded-Proto $scheme;
}
```

ASP.NET Core WebAPI 的默认部署端口是 5000，因此这里把对 `example.com/library/api` 的请求转发到了 `127.0.0.1:5000`。

> 我不知道 5000 这个端口是在后端的何处配置的，我通过网上搜索得到了这个端口，如果你需要修改，可能得自行查看修改方法。

> **安全性问题**：如果你正在公网部署这个项目，那么我强烈建议你为后端服务器配置 SSL，因为本项目登录和注册时的密码是明文传输的，使用 http 传输密码可能很不安全。或者，不要在本项目中使用自己其他账号的密码。

完成后，重载 Nginx 配置，然后访问前端并登录账号，查看后端是否正常工作。

## 设计文档

见 [此处](./OnlineLibrary/Doc/设计文档.md)。在开发中后期，我没有再更新过这个文档，因此它的内容可能已经不准确，而且有缺失。实际功能请以 [功能](#功能) 这一节为准。

## 代码量统计

Commit [53fb6ab](https://github.com/Eslzzyl/OnlineLibrary/commit/53fb6ab035eb352d957dab12145395c9bef7ab8a) 的代码量统计，统计工具是 [tokei](https://tokei.rs/)

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
 TypeScript             11          424          329           42           53
 YAML                    1         1124          974            0          150
-------------------------------------------------------------------------------
 Vue                    22          810          756            0           54
 |- CSS                  5           87           77            0           10
 |- HTML                22          838          817           10           11
 |- JavaScript          22         2394         2089           16          289
 (Total)                           4129         3739           26          364
===============================================================================
 Total                  41         2521         2158           75          288
===============================================================================
```

### 后端

排除了 `Migrations` 目录，因为该目录下的所有代码都是 EF Core 自动生成的。

```
===============================================================================
 Language            Files        Lines         Code     Comments       Blanks
===============================================================================
 C#                     38         2524         2186           35          303
 JSON                    3           71           71            0            0
 MSBuild                 1           33           29            0            4
-------------------------------------------------------------------------------
 Markdown                1          252            0          151          101
 |- JSON                 1           30           27            0            3
 (Total)                            282           27          151          104
===============================================================================
 Total                  43         2880         2286          186          408
===============================================================================
```

## 附录

本项目的图书数据（见 [./OnlineLibrary/Data/Books.csv](./OnlineLibrary/Data/Books.csv)）爬取自合肥工业大学图书馆的“热门推荐”页面，只爬取了“热门借阅”这个分类。这个页面应该只在校园网内部可用。

本项目在开发过程中得到了 ChatGPT 和 GitHub Copilot (Chat) 的极大帮助。
