FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

COPY conscoord-api/. ./
RUN dotnet restore
RUN dotnet build -c Release -o out

FROM build AS publish
RUN dotnet publish -c Release -o out/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /App/out .

RUN echo > .env

ENTRYPOINT ["dotnet", "conscoord.api.dll"]