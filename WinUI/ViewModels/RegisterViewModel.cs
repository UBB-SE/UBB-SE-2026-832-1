using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WinUI.Services;

namespace WinUI.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly IUserService? userService;

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string selectedRole = "Client";

    public ObservableCollection<string> Roles { get; } = new() { "Client", "Nutritionist", "Trainer" };

    public event EventHandler? RegistrationSuccessful;
    public event EventHandler? NavigateToUserData;
    public event EventHandler? NavigateToLogin;

    public RegisterViewModel()
    {
    }

    public RegisterViewModel(IUserService userService)
    {
        this.userService = userService;
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (this.userService == null) return;

        if (await this.userService.CheckIfUsernameExistsAsync(this.Username))
        {
            return;
        }

        var user = await this.userService.RegisterAsync(this.Username, this.Password, this.SelectedRole);

        if (user != null)
        {
            UserSession.UserId = user.Id;
            UserSession.Role = this.SelectedRole;

            if (this.SelectedRole == "Client")
            {
                this.NavigateToUserData?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                this.RegistrationSuccessful?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    [RelayCommand]
    private void GoToLogin()
    {
        this.NavigateToLogin?.Invoke(this, EventArgs.Empty);
    }
}