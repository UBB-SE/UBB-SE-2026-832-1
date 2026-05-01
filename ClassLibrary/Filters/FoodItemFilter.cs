namespace ClassLibrary.Filters;

public sealed class FoodItemFilter
{
    public bool IsKeto { get; set; }

    public bool IsVegan { get; set; }

    public bool IsNutFree { get; set; }

    public bool IsLactoseFree { get; set; }

    public bool IsGlutenFree { get; set; }

    public bool IsFavoriteOnly { get; set; }

    public string SearchTerm { get; set; } = string.Empty;
}
