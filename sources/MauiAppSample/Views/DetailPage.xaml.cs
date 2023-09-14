namespace MauiAppSample.Views;

using MauiAppSample.ViewModels;

public partial class DetailPage : ContentPage
{
	public DetailPage(DetailPageViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}