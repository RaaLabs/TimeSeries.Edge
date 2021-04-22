# Connectors.HealthMonitor
[![.NET 5.0](https://github.com/RaaLabs/Connectors.HealthMonitor/actions/workflows/dotnet.yml/badge.svg)](https://github.com/RaaLabs/Connectors.HealthMonitor/actions/workflows/dotnet.yml)

This document describes the Connectors.HealthMonitor module for RaaLabs Edge.

## What does it do?
The module periodically checks the buffer size on Edge boxes and periodically sends a ping test.

The connector is producing events of type [OutputName("output")] and should be routed to [IdentityMapper](https://github.com/RaaLabs/IdentityMapper).

## Configuration
The connector needs a json config file with the following format. `sampling` and `pingTimeout` are measured in milliseconds.
````json
{
    "sampling": 5000,
    "pingAddress": "255.255.255.255",
    "pingTimeout": 120
}
````

## IoT Edge Deployment
### $edgeAgent
In your `deployment.json` file, you will need to add the module. For more details on modules in IoT Edge, go [here](https://docs.microsoft.com/en-us/azure/iot-edge/module-composition).

The module has persistent state and it is assuming that this is in the `data` folder relative to where the binary is running.
Since this is running in a containerized environment, the state is not persistent between runs. To get this state persistent, you'll
need to configure the deployment to mount a folder on the host into the data folder.

In your `deployment.json` file where you added the module, inside the `HostConfig` property, you should add the
volume binding.

```json
"Binds": [
    "<mount-path>:/app/data"
]
```

```json
{
    "modulesContent": {
        "$edgeAgent": {
            "properties.desired.modules.HealthMonitor": {
                "settings": {
                    "image": "<repo-name>/connectors-healthmonitor:<tag>",
                    "createOptions": "{\"HostConfig\":{\"Binds\":[\"<mount-path>:/app/data\"]}}"
                },
                "type": "docker",
                "version": "1.0",
                "status": "running",
                "restartPolicy": "always"
            }
        }
    }
}
```

For production setup, the bind mount can be replaced by a docker volume.

### $edgeHub
The routes in edgeHub can be specified like the example below.

```json
{
    "$edgeHub": {
        "properties.desired.routes.HealthMonitorToIdentityMapper": "FROM /messages/modules/HealthMonitor/outputs/* INTO BrokeredEndpoint(\"/modules/IdentityMapper/inputs/events\")"
    }
}
```
