using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DTOs
{
    public class MealFilterDto
    {
        public string SearchTerm { get; set; } = string.Empty;

        public bool IsVegan { get; set; }
        public bool IsKeto { get; set; }
        public bool IsGlutenFree { get; set; }
        public bool IsLactoseFree { get; set; }
        public bool IsNutFree { get; set; }

        
        public bool IsFavoriteOnly { get; set; }

        
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}
