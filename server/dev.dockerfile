FROM microsoft/dotnet:2.2-sdk

WORKDIR /vsdbg

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    unzip \
    && rm -rf /var/lib/apt/lists/* \
    && curl -sSL https://aka.ms/getvsdbgsh \
    | bash /dev/stdin -v latest -l /vsdbg

ENV DOTNET_USE_POLLING_FILE_WATCHER 1

WORKDIR /app/server

COPY CodePaint.WebApi.csproj ./app/server/
RUN dotnet restore ./app/server/CodePaint.WebApi.csproj

ENTRYPOINT dotnet watch run --urls=http://+:5021
