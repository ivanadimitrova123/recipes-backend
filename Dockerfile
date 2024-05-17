# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /source
COPY . .
RUN dotnet restore "./recipes-backend/recipes-backend.csproj" --disable-parallel
RUN dotnet publish "./recipes-backend/recipes-backend.csproj" -c release -o /app --no-restore

# Serve Stage
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal
WORKDIR /app
COPY --from=build /app ./

EXPOSE 7222

ENTRYPOINT ["dotnet", "recipes-backend.dll"]