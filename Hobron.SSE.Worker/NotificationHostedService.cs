using Hobron.SSE.Worker.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hobron.SSE.Worker
{
    internal class NotificationHostedService : IHostedService, IAsyncDisposable
    {
        private INotificationRepository _notificationRepository;
        private Timer? _timer;
        public NotificationHostedService(INotificationRepository notificationRepository)
        {
            _notificationRepository= notificationRepository;
        }
        public async ValueTask DisposeAsync()
        {
            _timer?.Dispose();
            await ValueTask.CompletedTask;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(5000);
            _timer = new Timer(GetMessage,null,TimeSpan.Zero,TimeSpan.FromSeconds(4));
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(60),cancellationToken);
        }

        private void GetMessage(object? state)
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:5243");
            var notifications = _notificationRepository.GetAllNotifications().Result;
            foreach (var notification in notifications)
            {
                HttpContent content = new StringContent(JsonSerializer.Serialize(notification), Encoding.UTF8, "application/json");
                var response = httpClient.PostAsync("/api/BackChannelNotification", content).Result;
                if ((response != null) && (response.StatusCode== System.Net.HttpStatusCode.OK))
                {
                    notification.IsProcessed = true;
                }
            }
        }
    }
}
