FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Estoque.API/Estoque.API.csproj", "Estoque.API/"]
COPY ["Estoque.Application/Estoque.Application.csproj", "Estoque.Application/"]
COPY ["Estoque.Domain/Estoque.Domain.csproj", "Estoque.Domain/"]
COPY ["Estoque.Infrastructure/Estoque.Infrastructure.csproj", "Estoque.Infrastructure/"]
RUN dotnet restore "Estoque.API/Estoque.API.csproj"
COPY . .
WORKDIR "/src/Estoque.API"
RUN dotnet build "Estoque.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Estoque.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Estoque.API.dll"]
