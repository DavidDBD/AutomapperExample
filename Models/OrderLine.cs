namespace TestAutoMapCollection.Models
{
    public class OrderLine
    {
        public int OrderLineId { get; set; }
        
        public int OrderId { get; set; }
        
        public virtual Order Order { get; set; }

        public string Description { get; set; }
    }
}