namespace FunctionApp.Services;

internal class SettingsService
{
    public string? TableStorageConnectionString => Environment.GetEnvironmentVariable("TableStorageConnectionString");
}