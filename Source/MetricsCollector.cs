// Copyright (c) RaaLabs. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using RaaLabs.Edge.Modules.EventHandling;
using System.Net.Http;
using System.ComponentModel;

namespace RaaLabs.Edge.Connectors.HealthMonitor
{
    /// <summary>
    /// Represents a metrics collector for total incoming and outgoing bytes from the internet
    /// </summary>
    public class MetricsCollector : IRunAsync, IProduceEvent<Events.HealthMonitorDatapointOutput>
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
        public MetricsCollector(ILogger logger, ConnectorConfiguration configuration)
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
                    var parsedData = new TotalDataUsage();
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://"+ _configuration.DataTrafficScraper["ip"] + ":" + _configuration.DataTrafficScraper["port"] +"/metrics");
                        var responseTask = client.GetStringAsync("");
                        var responseResult = responseTask.Result;

                        parsedData = PrometheusParser.Parse<TotalDataUsage>(responseResult);
                    }
                    var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    var metricsDictionary = new Dictionary<string, double>();

                    foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(parsedData))
                    {
                        object value = property.GetValue(parsedData);
                        metricsDictionary.Add(property.Name, (double)value);
                    }

                    foreach (var metric in metricsDictionary)
                    {
                        var datapoint = new Events.HealthMonitorDatapointOutput
                        {
                            source = "Edge",
                            tag = metric.Key,
                            value = metric.Value,
                            timestamp = timestamp,
                        };
                        SendDatapoint(datapoint);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error while trying to collect metrics");
                }

                await Task.Delay(Int16.Parse(_configuration.DataTrafficScraper["scraping_interval"]));
            }

        }
    }
    class TotalDataUsage
    {
        public double TotalIncomming { get; set; }
        public double TotalOutgoing  { get; set; }

    }
}