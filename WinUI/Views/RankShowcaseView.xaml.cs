using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ClassLibrary.Proxies;
using WinUI.ViewModels;

namespace WinUI.Views;

public sealed partial class RankShowcaseView : Page
{
    public RankShowcaseViewModel ViewModel { get; }

    public RankShowcaseView()
    {
        this.InitializeComponent();
        this.ViewModel = new RankShowcaseViewModel(
            new RankShowcaseProxy(new HttpClient()),
            new UserSession());
        this.DataContext = this.ViewModel;
    }

    private async void Page_Loaded(object sender, RoutedEventArgs eventArgs)
    {
        await this.ViewModel.LoadAsync().ConfigureAwait(true);
    }
}

