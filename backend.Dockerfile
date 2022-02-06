FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

RUN apt-get update && apt-get install -y jq

COPY ./backend/DigitalSkynet.Boilerplate.sln ./backend/nuget.config ./
COPY ./backend/DigitalSkynet.Boilerplate.Api/DigitalSkynet.Boilerplate.Api.csproj ./DigitalSkynet.Boilerplate.Api/
COPY ./backend/DigitalSkynet.Boilerplate.Core/DigitalSkynet.Boilerplate.Core.csproj ./DigitalSkynet.Boilerplate.Core/
COPY ./backend/DigitalSkynet.Boilerplate.Data/DigitalSkynet.Boilerplate.Data.csproj ./DigitalSkynet.Boilerplate.Data/
COPY ./backend/DigitalSkynet.Boilerplate.Domain/DigitalSkynet.Boilerplate.Domain.csproj ./DigitalSkynet.Boilerplate.Domain/
COPY ./backend/DigitalSkynet.Boilerplate.Dtos/DigitalSkynet.Boilerplate.Dtos.csproj ./DigitalSkynet.Boilerplate.Dtos/
COPY ./backend/DigitalSkynet.Boilerplate.Foundation/DigitalSkynet.Boilerplate.Foundation.csproj ./DigitalSkynet.Boilerplate.Foundation/
COPY ./backend/DigitalSkynet.Boilerplate.ViewModels/DigitalSkynet.Boilerplate.ViewModels.csproj ./DigitalSkynet.Boilerplate.ViewModels/
COPY ./backend/DigitalSkynet.Boilerplate.Tests/DigitalSkynet.Boilerplate.Tests.csproj ./DigitalSkynet.Boilerplate.Tests/
COPY ./backend/DigitalSkynet.Boilerplate.Jobs/DigitalSkynet.Boilerplate.Jobs.csproj ./DigitalSkynet.Boilerplate.Jobs/
COPY ./backend/DigitalSkynet.Boilerplate.AdminCli/DigitalSkynet.Boilerplate.AdminCli.csproj ./DigitalSkynet.Boilerplate.AdminCli/

ARG NUGET_LOGIN=gitlab
ARG NUGET_PASSWORD=gitlabci

RUN sed -i "s|</configuration>|<packageSourceCredentials><Gitlab><add key=\"Username\" value=\"${NUGET_LOGIN}\" /><add key=\"ClearTextPassword\" value=\"${NUGET_PASSWORD}\" /></Gitlab></packageSourceCredentials></configuration>|" /src/nuget.config

RUN dotnet restore --configfile /src/nuget.config

COPY ./backend ./

RUN dotnet publish /src/DigitalSkynet.Boilerplate.AdminCli/DigitalSkynet.Boilerplate.AdminCli.csproj -c Release -o /build
RUN dotnet publish /src/DigitalSkynet.Boilerplate.Api/DigitalSkynet.Boilerplate.Api.csproj -c Release -o /build

ARG COMMIT_SHA=unknown
ARG COMMIT_REF_NAME=unknown

RUN cat /build/appsettings.json \
    | jq ".Version.Hash=\"${COMMIT_SHA}\"" \
    | jq ".Version.Branch=\"${COMMIT_REF_NAME}\"" \
    | jq ".Version.BuildDate=\"$(date +%FT%TZ)\"" \
    > /build/appsettings.tmp.json \
    && mv /build/appsettings.tmp.json /build/appsettings.json

#####

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS result
WORKDIR /app
EXPOSE 80
HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 CMD [ "curl -f http://localhost/health/liveness || exit 1" ]

RUN apt-get update && apt-get install -y libgdiplus jq

ENTRYPOINT [ "/bin/bash" ]
CMD [ "/app/backend-start.sh" ]

COPY ./deploy/backend-start.sh /app/
COPY --from=build /build /app
