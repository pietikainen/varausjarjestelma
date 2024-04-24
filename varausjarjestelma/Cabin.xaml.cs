using varausjarjestelma.Controller;

namespace varausjarjestelma;

public partial class Cabin : ContentPage
{
	public Cabin()
	{
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        GetAllCabinData();
    }

    private async void GetAllCabinData()
    {
        ActivityIndicator.IsRunning = true;
        ActivityIndicator.IsVisible = true;

        try
        {     
            var cabins = await CabinController.GetAllCabinDataAsync();
            CabinListView.ItemsSource = cabins;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Virhe ladattaessa mökkitietoja: " + ex.Message);
        }

        ActivityIndicator.IsRunning = false;
        ActivityIndicator.IsVisible = false;
    }

    private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        var keyword = SearchCabinEntry.Text;

        var allCabins = await CabinController.GetAllCabinDataAsync();

        if (string.IsNullOrEmpty(keyword))
        {
            CabinListView.ItemsSource = allCabins;
        }
        else
        {
            var filteredCabins = allCabins.Where(cabin => cabin.CabinName.ToLower().Contains(keyword.ToLower()));
            CabinListView.ItemsSource = filteredCabins;
        }

    }
    private async void AddCabinButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new AddCabinModal());
    }

    private async void CabinListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        String action = await DisplayActionSheet("Choose action for cabin: ", "Cancel", null, "Modify", "Delete");
        var cabin = e.Item as varausjarjestelma.Controller.CabinData;

        if (action == "Modify" && cabin != null)
        {
            await Navigation.PushModalAsync(new AddCabinModal(cabin));
        }
        else if (action == "Delete" && cabin != null)
        {
            var isAccepted = await DisplayAlert("Confirm deletion", "This deletion is permanent.", "Yes", "No");

            if (isAccepted && cabin != null)
            {
                await CabinController.DeleteCabinAsync(cabin.CabinId);
                await RefreshListView();
            }

        }
    }
    private async Task RefreshListView()
    {
        CabinListView.ItemsSource = await CabinController.GetAllCabinDataAsync();
    }

}