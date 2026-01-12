using MediatR;
using NewMultilloApi.Application.DTOs.SubOrbit;

namespace Application.Features.SubOrbit.Commands.ProcessWebhook;

public record ProcessWebhookCommand(AccessWebhookPayload Payload) : IRequest;