using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DTOs
{
    public class AddShoppingItemRequest
    {
        public string ItemName { get; set; } = string.Empty;
        public double Quantity { get; set; }
    }
}
