using GloboTicket.Admin.Mobile.ViewModels;

namespace GloboTicket.Admin.Mobile.Views;

public partial class EventOverviewPage : ContentPageBase
{
	public EventOverviewPage(EventListOverviewViewModel viewModel)
	{
		InitializeComponent();
		this.BindingContext = viewModel;
	}
}