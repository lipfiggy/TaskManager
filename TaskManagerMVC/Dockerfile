#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TaskManagerMVC/TaskManagerMVC.csproj", "TaskManagerMVC/"]
COPY ["TaskManagerModels/TaskManagerModels.csproj", "TaskManagerModels/"]
COPY ["TaskManagerWebApi/TaskManagerWebApi.csproj", "TaskManagerWebApi/"]
RUN dotnet restore "TaskManagerMVC/TaskManagerMVC.csproj"
COPY . .
WORKDIR "/src/TaskManagerMVC"
RUN dotnet build "TaskManagerMVC.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskManagerMVC.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskManagerMVC.dll"]