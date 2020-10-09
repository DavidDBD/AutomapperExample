using System.Collections.Generic;

namespace TestAutoMapCollection.DTOs
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        
        public List<OrderLineDTO> OrderLines { get; set; }

        public string Description { get; set; }
    }
}
