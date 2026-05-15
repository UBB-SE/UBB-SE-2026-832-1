using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ClassLibrary.Proxies;
using WinUI.ViewModels;

namespace WinUI.Views;

public sealed partial class DailyLogView : Page
{
    private const int MIN_SEARCH_LENGTH = 3;
    private const string NO_MATCHING_MEALS_TEXT = "No matching meals found";

    private readonly DailyLogViewModel viewModel;
    private readonly UserSession userSession;

    public DailyLogView()
    {
        this.InitializeComponent();

        this.userSession = new UserSession();
        this.viewModel = new DailyLogViewModel(
            new DailyLogProxy(new HttpClient()));
        this.DataContext = this.viewModel;

        this.Loaded += OnPageLoaded;
    }

    private async void OnPageLoaded(object sender, RoutedEventArgs args)
    {
        await this.viewModel.LoadDailySummaryAsync(this.userSession.CurrentClientId);
    }

    private async void OnMealSearchTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
        {
            return;
        }

        if (sender.Text.Length >= MIN_SEARCH_LENGTH)
        {
            await this.viewModel.SearchFoodItemsAsync(sender.Text);

            if (this.viewModel.FoodSearchResults.Count == 0)
            {
                sender.ItemsSource = new[] { NO_MATCHING_MEALS_TEXT };
            }
            else
            {
                var mealNames = new List<string>();
                foreach (var meal in this.viewModel.FoodSearchResults)
                {
                    mealNames.Add(meal.Name);
                }
                sender.ItemsSource = mealNames;
            }
        }
        else
        {
            sender.ItemsSource = null;
        }
    }

    private void OnMealSearchSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        string? selectedName = args.SelectedItem?.ToString();

        if (selectedName == NO_MATCHING_MEALS_TEXT)
        {
            return;
        }

        foreach (var meal in this.viewModel.FoodSearchResults)
        {
            if (meal.Name == selectedName)
            {
                this.viewModel.SelectedFoodItem = meal;
                break;
            }
        }

        sender.Text = selectedName ?? string.Empty;
    }

    private async void OnLogMealButtonClick(object sender, RoutedEventArgs args)
    {
        await this.viewModel.LogSelectedFoodItemAsync(this.userSession.CurrentClientId);
    }
}

