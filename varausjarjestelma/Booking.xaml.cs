using varausjarjestelma.Controller;
using varausjarjestelma.Database;
using System.Diagnostics;
using Microsoft.Maui.Controls;
using System.Drawing;
using System.ComponentModel.Design;
namespace varausjarjestelma;

public partial class Booking : ContentPage
{

    // get all areas from database and add them to the picker
    AreaController areaController = new AreaController();
    List<AreaData>? areas;


    public List<Service> servicesList = new List<Service>();



    // Init search bar


    public Booking()
    {
        InitializeAreaPicker();
        InitializeComponent();
        servicesList.Clear();
        ServicesCheckboxFrame.Clear();
    }

    private async void InitializeAreaPicker()
    {
        try
        {
            areas = await AreaController.GetAllAreaDataAsync();
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


                // If no cabins found, show a message in listview
                CabinData emptyCabinData = new CabinData();
                emptyCabinData.CabinName = "No cabins found";

                List<CabinData> nullCabins = new List<CabinData>();
                nullCabins.Add(emptyCabinData);

                if (cabins.Count != 0)
                {
                    Debug.WriteLine("Cabin count: " + cabins.Count);
                    listViewCabinMain.ItemsSource = cabins;
                }
                else
                {
                    listViewCabinMain.ItemsSource = nullCabins;
                    Debug.WriteLine("Cabin count: " + cabins.Count);
                }
                // Get all services for the selected area and add them to the frame
                ServicesDataList(selectedAreaId);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    private async void GetAllCustomersData()
    {
        try
        {
            var customers = await CustomerController.GetAllCustomerDataAsync();
            CustomerListView.ItemsSource = customers;
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

    // Search for customers

    public void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue;
    }

    private void UpdateFilteredCustomers()
    {

    }

    private async void ServicesDataList(int selectedAreaId)
    {
        try
        {
            var ctrl = new ServiceController();
            var services = await ctrl.GetServiceDataByAreaId(selectedAreaId);
            ServicesCheckboxFrame.Clear();
            servicesList.Clear();

            Debug.WriteLine("Services count: " + services.Count);

            // Checkboxes label string array

            // Form new frame of checkboxes about services
            foreach (ServiceData service in services)
            {
                CheckBox checkBox = new CheckBox();
                Label label = new Label
                {
                    VerticalOptions = LayoutOptions.Center
                };

                label.Text = service.Name;
                checkBox.IsChecked = false;

                // Add event listener to checkbox
                checkBox.CheckedChanged += (s, e) =>
                {
                    Debug.WriteLine("Checked: " + checkBox.IsChecked);
                    if (checkBox.IsChecked)
                    {
                        // Add service to list
                        
                    }
                    else
                    {
                        // when unchecked, remove service from list
                        
                    }
                };

                // Add checkbox and label to stacklayout
                StackLayout stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children = { checkBox, label }
                };

                ServicesCheckboxFrame.Children.Add(stackLayout);
            }
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

    // Super method to concat several classes to send to reservation details in the modal
    private ReservationDraft SuperMasterReservationDraftMethod(object sender, EventArgs e)
    {
        // Form ReservationDraft class from user input

        ReservationDraft reservationDraft = new ReservationDraft();

        // Add customer data to reservationDraft
        reservationDraft.customer = new Database.Customer {

            asiakas_id = 1,
            etunimi = "John",
            sukunimi = "Doe",
            lahiosoite = "Street 1",
            postinro = "12345",
            email = "",
            puhelinnro = "12394722"
        };


        // Add cabin data to reservationDraft
        reservationDraft.cabin = new Database.Cabin
        {
            mokki_id = 1,
            alue_id = 2,
            hinta = 100.00
        };

        // Add services data to reservationDraft

        reservationDraft.services = new List<ServiceOnReservation>();
        foreach (Service service in servicesList)
        {
            reservationDraft.services.Add(new ServiceOnReservation
            {
                varaus_id = 1,
                palvelu_id = service.palvelu_id,
                lkm = 1
            });
        }




        // return reservationDraft for params to modal

        return reservationDraft;
        



    }

    private void OnCabinTapped(object sender, ItemTappedEventArgs e)
    {
    }
    private void OnServicesTapped(object sender, ItemTappedEventArgs e) { }
    private void OnCabinSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // 
    }



    private async void OnBookNowClicked(object sender, EventArgs e)
    {
        // Open a modal with details + confirmation button

        await Navigation.PushModalAsync(new AddReservationModal());
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        // Tyhjennä lomake
    }


    //private void StartDatePicker_DateSelected(object sender, DateChangedEventArgs e)
    //{
    //    StartDay.Text = e.NewDate.ToString();
    //}

    //private void EndDatePicker_DateSelected(object sender, DateChangedEventArgs e)
    //{
    //    EndDay.Text = e.NewDate.ToString();
    //}

    private void MainMenuButtonClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MainPage());
    }
}
