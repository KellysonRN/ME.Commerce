using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ME.Commerce.Core.Features.ProductListing.Events;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Output.Xunit;
using PactNet.Verifier;
using Xunit;
using Xunit.Abstractions;

namespace ME.Commerce.Tests.ProductCatalog
{
    public class ProductCatalogPactTest : IDisposable
    {
        private static readonly Uri _providerUri = new("http://localhost:5000");

        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        private readonly IHost _server;

        private readonly PactVerifier _verifier;

        public ProductCatalogPactTest(ITestOutputHelper output)
        {
            _server = Host.CreateDefaultBuilder()
                              .ConfigureWebHostDefaults(webBuilder =>
                              {
                                  webBuilder.UseUrls(_providerUri.ToString());
                                  webBuilder.UseStartup<TestStartup>();
                              })
                              .Build();

            _server.Start();

            _verifier = new PactVerifier("Product Catalog", new PactVerifierConfig
            {
                LogLevel = PactLogLevel.Debug,
                Outputters = new List<IOutput>
                {
                    new XunitOutput(output)
                }
            });
        }

        public void Dispose()
        {
            _server.Dispose();
            _verifier.Dispose();
        }

        [Fact]
        public void Verify()
        {
            string pactPath = Path.Combine("..",
                                           "..",
                                           "..",
                                           "..",
                                           "ME.Commerce.Tests",
                                           "pacts",
                                           "me-commerce-product-catalog-api.json");

            _verifier
                .WithHttpEndpoint(_providerUri)
                .WithMessages(scenarios =>
                {
                    scenarios.Add("an event indicating that an order has been created", () => new ProductCreatedEvent(1));
                }, _options)
                .WithFileSource(new FileInfo(pactPath))
                .WithProviderStateUrl(new Uri(_providerUri, "/provider-states"))
                .Verify();
        }
    }
}
