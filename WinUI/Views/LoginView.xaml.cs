using System;
using Microsoft.UI.Xaml.Controls;
using WinUI.ViewModels;

namespace WinUI.Views;

public sealed partial class LoginView : Page
{
    public LoginViewModel ViewModel { get; }

    public LoginView()
    {
        this.InitializeComponent();
        this.ViewModel = new LoginViewModel();
        this.ViewModel.LoginSuccessful += OnLoginSuccessful;
        this.ViewModel.NavigateToRegister += OnNavigateToRegister;
    }

    private void OnLoginSuccessful(object? sender, EventArgs e)
    {
        this.Frame.Navigate(typeof(MainWindowView));
    }

    private void OnNavigateToRegister(object? sender, EventArgs e)
    {
        this.Frame.Navigate(typeof(RegisterView));
    }
}