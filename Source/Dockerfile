FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine3.13 AS build-env
WORKDIR /app

VOLUME [ "/app/data" ]

COPY Source/ ./Source/

WORKDIR /app/Source

RUN dotnet restore --runtime alpine-x64

RUN dotnet publish -c Release -o out --no-restore 

FROM mcr.microsoft.com/dotnet/runtime:5.0-alpine3.13
RUN apk add --no-cache iputils

WORKDIR /app
COPY --from=build-env /app/Source/out ./
COPY --from=build-env /app/Source/data ./data

RUN adduser -Ds /bin/sh moduleuser
USER moduleuser

ENTRYPOINT ["dotnet", "RaaLabs.Edge.Connectors.HealthMonitor.dll"]
