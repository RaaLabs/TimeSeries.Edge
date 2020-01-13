/*---------------------------------------------------------------------------------------------
 *  Copyright (c) RaaLabs. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Configuration;

namespace RaaLabs.TimeSeries.Edge
{
    /// <summary>
    /// Represents the configuration for <see cref="Connector"/>
    /// </summary>
    [Name("Connector")]
    public class ConnectorConfiguration : IConfigurationObject
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ConnectorConfiguration"/>
        /// </summary>
        /// <param name="sampling">The sampling rate</param>
        /// <param name="pingadress">The ping adress</param>
        /// <param name="pingtimeout">The ping timeout</param>

        public ConnectorConfiguration(int sampling, string pingadress, int pingtimeout)
        {
            Sampling = sampling;
            PingAdress = pingadress;
            PingTimeout = pingtimeout;
        }

        /// <summary>
        /// Gets the sampling rate that will be used for sampling buffersize
        /// </summary>
        public int Sampling { get; }
        /// <summary>
        /// Gets the ping adress
        /// </summary>
        public string PingAdress { get; }

           /// <summary>
        /// Gets the timeout for ping reply
        /// </summary>
        public int PingTimeout { get; }

    }
}