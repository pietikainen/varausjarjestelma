using varausjarjestelma.Controller;

namespace varausjarjestelma;

public partial class Services : ContentPage
{
	public Services()
	{
		InitializeComponent();

	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        GetAllServiceData();
    }
    private async void AddServicesButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new AddServiceModal());
    }
    private async void GetAllServiceData()
    {
        try
        {
            var services = await ServiceController.GetAllServiceDataAsync();
            ServicesListView.ItemsSource = services;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Virhe ladattaessa palvelun tietoja: " + ex.Message);
        }
    }
    private async void ServicesListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        string action = await DisplayActionSheet("Choose action for service: ", "Cancel", null, "Modify", "Delete");
        var service = e.Item as varausjarjestelma.Controller.ServiceData;
        if (action == "Modify" && service != null)
        {
            await Navigation.PushModalAsync(new AddServiceModal(service));

        }
        else if (action == "Delete" && service != null)
        {
            var confirmationMessage = $"Name: {service.Name}";
            var isAccepted = await DisplayAlert("Confirm deletion", "This deletion is permanent.", "Yes", "No");

            if (isAccepted && service != null)
            {
                await ServiceController.DeleteServiceAsync(service.ServiceId);
                await RefreshListView();
            }
        }
    }
    private async Task RefreshListView()
    {
        ServicesListView.ItemsSource = await ServiceController.GetAllServiceDataAsync();
    }

}