using ClassLibrary.DTOs;
using ClassLibrary.Models;

namespace WinUI.Services;

internal static class DataTransferObjectToDomainModelMappers
{
    public static IReadOnlyList<Achievement> MapAchievements(IReadOnlyList<AchievementDataTransferObject>? achievementDataTransferObjects)
    {
        if (achievementDataTransferObjects is null || achievementDataTransferObjects.Count == 0)
        {
            return Array.Empty<Achievement>();
        }

        return achievementDataTransferObjects.Select(MapAchievement).ToList();
    }

    public static IReadOnlyList<Client> MapClients(IReadOnlyList<ClientDataTransferObject>? clientDataTransferObjects)
    {
        if (clientDataTransferObjects is null || clientDataTransferObjects.Count == 0)
        {
            return Array.Empty<Client>();
        }

        return clientDataTransferObjects.Select(clientDataTransferObject => MapClient(clientDataTransferObject)).ToList();
    }

    public static IReadOnlyList<Notification> MapNotifications(IReadOnlyList<NotificationDataTransferObject>? notificationDataTransferObjects)
    {
        if (notificationDataTransferObjects is null || notificationDataTransferObjects.Count == 0)
        {
            return Array.Empty<Notification>();
        }

        return notificationDataTransferObjects.Select(MapNotification).ToList();
    }

    public static IReadOnlyList<WorkoutLog> MapWorkoutLogs(IReadOnlyList<WorkoutLogDataTransferObject>? workoutLogDataTransferObjects)
    {
        if (workoutLogDataTransferObjects is null || workoutLogDataTransferObjects.Count == 0)
        {
            return Array.Empty<WorkoutLog>();
        }

        return workoutLogDataTransferObjects.Select(MapWorkoutLog).ToList();
    }

    public static IReadOnlyList<WorkoutTemplate> MapWorkoutTemplates(IReadOnlyList<WorkoutTemplateDataTransferObject>? workoutTemplateDataTransferObjects)
    {
        if (workoutTemplateDataTransferObjects is null || workoutTemplateDataTransferObjects.Count == 0)
        {
            return Array.Empty<WorkoutTemplate>();
        }

        return workoutTemplateDataTransferObjects.Select(MapWorkoutTemplate).ToList();
    }

    public static Achievement MapAchievement(AchievementDataTransferObject achievementDataTransferObject)
    {
        return new Achievement
        {
            AchievementId = achievementDataTransferObject.AchievementId,
            Name = achievementDataTransferObject.Title,
            Description = achievementDataTransferObject.Description,
            Criteria = achievementDataTransferObject.Criteria,
            Icon = achievementDataTransferObject.Icon,
        };
    }

    public static Client MapClient(ClientDataTransferObject clientDataTransferObject)
    {
        return new Client
        {
            ClientId = clientDataTransferObject.ClientId,
            Email = clientDataTransferObject.Email,
            FullName = clientDataTransferObject.FullName,
            Weight = clientDataTransferObject.Weight,
            Height = clientDataTransferObject.Height,
            PrimaryGoal = clientDataTransferObject.PrimaryGoal,
        };
    }

    public static Client MapClient(ClientDataTransferObject clientDataTransferObject, int clientId)
    {
        var client = MapClient(clientDataTransferObject);
        client.ClientId = clientId;
        return client;
    }

    public static Notification MapNotification(NotificationDataTransferObject notificationDataTransferObject)
    {
        return new Notification
        {
            NotificationId = notificationDataTransferObject.NotificationId,
            Client = MapClient(notificationDataTransferObject.Client),
            Title = notificationDataTransferObject.Title,
            Message = notificationDataTransferObject.Message,
            Type = ParseNotificationType(notificationDataTransferObject.Type),
            RelatedId = notificationDataTransferObject.RelatedId,
            DateCreated = notificationDataTransferObject.DateCreated,
            IsRead = notificationDataTransferObject.IsRead,
        };
    }

    public static WorkoutLog MapWorkoutLog(WorkoutLogDataTransferObject workoutLogDataTransferObject)
    {
        return new WorkoutLog
        {
            WorkoutLogId = workoutLogDataTransferObject.WorkoutLogId,
            Client = MapClient(workoutLogDataTransferObject.Client),
            WorkoutName = workoutLogDataTransferObject.WorkoutName,
            Date = workoutLogDataTransferObject.Date,
            Duration = workoutLogDataTransferObject.Duration,
            SourceTemplateId = workoutLogDataTransferObject.SourceTemplateId,
            Type = ParseWorkoutType(workoutLogDataTransferObject.Type),
            TotalCaloriesBurned = workoutLogDataTransferObject.TotalCaloriesBurned,
            AverageMetabolicEquivalent = workoutLogDataTransferObject.AverageMetabolicEquivalent,
            IntensityTag = workoutLogDataTransferObject.IntensityTag,
            Rating = workoutLogDataTransferObject.Rating ?? 0,
            TrainerNotes = workoutLogDataTransferObject.TrainerNotes,
        };
    }

    public static WorkoutTemplate MapWorkoutTemplate(WorkoutTemplateDataTransferObject workoutTemplateDataTransferObject)
    {
        return new WorkoutTemplate
        {
            WorkoutTemplateId = workoutTemplateDataTransferObject.WorkoutTemplateId,
            Client = MapClient(workoutTemplateDataTransferObject.Client, workoutTemplateDataTransferObject.ClientId),
            Name = workoutTemplateDataTransferObject.Name,
            Type = ParseWorkoutType(workoutTemplateDataTransferObject.Type),
            Exercises = [],
        };
    }

    private static NotificationType ParseNotificationType(string notificationType)
    {
        if (Enum.TryParse<NotificationType>(notificationType, ignoreCase: true, out var parsedNotificationType))
        {
            return parsedNotificationType;
        }

        return NotificationType.Info;
    }

    private static WorkoutType ParseWorkoutType(string workoutType)
    {
        if (Enum.TryParse<WorkoutType>(workoutType, ignoreCase: true, out var parsedWorkoutType))
        {
            return parsedWorkoutType;
        }

        return WorkoutType.CUSTOM;
    }

    public static IReadOnlyList<Conversation> MapConversations(IReadOnlyList<ConversationDto>? conversationDtos)
    {
        if (conversationDtos is null || conversationDtos.Count == 0)
        {
            return Array.Empty<Conversation>();
        }

        return conversationDtos.Select(MapConversation).ToList();
    }

    public static IReadOnlyList<Message> MapMessages(IReadOnlyList<MessageDto>? messageDtos)
    {
        if (messageDtos is null || messageDtos.Count == 0)
        {
            return Array.Empty<Message>();
        }

        return messageDtos.Select(MapMessage).ToList();
    }

    public static Conversation MapConversation(ConversationDto conversationDto)
    {
        return new Conversation
        {
            Id = conversationDto.ConversationId,
            HasUnanswered = conversationDto.HasUnanswered,
            User = new User
            {
                UserId = conversationDto.UserId,
                Username = conversationDto.UserName
            },
            Messages = []
        };
    }

    public static Message MapMessage(MessageDto messageDto)
    {
        return new Message
        {
            Id = messageDto.MessageId,
            SentAt = messageDto.SentAt,
            TextContent = messageDto.TextContent,
            Sender = new User
            {
                Username = messageDto.SenderUsername,
                Role = messageDto.SenderRole
            },
            Conversation = new Conversation
            {
                Id = messageDto.ConversationId
            }
        };
    }
}