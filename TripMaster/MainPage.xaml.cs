using Microsoft.Maui.Controls;

namespace TripMaster;

public partial class MainPage : ContentPage
{
    private readonly MainPageViewModel vm;

    public MainPage(MainPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        this.vm = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        DeviceDisplay.Current.KeepScreenOn = true;
    }

    protected async void StopButton_Clicked(object sender, EventArgs e)
    {
        if (await DisplayAlert(@"/!\ ARRÊTER LA SESSION ? /!\", "", "Confirmer", "Annuler"))
            vm.StopCommand.Execute(null);
    }
}