using Microsoft.Extensions.Logging;
using Yahtzee.Services;
using Yahtzee.ViewModels;
using Yahtzee.Views;
using CommunityToolkit.Maui;
using Yahtzee.Data;

namespace Yahtzee;

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
        return builder;
    }

    public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<HighScores>();
        builder.Services.AddTransient<Services.INavigation, Navigation>();
        builder.Services.AddSingleton<DiceSet>();
        builder.Services.AddSingleton<ScoreSheet>();

        builder.Services.AddSingleton<YahtzeeDatabase>();
        return builder;
    }

    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<MainPageViewModel>();
        builder.Services.AddTransient<GamePageViewModel>();
        return builder;
    }

    public static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<GamePage>();
        return builder;
    }
}
