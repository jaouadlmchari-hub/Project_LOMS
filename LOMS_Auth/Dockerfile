# Image de base pour l'exécution
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8081 

# Image pour la compilation
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# --- CORRECTION ICI : Garde la même structure de dossiers ---
COPY ["LOMS_Auth/LOMS_Auth_API/LOMS_Auth_API.csproj", "LOMS_Auth/LOMS_Auth_API/"]
COPY ["LOMS_Auth/LOMS_Auth_Business/LOMS_Auth_Business.csproj", "LOMS_Auth/LOMS_Auth_Business/"]
COPY ["LOMS_Auth/LOMS_Auth_Shared/LOMS_Auth_Shared.csproj", "LOMS_Auth/LOMS_Auth_Shared/"]

# Le chemin du restore doit pointer vers le fichier copié juste au-dessus
RUN dotnet restore "LOMS_Auth/LOMS_Auth_API/LOMS_Auth_API.csproj"

# Compilation
COPY . .
WORKDIR "/src/LOMS_Auth/LOMS_Auth_API"
RUN dotnet build "LOMS_Auth_API.csproj" -c Release -o /app/build

# Publication
FROM build AS publish
RUN dotnet publish "LOMS_Auth_API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Image finale
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8081
ENTRYPOINT ["dotnet", "LOMS_Auth_API.dll"]