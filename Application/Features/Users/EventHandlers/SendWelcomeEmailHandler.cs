using Domain.Events;
using MediatR;

namespace Application.Features.Users.EventHandlers
{
    //public class SendWelcomeEmailHandler : INotificationHandler<AppUserCreatedEvent>
    //{
    //    private readonly IEmailService _emailService;

    //    public SendWelcomeEmailHandler(IEmailService emailService)
    //    {
    //        _emailService = emailService;
    //    }

    //    public async Task Handle(AppUserCreatedEvent notification, CancellationToken cancellationToken)
    //    {
    //        var subject = $"Hoşgeldin {notification.FullName}!";
    //        var body = "Multillo platformuna kaydınız başarıyla oluşturuldu.";

    //        await _emailService.SendEmailAsync(notification.Email, subject, body);
    //    }
    //}
}
