using CommunityToolkit.Maui;
using GloboTicket.Admin.Mobile.Repositories;
using GloboTicket.Admin.Mobile.Services;
using GloboTicket.Admin.Mobile.ViewModels;
using GloboTicket.Admin.Mobile.Views;
using Microsoft.Extensions.Logging;

namespace GloboTicket.Admin.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIconsRegular");
                })
                .RegisterRepositories()
                .RegisterServices()
                .RegisterViewModels()
                .RegisterViews();

#if DEBUG
            builder.Logging.AddDebug();
#endif            

            return builder.Build();
        }

        public static MauiAppBuilder RegisterRepositories(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<IEventRepository, EventRepository>();
            builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();

            builder.Services.AddHttpClient("GloboTicketAdminApiClient", options =>
            {
                options.BaseAddress = (DeviceInfo.Platform == DevicePlatform.Android) 
                    ? new Uri("http://10.0.2.2:5191")
                    : new Uri("https://localhost:7185");
                options.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            return builder;
        }

        public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<IEventService, EventService>();
            builder.Services.AddTransient<INavigationService, NavigationService>();
            builder.Services.AddTransient<ICategoryService, CategoryService>();
            builder.Services.AddSingleton<IDialogService, DialogService>();

            return builder;
        }

        private static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<EventListOverviewViewModel>();
            builder.Services.AddTransient<EventDetailViewModel>();
            builder.Services.AddTransient<EventAddEditViewModel>();

            return builder;
        }

        private static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<EventOverviewPage>();
            builder.Services.AddTransient<EventDetailPage>();
            builder.Services.AddTransient<EventAddEditPage>();

            return builder;
        }
    }
}