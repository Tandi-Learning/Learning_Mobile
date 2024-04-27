using GloboTicket.Admin.Mobile.Models;

namespace GloboTicket.Admin.Mobile.Services;

public interface INavigationService
{
    Task GoToEventDetail(Guid selectedEventId);
    Task GoToAddEvent();
    Task GoToEditEvent(EventModel detailModel);
    Task GoToOverview();
    Task GoBack();
}
