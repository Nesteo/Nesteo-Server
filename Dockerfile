### Compile server

# SDK-Image
FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build

# Copy source code to /src
WORKDIR /src
COPY . .

# Compile and pack server as self contained app
RUN dotnet publish /src/Nesteo.Server/Nesteo.Server.csproj -c Release -o /app --self-contained --runtime debian-x64

# Compile and pack sample data generation tool as self contained app
RUN dotnet publish /src/Nesteo.Server.SampleDataGenerator/Nesteo.Server.SampleDataGenerator.csproj -c Release -o /app --runtime debian-x64


### Compile management frontend

# NodeJS-Image
FROM node:latest AS build-web

# Copy source code to /src
COPY . /src
WORKDIR /src/Nesteo.Server

# Restore packages
RUN npm install --with-dev

# Run build task
RUN npx grunt default


### Build final image

# Minimal base image without dotnet runtime
FROM mcr.microsoft.com/dotnet/core/runtime-deps:3.0-buster-slim

# Copy self contained app to /app
WORKDIR /app
COPY --from=build /app .
COPY --from=build-web /src/Nesteo.Server/wwwroot ./wwwroot/

# Add curl for health check execution
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Server will run on port 80
EXPOSE 80

# Server entrypoint
ENTRYPOINT ["/app/Nesteo.Server"]
