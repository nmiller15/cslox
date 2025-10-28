#!/bin/bash
cd ~/Projects/cslox/cslox.App

dotnet publish -r osx-arm64 -c Release /p:PublishSingleFile=true --self-contained true

cp ~/Projects/cslox/cslox.App/bin/Release/net9.0/osx-arm64/publish/* /usr/local/bin/

