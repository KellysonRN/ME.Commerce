using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using FluentAssertions.Extensions;
using ME.Commerce.Core.Features.ProductListing.Events;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PactNet;
using PactNet.Output.Xunit;
using PactNet.Verifier;
using Xunit;
using Xunit.Abstractions;

namespace ME.Commerce.Tests.ProductCatalog
{
    public class ProductCatalogPactTest : IDisposable
    {
        private static readonly Uri _providerUri = new("http://localhost:5000");

        private static readonly Uri _pactServerUri = new("http://localhost:9292");

        private static readonly JsonSerializerOptions _options =
            new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

        private readonly IHost _server;

        private readonly PactVerifier _verifier;

        public ProductCatalogPactTest(ITestOutputHelper output)
        {
            _server = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseEnvironment("Development");
                    webBuilder.UseUrls(_providerUri.ToString());
                    webBuilder.UseStartup<TestStartup>();
                })
                .Build();

            _server.Start();

            _verifier = new PactVerifier(
                "ProductCatalogApi",
                new PactVerifierConfig
                {
                    LogLevel = PactLogLevel.Debug,
                    Outputters = [new XunitOutput(output)]
                }
            );
        }

        public void Dispose()
        {
            _server.Dispose();
            _verifier.Dispose();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void EnsureProductApiHonoursPactWithConsumer()
        {
            Assert.True(IsServerUp(), "O servidor do provedor não está respondendo.");

            string pactPath = Path.Combine(
                "..",
                "..",
                "..",
                "..",
                "ME.Commerce.Tests",
                "pacts",
                "ShoppingCartApi-ProductCatalogApi.json"
            );

            File.Exists(pactPath).Should().BeTrue();

            _verifier
                .WithHttpEndpoint(_providerUri)
                .WithMessages(
                    scenarios =>
                    {
                        scenarios.Add(
                            "an event indicating that an product exists",
                            () => new ProductCreatedEvent(1)
                        );
                    },
                    _options
                )
                .WithFileSource(new FileInfo(pactPath))
                .WithProviderStateUrl(new Uri(_providerUri, "/provider-states"))
                .Verify();
        }

        [Fact]
        public void EnsureProductApiHonoursPactWithConsumerAndPublishResultsOnPactBroker()
        {
             _verifier
                    .WithHttpEndpoint(_providerUri)
                    .WithPactBrokerSource(_pactServerUri, options =>
                     {
                         options.ConsumerTags("main", "latest")
                                .ConsumerVersionSelectors(new ConsumerVersionSelector { DeployedOrReleased = true },
                                                          new ConsumerVersionSelector { MainBranch = true, Latest = true })
                                .EnablePending()
                                .PublishResults($"{DateTime.UtcNow}", publish =>
                                {
                                    publish.BuildUri(new Uri("https://ci.example.org/builds/1234"))
                                           .ProviderBranch("main")
                                           .ProviderTags("main", "latest");
                                });
                     })
                    .WithProviderStateUrl(new Uri(_providerUri, "/provider-states"))
                    //.WithFilter("description", "state")
                    .WithRequestTimeout(10.Seconds())
                    .WithSslVerificationDisabled()
                    .Verify();
        }


        private bool IsServerUp()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(
                        HttpMethod.Post,
                        new Uri(_providerUri, "/provider-states")
                    );
                    request.Content = new StringContent(
                        @"{
                    ""state"": ""There is a product with ID '1'"",
                    ""params"": { ""id"" : 1 }
                }",
                        Encoding.UTF8,
                        "application/json"
                    );

                    var response = client.SendAsync(request).Result;
                    var responseBody = response.Content.ReadAsStringAsync().Result;

                    Console.WriteLine(
                        $"HTTP Status Code: {(int)response.StatusCode} {response.StatusCode}"
                    );
                    Console.WriteLine("Response Body:");
                    Console.WriteLine(responseBody);

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
