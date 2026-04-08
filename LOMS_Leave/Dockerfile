# 1. Image SDK pour la compilation
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 2. CORRECTION : On garde la structure de dossiers parent/enfant
# Source (PC) -> Destination (Docker)
COPY ["LOMS_Leave/LOMS_Leave_API/LOMS_Leave_API.csproj", "LOMS_Leave/LOMS_Leave_API/"]
COPY ["LOMS_Leave/LOMS_Leave_Buisness/LOMS_Leave_Buisness.csproj", "LOMS_Leave/LOMS_Leave_Buisness/"]
COPY ["LOMS_Leave/LOMS_Leave_DataAccess/LOMS_Leave_DataAccess.csproj", "LOMS_Leave/LOMS_Leave_DataAccess/"]
COPY ["LOMS_Leave/LOMS_Leave_Shared/LOMS_Leave_Shared.csproj", "LOMS_Leave/LOMS_Leave_Shared/"]

# 3. Restauration : Le chemin correspond maintenant exactement au COPY
RUN dotnet restore "LOMS_Leave/LOMS_Leave_API/LOMS_Leave_API.csproj"

# 4. Copie de tout le code source
COPY . .

# 5. Build et Publication
# On se place dans le dossier de l'API pour compiler
WORKDIR "/src/LOMS_Leave/LOMS_Leave_API"
RUN dotnet publish "LOMS_Leave_API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 6. Image Runtime finale
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Configuration du port 8083 (Interne Docker)
EXPOSE 8083
ENV ASPNETCORE_URLS=http://+:8083

ENTRYPOINT ["dotnet", "LOMS_Leave_API.dll"]