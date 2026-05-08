using Microsoft.UI.Xaml.Controls;
using WinUI.ViewModels;

namespace WinUI.Views.WorkoutLog;

public sealed partial class FocusModeView : UserControl
{
    public ActiveWorkoutViewModel ViewModel { get; }

    public FocusModeView(ActiveWorkoutViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }
}
