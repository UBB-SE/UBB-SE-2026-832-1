using System;
using Microsoft.UI.Xaml.Controls;
using WinUI.Services;
using WinUI.ViewModels;

namespace WinUI.Views;

public sealed partial class LoginView : Page
{
    public UserViewModel ViewModel { get; }

    public LoginView()
    {
        this.InitializeComponent();
        this.ViewModel = new UserViewModel(new UserService());

        this.ViewModel.LoginSuccess += (s, e) => this.Frame.Navigate(typeof(MainWindowView));
        this.ViewModel.NavigateToRegister += (s, e) => this.Frame.Navigate(typeof(RegisterView));
    }
}