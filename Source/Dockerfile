FROM mcr.microsoft.com/dotnet/sdk:5.0 AS base

ARG CONFIGURATION=Release

RUN echo Configuration = $CONFIGURATION

RUN if [ "$CONFIGURATION" = "Debug" ] ; then apt-get update && \
    apt-get install -y --no-install-recommends unzip procps && \
    rm -rf /var/lib/apt/lists/* \
    ; fi

RUN useradd -ms /bin/bash moduleuser

RUN apt-get update && apt-get install -y iputils-ping

USER moduleuser

RUN if [ "$CONFIGURATION" = "Debug" ] ; then curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l ~/vsdbg ; fi


FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

ARG CONFIGURATION

VOLUME [ "/app/data" ]

COPY Source/ ./Source/

WORKDIR /app/Source/

RUN dotnet restore

RUN dotnet publish -c $CONFIGURATION -o out


FROM base

WORKDIR /app
COPY --from=build-env /app/Source/out ./
COPY --from=build-env /app/Source/data ./data

ENTRYPOINT ["dotnet", "RaaLabs.Edge.Connectors.Edge.dll"]
