using System.Net.Http;
using ClassLibrary.DTOs;
using Microsoft.UI.Xaml.Controls;
using ClassLibrary.Proxies;
using WinUI.ViewModels;

namespace WinUI.Views.PantryView;

public sealed partial class PantryView : Page
{
    public PantryViewModel ViewModel { get; }

    public PantryView()
    {
        var userSession = new UserSession();
        this.ViewModel = new PantryViewModel(
            new InventoryProxy(new HttpClient()),
            userSession.CurrentClientId);
        this.DataContext = this.ViewModel;
        this.InitializeComponent();
        this.Loaded += this.OnPageLoaded;
    }

    private async void OnPageLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs eventArgs)
    {
        await this.ViewModel.LoadIngredientsAsync();
        await this.ViewModel.LoadInventoryAsync();
    }

    private void IngredientSuggestBox_SuggestionChosen(
        AutoSuggestBox sender,
        AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.SelectedItem is IngredientDataTransferObject ingredient)
        {
            this.ViewModel.SelectedIngredient = ingredient;
        }
    }
}

