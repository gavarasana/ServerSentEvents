using Hobron.SSE.Worker.Dto;

namespace Hobron.SSE.Worker.Repository
{
    public interface INotificationRepository
    {
        public Task<List<Notification>> GetAllNotifications();
        public Task<Notification> GetNotification(string id);
        public Task AddNotification(Notification notification);
    }

    public class NotificationRepository : INotificationRepository
    {
        private readonly List<Notification> _notifications = new List<Notification>();
        public NotificationRepository()
        {
            _notifications.Add(new Notification { Id = "1", Message = "This is the first message", MessageTime = DateTime.UtcNow.AddDays(-3) });
            _notifications.Add(new Notification { Id = "2", Message = "This is the second message", MessageTime = DateTime.UtcNow.AddDays(-1) });
            _notifications.Add(new Notification { Id = "3", Message = "This is the third message", MessageTime = DateTime.UtcNow.AddDays(2) });
        }
        public async Task AddNotification(Notification notification)
        {
            _notifications.Add(notification);
            await Task.CompletedTask;
        }

        public async Task<List<Notification>> GetAllNotifications()
        {
            return await Task.FromResult(_notifications);
        }

        public async Task<Notification> GetNotification(string id)
        {
            return await Task.FromResult<Notification>(_notifications.FirstOrDefault(n => n.Id == id) ?? new Notification { Id = "999", Message = "Not found", MessageTime = DateTime.UtcNow });
        }
    }
}
