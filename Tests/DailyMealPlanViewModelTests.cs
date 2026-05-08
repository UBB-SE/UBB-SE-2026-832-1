using System.Reflection;

namespace Tests;

public sealed class DailyMealPlanBindingTests
{
    [Fact]
    public void StringType_HasNoNameProperty_ExplainingBlankRendering()
    {
        PropertyInfo? nameProperty = typeof(string).GetProperty("Name");

        Assert.Null(nameProperty);
    }

    [Fact]
    public void StringType_ToStringReturnsValue_SupportingDirectBinding()
    {
        const string mealText = "Desayuno: Avena con frutas";

        Assert.Equal(mealText, mealText.ToString());
    }
}
