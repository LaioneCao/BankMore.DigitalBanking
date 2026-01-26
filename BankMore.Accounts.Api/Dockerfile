FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["BankMore.Accounts.Api.csproj", "./"]
RUN dotnet restore "BankMore.Accounts.Api.csproj"

COPY . .
RUN dotnet publish "BankMore.Accounts.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BankMore.Accounts.Api.dll"]
