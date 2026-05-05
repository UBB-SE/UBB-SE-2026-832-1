using System.Net.Http;
using Microsoft.UI.Xaml.Controls;
using WinUI.Services;
using WinUI.ViewModels;

namespace WinUI.Views;

public sealed partial class MealPlanView : Page
{
    private readonly MealPlanViewModel viewModel;
    private readonly UserSession userSession;

    public MealPlanView()
    {
        this.InitializeComponent();

        this.userSession = new UserSession();
        this.viewModel = new MealPlanViewModel(new MealPlanService(new MealPlanServiceProxy(new HttpClient())));
        this.DataContext = this.viewModel;

        this.Loaded += async (_, _) =>
            await this.viewModel.LoadMealPlansForUserAsync(this.userSession.CurrentClientId).ConfigureAwait(true);

        this.MealPlansListView.SelectionChanged += async (_, _) =>
            await this.viewModel.LoadFoodItemsForSelectedPlanAsync().ConfigureAwait(true);
    }
}
