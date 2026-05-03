namespace ClassLibrary.DTOs;

public sealed class CreateNutritionPlanRequestDataTransferObject
{
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int ClientId { get; set; }
}
