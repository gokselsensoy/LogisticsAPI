using Application.Abstractions.Services;
using Application.DTOs.SubOrbit;
using Microsoft.Extensions.Options;
using NewMultilloApi.Application.DTOs.SubOrbit;
using NewMultilloApi.Application.Settings;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;

namespace Integration.Services;

public class SubOrbitService : ISubOrbitService
{
    private readonly HttpClient _httpClient;
    private readonly SubOrbitSettings _settings;
    public SubOrbitService(HttpClient httpClient, IOptions<SubOrbitSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;

        // Base URL ve API Key ayarlanıyor
        _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", _settings.ApiKey);
    }

    public async Task<List<PublicProductDto>> GetPublicCatalog(int typeCode)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<PublicProductDto>>($"api/products/public/catalog?typeCode={typeCode}");
            return response ?? new List<PublicProductDto>();
        }
        catch (Exception ex)
        {
            // Loglama eklenebilir
            throw new Exception("SubOrbit planları alınırken hata oluştu.", ex);
        }
    }

    public async Task<List<ProductFeatureMatrixDto>> GetProductFeatureMatrixAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<ProductFeatureMatrixDto>>($"api/productFeature/getAllFeatures");
            return response ?? new List<ProductFeatureMatrixDto>();
        }
        catch (Exception ex)
        {
            // Loglama eklenebilir
            throw new Exception("SubOrbit planları alınırken hata oluştu.", ex);
        }
    }

    public async Task<InitiateSubscriptionResponse> InitiateCheckoutAsync(InitiateSubscriptionDto request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/subscriptions/initiate", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception($"SubOrbit ödeme başlatılamadı: {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<InitiateSubscriptionResponse>(cancellationToken: cancellationToken);
        return result ?? throw new Exception("SubOrbit'ten boş cevap döndü.");
    }

    public async Task<ValidateCouponResponse?> ValidateCouponAsync(string code, Guid? productId = null)
    {
        try
        {
            var query = $"api/coupons/validate?code={Uri.EscapeDataString(code)}";

            if (productId is not null)
            {
                query += $"&productId={productId}";
            }

            using var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ValidateCouponResponse>();
        }
        catch
        {
            return null;
        }
    }




    public bool VerifySignature(string signature, string jsonBody)
    {
        if (string.IsNullOrEmpty(_settings.ApiKey)) return false;

        try
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_settings.ApiKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(jsonBody));
            var computedSignature = Convert.ToBase64String(hash);

            return signature == computedSignature;
        }
        catch
        {
            return false;
        }
    }
}