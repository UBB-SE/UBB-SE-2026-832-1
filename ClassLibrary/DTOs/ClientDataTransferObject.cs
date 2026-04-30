namespace ClassLibrary.DTOs;

public sealed class ClientDataTransferObject
{
    public int ClientId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public double Weight { get; set; }

    public double Height { get; set; }

    public string PrimaryGoal { get; set; } = string.Empty;
}
