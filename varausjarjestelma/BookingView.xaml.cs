using varausjarjestelma.Controller;
using varausjarjestelma.Database;
using System.Diagnostics;


namespace varausjarjestelma;

public partial class BookingView : ContentPage
{

    public BookingView()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ListViewPlaceholder();
        GetAllBookingData();
    }

    private async void GetAllBookingData()
    {
        try
        {
            BookingListActivityIndicator.IsRunning = true;
            BookingListActivityIndicator.IsVisible = true;

            var customers = await ReservationController.GetAllReservationDataAsync();
            BookingListView.ItemsSource = customers;

            BookingListActivityIndicator.IsRunning = false;
            BookingListActivityIndicator.IsVisible = false;
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

    //  Delete reservation button clicked:

    private async void DeleteReservationButtonClicked(object sender, EventArgs e) {
            var reservation = (sender as Button).CommandParameter as Reservation;
           var isAccepted = await DisplayAlert("Confirm deletion", "This deletion is permanent.", "Yes", "No");
    
           if (isAccepted && reservation != null)
        {
            try { 
            await ServicesOnReservationController.DeleteAllServicesOnReservationByReservationIdAsync(reservation.varaus_id);
            await ReservationController.DeleteReservationAsync(reservation.varaus_id);
                }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            await RefreshListView();
        }
        else
        {
            await DisplayAlert("Error", "Reservation is null. Cannot delete.", "OK");
        }
    }


    private async void BookingListViewItemTapped(object sender, ItemTappedEventArgs e)
    {
        string action = await DisplayActionSheet("Choose action for customer: ", "Cancel", null, "Modify", "Delete");
        var customer = e.Item as varausjarjestelma.Controller.CustomerData;

        if (action == "Modify" && customer != null)
        {
            await Navigation.PushModalAsync(new AddCustomerModal(customer));
        }
        else if (action == "Delete" && customer != null)
        {
            var confirmationMessage = $"Name: {customer.FullName}\nAddress: {customer.Address}\nPostal Code: {customer.PostalCode}" +
                $"\nPhone: {customer.Phone}\nEmail: {customer.Email}\n";
            var isAccepted = await DisplayAlert("Confirm deletion", "This deletion is permanent.", "Yes", "No");

            if (isAccepted && customer != null)
            {
                await CustomerController.DeleteCustomerAsync(customer.CustomerId);
                await RefreshListView();
            }
            else
            {
                await DisplayAlert("Error", "Customer is null. Cannot delete.", "OK");
            }

        }
    }

    private void ListViewPlaceholder()
    {
        if (!BookingListView.IsVisible)
        {
            listViewSpinner.IsVisible = true;
        }
        else
        {
            listViewSpinner.IsVisible = false;
        }
    }

    // Search bar to filter customers

    private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        var keyword = SearchCustomerEntry.Text;

        var allCustomers = await ReservationController.GetAllReservationDataAsync();

        if (string.IsNullOrEmpty(keyword))
        {
            BookingListView.ItemsSource = allCustomers;
        }
        else
        {
            var filteredCustomers = allCustomers;
            BookingListView.ItemsSource = filteredCustomers;
        }

    }

    // OBS!! WORK IN PROGRESS


    // Sort customers by name

    //private async void SortButton_Clicked(object sender, EventArgs e)
    //{
    //    var allCustomers = await CustomerController.GetAllCustomerDataAsync();
    //    var sortedCustomers = allCustomers.OrderBy(customer => customer.FullName);
    //    CustomerListView.ItemsSource = sortedCustomers;
    //}



    private async void ModifyReservationButtonClicked(object sender, EventArgs e) { }

    private async void SendInvoiceButtonClicked(object sender, EventArgs e) { }

    private async void RemoveReservationButtonClicked(object sender, EventArgs e)










    private async void AddBookingButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Booking());
    }



    private async Task RefreshListView()
    {
        BookingListView.ItemsSource = await ReservationController.GetAllReservationDataAsync();
    }



}