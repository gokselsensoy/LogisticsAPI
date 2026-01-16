using Application.Shared.ResultModels;
using FluentValidation;
using MediatR;

namespace Application.Pipelines
{
    namespace Application.Pipelines
    {
        public class ValidationPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
        {
            private readonly IEnumerable<IValidator<TRequest>> _validators;

            public ValidationPipelineBehaviour(IEnumerable<IValidator<TRequest>> validators)
            {
                _validators = validators;
            }

            public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
            {
                if (!_validators.Any())
                {
                    return await next();
                }

                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken))
                );

                var failures = validationResults
                    .SelectMany(r => r.Errors)
                    .Where(f => f != null)
                    .Select(f => f.ErrorMessage) // Mesajları string listesi olarak al
                    .Distinct()
                    .ToList();

                if (failures.Count != 0)
                {
                    // --- DEĞİŞİKLİK BURADA: Exception yerine Result Dönüyoruz ---

                    // TResponse bir IResult mı? (Result veya Result<T>)
                    if (typeof(IResult).IsAssignableFrom(typeof(TResponse)))
                    {
                        // Reflection ile "Failure" metodunu bul ve çalıştır.
                        // Çünkü TResponse generic olduğu için direkt "new Result()" diyemeyiz.
                        var responseType = typeof(TResponse);

                        // Result.Failure(List<string> errors) metodunu bul
                        var failureMethod = responseType.GetMethod("Failure", new[] { typeof(List<string>) });

                        if (failureMethod != null)
                        {
                            var result = failureMethod.Invoke(null, new object[] { failures });
                            return (TResponse)result!;
                        }
                    }

                    // Eğer TResponse bir Result tipi değilse (eski kodlar vs.) fallback olarak Exception fırlat
                    throw new ValidationException(failures.Select(f => new FluentValidation.Results.ValidationFailure("", f)));
                }

                return await next();
            }
        }
    }
}
