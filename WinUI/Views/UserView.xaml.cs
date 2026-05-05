using System.Net.Http;
using Microsoft.UI.Xaml.Controls;
using WinUI.Services;
using WinUI.ViewModels;

namespace WinUI.Views;

public sealed partial class UserView : Page
{
    private readonly UserViewModel viewModel;

    public UserView()
    {
        this.InitializeComponent();

        this.viewModel = new UserViewModel(new UserService(new UserServiceProxy(new HttpClient())));
        this.DataContext = this.viewModel;
    }
}
