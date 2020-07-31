﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace CustomerKeyStore
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Logging;
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .ConfigureLogging((context, logging) =>
            {
                logging.AddEventLog(eventLogSettings =>
                {
                });
            });
    }
}
