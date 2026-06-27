namespace VOEConsulting.Flame.BasketContext.Application.Loyalty.Dtos
{
    public class LoyaltyAccountDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public int AvailablePoints { get; set; }
    }
}
