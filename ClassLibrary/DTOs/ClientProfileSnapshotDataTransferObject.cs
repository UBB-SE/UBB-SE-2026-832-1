namespace ClassLibrary.DTOs;

public sealed class ClientProfileSnapshotDataTransferObject
{
    public string CaloriesSummary { get; set; } = string.Empty;

    public string LatestSessionHint { get; set; } = string.Empty;

    public List<LoggedExerciseDataTransferObject> LoggedExercises { get; set; } = new();

    public List<MealDataTransferObject> Meals { get; set; } = new();
}
