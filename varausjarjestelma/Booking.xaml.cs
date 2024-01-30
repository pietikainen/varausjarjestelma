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
        // T�yt� CityPicker kaupunkien nimill� tietokannasta
    }

    private void OnCottageSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // N�yt� valitun m�kin tiedot CottageDetails StackLayoutissa
        // ja aseta IsVisible=true n�ytt��ksesi sen
    }

    private void OnBookNowClicked(object sender, EventArgs e)
    {
        // Ker�� lomakkeen tiedot ja tee varaus
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