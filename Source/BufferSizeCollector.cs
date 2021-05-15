// Copyright (c) RaaLabs. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using RaaLabs.Edge.Modules.EventHandling;

namespace RaaLabs.Edge.Connectors.HealthMonitor
{
    /// <summary>
    /// Collecting buffer size
    /// </summary>
    public class BufferSizeCollector : IRunAsync, IProduceEvent<Events.HealthMonitorDatapointOutput>
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
        public BufferSizeCollector(ILogger logger, ConnectorConfiguration configuration)
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
                    var targetFolder = "/app/data";
                    var dirsize = Directory.GetFiles(targetFolder, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
                    var size = (float)dirsize / (float)1000000;

                    var bufferSize = new Events.HealthMonitorDatapointOutput
                    {
                        Source = "Edge",
                        Tag = "Buffersize",
                        Value = size,
                        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
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
    }
}