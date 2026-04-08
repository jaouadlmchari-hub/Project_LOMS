# 1. Utilisation de l'image SDK .NET 8 pour la compilation
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 2. CORRECTION : La destination doit inclure le dossier parent pour correspondre au restore
COPY ["LOMS_Employee/LOMS_Employee_API/LOMS_Employee_API.csproj", "LOMS_Employee/LOMS_Employee_API/"]
COPY ["LOMS_Employee/LOMS_Employee_Business/LOMS_Employee_Business.csproj", "LOMS_Employee/LOMS_Employee_Business/"]
COPY ["LOMS_Employee/LOMS_Employee_DataAccess/LOMS_Employee_DataAccess.csproj", "LOMS_Employee/LOMS_Employee_DataAccess/"]
COPY ["LOMS_Employee/LOMS_Employee_Shared/LOMS_Employee_Shared.csproj", "LOMS_Employee/LOMS_Employee_Shared/"]

# 3. Restauration des packages NuGet (Le chemin correspond maintenant au COPY)
RUN dotnet restore "LOMS_Employee/LOMS_Employee_API/LOMS_Employee_API.csproj"

# 4. Copie de tout le reste du code source
COPY . .

# 5. Compilation du projet API
WORKDIR "/src/LOMS_Employee/LOMS_Employee_API"
RUN dotnet build "LOMS_Employee_API.csproj" -c Release -o /app/build

# 6. Publication
FROM build AS publish
RUN dotnet publish "LOMS_Employee_API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 7. Image finale légère
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Port interne Docker (8080)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "LOMS_Employee_API.dll"]