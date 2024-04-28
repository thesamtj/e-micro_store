using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basket.Application.Requests
{
    public class ShoppingCartItemRequest
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ImageFile { get; set; }
        public string ProductName { get; set; }
    }
}
