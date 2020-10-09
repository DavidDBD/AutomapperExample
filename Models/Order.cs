using System.Collections.Generic;
using System.Net.Http.Headers;

namespace TestAutoMapCollection.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        
        public virtual List<OrderLine> OrderLines { get; set; }

        public string Description { get; set; }
    }
}
