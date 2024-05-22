public class CountryName
{
    public string Common { get; set; }
    public string Official { get; set; }
    public Dictionary<string, CountryName> NativeName { get; set; }
}

public class Country
{
    public CountryName Name { get; set; }
    public string Cca2 { get; set; }
    
    public string CallingCode { get; set; }

}