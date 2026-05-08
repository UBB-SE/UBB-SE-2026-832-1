using System.Net.Http;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI.Services;
using WinUI.ViewModels;

namespace WinUI.Views;

public sealed partial class DailyLogView : Page
{
    private readonly DailyLogViewModel viewModel;
    private readonly UserSession userSession;

    public DailyLogView()
    {
        this.InitializeComponent();

        this.userSession = new UserSession();
        this.viewModel = new DailyLogViewModel(
            new DailyLogService(new HttpClient()));
        this.DataContext = this.viewModel;

        this.Loaded += OnPageLoaded;
        this.SearchButton.Click += OnSearchButtonClick;
        this.LogFoodButton.Click += OnLogFoodButtonClick;
    }

    private async void OnPageLoaded(object sender, RoutedEventArgs args)
    {
        await this.viewModel.LoadDailySummaryAsync(this.userSession.CurrentClientId).ConfigureAwait(true);
    }

    private async void OnSearchButtonClick(object sender, RoutedEventArgs args)
    {
        await this.viewModel.SearchFoodItemsAsync(this.SearchBox.Text).ConfigureAwait(true);
    }

    private async void OnLogFoodButtonClick(object sender, RoutedEventArgs args)
    {
        await this.viewModel.LogSelectedFoodItemAsync(this.userSession.CurrentClientId).ConfigureAwait(true);
    }
}
