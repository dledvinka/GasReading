namespace MauiAppSample.Services;

using MauiAppSample.Interfaces;
using MauiAppSample.Models;

internal class GasMeterReadingService : IGasMeterReadingService
{
    private List<GasMeterReading> _readings = Enumerable.Range(0, 25).Select(i => new GasMeterReading()
    {
        Created = new DateTime(2020, 1, 1).AddMonths(i),
        MeterValue = 100.0M + i * 100.0M,
        Image = "https://avatars.githubusercontent.com/u/42899101?v=4"
    }).ToList();
        
    public async Task<List<GasMeterReading>> GetAllAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(5));
        return _readings;
    }
}