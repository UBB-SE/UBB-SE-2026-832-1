namespace WinUI.ViewModels;

public sealed class TabItemModel
{
    public string Title { get; set; } = string.Empty;
    public Type PageType { get; set; }

    public TabItemModel(Type pageType)
    {
        ArgumentNullException.ThrowIfNull(pageType);
        PageType = pageType;
    }
}
