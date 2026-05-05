using Microsoft.UI.Xaml.Controls;
using WinUI.ViewModels; 

namespace WinUI.Views;


public sealed partial class DailyMealPlanView : Page
{
    public DailyMealPlanViewModel ViewModel { get; }

    public DailyMealPlanView()
    {
        this.InitializeComponent();

        
        ViewModel = new DailyMealPlanViewModel();
    }
}