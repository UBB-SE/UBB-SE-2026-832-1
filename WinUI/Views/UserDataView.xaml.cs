using System;
using Microsoft.UI.Xaml.Controls;
using WinUI.Services;
using WinUI.ViewModels;

namespace WinUI.Views;

public sealed partial class UserDataView : Page
{
    public UserViewModel ViewModel { get; }

    public UserDataView()
    {
        this.InitializeComponent();
        this.ViewModel = new UserViewModel(new UserService());
        this.ViewModel.SaveDataSuccess += (s, e) => this.Frame.Navigate(typeof(MainWindowView));
    }
}