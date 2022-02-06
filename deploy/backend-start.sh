#!/bin/bash

chmod +x /app/DigitalSkynet.Boilerplate.AdminCli

echo "Performing migrations if possible and running the app"
/app/DigitalSkynet.Boilerplate.AdminCli migrate --direction=up || exit 1

dotnet /app/DigitalSkynet.Boilerplate.Api.dll
