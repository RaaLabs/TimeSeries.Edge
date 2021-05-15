// Copyright (c) RaaLabs. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Serilog;
using RaaLabs.Edge.Modules.EventHandling;

namespace RaaLabs.Edge.Connectors.HealthMonitor
{
    /// <summary>
    /// Represents a ping collector
    /// </summary>
    public class PingReplyCollector : IRunAsync, IProduceEvent<Events.HealthMonitorDatapointOutput>
    {
        /// <inheritdoc/>
        public event EventEmitter<Events.HealthMonitorDatapointOutput> SendDatapoint;
        private readonly ILogger _logger;
        private readonly ConnectorConfiguration _configuration;


        /// <summary>
        /// Initializes a new instance of <see cref="Connector"/>
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        /// <param name="configuration"><see cref="ConnectorConfiguration"/> holding all configuration</param>
        public PingReplyCollector(ILogger logger, ConnectorConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Implmentation of <see cref="IRunAsync"/>.
        /// </summary>
        public async Task Run()
        {
            while (true)
            {
                try
                {
                    List<long> pingReplies = new List<long>();
                    Ping pingSender = new Ping();

                    for (int i = 0; i < 4; i++)
                    {
                        PingReply reply = pingSender.Send(_configuration.PingAddress, _configuration.PingTimeout);
                        if (reply.Status == IPStatus.Success)
                        {
                            pingReplies.Add(reply.RoundtripTime);
                        }
                        else
                        {
                            _logger.Information($"Ping reply: '{reply.Status}'");
                        }
                    }
                    var pingReply = new Events.HealthMonitorDatapointOutput
                    {
                        Source = "Edge",
                        Tag = "Pingreply",
                        Value = pingReplies.Average(),
                        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
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