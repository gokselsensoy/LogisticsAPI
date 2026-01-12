namespace Application.DTOs.SubOrbit;

public class ProductFeatureMatrixDto
{
    public Guid ProductId { get; set; }
    public Guid FeatureId { get; set; }
    public string FeatureCode { get; set; } = string.Empty;
    public string FeatureName { get; set; } = string.Empty;
}
