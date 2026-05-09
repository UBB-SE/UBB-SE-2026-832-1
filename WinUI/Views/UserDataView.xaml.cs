using System;
using Microsoft.UI.Xaml.Controls;
using WinUI.ViewModels;

namespace WinUI.Views;

public sealed partial class UserDataView : Page
{
    public UserDataViewModel ViewModel { get; }

    public UserDataView()
    {
        this.InitializeComponent();
        this.ViewModel = new UserDataViewModel();
        this.ViewModel.DataSavedSuccessful += OnDataSavedSuccessful;
    }

    private void OnDataSavedSuccessful(object? sender, EventArgs e)
    {
        this.Frame.Navigate(typeof(MainWindowView));
    }
}