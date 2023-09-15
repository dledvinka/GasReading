namespace ConsoleApp;

public sealed class Settings
{
    public required string AzureCognitiveServicesApiKey { get; set; }
    public required string AzureCognitiveServicesResourceName { get; set; }
    public required string AzureTableStorageAccountName { get; set; }
    public required string AzureTableStoragePassword { get; set; }
    public required string AzureTableStorageServiceEndpoint { get; set; }
}