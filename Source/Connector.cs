/*---------------------------------------------------------------------------------------------
 *  Copyright (c) RaaLabs. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
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
    /// Represents a <see cref="IAmAStreamingConnector">stream connector</see> 
    /// </summary>
    public class Connector : IAmAStreamingConnector
    {
        /// <inheritdoc/>

        public event DataReceived DataReceived = (tag, ValueTask, timestamp) => { };

        readonly ILogger _logger;
        readonly ConnectorConfiguration _configuration;


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


        /// <inheritdoc/>
        public Source Name => "Edge";

        /// <inheritdoc/>
        public void Connect()
        {
            Task.Run(Buffer);
            Task.Run(Ping);
        }
        /// <inheritdoc/>

        Task Buffer()
        {
            while (true)
            {
                try
                {
                    var targetFolder = "/app/data";
                    var dirsize = Directory.GetFiles(targetFolder, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
                    var size = (float)dirsize / (float)1000000;
                    DataReceived("Buffersize", size, Timestamp.UtcNow);
                    Thread.Sleep(_configuration.Sampling);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error while trying to get buffer");
                }
            }

        }

        /// <inheritdoc/>

        Task Ping()
        {
            while (true)
            {
                try
                {
                    List<long> pingreplyList = new List<long>();
                    Ping pingSender = new Ping();

                    for (int i = 0; i < 4; i++)
                    {
                        PingReply reply = pingSender.Send(_configuration.PingAdress, _configuration.PingTimeout);
                        if (reply.Status == IPStatus.Success)
                        {
                            pingreplyList.Add(reply.RoundtripTime);

                        }
                        else
                        {
                            _logger.Information($"Ping reply: '{reply.Status}'");
                        }
                    }
                    DataReceived("Pingreply", pingreplyList.Average(), Timestamp.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error while trying to ping");
                }
                Thread.Sleep(_configuration.Sampling);
            }
        }

    }
}