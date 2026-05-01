namespace ClassLibrary.DTOs;

public sealed class PreviousBestWeightsDataTransferObject
{
    public Dictionary<string, double> BestWeightsByExercise { get; set; } = new();
}
