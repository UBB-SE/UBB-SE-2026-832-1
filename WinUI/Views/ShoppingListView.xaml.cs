using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI.Services;
using WinUI.ViewModels;

namespace WinUI.Views;

public sealed partial class ShoppingListView : Page
{
    public ShoppingListViewModel ViewModel { get; }

    public ShoppingListView()
    {
        this.InitializeComponent();
        this.ViewModel = new ShoppingListViewModel(new ShoppingListService(new HttpClient()), new UserSession());
        this.DataContext = this.ViewModel;
        this.Loaded += this.OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs eventArgs)
    {
        await this.ViewModel.LoadItemsAsync().ConfigureAwait(true);
    }
}
