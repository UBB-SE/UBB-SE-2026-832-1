using CommunityToolkit.Mvvm.ComponentModel;
using ClassLibrary.Proxies.Interfaces;

namespace WinUI.ViewModels.CalendarIntegration;

public sealed partial class DaySelectionItem : ObservableObject
{
    private bool isSelected;

    public int DayOfWeekIndex { get; }

    public string DayName { get; }

    public bool IsSelected
    {
        get => this.isSelected;
        set => SetProperty(ref this.isSelected, value);
    }

    public DaySelectionItem(int dayOfWeekIndex, string dayName, bool initialSelection = false)
    {
        this.isSelected = initialSelection;
        DayOfWeekIndex = dayOfWeekIndex;
        DayName = dayName;
    }
}

