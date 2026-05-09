using Microsoft.UI.Xaml.Controls;

namespace WinUI.Views;

public sealed partial class CreateWorkoutView : UserControl
{
    public object? ViewModel { get; }

    public CreateWorkoutView()
    {
        this.InitializeComponent();

        // Stub: viewmodel will be implemented later
        this.ViewModel = null;
        this.DataContext = this.ViewModel;
    }
}
