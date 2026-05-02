using System;
using System.Threading.Tasks;
using ClassLibrary.DTOs;
using ClassLibrary.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WinUI.Services;

namespace WinUI.ViewModels;

public partial class UserViewModel : ObservableObject
{
    private const string ROLE_NUTRITIONIST = "Nutritionist";
    private const string ROLE_USER = "User";
    private const string ERROR_USERNAME_EXISTS = "Username already exists. Please choose another one.";
    private const string ERROR_INVALID_BIRTHDATE = "Please select a valid birthdate.";
    private const string ERROR_REGISTRATION_FAILED = "Registration failed. Username might already exist.";
    private const string ERROR_USERNAME_PASSWORD_REQUIRED = "Username and Password are required.";
    private const string ERROR_INVALID_CREDENTIALS = "Invalid username or password.";
    private const string ERROR_SAVING_DATA_FORMAT = "An error occurred while saving: {0}";

    private readonly IUserService userService;

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool isNutritionistChecked;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    [ObservableProperty]
    private DateTimeOffset selectedDate = DateTimeOffset.Now;

    [ObservableProperty]
    private int weight;

    [ObservableProperty]
    private int height;

    [ObservableProperty]
    private string gender = string.Empty;

    [ObservableProperty]
    private string goal = string.Empty;

    public int? LoggedInUserId { get; private set; }

    public string? LoggedInUsername { get; private set; }

    public event EventHandler? RegistrationValid;

    public event EventHandler? LoginSuccess;

    public event EventHandler? SaveDataSuccess;

    public UserViewModel(IUserService userService)
    {
        this.userService = userService;
    }

    [RelayCommand]
    private async Task OnRegisterAsync()
    {
        this.StatusMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(this.Username) || string.IsNullOrWhiteSpace(this.Password))
        {
            this.StatusMessage = ERROR_USERNAME_PASSWORD_REQUIRED;
            return;
        }

        string role = this.IsNutritionistChecked ? ROLE_NUTRITIONIST : ROLE_USER;

        if (await this.userService.CheckIfUsernameExistsAsync(this.Username))
        {
            this.StatusMessage = ERROR_USERNAME_EXISTS;
            return;
        }

        if (role == ROLE_USER)
        {
            this.RegistrationValid?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            var registered = await this.userService.RegisterAsync(this.Username, this.Password, role);
            if (registered != null)
            {
                this.LoggedInUserId = registered.Id;
                this.LoggedInUsername = registered.Username;
                this.LoginSuccess?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                this.StatusMessage = ERROR_REGISTRATION_FAILED;
            }
        }
    }

    [RelayCommand]
    private async Task OnSaveDataAsync()
    {
        this.StatusMessage = string.Empty;

        try
        {
            int age = NutritionCalculator.CalculateAge(this.SelectedDate);
            if (age <= 0)
            {
                this.StatusMessage = ERROR_INVALID_BIRTHDATE;
                return;
            }

            string role = this.IsNutritionistChecked ? ROLE_NUTRITIONIST : ROLE_USER;
            var registered = await this.userService.RegisterAsync(this.Username, this.Password, role);
            if (registered == null)
            {
                this.StatusMessage = ERROR_REGISTRATION_FAILED;
                return;
            }

            this.LoggedInUserId = registered.Id;
            this.LoggedInUsername = registered.Username;

            var userDataDto = new UserDataDto
            {
                UserId = registered.Id,
                Weight = this.Weight,
                Height = this.Height,
                Age = age,
                Gender = this.Gender,
                Goal = this.Goal,
            };

            await this.userService.AddUserDataAsync(userDataDto);
            this.SaveDataSuccess?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            this.StatusMessage = string.Format(ERROR_SAVING_DATA_FORMAT, ex.Message);
        }
    }

    [RelayCommand]
    private async Task OnLoginAsync()
    {
        this.StatusMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(this.Username) || string.IsNullOrWhiteSpace(this.Password))
        {
            this.StatusMessage = ERROR_USERNAME_PASSWORD_REQUIRED;
            return;
        }

        try
        {
            var user = await this.userService.LoginAsync(this.Username, this.Password);

            if (user != null)
            {
                this.LoggedInUserId = user.Id;
                this.LoggedInUsername = user.Username;
                this.LoginSuccess?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                this.StatusMessage = ERROR_INVALID_CREDENTIALS;
            }
        }
        catch (Exception ex)
        {
            this.StatusMessage = string.Format(ERROR_SAVING_DATA_FORMAT, ex.Message);
        }
    }
}
