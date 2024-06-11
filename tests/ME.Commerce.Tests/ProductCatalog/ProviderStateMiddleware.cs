using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ME.Commerce.Core.Features.ProductListing.Contracts;
using ME.Commerce.Core.Features.ProductListing.Models;
using Microsoft.AspNetCore.Http;

namespace ME.Commerce.Tests.ProductCatalog
{
    public class ProviderStateMiddleware
    {
        private static readonly JsonSerializerOptions _options =
            new() { PropertyNameCaseInsensitive = true };

        private readonly IDictionary<
            string,
            Func<IDictionary<string, object>, Task>
        > _providerStates;

        private readonly RequestDelegate _next;

        private readonly IProductRepository _productRepository;

        public ProviderStateMiddleware(RequestDelegate next, IProductRepository productRepository)
        {
            _next = next;
            _productRepository = productRepository;

            _providerStates = new Dictionary<string, Func<IDictionary<string, object>, Task>>
            {
                ["an order with ID {id} exists"] = EnsureEventExistsAsync
            };
        }

        private async Task EnsureEventExistsAsync(IDictionary<string, object> parameters)
        {
            JsonElement id = (JsonElement)parameters["id"];

            await _productRepository.InsertAsync(new Product(id.GetInt32(), "Laptop", 3000.0m));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!(context.Request.Path.Value?.StartsWith("/provider-states") ?? false))
            {
                await _next.Invoke(context);
                return;
            }

            context.Response.StatusCode = StatusCodes.Status200OK;

            if (context.Request.Method == HttpMethod.Post.ToString())
            {
                string jsonRequestBody;

                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    jsonRequestBody = await reader.ReadToEndAsync();
                }

                try
                {
                    ProviderState providerState = JsonSerializer.Deserialize<ProviderState>(
                        jsonRequestBody,
                        _options
                    );

                    if (!string.IsNullOrEmpty(providerState?.State))
                    {
                        await _providerStates[providerState.State].Invoke(providerState.Params);
                    }
                }
                catch (Exception e)
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync(
                        "Failed to deserialise JSON provider state body:"
                    );
                    await context.Response.WriteAsync(jsonRequestBody);
                    await context.Response.WriteAsync(string.Empty);
                    await context.Response.WriteAsync(e.ToString());
                }
            }
        }
    }
}
