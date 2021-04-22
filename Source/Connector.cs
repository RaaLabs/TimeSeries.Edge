// Copyright (c) RaaLabs. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using RaaLabs.Edge.Modules.EventHandling;


namespace RaaLabs.Edge.Connectors.HealthMonitor
{
    /// <summary>
    /// Represents a <see cref="IAmAStreamingConnector">stream connector</see> 
    /// </summary>
    public class Connector : IRunAsync, IProduceEvent<events.HealthMonitorDatapointOutput>
    {
        /// <inheritdoc/>
        public event EventEmitter<events.HealthMonitorDatapointOutput> SendDatapoint;

        private readonly ILogger _logger;

        private readonly ConnectorConfiguration _configuration;


        /// <summary>
        /// Initializes a new instance of <see cref="Connector"/>
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        /// <param name="configuration"><see cref="ConnectorConfiguration"/> holding all configuration</param>
        public Connector(ILogger logger, ConnectorConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Implmentation of <see cref="IRunAsync"/>.
        /// </summary>
        public async Task Run()
        {
            Task bufferTask = Buffer();
            Task pingTask = Ping();

            await Task.WhenAll(bufferTask, pingTask);
        }

        private async Task Buffer()
        {
            while (true)
            {
                try
                {
                    var targetFolder = "/app/data";
                    var dirsize = Directory.GetFiles(targetFolder, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
                    var size = (float)dirsize / (float)1000000;

                    var bufferSize = new events.HealthMonitorDatapointOutput
                    {
                        source = "Edge",
                        tag = "Buffersize",
                        value = size,
                        timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    };

                    SendDatapoint(bufferSize);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error while trying to get buffer");
                }

                await Task.Delay(_configuration.Sampling);
            }
        }

        private async Task Ping()
        {
            while (true)
            {
                try
                {
                    List<long> pingReplies = new List<long>();
                    Ping pingSender = new Ping();

                    for (int i = 0; i < 4; i++)
                    {
                        PingReply reply = pingSender.Send(_configuration.PingAdress, _configuration.PingTimeout);
                        if (reply.Status == IPStatus.Success)
                        {
                            pingReplies.Add(reply.RoundtripTime);
                        }
                        else
                        {
                            _logger.Information($"Ping reply: '{reply.Status}'");
                        }
                    }
                    var pingReply = new events.HealthMonitorDatapointOutput
                    {
                        source = "Edge",
                        tag = "Pingreply",
                        value = pingReplies.Average(),
                        timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    };

                    SendDatapoint(pingReply);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error while trying to ping");
                }

                await Task.Delay(_configuration.Sampling);
            }
        }
    }
}