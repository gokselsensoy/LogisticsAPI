using Application.Shared.ResultModels;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Pipelines
{
    public class PerformanceLoggingPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<PerformanceLoggingPipelineBehaviour<TRequest, TResponse>> _logger;
        private readonly Stopwatch _stopwatch;

        public PerformanceLoggingPipelineBehaviour(ILogger<PerformanceLoggingPipelineBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
            _stopwatch = new Stopwatch();
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogDebug("İşlem Başlıyor: {RequestName} ({@Request})", requestName, request);
            _stopwatch.Start();

            try
            {
                var response = await next();

                _stopwatch.Stop();
                var elapsed = _stopwatch.ElapsedMilliseconds;

                // --- DEĞİŞİKLİK: Result Başarısızsa Uyarı Logu Bas ---
                if (response is IResult result && !result.Succeeded)
                {
                    // İşlem teknik olarak çalıştı ama mantıksal hata döndü (Örn: Stok yetersiz)
                    // Errors listesini loga ekliyoruz ki Kibana/Seq üzerinde görelim.
                    _logger.LogWarning("İşlem Mantıksal Hata ile Tamamlandı: {RequestName}. Süre: {Elapsed} ms. Hatalar: {@Errors}",
                        requestName, elapsed, result.Errors);
                }
                else
                {
                    // Başarılı durum
                    if (elapsed > 500) // Örnek: 500ms'den uzun sürenleri Warning olarak işaretle (Slow Request)
                    {
                        _logger.LogWarning("Uzun Süren İşlem Tespit Edildi: {RequestName}. Süre: {Elapsed} ms.", requestName, elapsed);
                    }
                    else
                    {
                        _logger.LogDebug("İşlem Başarıyla Tamamlandı: {RequestName}. Süre: {Elapsed} ms.", requestName, elapsed);
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                _stopwatch.Stop();
                _logger.LogError(ex, "İşlem Sırasında CRITICAL HATA (Exception): {RequestName}. Süre: {Elapsed} ms. Hata: {ErrorMessage}",
                    requestName,
                    _stopwatch.ElapsedMilliseconds,
                    ex.Message);

                throw;
            }
        }
    }
}
