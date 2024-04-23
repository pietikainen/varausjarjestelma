using varausjarjestelma.Controller;
using varausjarjestelma.Database;
using System.Diagnostics;
using System.Drawing.Text;
namespace varausjarjestelma;

public partial class Booking : ContentPage
{

    // get all areas from database and add them to the picker
    AreaController areaController = new AreaController();
    List<AreaData>? areas;
    private int selectedAreaId;

    // Init search bar


    public Booking()
    {
        // Initialize
        InitializeAreaPicker();
        InitializeComponent();

        // Clear forms
        ResetAllFields();

        // Populate all customers
        GetAllCustomersData();
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
                int selectedAreaId = 0;

                foreach (AreaData area in areas)
                {
                    if (area.Name == selectedArea)
                    {
                        selectedAreaId = area.AreaId;
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
                //Get all services for the selected area and add them to the frame

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
            List<CustomerData> customers = await CustomerController.GetAllCustomerDataAsync();

            // in the rare case of no customers, show a message in listview
            CustomerData emptyCustomerData = new CustomerData();
            emptyCustomerData.FullName = "No customers found";

            List<CustomerData> nullCustomers = new List<CustomerData>();
            nullCustomers.Add(emptyCustomerData);

            if (customers.Count != 0)
            {
                CustomerListView.ItemsSource = customers;
            }
            else
            {
                CustomerListView.ItemsSource = nullCustomers;
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

    // Search for customers

    private async void CustomerSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        var keyword = CustomerSearchBar.Text;

        var allCustomers = await CustomerController.GetAllCustomerDataAsync();

        if (string.IsNullOrEmpty(keyword))
        {
            CustomerListView.ItemsSource = allCustomers;
        }
        else
        {
            var filteredCustomers = allCustomers.Where(customer => customer.FullName.ToLower().Contains(keyword.ToLower()));
            CustomerListView.ItemsSource = filteredCustomers;
        }

    }




    private async void ServicesDataList(int selectedAreaId)
    {
        try
        {
            var ctrl = new ServiceController();
            var services = await ctrl.GetServiceDataByAreaId(selectedAreaId);

            ServicesPicker.Children.Clear();

            Debug.WriteLine("Services count: " + services.Count);

            if (services.Count == 0)
            {
                Label noServicesLabel = new Label
                {
                    Text = "No services found",
                    HorizontalOptions = LayoutOptions.Center
                };
                ServicesPicker.Children.Add(noServicesLabel);
            }


            foreach (ServiceData service in services)
            {
                Stepper serviceStepper = new Stepper
                {
                    Minimum = 0,
                    Maximum = 10,
                    Increment = 1,
                    HorizontalOptions = LayoutOptions.Center,
                };

                Label quantityLabel = new Label
                {
                    Text = "0",
                    HorizontalOptions = LayoutOptions.Center
                };

                Label serviceNameLabel = new Label
                {
                    Text = service.Name,
                    HorizontalOptions = LayoutOptions.Center
                };

                serviceNameLabel.ClassId = service.ServiceId.ToString();

                serviceStepper.ValueChanged += (s, e) =>
                {
                    quantityLabel.Text = e.NewValue.ToString();
                    int quantity = (int)e.NewValue;

                };

                StackLayout stepperLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children = { quantityLabel, serviceStepper, serviceNameLabel }
                };

                ServicesPicker.Children.Add(stepperLayout);

                
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

    // method to gather info from page entries and selections and to send to modal:


    private async void OnBookNowClicked(object sender, EventArgs e)
    {
        // Form ReservationInfo class from user input
        ReservationInfo reservationInfo = new ReservationInfo();

        Dictionary<int, int> servicesDictToBooking = new Dictionary<int, int>();


        // Add services to reservationInfo

        reservationInfo.Services = new Dictionary<int, int>();
        foreach (var stepperLayout in ServicesPicker.Children)
        {
            if (stepperLayout is StackLayout stackLayout)
            {
                foreach (var child in stackLayout.Children)
                {
                    if (child is Stepper stepper)
                    {
                        try
                        {
                            Label serviceNameLabel = stackLayout.Children.OfType<Label>().FirstOrDefault(l => l.ClassId != null);
                            if (serviceNameLabel != null)
                            {
                                int serviceId = int.Parse(serviceNameLabel.ClassId);
                                int quantity = (int)stepper.Value;
                                reservationInfo.Services.Add(serviceId, quantity);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Error: " + ex.Message);
                        }
                    }
                }
            }
        }
        Debug.WriteLine("Services to booking: " + servicesDictToBooking.Count);


        if (CustomerListView.SelectedItem != null)
        {
            reservationInfo.CustomerId = (CustomerListView.SelectedItem as CustomerData).CustomerId;
        }
        else
        {
            Debug.WriteLine("Customer not selected");
        }

        if (listViewCabinMain.SelectedItem != null)
        {
            reservationInfo.CabinId = (listViewCabinMain.SelectedItem as CabinData).CabinId;
        }
        else
        {
            Debug.WriteLine("Cabin not selected");
        }

        if (CheckInDatePicker.Date >= CheckOutDatePicker.Date)
        {
            Debug.WriteLine("Check-in date must be before check-out date");
            return;
        }

        reservationInfo.StartDate = CheckInDatePicker.Date;
        reservationInfo.EndDate = CheckOutDatePicker.Date;

        // Open a modal with details + confirmation button
        Debug.WriteLine("Reservation info: " + reservationInfo.CustomerId + " " + reservationInfo.CabinId + " " + reservationInfo.StartDate + " " + reservationInfo.EndDate);
        await Navigation.PushModalAsync(new AddReservationModal(reservationInfo));
        ResetAllFields();
    }

    public void ResetAllFields()
    {
        // Reset Entries
        firstNameEntry.Text = string.Empty;
        lastNameEntry.Text = string.Empty;
        addressEntry.Text = string.Empty;
        postalCodeEntry.Text = string.Empty;
        cityEntry.Text = string.Empty;
        emailEntry.Text = string.Empty;
        phoneNumberEntry.Text = string.Empty;


        // Reset Pickers
        AreaPicker.SelectedIndex = -1; // Reset Picker to default selection
        listViewCabinMain.SelectedItem = null; // Deselect ListView item
        ServicesPicker.Children.Clear(); // Clear the StackLayout that contains services

        // Reset DatePickers
        CheckInDatePicker.Date = DateTime.Today;
        CheckOutDatePicker.Date = DateTime.Today;

        // Reset cabin listview
        listViewCabinMain.SelectedItem = null;

        // Reset ListView
        CustomerListView.SelectedItem = null;
    }






    private void OnCancelClicked(object sender, EventArgs e)
    {
        ResetAllFields();
    }

}
//private void StartDatePicker_DateSelected(object sender, DateChangedEventArgs e)
//{
//    StartDay.Text = e.NewDate.ToString();
//}

//private void EndDatePicker_DateSelected(object sender, DateChangedEventArgs e)
//{
//    EndDay.Text = e.NewDate.ToString();
//}
