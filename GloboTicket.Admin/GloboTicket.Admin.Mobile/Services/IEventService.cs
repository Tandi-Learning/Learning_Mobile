using GloboTicket.Admin.Mobile.Models;

namespace GloboTicket.Admin.Mobile.Services;

public interface IEventService
{
    Task<List<EventModel>> GetEvents();
    Task<EventModel?> GetEvent(Guid id);
    Task<bool> UpdateStatus(Guid id, EventStatusModel status);
    Task<bool> CreateEvent(EventModel model);
    Task<bool> EditEvent(EventModel model);
    Task<bool> DeleteEvent(Guid id);
}
