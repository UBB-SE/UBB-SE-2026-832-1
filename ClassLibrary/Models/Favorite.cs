namespace ClassLibrary.Models;

public sealed class Favorite
{
    public int FavoriteId { get; set; }

    public User User { get; set; } = default!;

    public FoodItem FoodItem { get; set; } = default!;
}
