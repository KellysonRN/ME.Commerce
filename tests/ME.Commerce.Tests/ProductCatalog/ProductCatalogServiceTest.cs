using FluentAssertions;
using ME.Commerce.Core.Features.ProductListing.Contracts;
using ME.Commerce.Core.Features.ProductListing.Models;
using Moq;

namespace ME.Commerce.Tests.ProductCatalog
{
    public class ProductCatalogServiceTest
    {
        [Fact]
        public async Task GetProductAsync_ShouldReturnExpectedProduct_WhenCalledWithValidProductId()
        {
            var productId = 1;
            var expectedProduct = new Product(id: productId, name: "Laptop", price: 1000.0m);

            var mockProductCatalogService = new Mock<IProductCatalogService>();

            mockProductCatalogService
                .Setup(service => service.GetProductAsync(productId))
                .ReturnsAsync(expectedProduct);

            var productCatalogService = mockProductCatalogService.Object;

            var product = await productCatalogService.GetProductAsync(productId);

            product.Should().BeEquivalentTo(expectedProduct);

            mockProductCatalogService.Verify(
                service => service.GetProductAsync(productId),
                Times.Once
            );
        }
    }
}
