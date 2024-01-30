namespace varausjarjestelma;

public partial class AreaManagement : ContentPage
{
    public AreaManagement()
    {
        InitializeComponent();
    }


    private void AreaListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {

    }

    private void AreaPopupOkButtonClicked(object sender, EventArgs e)
    {
        PopupFrame.IsVisible = false;
        LodgingAndService.IsVisible = true;
        MainMenuButton.IsVisible = true;
        
    }
    private void AddLodgingButton_Clicked(object sender, EventArgs e)
    {
        Addlodging.IsVisible = true;
        LodgingAndService.IsVisible = false;
        AddNewLodgingButton.IsVisible = true;
        LodgesLabel.IsVisible = true;
        ReturnLodgingButton.IsVisible = true;
        SaveLodgingButton.IsVisible = true;
        MainMenuButton.IsVisible = false;
    }

    private void AddServiceButton_Clicked(object sender, EventArgs e)
    {
        AddLodgingButton.IsVisible = false;
        LodgingAndService.IsVisible = false;
        AddService.IsVisible = true;
        MainMenuButton.IsVisible = false;
    }
    private void AreaPopupCancelButtonClicked(object sender, EventArgs e)
    {

        LodgingAndService.IsVisible = true;
        PopupFrame.IsVisible = false;
        MainMenuButton.IsVisible = true;
    }



    private void ServiceListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {

    }

    private void LodgingListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {

    }

    private void AddLodgingListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {

    }

    private void AddServiceListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {

    }

    private void AddNewServiceButton_Clicked(object sender, EventArgs e)
    {

    }

    private void DeleteServiceButton_Clicked(object sender, EventArgs e)
    {

    }

    private void AddNewLodgingButton_Clicked(object sender, EventArgs e)
    {
        AddLodgingButton.IsVisible = true;

    }

    async void MainMenuButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }

    private void SaveLodgingButton_Clicked(object sender, EventArgs e)
    {

    }

    private void ReturnLodgingButton_Clicked(object sender, EventArgs e)
    {
        Addlodging.IsVisible = false;
        LodgingAndService.IsVisible = true;
        AddNewLodgingButton.IsVisible = false;
        LodgesLabel.IsVisible = false;
        ReturnLodgingButton.IsVisible = false;
        SaveLodgingButton.IsVisible = false;
        MainMenuButton.IsVisible = true;

    }

    private void ReturnServiceButton_Clicked(object sender, EventArgs e)
    {
        AddLodgingButton.IsVisible = true;
        LodgingAndService.IsVisible=true;
        AddService.IsVisible = false;
        MainMenuButton.IsVisible=true;
    }

    private void ChooseCityButton_Clicked(object sender, EventArgs e)
    {
        LodgingAndService.IsVisible=false;
        PopupFrame.IsVisible = true;
        MainMenuButton.IsVisible = false;
    }

    private void AddCitybutton_Clicked(object sender, EventArgs e)
    {
        LodgingAndService.IsVisible=false;
        MainMenuButton.IsVisible = false;
        AddCitybutton.IsVisible = true;
        AddCityEntry.IsVisible = true;
        CancelAddingCityButton.IsVisible = true;
        
    }

    private void AddNewCityButton_Clicked(object sender, EventArgs e)
    {
        MainMenuButton.IsVisible = true;
        
        AddCityEntry.IsVisible = false;
        CancelAddingCityButton.IsVisible = false;
        LodgingAndService.IsVisible = true;
    }

    private void CancelAddingCityButton_Clicked(object sender, EventArgs e)
    {
        MainMenuButton.IsVisible = true;
        
        AddCityEntry.IsVisible = false;
        CancelAddingCityButton.IsVisible = false;
        LodgingAndService.IsVisible = true;
    }
}