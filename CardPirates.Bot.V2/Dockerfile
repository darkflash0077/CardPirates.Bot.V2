#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CardPirates.Bot.V2/CardPirates.Bot.V2.csproj", "CardPirates.Bot.V2/"]
RUN dotnet restore "CardPirates.Bot.V2/CardPirates.Bot.V2.csproj"
COPY . .
WORKDIR "/src/CardPirates.Bot.V2"
RUN dotnet build "CardPirates.Bot.V2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CardPirates.Bot.V2.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CardPirates.Bot.V2.dll"]