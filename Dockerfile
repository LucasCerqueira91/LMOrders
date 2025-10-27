FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/LM.Orders.Api/LM.Orders.Api.csproj", "src/LM.Orders.Api/"]
COPY ["src/LM.Orders.Application/LM.Orders.Application.csproj", "src/LM.Orders.Application/"]
COPY ["src/LM.Orders.Domain/LM.Orders.Domain.csproj", "src/LM.Orders.Domain/"]
COPY ["src/LM.Orders.Infrastructure/LM.Orders.Infrastructure/LM.Orders.Infrastructure.csproj", "src/LM.Orders.Infrastructure/LM.Orders.Infrastructure/"]

RUN dotnet restore "src/LM.Orders.Api/LM.Orders.Api.csproj"
COPY . .
WORKDIR "/src/src/LM.Orders.Api"
RUN dotnet build "LM.Orders.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LM.Orders.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LM.Orders.Api.dll"]
