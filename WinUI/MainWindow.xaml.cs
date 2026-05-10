using Microsoft.UI.Xaml;
using WinUI.Views;

namespace WinUI;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        rootFrame.Navigate(typeof(RankShowcaseView));
    }
}

