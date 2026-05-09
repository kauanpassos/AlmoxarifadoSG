FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Pedido.API/Pedido.API.csproj", "Pedido.API/"]
COPY ["Pedido.Application/Pedido.Application.csproj", "Pedido.Application/"]
COPY ["Pedido.Domain/Pedido.Domain.csproj", "Pedido.Domain/"]
COPY ["Pedido.Infrastructure/Pedido.Infrastructure.csproj", "Pedido.Infrastructure/"]
RUN dotnet restore "Pedido.API/Pedido.API.csproj"
COPY . .
WORKDIR "/src/Pedido.API"
RUN dotnet build "Pedido.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pedido.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pedido.API.dll"]
