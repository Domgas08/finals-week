# Builds the Finals Week web app into a small image that only contains the runtime.
# Used by deploy/docker-compose.prod.yml. See docs/deploy.md.

# ---- Stage 1: build ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy only the project files first, then restore. Docker caches this step,
# so packages are downloaded again only when a .csproj changes.
COPY Directory.Build.props .editorconfig ./
COPY src/FinalsWeek.Core/FinalsWeek.Core.csproj src/FinalsWeek.Core/
COPY src/FinalsWeek.Application/FinalsWeek.Application.csproj src/FinalsWeek.Application/
COPY src/FinalsWeek.Web/FinalsWeek.Web.csproj src/FinalsWeek.Web/
RUN dotnet restore src/FinalsWeek.Web/FinalsWeek.Web.csproj

COPY src/ src/
RUN dotnet publish src/FinalsWeek.Web/FinalsWeek.Web.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

# ---- Stage 2: run ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# ASP.NET Core encrypts the login cookie with keys it keeps in this folder.
# The compose file mounts a volume here, so the keys survive a redeploy and
# users stay logged in. The folder must exist and belong to the "app" user.
RUN mkdir -p /home/app/.aspnet/DataProtection-Keys \
    && chown -R app:app /home/app/.aspnet

COPY --from=build /app/publish .

# Run as the non-root user that the .NET images provide.
USER app

# Listen on plain HTTP inside the container. Cloudflare handles HTTPS.
# FORWARDEDHEADERS_ENABLED makes the app trust the X-Forwarded-Proto header
# that Cloudflare sends, so it knows the browser used HTTPS (secure cookies,
# no redirect loop).
ENV ASPNETCORE_HTTP_PORTS=8080 \
    ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
EXPOSE 8080

ENTRYPOINT ["dotnet", "FinalsWeek.Web.dll"]
