FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

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

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RuoYi.Admin.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RuoYi.Admin.dll"]
