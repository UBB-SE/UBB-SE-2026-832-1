/*using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;

namespace WinUI;

public partial class App : Application
{
    private Window? _window;

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();
    }
}
*/
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using WinUI.Extensions;

namespace WinUI;

public partial class App : Application
{
    private Window? _window;

    public static IServiceProvider Services { get; private set; } = null!;

    public App()
    {
        Services = ConfigureServices();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        
        _window = new MainWindow();
        _window.Activate();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddWinUiServices();

        return services.BuildServiceProvider();
    }
}

