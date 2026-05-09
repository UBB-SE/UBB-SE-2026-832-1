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
        this.InitializeComponent();
        this.ViewModel = new UserViewModel(new UserService());

        this.ViewModel.RegistrationValid += (s, e) => this.Frame.Navigate(typeof(UserDataView));
        this.ViewModel.LoginSuccess += (s, e) => this.Frame.Navigate(typeof(MainWindowView));
        this.ViewModel.NavigateToLogin += (s, e) => this.Frame.Navigate(typeof(LoginView));
    }
}