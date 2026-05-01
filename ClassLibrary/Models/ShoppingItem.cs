namespace ClassLibrary.Models;

public class ShoppingItem
{
    public int ShoppingItemId { get; set; }
    public User User { get; set; } = null!;
    public Ingredient Ingredient { get; set; } = null!;

    public double QuantityGrams { get; set; }

    public bool IsChecked { get; set; }
}
