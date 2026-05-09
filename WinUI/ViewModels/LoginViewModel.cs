using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using WinUI.Services;

namespace WinUI.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IUserService? userService;

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    public event EventHandler? LoginSuccessful;
    public event EventHandler? NavigateToRegister;

    public LoginViewModel()
    {
    }

    public LoginViewModel(IUserService userService)
    {
        this.userService = userService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (this.userService == null) return;

        var user = await this.userService.LoginAsync(this.Username, this.Password);

        if (user != null)
        {
            UserSession.UserId = user.Id;
            UserSession.Role = user.Role;
            this.LoginSuccessful?.Invoke(this, EventArgs.Empty);
        }
    }

    [RelayCommand]
    private void GoToRegister()
    {
        this.NavigateToRegister?.Invoke(this, EventArgs.Empty);
    }
}