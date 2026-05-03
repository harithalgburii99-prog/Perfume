FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PerfumeStore.csproj", "./"]
RUN dotnet restore "PerfumeStore.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "PerfumeStore.csproj" -c Release -o /app/build
RUN dotnet publish "PerfumeStore.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

# Note: The database file "app.db" will be created automatically in /app.
# If using a persistent volume in Railway, map it to a folder and update connection string.

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PerfumeStore.dll"]
