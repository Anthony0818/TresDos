namespace TresDos.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }

        public Product(string name, string description, decimal price)
        {
            Validate(name, description, price);
        }
        public void Validate(string name, string description, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required");

            if (price < 0)
                throw new ArgumentException("Price cannot be negative");

            Name = name;
            Price = price;
            Description = description;
        }
    }
}
