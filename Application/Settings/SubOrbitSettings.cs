namespace NewMultilloApi.Application.Settings;

public class SubOrbitSettings
{
    public const string SectionName = "SubOrbitSettings";

    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
}