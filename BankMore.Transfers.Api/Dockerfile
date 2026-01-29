FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["BankMore.Transfers.Api/BankMore.Transfers.Api.csproj", "BankMore.Transfers.Api/"]
COPY ["BankMore.Transfers.Application/BankMore.Transfers.Application.csproj", "BankMore.Transfers.Application/"]
COPY ["BankMore.Transfers.Domain/BankMore.Transfers.Domain.csproj", "BankMore.Transfers.Domain/"]
COPY ["BankMore.Transfers.Infra/BankMore.Transfers.Infra.csproj", "BankMore.Transfers.Infra/"]

RUN dotnet restore "BankMore.Transfers.Api/BankMore.Transfers.Api.csproj"

COPY . .
RUN dotnet publish "BankMore.Transfers.Api/BankMore.Transfers.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BankMore.Transfers.Api.dll"]
