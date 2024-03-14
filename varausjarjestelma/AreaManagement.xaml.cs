using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using varausjarjestelma.Database;
using varausjarjestelma.Controller;
using static System.Net.Mime.MediaTypeNames;

namespace varausjarjestelma;

public partial class AreaManagement : ContentPage
{
    public AreaManagement()
    {
        InitializeComponent();
        BindingContext = new ServiceViewModel();
        ServicesListData();
    }

    private async void ServicesListData()
    {
        try
        {
            var dataB = new ServiceController();
            var services = await dataB.GetAllServiceDataAsync();
            ServicesListView.ItemsSource = services;
            AddServicesListView.ItemsSource = services;
        }
        catch (AggregateException ae)
        {
            foreach (var innerException in ae.InnerExceptions)
            {
                Debug.WriteLine($"Inner Exception: {innerException.Message}");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An error occurred: {ex.Message}");
        }
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
        AddNewCityButton.IsVisible = false;
        CancelAddingCityButton.IsVisible = false;
    }

    private void AddServiceButton_Clicked(object sender, EventArgs e)
    {
        AddLodgingButton.IsVisible = false;
        LodgingAndService.IsVisible = false;
        AddService.IsVisible = true;
        MainMenuButton.IsVisible = false;
        AddCitybutton.IsVisible = false;
        CancelAddingCityButton.IsVisible = false;
        AddNewCityButton.IsVisible= false;
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
        var context = new varausjarjestelma.Database.AppContext();

        Service newService = new Service()
        {
            //palvelu_id = 40004, // lis‰‰ kantaan palvelu_id:lle AUTO_INCREMENT
            alue_id = Convert.ToInt32(ServiceTypeEntry.Text),
            nimi = ServiceNameEntry.Text.ToString(),
            kuvaus = ServiceDescEntry.Text.ToString(),
            tyyppi = Convert.ToInt32(ServiceTypeEntry.Text),
            hinta = Convert.ToInt32(ServicePriceEntry.Text),
            alv = Convert.ToInt32(ServiceVatEntry.Text)
        };

        context.palvelu.Add(newService);
        context.SaveChanges();
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
        AddCitybutton.IsVisible = true;
        AddNewCityButton.IsVisible = true;
    }

    private void ChooseCityButton_Clicked(object sender, EventArgs e)
    {
        LodgingAndService.IsVisible=false;
        PopupFrame.IsVisible = true;
        MainMenuButton.IsVisible = false;
        AddNewCityButton.IsVisible=false;
    }

    private void AddCitybutton_Clicked(object sender, EventArgs e)
    {
        LodgingAndService.IsVisible=false;
        MainMenuButton.IsVisible = false;
        AddCitybutton.IsVisible = true;
        AddCityEntry.IsVisible = true;
        CancelAddingCityButton.IsVisible = true;
        AddNewCityButton.IsVisible = true;
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