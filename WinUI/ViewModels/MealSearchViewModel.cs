using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WinUI.Services;


namespace WinUI.ViewModels;

public partial class MealSearchViewModel : ObservableObject
{
    private readonly IMealSearchService mealSearchService;

    
    [ObservableProperty]
    private string searchQuery = string.Empty;

    [ObservableProperty]
    private bool isSearching;

    
    public ObservableCollection<object> MealSearchResults { get; } = new ObservableCollection<object>();

    public MealSearchViewModel(IMealSearchService mealSearchService)
    {
        this.mealSearchService = mealSearchService;
    }

    [RelayCommand]
    public async Task ExecuteMealSearchAsync()
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            return;
        }

        try
        {
            IsSearching = true;
            MealSearchResults.Clear();

           
            await mealSearchService.SearchMealsAsync();

            
        }
        catch (Exception error)
        {
          
            Console.WriteLine(error.Message);
        }
        finally
        {
            IsSearching = false;
        }
    }
}