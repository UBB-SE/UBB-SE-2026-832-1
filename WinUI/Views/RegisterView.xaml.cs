using System;
using Microsoft.UI.Xaml.Controls;
using WinUI.ViewModels;

namespace WinUI.Views;

public sealed partial class RegisterView : Page
{
    public RegisterViewModel ViewModel { get; }

    public RegisterView()
    {
        this.InitializeComponent();
        this.ViewModel = new RegisterViewModel();
        this.ViewModel.RegistrationSuccessful += OnRegistrationSuccessful;
        this.ViewModel.NavigateToUserData += OnNavigateToUserData;
        this.ViewModel.NavigateToLogin += OnNavigateToLogin;
    }

    private void OnRegistrationSuccessful(object? sender, EventArgs e)
    {
        this.Frame.Navigate(typeof(MainWindowView));
    }

    private void OnNavigateToUserData(object? sender, EventArgs e)
    {
        this.Frame.Navigate(typeof(UserDataView));
    }

    private void OnNavigateToLogin(object? sender, EventArgs e)
    {
        this.Frame.Navigate(typeof(LoginView));
    }
}