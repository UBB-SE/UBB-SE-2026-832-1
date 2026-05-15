using ClassLibrary.Proxies.Interfaces;
namespace WinUI.ViewModels
{
    using ClassLibrary.Models;
    using CommunityToolkit.Mvvm.ComponentModel;

    public partial class MessageViewModel : ObservableObject
    {
        private readonly Message message;

        public MessageViewModel(Message message)
        {
            this.message = message;
        }

        public int Id => message.Id;
        public string TextContent => message.TextContent;
        public string SenderUsername => message.Sender.Username;
        public string SenderRole => message.Sender.Role;
        public string SentAtFormatted => message.SentAt.ToString("MMM dd, HH:mm");
    }
}

