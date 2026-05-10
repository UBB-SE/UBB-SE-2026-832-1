using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WinUI.ViewModels;

public partial class DailyMealPlanViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<string> plannedMeals = new();

    public DailyMealPlanViewModel()
    {
        plannedMeals.Add("Desayuno: Avena con frutas");
        plannedMeals.Add("Almuerzo: Pollo con arroz");
    }
}