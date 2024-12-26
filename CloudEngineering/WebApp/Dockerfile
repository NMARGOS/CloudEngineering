
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src


COPY WebApp.csproj ./
RUN dotnet restore "./WebApp.csproj"




COPY . ./ 
WORKDIR "/src"
RUN dotnet build "./WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build




FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false



FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish ./
EXPOSE 5001
ENTRYPOINT ["dotnet", "WebApp.dll"]
