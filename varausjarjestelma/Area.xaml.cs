using varausjarjestelma.Controller;
namespace varausjarjestelma;

public partial class Area : ContentPage
{
	public Area()
	{
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        GetAllAreaData();
    }

    private async void GetAllAreaData()
    {
        ActivityIndicator.IsVisible = true;
        ActivityIndicator.IsRunning = true;


        try
        {
            var areas = await AreaController.GetAllAreaDataAsync();
            AreaListView.ItemsSource = areas;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Virhe ladattaessa mökkitietoja: " + ex.Message);
        }

        ActivityIndicator.IsVisible = false;
        ActivityIndicator.IsRunning = false;
    }


    private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        var keyword = SearchAreaEntry.Text;

        var allAreas = await AreaController.GetAllAreaDataAsync();

        if (string.IsNullOrEmpty(keyword))
        {
            AreaListView.ItemsSource = allAreas;
        }
        else
        {
            var filteredAreas = allAreas.Where(area => area.Name.ToLower().Contains(keyword.ToLower()));
            AreaListView.ItemsSource = filteredAreas;
        }

    }

    private async void AddAreaButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new AddAreaModal());
    }

    private async void AreaListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        string action = await DisplayActionSheet("Choose action for customer: ", "Cancel", null, "Modify", "Delete");
        var area = e.Item as varausjarjestelma.Controller.AreaData;
        if (action == "Modify" && area != null)
        {
            await Navigation.PushModalAsync(new AddAreaModal(area));

        }
        else if (action == "Delete" && area != null)
        {
            var confirmationMessage = $"Name: {area.Name}";
            var isAccepted = await DisplayAlert("Confirm deletion", "This deletion is permanent.", "Yes", "No");

            if (isAccepted && area != null)
            {
                await AreaController.DeleteAreaAsync(area.AreaId);
                await RefreshListView();
            }
            else
            {

            }
        }
    }
    private async Task RefreshListView()
    {
        AreaListView.ItemsSource = await AreaController.GetAllAreaDataAsync();
    }
}