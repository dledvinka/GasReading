namespace MauiAppSample.Interfaces;

using MauiAppSample.Models;

public interface IGasMeterReadingService
{
    Task<List<GasMeterReading>> GetAllAsync();
}