namespace NotificationService.Contract.DTOs.NotificationDtos;

//public class ChatNotificationDto
//{
//    public string Id { get; set; }
//    public string UserId { get; set; }
//    public string FullName { get; set; }
//    public string UserImage { get; set; }
//    public string MessageId { get; set; }
//    public string MessageContent { get; set; }
//    public string GroupId { get; set; }
//    public string GroupName { get; set; }
//    public string SenderId { get; set; }
//}


public class ChatNotificationDto
{
    public string SenderId { get; set; }
    public string GroupId { get; set; }
    public string Title { get; set; }
    public string MessageContent { get; set; }
}