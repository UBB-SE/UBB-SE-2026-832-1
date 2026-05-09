using ClassLibrary.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Net.Http;
using WinUI.Services;
using WinUI.ViewModels;

namespace WinUI.Views.ChatView
{
    public sealed partial class NutritionistChatView : Page
    {
        public NutritionistChatViewModel ViewModel { get; }

        public NutritionistChatView()
        {
            this.InitializeComponent();
            this.ViewModel = new NutritionistChatViewModel(
                new ChatService(new HttpClient()),
                new UserSession()
            );
            this.DataContext = ViewModel;
        }

        private void MessagesItems_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (MessagesScrollViewer.ScrollableHeight > 0)
            {
                MessagesScrollViewer.ChangeView(null, MessagesScrollViewer.ScrollableHeight, null, true);
            }
        }

        protected override void OnNavigatedFrom(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ViewModel.StopAutoRefresh();
        }
    }
}