using Yahtzee;

namespace Yahtzee;

public partial class App : Application
{
    public IServiceProvider Services { get; }

    // MAUI host will inject IServiceProvider when you use UseMauiApp<App>()
    public App(IServiceProvider services)
    {
        InitializeComponent();
        Services = services;
    }

    protected override Window CreateWindow(Microsoft.Maui.IActivationState activationState)
    {
        return new Window(new AppShell());
    }
}
