namespace TestAutoMapCollection.DTOs
{
    public class OrderLineDTO
    {
        public int OrderLineId { get; set; }
        
        public OrderDTO Order { get; set; }

        public string Description { get; set; }
    }
}