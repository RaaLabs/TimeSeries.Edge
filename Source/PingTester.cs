/*---------------------------------------------------------------------------------------------
 *  Copyright (c) RaaLabs. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using RaaLabs.TimeSeries.Modules;
using RaaLabs.TimeSeries.Modules.Connectors;

namespace RaaLabs.TimeSeries.Edge
{
    /// <summary>
    /// 
    /// </summary>
    public class PingTester
    {
        /// <inheritdoc/>
        public event DataReceived DataReceived = (tag, ValueTask, timestamp) => { };
        readonly ILogger _logger;
        readonly ConnectorConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of <see cref="PingTester"/>
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        /// <param name="configuration"><see cref="ConnectorConfiguration"/> holding all configuration</param>

        public PingTester(ConnectorConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

      

        /// <inheritdoc/>

        public async Task TestPingAsync()
        {
            while (true)
            {
                try
                {
                    Ping pingSender = new Ping();
                    PingReply reply = pingSender.Send(_configuration.PingAdress, _configuration.PingTimeout);

                    if (reply.Status == IPStatus.Success)
                    {
                        DataReceived("Pingreply", reply.RoundtripTime, Timestamp.UtcNow);
                        _logger.Information($"Pingreplyersize, {reply.RoundtripTime}, {Timestamp.UtcNow}");

                    }
                    else
                    {
                        _logger.Information($"Ping reply: '{reply.Status}'");
                    }
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