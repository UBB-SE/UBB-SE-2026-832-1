namespace ClassLibrary.DTOs;

public sealed class UserDataDto
{
    public int UserDataId { get; set; }

    public int UserId { get; set; }

    public int Weight { get; set; }

    public int Height { get; set; }

    public int Age { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string Goal { get; set; } = string.Empty;

    public double BodyMassIndex { get; set; }

    public int CalorieNeeds { get; set; }

    public int ProteinNeeds { get; set; }

    public int CarbohydrateNeeds { get; set; }

    public int FatNeeds { get; set; }
}
