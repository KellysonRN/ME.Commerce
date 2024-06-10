namespace ME.Commerce.Core.Features.ProductListing.Models
{
    public class Product
    {
        public int Id { get; private set; }

        public string Name { get; private set; }

        public decimal Price { get; private set; }

        public Product(int id, string name, decimal price)
        {
            Id = id;
            Name = name;
            Price = price;
        }
    }
}
