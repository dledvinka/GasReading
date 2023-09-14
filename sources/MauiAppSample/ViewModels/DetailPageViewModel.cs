namespace MauiAppSample.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using MauiAppSample.Models;

[QueryProperty("Reading", "Reading")]
public partial class DetailPageViewModel : BaseViewModel
{
    [ObservableProperty] 
    private GasMeterReading reading;

    public DetailPageViewModel()
    {
        Title = "Gas meter reading detail";
    }
}