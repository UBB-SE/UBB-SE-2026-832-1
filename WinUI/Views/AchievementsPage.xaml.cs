using System;
using System.Net.Http;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ClassLibrary.Proxies;
using WinUI.ViewModels;

namespace WinUI.Views;

public sealed partial class AchievementsPage : Page
{
    private readonly AchievementsViewModel viewModel;
    private readonly UserSession userSession;

    public AchievementsViewModel ViewModel => this.viewModel;

    public AchievementsPage()
    {
        this.InitializeComponent();

        this.userSession = new UserSession();
        this.viewModel = new AchievementsViewModel(new AchievementsProxy(new HttpClient()));
        this.DataContext = this.viewModel;

        this.Loaded += this.Page_Loaded;
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            await this.viewModel.LoadAchievementsCommand.ExecuteAsync(this.userSession.CurrentClientId).ConfigureAwait(true);
        }
        catch (Exception)
        {
            // Keep the page usable even if the service call fails.
        }
    }
}

