# 1. Image SDK pour la compilation
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 2. Copie des fichiers .csproj (Note l'absence du préfixe LOMS_Applications/)
# On considère que tu lances le build depuis le dossier "LOMS_Applications"
COPY ["LOMS_Applications_API/LOMS_Applications_API.csproj", "LOMS_Applications_API/"]
COPY ["LOMS_Applications_Buisness/LOMS_Applications_Buisness.csproj", "LOMS_Applications_Buisness/"]
COPY ["LOMS_Applications_DataAccess/LOMS_Applications_DataAccess.csproj", "LOMS_Applications_DataAccess/"]
COPY ["LOMS_Applications_Shared/LOMS_Applications_Shared.csproj", "LOMS_Applications_Shared/"]

# 3. Restauration
RUN dotnet restore "LOMS_Applications_API/LOMS_Applications_API.csproj"

# 4. Copie du reste du code
COPY . .

# 5. Build et Publication
WORKDIR "/src/LOMS_Applications_API"
RUN dotnet publish "LOMS_Applications_API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 6. Image Runtime finale
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8082
ENV ASPNETCORE_URLS=http://+:8082

ENTRYPOINT ["dotnet", "LOMS_Applications_API.dll"]