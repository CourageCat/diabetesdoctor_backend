namespace NotificationService.Contract.Common.Constants;

public static class KafkaTopicConstraints
{
    //User
    public const string UserTopic = "user_topic";
    public const string UserGroup = "notification_service-user_group";     
    //Conversation
    public const string ConversationTopic = "conversation_topic";
    public const string ConversationGroup = "notification_service-conversation_group";
    //Group
    public const string GroupTopic = "group_topic";
    public const string NotificationServiceGroupConsumerGroup = "notification_service-group_topic";
    //Post
    public const string PostTopic = "post_topic";
    public const string PostGroup = "notification_service-post_group";

    public const string DeadTopic = "post_dead_topic";
    public const string RetryTopic = "post_retry_topic";
}