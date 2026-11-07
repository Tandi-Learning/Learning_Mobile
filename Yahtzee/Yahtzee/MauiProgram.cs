using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using Yahtzee.Data;
using Yahtzee.Data.Repositories;

namespace Yahtzee
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            MauiAppBuilder builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                // Kept for the flyout's theme toggle; the tracker's chart controls are gone.
                .ConfigureSyncfusionToolkit()
                .ConfigureMauiHandlers(handlers =>
                {
#if WINDOWS
                    Microsoft.Maui.Controls.Handlers.Items.CollectionViewHandler.Mapper.AppendToMapping("KeyboardAccessibleCollectionView", (handler, view) =>
                    {
                        handler.PlatformView.SingleSelectionFollowsFocus = false;
                    });
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                    fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
                });

#if DEBUG
            builder.Logging.AddDebug();
            builder.Services.AddLogging(configure => configure.AddDebug());
#endif

            builder.Services.AddSingleton<GameStateRepository>();
            builder.Services.AddSingleton<ResultRepository>();
            builder.Services.AddSingleton<DatabaseInitializationService>();
            builder.Services.AddSingleton<ModalErrorHandler>();
            builder.Services.AddSingleton<MainPageModel>();
            builder.Services.AddTransient<GamePageModel>();
            builder.Services.AddTransient<ScoreHistoryPageModel>();

            builder.Services.AddTransientWithShellRoute<GamePage, GamePageModel>("game");
            builder.Services.AddTransientWithShellRoute<ScoreHistoryPage, ScoreHistoryPageModel>("scorehistory");

            MauiApp app = builder.Build();
            DatabaseInitializationService databaseInitializationService =
                app.Services.GetRequiredService<DatabaseInitializationService>();

            databaseInitializationService.InitializeAsync().GetAwaiter().GetResult();

            return app;
        }
    }
}
