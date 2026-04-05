# 1. Utilisation de l'image SDK .NET 8 pour la compilation
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 2. Copie des fichiers .csproj pour restaurer les dépendances (Optimisation du cache)
COPY ["LOMS_Employee_API/LOMS_Employee_API.csproj", "LOMS_Employee_API/"]
COPY ["LOMS_Employee_Business/LOMS_Employee_Business.csproj", "LOMS_Employee_Business/"]
COPY ["LOMS_Employee_DataAccess/LOMS_Employee_DataAccess.csproj", "LOMS_Employee_DataAccess/"]
COPY ["LOMS_Employee_Shared/LOMS_Employee_Shared.csproj", "LOMS_Employee_Shared/"]

# 3. Restauration des packages NuGet
RUN dotnet restore "LOMS_Employee_API/LOMS_Employee_API.csproj"

# 4. Copie de tout le reste du code source
COPY . .

# 5. Compilation du projet API
WORKDIR "/src/LOMS_Employee_API"
RUN dotnet build "LOMS_Employee_API.csproj" -c Release -o /app/build

# 6. Publication (Génération des fichiers prêts pour la prod)
FROM build AS publish
RUN dotnet publish "LOMS_Employee_API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 7. Image finale légère pour l'exécution (Runtime uniquement)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Port exposé par l'API (7175 comme dans tes réglages)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "LOMS_Employee_API.dll"]