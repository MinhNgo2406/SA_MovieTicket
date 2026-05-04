namespace BookingService.Events
{
    public class PaymentSuccessEvent
    {
        public int OrderId { get; set; }

        public decimal Amount { get; set; }

        public string TransactionId { get; set; }
    }   
}
