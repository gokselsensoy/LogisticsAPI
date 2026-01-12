using Application.Abstractions.Repositories;
using Domain.Entities.Subscriptions;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Logging;
using NewMultilloApi.Application.DTOs.SubOrbit;
using System.Threading;

namespace Application.Features.SubOrbit.Commands.ProcessWebhook;

public class ProcessWebhookCommandHandler : IRequestHandler<ProcessWebhookCommand>
{
    private readonly ILogger<ProcessWebhookCommandHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppUserSubscription _subscriptionRepository;

    private static class SubOrbitEvents
    {
        public const string AccessGranted = "access.granted";
        public const string AccessModified = "access.modified";
        public const string AccessRevoked = "access.revoked";
        public const string NewFeatures = "new.features";
    }

    public ProcessWebhookCommandHandler(
        ILogger<ProcessWebhookCommandHandler> logger,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IAppUserSubscription subscriptionRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task Handle(ProcessWebhookCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        _logger.LogInformation("SubOrbit Webhook: {Event}, User: {Email}", payload.Event, payload.User.Email);

        if (!Guid.TryParse(payload.User.ExternalId, out var userId))
        {
            _logger.LogError("Geçersiz User ID formatı: {ExternalId}", payload.User.ExternalId);
            return;
        }

        switch (payload.Event)
        {
            case SubOrbitEvents.AccessGranted:
                await UpdateSubscriptionAsync(userId, payload.Access, cancellationToken);
                break;

            case SubOrbitEvents.AccessModified:
                break;

            case SubOrbitEvents.AccessRevoked:
                await SuspendedSubscription(userId, cancellationToken);
                break;

            case SubOrbitEvents.NewFeatures:
                break;

            default:
                _logger.LogWarning("Tanımsız SubOrbit Event'i: {Event}", payload.Event);
                break;
        }
    }

    /// <summary>
    /// Aboneden ücret alınamadığı takdirde aboneliğin askıya alınması
    /// </summary>               
    /// 
    private async Task SuspendedSubscription(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("Webhook işlenemedi: Kullanıcı bulunamadı (ID: {UserId})", userId);
            return;
        }

        var subscription = await _subscriptionRepository.GetAppUserSubscriptionWithAppUserIdAsync(user.Id);

        if (subscription != null)
        {
            subscription.Status = "Suspended";
            _subscriptionRepository.Update(subscription);
        }
        user.IsActive = false;
        _userRepository.Update(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

    }

    /// <summary>
    /// Abonelik oluşturma veya güncelleme mantığını tek bir yerde toplar.
    /// </summary>
    private async Task UpdateSubscriptionAsync(Guid userId, WebhookAccessDetails accessDetails, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("Webhook işlenemedi: Kullanıcı bulunamadı (ID: {UserId})", userId);
            return;
        }

        var subscription = await _subscriptionRepository.GetAppUserSubscriptionWithAppUserIdAsync(user.Id);

        if (subscription == null)
        {
            // Yeni Abonelik Oluştur
            subscription = new AppUserSubscription { AppUserId = user.Id };
            MapSubscriptionData(subscription, accessDetails);
            _subscriptionRepository.Add(subscription);
        }
        else
        {
            // Mevcut Aboneliği Güncelle
            MapSubscriptionData(subscription, accessDetails);
            _subscriptionRepository.Update(subscription);
        }

        // Kullanıcı durumunu abonelik durumuna göre senkronize et
        // "Active" string'i SubOrbit'ten gelen statüye göre değişebilir, Enum kullanmak daha güvenlidir.
        user.IsActive = accessDetails.Status.Equals("Active", StringComparison.OrdinalIgnoreCase);

        _userRepository.Update(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Kullanıcı aboneliği güncellendi. User: {UserId}, Status: {Status}", user.Id, accessDetails.Status);
    }

    /// <summary>
    /// Gelen veriyi Entity'e eşleyen yardımcı metot. Kod tekrarını önler.
    /// </summary>
    private void MapSubscriptionData(AppUserSubscription subscription, WebhookAccessDetails accessDetails)
    {
        if (Guid.TryParse(accessDetails.PlanId, out var planId))
        {
            subscription.SubOrbitProductId = planId;
        }

        subscription.PlanName = accessDetails.PlanCode;
        subscription.Status = accessDetails.Status;
        subscription.ValidUntil = accessDetails.ValidUntil;
    }
}