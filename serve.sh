#!/usr/bin/env bash

~/.dotnet/tools/dotnet-serve --mime .wasm=application/wasm --mime .js=text/javascript --mime .json=application/json --directory ./bin/Release/net8.0/browser-wasm/AppBundle/
