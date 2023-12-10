#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "MMBot.Discord/MMBot.Discord.csproj"
WORKDIR "/src/MMBot.Discord"
RUN dotnet build "MMBot.Discord.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MMBot.Discord.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MMBot.Discord.dll"]