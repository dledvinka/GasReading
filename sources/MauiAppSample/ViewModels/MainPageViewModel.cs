namespace MauiAppSample.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiAppSample.Interfaces;
using MauiAppSample.Models;

public partial class MainPageViewModel : BaseViewModel
{
    private readonly IGasMeterReadingService _gasMeterReadingService;

    [ObservableProperty] 
    private List<GasMeterReading> _readings;


    public MainPageViewModel(IGasMeterReadingService gasMeterReadingService)
    {
        Title = "Reading list";
        _gasMeterReadingService = gasMeterReadingService;
    }

    [RelayCommand]
    public async Task GetReadingsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            Readings = await _gasMeterReadingService.GetAllAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error!", $"Unable to get data: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}