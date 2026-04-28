namespace ClassLibrary.Models;

public class ShoppingItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;

    public double QuantityGrams { get; set; }

    public bool IsChecked { get; set; }
}
