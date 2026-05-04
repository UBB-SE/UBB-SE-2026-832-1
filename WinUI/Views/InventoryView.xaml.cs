using System.Net.Http;
using Microsoft.UI.Xaml.Controls;
using WinUI.Services;
using WinUI.ViewModels;

namespace WinUI.Views;

public sealed partial class InventoryView : Page
{
    private readonly InventoryViewModel viewModel;
    private readonly UserSession userSession;

    public InventoryView()
    {
        this.InitializeComponent();

        this.userSession = new UserSession();
        this.viewModel = new InventoryViewModel(new InventoryService(new InventoryServiceProxy(new HttpClient())));
        this.DataContext = this.viewModel;

        this.Loaded += async (_, _) =>
        {
            await this.viewModel.LoadAllIngredientsAsync().ConfigureAwait(true);
            await this.viewModel.LoadUserInventoryAsync(this.userSession.CurrentClientId).ConfigureAwait(true);
        };

        this.AddBySelectionButton.Click += async (_, _) =>
            await this.viewModel.AddToPantryAsync(this.userSession.CurrentClientId).ConfigureAwait(true);

        this.AddByNameButton.Click += async (_, _) =>
            await this.viewModel.AddIngredientByNameToPantryAsync(this.userSession.CurrentClientId).ConfigureAwait(true);

        this.RemoveItemButton.Click += async (_, _) =>
            await this.viewModel.RemoveSelectedItemAsync(this.userSession.CurrentClientId).ConfigureAwait(true);
    }
}
