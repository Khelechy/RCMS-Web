#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["RCMS_web.csproj", ""]
RUN dotnet restore "./RCMS_web.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "RCMS_web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RCMS_web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RCMS_web.dll"]