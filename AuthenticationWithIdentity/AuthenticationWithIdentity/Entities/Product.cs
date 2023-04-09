namespace AuthenticationWithIdentity.Entities
{
    public partial class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }
        public string SKU { get; set; }
        public bool Deleted { get; set; }
    }
}
