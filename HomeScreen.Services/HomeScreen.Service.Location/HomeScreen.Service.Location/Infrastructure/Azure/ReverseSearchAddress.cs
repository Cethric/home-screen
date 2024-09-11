namespace HomeScreen.Service.Location.Infrastructure.Azure;

public class ReverseSearchAddress
{
    public string AddressLine { get; set; } = string.Empty;
    public string Locality { get; set; } = string.Empty;
    public string Neighborhood { get; set; } = string.Empty;
    public IReadOnlyList<string> AdminDistricts { get; set; } = null!;
    public string PostalCode { get; set; } = string.Empty;
    public string CountryRegion { get; set; } = string.Empty;
    public string FormattedAddress { get; set; } = string.Empty;
    public string Intersection { get; set; } = string.Empty;
}
