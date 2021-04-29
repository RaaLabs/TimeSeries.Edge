#!/bin/bash
docker build -f ./Source/Dockerfile -t raalabs/connectors-healthmonitor . --build-arg CONFIGURATION="Debug"
iotedgehubdev start -d ./config/deployment.amd64.json -v