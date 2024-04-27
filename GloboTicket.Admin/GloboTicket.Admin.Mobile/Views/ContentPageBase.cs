using GloboTicket.Admin.Mobile.ViewModels.Base;

namespace GloboTicket.Admin.Mobile.Views;

public class ContentPageBase : ContentPage
{
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is not IViewModelBase viewModel) return;

        await viewModel.InitializeAsyncCommand.ExecuteAsync(null);
    }
}
