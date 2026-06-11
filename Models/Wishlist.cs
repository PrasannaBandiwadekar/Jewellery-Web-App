namespace NageshaJewellers.Models
{
    public class Wishlist
    {
        public int Id { get; set; }

        public string UserEmail { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }
    }
}