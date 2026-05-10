using System;
using Microsoft.UI.Xaml.Controls;
using WinUI.Services;
using WinUI.ViewModels;

namespace WinUI.Views;

public sealed partial class RegisterView : Page
{
    public UserViewModel ViewModel { get; }

    public RegisterView()
    {
        this.ViewModel = new UserViewModel(new UserService());
        this.InitializeComponent();

        this.ViewModel.RegistrationValid += (s, e) =>
        this.Frame.Navigate(typeof(UserDataView), this.ViewModel);
        this.ViewModel.LoginSuccess += (s, e) => this.Frame.Navigate(typeof(MainWindowView));
        this.ViewModel.NavigateToLogin += (s, e) => this.Frame.Navigate(typeof(LoginView));
    }
}