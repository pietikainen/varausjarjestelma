namespace varausjarjestelma;

public partial class Booking : ContentPage
{
	public Booking()
	{
        InitializeCityPicker();
        InitializeComponent();
	}
    private void InitializeCityPicker()
    {
        // Täytä CityPicker kaupunkien nimillä tietokannasta
    }

    private void OnCottageSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // Näytä valitun mökin tiedot CottageDetails StackLayoutissa
        // ja aseta IsVisible=true näyttääksesi sen
    }

    private void OnBookNowClicked(object sender, EventArgs e)
    {
        // Kerää lomakkeen tiedot ja tee varaus
    }

    private void StartDatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        StartDay.Text = e.NewDate.ToString();
    }

    private void EndDatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        EndDay.Text = e.NewDate.ToString();
    }

    private void MainMenuButtonClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MainPage());
    }
}