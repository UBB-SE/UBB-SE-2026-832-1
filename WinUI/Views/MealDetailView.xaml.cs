using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI.Services;
using WinUI.ViewModels;

namespace WinUI.Views;

public sealed partial class MealDetailView : Page
{
    public MealDetailViewModel ViewModel { get; }

    public MealDetailView()
    {
        this.InitializeComponent();
        this.ViewModel = new MealDetailViewModel(new MealService(new HttpClient()), new UserSession());
        this.DataContext = this.ViewModel;
        this.Loaded += this.OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs eventArgs)
    {
        await this.ViewModel.LoadAsync().ConfigureAwait(true);
    }
}
