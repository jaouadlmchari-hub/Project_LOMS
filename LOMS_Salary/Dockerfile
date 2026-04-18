# 1. Image SDK pour la compilation
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 2. Copie des fichiers projets pour la restauration des dépendances
# On respecte ta structure de dossiers : LOMS_Salary/Nom_Du_Projet
COPY ["LOMS_Salary/LOMS_Salary_API/LOMS_Salary_API.csproj", "LOMS_Salary/LOMS_Salary_API/"]
COPY ["LOMS_Salary/LOMS_Salary_Buisness/LOMS_Salary_Buisness.csproj", "LOMS_Salary/LOMS_Salary_Buisness/"]
COPY ["LOMS_Salary/LOMS_Salary_DataAccess/LOMS_Salary_DataAccess.csproj", "LOMS_Salary/LOMS_Salary_DataAccess/"]
COPY ["LOMS_Salary/LOMS_Salary_Shared/LOMS_Salary_Shared.csproj", "LOMS_Salary/LOMS_Salary_Shared/"]

# 3. Restauration des packages NuGet
RUN dotnet restore "LOMS_Salary/LOMS_Salary_API/LOMS_Salary_API.csproj"

# 4. Copie de tout le code source vers l'image de build
COPY . .

# 5. Compilation et Publication
# On se place dans le dossier du nouveau service Salary
WORKDIR "/src/LOMS_Salary/LOMS_Salary_API"
RUN dotnet publish "LOMS_Salary_API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 6. Image Runtime finale (plus légère)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# 7. Configuration réseau
# On utilise le port 8084 pour le service Salary
EXPOSE 8084
ENV ASPNETCORE_URLS=http://+:8084

ENTRYPOINT ["dotnet", "LOMS_Salary_API.dll"]