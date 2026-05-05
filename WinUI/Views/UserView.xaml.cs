using Microsoft.UI.Xaml.Controls;
using WinUI.ViewModels;

namespace WinUI.Views;

public sealed partial class UserView : Page
{
    public UserViewModel ViewModel { get; }

    public UserView()
    {
        this.InitializeComponent();
        this.ViewModel = new UserViewModel();
    }
}