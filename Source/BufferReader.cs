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
    public class BufferReader
    {
        /// <inheritdoc/>
        public event DataReceived DataReceived = (tag, ValueTask, timestamp) => { };
        private readonly ConnectorConfiguration _configuration;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="BufferReader"/>
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        /// <param name="configuration"><see cref="ConnectorConfiguration"/> holding all configuration</param>
        public BufferReader(ConnectorConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }



        /// <inheritdoc/>
        public async Task ReadBufferSizeAsync()
        {
            while (true)
            {
                var targetFolder = "/app/data";
                targetFolder = "/data";
                var dirsize = Directory.GetFiles(targetFolder, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
                var size = (float)dirsize / (float)1000000;
                DataReceived("Buffersize", size, Timestamp.UtcNow);
                _logger.Information($"Buffersize, {size}, {Timestamp.UtcNow}");
                await Task.Delay(_configuration.Sampling);            
                }
        }

    }
}