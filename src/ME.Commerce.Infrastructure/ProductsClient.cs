using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;


using ME.Commerce.Core.Features.ProductListing.Models;

namespace ME.Commerce.Infrastructure
{
    public class ProductsClient : IProductsClient
    {
        private static readonly JsonSerializerOptions Options =
            new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };

        private readonly IHttpClientFactory factory;

        public ProductsClient(IHttpClientFactory factory)
        {
            this.factory = factory;
        }

        public async Task<Product> GetProductAsync(int productId)
        {
            using HttpClient client = factory.CreateClient("Products");

            Product product = await client.GetFromJsonAsync<Product>(
                $"/api/products/{productId}",
                Options
            );
            return product;
        }
    }
}
