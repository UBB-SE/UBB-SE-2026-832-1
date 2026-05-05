using System;

namespace ClassLibrary.DTOs;

public sealed class GetAvailableWorkoutsRequestDataTransferObject
{
    public int ClientId { get; set; }

    public TimeSpan Timeout { get; set; }
}
