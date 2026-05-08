using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI.ViewModels;

namespace WinUI.Views.WorkoutLog;

public sealed partial class FocusModeView : UserControl
{
    public ActiveWorkoutViewModel ViewModel { get; }

    public event Action? ExitRequested;

    public FocusModeView(ActiveWorkoutViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        ExitRequested?.Invoke();
    }
}
