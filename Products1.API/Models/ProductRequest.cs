namespace Products1.API.Models
{
    using Domain;

    public class ProductRequest : Product
    {
        public byte[] ImageArray { get; set; }
    }
}