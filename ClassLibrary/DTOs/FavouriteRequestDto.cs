using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DTOs
{
    public class FavoriteRequestDto
    {
        public int UserId { get; set; }
        public int MealId { get; set; }
        public bool IsFavorite { get; set; }
    }
}
