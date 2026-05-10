using System.Collections.ObjectModel;

namespace WinUI.ViewModels;

public sealed class ExerciseSetGroupViewModel
{
    public string ExerciseName { get; set; } = string.Empty;

    public ObservableCollection<SetDetailRowViewModel> Sets { get; } = new();
}
