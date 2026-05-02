using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DTOs
{
    public class ShoppingItemDto
    {
        public int Id { get; set; }
        public string IngredientName { get; set; } = string.Empty;
        public double QuantityGrams { get; set; }
        public bool IsChecked { get; set; }
    }
}
