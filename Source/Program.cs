﻿// Copyright (c) RaaLabs. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RaaLabs.Edge.Modules.EventHandling;
using RaaLabs.Edge.Modules.EdgeHub;
using RaaLabs.Edge.Modules.Configuration;

namespace RaaLabs.Edge.Connectors.HealthMonitor
{
    static class Program
    {
        static void Main(string[] args)
        {
            var application = new ApplicationBuilder()
                .WithModule<EventHandling>()
                .WithModule<EdgeHub>()
                .WithModule<Configuration>()
                .WithTask<PingReplyCollector>()
                .WithTask<BufferSizeCollector>()
                .WithTask<MetricsCollector>()
                .Build();

            application.Run().Wait();
        }
    }
}