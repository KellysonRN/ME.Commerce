using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FluentAssertions;
using ME.Commerce.Core.Features.ProductListing.Models;
using ME.Commerce.Infrastructure;
using Moq;
using PactNet;
using PactNet.Output.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace ME.Commerce.Tests.ShoppingCart
{
    public class ShoppinCartPactTest
    {
        private readonly IPactBuilderV4 _pactBuilder;

        private readonly Mock<IHttpClientFactory> _mockFactory = new();

        public ShoppinCartPactTest(ITestOutputHelper output)
        {
            PactConfig config = new PactConfig
            {
                PactDir = "../../../pacts/",
                Outputters =
                [
                    new XunitOutput(output)
                ],
                DefaultJsonSettings = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                },
                LogLevel = PactLogLevel.Debug
            };

            _pactBuilder = Pact.V4("ShoppingCartApi", "ProductCatalogApi", config).WithHttpInteractions();

        }

        [Fact]
        public async Task GET_Product_WhenProductExists_ReturnsTheProduct()
        {
            var expected = new Product(1, "Laptop", 3000.0m);

            _pactBuilder
                .UponReceiving("A GET request to retrieve the product")
                    .Given($"There is a product with ID '1'", new Dictionary<string, string> { ["id"] = "1" })
                    .WithRequest(HttpMethod.Get, "/api/products/1")
                    .WithHeader("Accept", "application/json")
                .WillRespond()
                    .WithStatus(HttpStatusCode.OK)
                .WithJsonBody(
                    new
                    {
                        Id = PactNet.Matchers.Match.Integer(expected.Id),
                        name = PactNet.Matchers.Match.Type(expected.Name),
                        price = PactNet.Matchers.Match.Type(expected.Price)
                    }
                );

            await _pactBuilder.VerifyAsync(async ctx =>
            {
                _mockFactory
                    .Setup(f => f.CreateClient("Products"))
                    .Returns(() => new HttpClient
                    {
                        BaseAddress = ctx.MockServerUri,
                        DefaultRequestHeaders =
                        {
                            Accept = { MediaTypeWithQualityHeaderValue.Parse("application/json") }
                        }
                    });

                var client = new ProductsClient(_mockFactory.Object);

                Product product = await client.GetProductAsync(1);

                product.Should().Be(expected);
            });
        }
    }
}
