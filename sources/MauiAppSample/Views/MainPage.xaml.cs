namespace MauiAppSample.Views;

using MauiAppSample.ViewModels;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}