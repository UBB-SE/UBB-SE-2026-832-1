namespace TeamNut.ViewModels
{
    using ClassLibrary.Models;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using WinUI.Services;

    using UserSession = ClassLibrary.Models.UserSession;

    public partial class NutritionistChatViewModel : ObservableObject
    {
        private readonly IChatService chatService;
        private CancellationTokenSource? autoRefreshCancellationTokenSource;
        private int? currentConversationId;

        private const int MaxMessageLength = 1000;
        private const int AutoRefreshSeconds = 5;
        private const int InvalidUserId = 0;
        private const string NutritionistRole = "Nutritionist";
        private const string StatusSelectConversation = "Please select a conversation to respond.";
        private const string StatusMessageTooLong = "Message too long.";
        private const string StatusInvalidCharacters = "Only alphanumeric characters and basic punctuation are allowed.";
        private const string StatusNoActiveConversations = "No active user inquiries at this time.";
        private const string StatusNutritionistCannotStartConversation = "Nutritionists can only respond to existing conversations.";

        private static readonly Regex AllowedMessageRegex = new Regex("^[a-zA-Z0-9 .,!?'\\-()]+$", RegexOptions.Compiled);

        [ObservableProperty]
        private ObservableCollection<Conversation> conversations;

        [ObservableProperty]
        private ObservableCollection<Message> messages;

        [ObservableProperty]
        private string inputText;

        [ObservableProperty]
        private bool canSend;

        [ObservableProperty]
        private string statusMessage;

        [ObservableProperty]
        private bool isNutritionistView;

        [ObservableProperty]
        private Conversation? selectedConversation;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEmptyPlaceholderVisible))]
        private bool hasMessages;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEmptyPlaceholderVisible))]
        private bool isNutritionistUser;

        public bool IsEmptyPlaceholderVisible => !IsNutritionistUser && !HasMessages;

        public NutritionistChatViewModel(IChatService chatService)
        {
            Conversations = new ObservableCollection<Conversation>();
            Messages = new ObservableCollection<Message>();
            InputText = string.Empty;
            StatusMessage = string.Empty;

            this.chatService = chatService;
            IsNutritionistUser = UserSession.Role == NutritionistRole;

            _ = LoadConversationsAsync();

            autoRefreshCancellationTokenSource = new CancellationTokenSource();
            _ = AutoRefreshLoop(autoRefreshCancellationTokenSource.Token);
        }

        partial void OnInputTextChanged(string value)
        {
            if (UserSession.Role == NutritionistRole && currentConversationId == null)
            {
                CanSend = false;
                StatusMessage = StatusSelectConversation;
                return;
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                CanSend = false;
                StatusMessage = string.Empty;
                return;
            }

            if (value.Length > MaxMessageLength)
            {
                CanSend = false;
                StatusMessage = StatusMessageTooLong;
                return;
            }

            if (!AllowedMessageRegex.IsMatch(value))
            {
                CanSend = false;
                StatusMessage = StatusInvalidCharacters;
                return;
            }

            CanSend = true;
            StatusMessage = string.Empty;
        }

        public async Task LoadConversationsAsync()
        {
            IEnumerable<Conversation> fetchedConversations;

            if (UserSession.Role == NutritionistRole)
            {
                fetchedConversations = IsNutritionistView
                    ? await chatService.GetConversationsWithUserMessagesAsync()
                    : await chatService.GetConversationsWhereNutritionistRespondedAsync(UserSession.UserId ?? InvalidUserId);

                fetchedConversations ??= Enumerable.Empty<Conversation>();
            }
            else
            {
                var conversation = await chatService.GetOrCreateConversationForUserAsync(UserSession.UserId ?? InvalidUserId);

                fetchedConversations = conversation != null
                    ? new[] { conversation }
                    : Enumerable.Empty<Conversation>();
            }

            Conversations.Clear();
            foreach (var conversationItem in fetchedConversations)
            {
                Conversations.Add(conversationItem);
            }

            if (UserSession.Role != NutritionistRole
                && currentConversationId == null
                && Conversations.Count > 0)
            {
                SelectedConversation = Conversations[0];
            }

            StatusMessage = Conversations.Any()
                ? string.Empty
                : StatusNoActiveConversations;
        }

        public async Task LoadMessagesForConversationAsync(int conversationId)
        {
            currentConversationId = conversationId;

            var fetchedMessages = (await chatService.GetMessagesForConversationAsync(conversationId)).ToList();

            if (Messages.Count == fetchedMessages.Count
                && Messages.Zip(fetchedMessages, (existingMessage, newMessage) => existingMessage.Id == newMessage.Id).All(isEqual => isEqual))
            {
                HasMessages = Messages.Count > 0;
                return;
            }

            Messages.Clear();

            foreach (var messageItem in fetchedMessages)
            {
                Messages.Add(messageItem);
            }

            HasMessages = Messages.Count > 0;
        }

        partial void OnSelectedConversationChanged(Conversation? value)
        {
            if (value != null)
            {
                _ = LoadMessagesForConversationAsync(value.Id);
            }
        }

        partial void OnIsNutritionistViewChanged(bool value)
        {
            _ = LoadConversationsAsync();
        }

        private async Task AutoRefreshLoop(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(AutoRefreshSeconds), token);
                    await LoadConversationsAsync();

                    if (currentConversationId != null)
                    {
                        await LoadMessagesForConversationAsync(currentConversationId.Value);
                    }
                }
            }
            catch (TaskCanceledException)
            {
            }
        }

        public void StopAutoRefresh()
        {
            autoRefreshCancellationTokenSource?.Cancel();
        }

        [RelayCommand]
        public async Task SendMessageAsync()
        {
            if (string.IsNullOrWhiteSpace(InputText)) return;

            if (InputText.Length > MaxMessageLength)
            {
                StatusMessage = StatusMessageTooLong;
                return;
            }

            if (!AllowedMessageRegex.IsMatch(InputText))
            {
                StatusMessage = StatusInvalidCharacters;
                return;
            }

            if (currentConversationId == null)
            {
                if (UserSession.Role == NutritionistRole)
                {
                    StatusMessage = StatusNutritionistCannotStartConversation;
                    return;
                }

                if (UserSession.UserId == null) return;

                var conversation = await chatService.GetOrCreateConversationForUserAsync(UserSession.UserId.Value);
                if (conversation == null) return;

                currentConversationId = conversation.Id;
            }

            if (UserSession.UserId == null) return;

            int senderId = UserSession.UserId.Value;
            bool isNutritionist = UserSession.Role == NutritionistRole;

            await chatService.AddMessageAsync(
                currentConversationId.Value,
                senderId,
                InputText.Trim(),
                isNutritionist);

            InputText = string.Empty;
            await LoadMessagesForConversationAsync(currentConversationId.Value);
        }
    }
}