using ClassLibrary.Proxies.Interfaces;
namespace WinUI.ViewModels
{
    using ClassLibrary.Models;
    using ClassLibrary.Proxies;

    public class ConversationViewModel
    {
        private readonly Conversation conversation;
        private readonly IUserSession userSession;

        public ConversationViewModel(Conversation conversation, IUserSession userSession)
        {
            this.conversation = conversation;
            this.userSession = userSession;
        }

        public int Id => conversation.Id;
        public User User => conversation.User;
        public bool HasUnanswered => conversation.HasUnanswered;

        public bool ShouldHighlight => conversation.HasUnanswered && userSession.CurrentRole == "Nutritionist";
    }
}

