using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public class ShoppingItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int IngredientId { get; set; }

        [Required]
        [MaxLength(200)]
        public string IngredientName { get; set; } = string.Empty;

        [Required]
        public double QuantityGrams { get; set; }

        [Required]
        public bool IsChecked { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [ForeignKey("IngredientId")]
        public Ingredient? Ingredient { get; set; }
    }
}