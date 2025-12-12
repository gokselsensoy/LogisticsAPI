using Domain.Events.WorkerEvents;
using MediatR;

namespace Application.Features.Workers.EventHandlers
{
    public class SendWelcomeEmailHandler : INotificationHandler<WorkerCreatedEvent>
    {
        // private readonly IEmailService _emailService;

        public SendWelcomeEmailHandler()
        {
        }

        public async Task Handle(WorkerCreatedEvent notification, CancellationToken cancellationToken)
        {
            // Burası gerçek email servisi ile değiştirilecek
            // Örnek:
            // await _emailService.SendAsync(
            //     to: notification.Email, 
            //     subject: "Multillo'ya Hoşgeldiniz",
            //     body: $"Hesabınız oluşturuldu. Şifreniz: {notification.TempPassword} Lütfen giriş yaptıktan sonra şifrenizi değiştiriniz."
            // );

            Console.WriteLine($"[EMAIL SENT] To: {notification.Email}, Pwd: {notification.TempPassword}");

            await Task.CompletedTask;
        }
    }
}
