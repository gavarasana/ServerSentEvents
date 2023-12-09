using Hobron.SSE.Worker;
using Hobron.SSE.Worker.Repository;


var builder = Host.CreateDefaultBuilder(args);
//builder.ConfigureServices(services => services.AddHostedService<Worker>());
builder.ConfigureServices(services => services.AddSingleton<INotificationRepository, NotificationRepository>());
builder.ConfigureServices(services => services.AddHostedService<NotificationHostedService>());

var host = builder.Build();
host.Run();