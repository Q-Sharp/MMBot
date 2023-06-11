#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MMBot.Blazor/Server/MMBot.Blazor.Server.csproj", "MMBot.Blazor/Server/"]
COPY ["MMBot.Data/MMBot.Data.csproj", "MMBot.Data/"]
COPY ["MMBot.Data.Contracts/MMBot.Data.Contracts.csproj", "MMBot.Data.Contracts/"]
COPY ["MMBot.Blazor/Client/MMBot.Blazor.Client.csproj", "MMBot.Blazor/Client/"]
COPY ["MMBot.Blazor/Shared/MMBot.Blazor.Shared.csproj", "MMBot.Blazor/Shared/"]
RUN dotnet restore "MMBot.Blazor/Server/MMBot.Blazor.Server.csproj"
COPY . .
WORKDIR "/src/MMBot.Blazor/Server"
RUN dotnet build "MMBot.Blazor.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MMBot.Blazor.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MMBot.Blazor.Server.dll"]
