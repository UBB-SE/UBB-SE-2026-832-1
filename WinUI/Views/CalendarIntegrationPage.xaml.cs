using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinRT.Interop;
using WinUI.Services;
using WinUI.ViewModels;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace WinUI.Views;

public sealed partial class CalendarIntegrationPage : Page
{
    private readonly CalendarIntegrationViewModel viewModel;

    public CalendarIntegrationPage()
    {
        this.InitializeComponent();

        this.viewModel = new CalendarIntegrationViewModel(
            new CalendarIntegrationService(new HttpClient()),
            new UserSession());
        this.DataContext = this.viewModel;

        this.Loaded += async (_, _) =>
        {
            await this.viewModel.EnsureWorkoutsLoadedAsync().ConfigureAwait(true);
        };

        this.GenerateCalendarButton.Click += this.GenerateCalendarButton_Click;
    }

    private async void GenerateCalendarButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.GenerateCalendarButton.IsEnabled = false;

            string? validationError = this.viewModel.ValidateInput();
            if (validationError is not null)
            {
                this.ShowError(validationError);
                return;
            }

            string? icsContent = await this.viewModel.GenerateCalendarAsync().ConfigureAwait(true);

            if (string.IsNullOrEmpty(icsContent))
            {
                this.ShowError("Failed to generate calendar file. Please try again.");
                return;
            }

            var savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.Downloads,
            };
            savePicker.FileTypeChoices.Add("iCalendar", new System.Collections.Generic.List<string> { ".ics" });

            IntPtr hWnd = GetActiveWindow();
            if (hWnd == IntPtr.Zero)
            {
                this.ShowError("Unable to get window handle for save dialog.");
                return;
            }

            InitializeWithWindow.Initialize(savePicker, hWnd);

            StorageFile? file = await savePicker.PickSaveFileAsync();

            if (file is null)
            {
                return;
            }

            await FileIO.WriteTextAsync(file, icsContent);

            this.ShowSuccess($"Calendar file '{file.Name}' saved successfully! You can now import it into your calendar application.");
        }
        catch (InvalidOperationException ex)
        {
            this.ShowError(ex.Message);
        }
        catch (Exception ex)
        {
            if (ex is COMException)
            {
                string? fallbackPath = await this.SaveToDownloadsFallbackAsync().ConfigureAwait(true);
                if (!string.IsNullOrWhiteSpace(fallbackPath))
                {
                    this.ShowSuccess($"Save dialog unavailable. Calendar saved to: {fallbackPath}");
                }
                else
                {
                    this.ShowError("Error saving calendar file: could not open the save dialog.");
                }
            }
            else
            {
                this.ShowError($"Error saving calendar file: {ex.Message}");
            }
        }
        finally
        {
            this.GenerateCalendarButton.IsEnabled = true;
        }
    }

    private void ShowError(string message)
    {
        this.StatusInfoBar.Severity = InfoBarSeverity.Error;
        this.StatusInfoBar.Title = "Error";
        this.StatusInfoBar.Message = message;
        this.StatusInfoBar.IsOpen = true;
    }

    private void ShowSuccess(string message)
    {
        this.StatusInfoBar.Severity = InfoBarSeverity.Success;
        this.StatusInfoBar.Title = "Success";
        this.StatusInfoBar.Message = message;
        this.StatusInfoBar.IsOpen = true;
    }

    private async Task<string?> SaveToDownloadsFallbackAsync()
    {
        if (string.IsNullOrEmpty(this.viewModel.GeneratedIcsContent))
        {
            return null;
        }

        try
        {
            string downloadsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads");
            Directory.CreateDirectory(downloadsPath);

            string safeWorkoutName = (this.viewModel.SelectedWorkout?.Name ?? "Workout")
                .Replace(" ", "-", StringComparison.Ordinal)
                .Replace("/", "-", StringComparison.Ordinal)
                .Replace("\\", "-", StringComparison.Ordinal);

            string fileName = $"{safeWorkoutName}-{DateTime.Now:yyyyMMdd-HHmmss}.ics";
            string fullPath = Path.Combine(downloadsPath, fileName);

            await File.WriteAllTextAsync(fullPath, this.viewModel.GeneratedIcsContent).ConfigureAwait(true);
            return fullPath;
        }
        catch
        {
            return null;
        }
    }

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern IntPtr GetActiveWindow();
}
