// Copyright (c) RaaLabs. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RaaLabs.Edge.Modules.Configuration;

namespace RaaLabs.Edge.Connectors.HealthMonitor
{
    /// <summary>
    /// Represents the configuration for <see cref="Connector"/>
    /// </summary>
    [Name("connector.json")]
    [RestartOnChange]
    public class ConnectorConfiguration : IConfiguration
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ConnectorConfiguration"/>
        /// </summary>
        /// <param name="sampling">The sampling rate</param>
        /// <param name="pingAddress">The ping adress</param>
        /// <param name="pingTimeout">The ping timeout</param>
        /// <param name="dataTrafficScraper">The data metrics specifications</param>
        public ConnectorConfiguration(int sampling, string pingAddress, int pingTimeout, DataTrafficScraperConfiguration dataTrafficScraper)
        {
            Sampling = sampling;
            PingAddress = pingAddress;
            PingTimeout = pingTimeout;
            DataTrafficScraper = dataTrafficScraper;
        }

        /// <summary>
        /// Gets the sampling rate that will be used for sampling buffersize
        /// </summary>
        public int Sampling { get; }

        /// <summary>
        /// Gets the ping adress
        /// </summary>
        public string PingAddress { get; }

        /// <summary>
        /// Gets the timeout for ping reply
        /// </summary>
        public int PingTimeout { get; }

        /// <summary>
        /// Holds data traffic scraper configuration
        /// </summary>
        public DataTrafficScraperConfiguration DataTrafficScraper { get; }

    }
    public class DataTrafficScraperConfiguration
    {
        /// <summary>
        /// Gets the ip for the data traffic metrics collector
        /// </summary> 
        public string Ip { get; set; }
        /// <summary>
        /// Gets the port for the data traffic metrics collector
        /// </summary> 
        public string Port { get; set; }
        /// <summary>
        /// Gets the scraping interval for the data traffic metrics collector
        /// </summary> 
        public int ScrapingInterval { get; set; }
    }
}