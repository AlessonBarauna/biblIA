FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
RUN mkdir -p /data

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["BiBlIA.API/BiBlIA.API.csproj", "BiBlIA.API/"]
RUN dotnet restore "BiBlIA.API/BiBlIA.API.csproj"
COPY BiBlIA.API/ BiBlIA.API/
WORKDIR "/src/BiBlIA.API"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BiBlIA.API.dll"]
