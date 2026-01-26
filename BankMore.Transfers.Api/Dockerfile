FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["BankMore.Transfers.Api.csproj", "./"]
RUN dotnet restore "BankMore.Transfers.Api.csproj"

COPY . .
RUN dotnet publish "BankMore.Transfers.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BankMore.Transfers.Api.dll"]
