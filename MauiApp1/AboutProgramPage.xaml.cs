namespace MauiApp1;

public partial class AboutProgramPage : ContentPage
{
    public AboutProgramPage()
    {
        InitializeComponent();
    }

    private async void OnMainPageButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
