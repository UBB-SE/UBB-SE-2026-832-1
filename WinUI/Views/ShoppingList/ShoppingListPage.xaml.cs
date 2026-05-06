using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using ClassLibrary.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI.Services;
using WinUI.ViewModels;

namespace WinUI.Views.ShoppingList;

public sealed partial class ShoppingListPage : Page
{
    private const int MIN_SEARCH_LENGTH = 3;
    private const string NO_MATCHING_INGREDIENTS_TEXT = "no matching ingredients found";
    private const string BUTTON_YES = "Yes";
    private const string BUTTON_CANCEL = "Cancel";
    private const string TITLE_CONFIRM_PANTRY_TRANSFER = "Confirm Pantry Transfer";
    private const string TITLE_CONFIRM_DELETION = "Confirm Deletion";
    private const string MSG_CONFIRM_PANTRY_TRANSFER = "Are you sure you want to remove this item and add it to your pantry?";
    private const string MSG_CONFIRM_DELETION = "Are you sure you want to remove this item from the shopping list?";

    public ShoppingListViewModel ViewModel { get; }

    public ShoppingListPage()
    {
        this.InitializeComponent();
        var service = new ShoppingListService(new HttpClient());
        this.ViewModel = new ShoppingListViewModel(service, new WinUI.Services.UserSession());
        this.DataContext = this.ViewModel;
        this.Loaded += this.OnPageLoaded;
    }

    private async void OnPageLoaded(object sender, RoutedEventArgs eventArgs)
    {
        await this.ViewModel.LoadItemsAsync().ConfigureAwait(true);
    }

    private async void AddButton_Click(object sender, RoutedEventArgs eventArgs)
    {
        string text = this.IngredientSearchBox.Text;

        if (!string.IsNullOrWhiteSpace(text) && text != NO_MATCHING_INGREDIENTS_TEXT)
        {
            this.ViewModel.PendingIngredientName = text;
            await this.ViewModel.AddItemCommand.ExecuteAsync(null);
            this.IngredientSearchBox.Text = string.Empty;
            this.IngredientSearchBox.ItemsSource = null;
        }
    }

    private async void IngredientSearchBox_TextChanged(
        AutoSuggestBox sender,
        AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
        {
            return;
        }

        if (sender.Text.Length >= MIN_SEARCH_LENGTH)
        {
            var results = await this.ViewModel.SearchIngredientsAsync(sender.Text);

            sender.ItemsSource = results.Count == 0
                ? new List<string> { NO_MATCHING_INGREDIENTS_TEXT }
                : results.Select(result => result.Value).ToList();
        }
        else
        {
            sender.ItemsSource = null;
        }
    }

    private void IngredientSearchBox_SuggestionChosen(
        AutoSuggestBox sender,
        AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        string? selectedName = args.SelectedItem?.ToString();

        if (selectedName == NO_MATCHING_INGREDIENTS_TEXT)
        {
            sender.Text = string.Empty;
            return;
        }

        sender.Text = selectedName ?? string.Empty;
    }

    private async void AcceptButton_Click(object sender, RoutedEventArgs eventArgs)
    {
        if (sender is not Button { DataContext: ShoppingListItem item })
        {
            return;
        }

        var dialog = new ContentDialog
        {
            Title = TITLE_CONFIRM_PANTRY_TRANSFER,
            Content = MSG_CONFIRM_PANTRY_TRANSFER,
            PrimaryButtonText = BUTTON_YES,
            CloseButtonText = BUTTON_CANCEL,
            XamlRoot = this.XamlRoot,
        };

        if (await dialog.ShowAsync() == ContentDialogResult.Primary)
        {
            this.ViewModel.MoveToPantryCommand.Execute(item);
        }
    }

    private async void CancelButton_Click(object sender, RoutedEventArgs eventArgs)
    {
        if (sender is not Button { DataContext: ShoppingListItem item })
        {
            return;
        }

        var dialog = new ContentDialog
        {
            Title = TITLE_CONFIRM_DELETION,
            Content = MSG_CONFIRM_DELETION,
            PrimaryButtonText = BUTTON_YES,
            CloseButtonText = BUTTON_CANCEL,
            XamlRoot = this.XamlRoot,
        };

        if (await dialog.ShowAsync() == ContentDialogResult.Primary)
        {
            this.ViewModel.RemoveItemCommand.Execute(item);
        }
    }
}
