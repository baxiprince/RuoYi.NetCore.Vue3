﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
LABEL author="ouhuanhua" wechat="vbee_club" office="办公魔盒" data="2024-11-09"
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# =====构建指令
# cd xxxx/RuoYi.Net
# docker build -t ruoyi-admin -f RuoYi.Admin/Dockerfile .
# =====运行指令
# docker run -itd --name ruoyi-admin -p 10086:8080 ruoyi-admin:latest

# 第一阶段,构建
# ※注:如果有新增项目,根据COPY指令行添加即可
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RuoYi.Admin/RuoYi.Admin.csproj", "RuoYi.Admin/"]
COPY ["RuoYi.Common/RuoYi.Common.csproj", "RuoYi.Common/"]
COPY ["RuoYi.Data/RuoYi.Data.csproj", "RuoYi.Data/"]
COPY ["RuoYi.Framework/RuoYi.Framework.csproj", "RuoYi.Framework/"]
COPY ["RuoYi.Generator/RuoYi.Generator.csproj", "RuoYi.Generator/"]
COPY ["RuoYi.System/RuoYi.System.csproj", "RuoYi.System/"]
COPY ["RuoYi.Quartz/RuoYi.Quartz.csproj", "RuoYi.Quartz/"]
RUN dotnet restore "RuoYi.Admin/RuoYi.Admin.csproj"
COPY . .
WORKDIR "/src/RuoYi.Admin"
RUN dotnet build "RuoYi.Admin.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 第二阶段,发布
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RuoYi.Admin.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 第三阶段,复制发布后的程序到最终镜像
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RuoYi.Admin.dll"]
