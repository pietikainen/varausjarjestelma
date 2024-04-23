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
        try
        {     
            var cabins = await CabinController.GetAllCabinDataAsync();
            CabinListView.ItemsSource = cabins;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Virhe ladattaessa mökkitietoja: " + ex.Message);
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
        CabinListView.ItemsSource = await CustomerController.GetAllCustomerDataAsync();
    }

}