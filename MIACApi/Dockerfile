FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MIACApi.csproj", "Api/"]
RUN dotnet restore "Api/MIACApi.csproj"
COPY . ./Api
WORKDIR "/src/Api"
RUN dotnet build "MIACApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MIACApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MIACApi.dll"]
