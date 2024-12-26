
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src


COPY RedactorPDF.csproj ./
RUN dotnet restore "./RedactorPDF.csproj"

# Install required fonts for PDF generation (including Arial)
RUN apt-get update && apt-get install -y --no-install-recommends \
    fontconfig \
    curl \
    && curl -fsSL http://ftp.debian.org/debian/pool/contrib/m/msttcorefonts/ttf-mscorefonts-installer_3.7_all.deb -o ttf-mscorefonts-installer.deb \
    && dpkg -i ttf-mscorefonts-installer.deb || true \
    && fc-cache -f -v \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/* ttf-mscorefonts-installer.deb


COPY . ./  
WORKDIR "/src"
RUN dotnet build "./RedactorPDF.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RedactorPDF.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish ./


RUN apt-get update && apt-get install -y --no-install-recommends \
    fontconfig \
    curl \
    && curl -fsSL http://ftp.debian.org/debian/pool/contrib/m/msttcorefonts/ttf-mscorefonts-installer_3.7_all.deb -o ttf-mscorefonts-installer.deb \
    && dpkg -i ttf-mscorefonts-installer.deb || true \
    && fc-cache -f -v \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/* ttf-mscorefonts-installer.deb

ENTRYPOINT ["dotnet", "RedactorPDF.dll"]
