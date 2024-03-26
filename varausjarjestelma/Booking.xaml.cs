using varausjarjestelma.Controller;
using System.Diagnostics;
namespace varausjarjestelma;

public partial class Booking : ContentPage
{

    // get all areas from database and add them to the picker
    AreaController areaController = new AreaController();
    List<AreaData>? areas;

    public Booking()
	{
        InitializeAreaPicker();
        InitializeComponent();


    }
    private async void InitializeAreaPicker()
    {
        try
        {
            areas = await areaController.GetAllAreaDataAsync();

            foreach (AreaData area in areas)
            {
                AreaPicker.Items.Add(area.Name);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    private async void AreaPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            
            if (AreaPicker.SelectedIndex != -1)
            {
                string selectedArea = AreaPicker.SelectedItem.ToString();
                Debug.WriteLine("Selected area: " + selectedArea);
                int selectedAreaId = 0;

                foreach (AreaData area in areas)
                {
                    if (area.Name == selectedArea)
                    {
                        selectedAreaId = area.AreaId;
                        Debug.WriteLine("Selected area id: " + selectedAreaId);
                        break;
                    }
                }

                var cabinController = new CabinController();
                List<CabinData> cabins = await cabinController.GetCabinsByAreaIdAsync(selectedAreaId);
                if (cabins.Count != 0)
                {

                    Debug.WriteLine("Cabin count: " + cabins.Count);
                    CabinsListView.ItemsSource = cabins;
                    
                    CabinsNullFrame.IsVisible = false;
                    CabinsFrame.IsVisible = true;
                }
                else
                {
                    CabinsFrame.IsVisible = false;
                    CabinsNullFrame.IsVisible = true;
                    Debug.WriteLine("Cabin count: " + cabins.Count);
                    
                }
            }
        } catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }


    private void OnCabinTapped(object sender, ItemTappedEventArgs e) { }
    private void OnCabinSelected(object sender, SelectedItemChangedEventArgs e)
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