using ClassLibrary.DTOs;

namespace WebAPI.IServices;

public interface ICalendarExportService
{
    Task<CalendarExportResponseDataTransferObject> GenerateCalendarAsync(GenerateCalendarRequestDataTransferObject request);
    Task<SaveCalendarResponseDataTransferObject> SaveCalendarAsync(SaveCalendarRequestDataTransferObject request);
}
