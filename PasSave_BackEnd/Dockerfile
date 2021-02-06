#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["PasSave_BackEnd.csproj", "PasSave_BackEnd/"]
RUN dotnet restore "PasSave_BackEnd/PasSave_BackEnd.csproj"
WORKDIR "/src/PasSave_BackEnd"
COPY . .
RUN dotnet build "PasSave_BackEnd.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PasSave_BackEnd.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet PasSave_BackEnd.dll
