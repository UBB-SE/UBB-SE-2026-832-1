namespace ClassLibrary.Models;

public sealed class Favorite
{
    public int FavoriteId { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = default!;

    public int FoodItemId { get; set; }

    public FoodItem FoodItem { get; set; } = default!;
}
