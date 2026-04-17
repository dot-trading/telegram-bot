FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["DotTelegramBot.slnx", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["src/TelegramBot.UI/TelegramBot.UI.csproj", "src/TelegramBot.UI/"]
COPY ["src/TelegramBot.Application/TelegramBot.Application.csproj", "src/TelegramBot.Application/"]
COPY ["src/TelegramBot.Domain/TelegramBot.Domain.csproj", "src/TelegramBot.Domain/"]
COPY ["src/TelegramBot.Infrastructure/TelegramBot.Infrastructure.csproj", "src/TelegramBot.Infrastructure/"]

RUN dotnet restore "DotTelegramBot.slnx"

COPY . .
RUN dotnet publish "src/TelegramBot.UI/TelegramBot.UI.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y curl && \
    curl -LO "https://dl.k8s.io/release/v1.29.0/bin/linux/amd64/kubectl" && \
    install -o root -g root -m 0755 kubectl /usr/local/bin/kubectl && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TelegramBot.UI.dll"]
