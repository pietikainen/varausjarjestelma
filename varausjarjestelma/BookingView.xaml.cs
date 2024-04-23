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


    // Delete reservation

    private async void DeleteReservation(int id)
    {
        var isDeleted = await ReservationController.DeleteReservationAsync(id);

        if (isDeleted)
        {
            await DisplayAlert("Success", "Reservation deleted successfully", "OK");
            
        }
        else
        {
            await DisplayAlert("Error", "An error occurred while deleting reservation", "OK");
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


    private async void SetConfirmedButtonClicked(object sender, EventArgs e) 
    {

        var button = sender as ImageButton;
        if (button == null)
        {
            Debug.WriteLine("Button is null");
            return;
        }

        var reservation = button.BindingContext as ReservationListViewItems;
        if (reservation == null)
        {
            Debug.WriteLine("Reservation is null");
            return;
        }

        var isAccepted = await DisplayAlert("Confirm", "Are you sure you want to confirm this reservation?", "Yes", "No");

        if (!isAccepted)
        {
            Debug.WriteLine("Confirmation cancelled");
            return;
        }

        await ReservationController.SetReservationConfirmedAsync(reservation.reservationId);
        await RefreshListView();
    }


    private async void ModifyReservationButtonClicked(object sender, EventArgs e) { }

    private async void CreateInvoiceButtonClicked(object sender, EventArgs e) {

        var button = sender as ImageButton;
        if (button == null)
        {
            Debug.WriteLine("Button is null");
            return;
        }

        var reservation = button.BindingContext as ReservationListViewItems;
        if (reservation == null)
        {
            Debug.WriteLine("Reservation is null");
            return;
        }

        //TÄMÄ ON KESKEN

        var isCreated = await InvoiceController.CreateInvoiceAsync(reservation.reservationId);
        if (!isCreated) {
            await DisplayAlert("Error", "An error occurred while creating invoice", "OK");
        }
        else
        {
            await DisplayAlert("Confirmation", $"Invoice for reservation # {reservation.reservationId} has been created.", "OK");
        }

    }

    private async void RemoveReservationButtonClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        if (button == null)
        {
            Debug.WriteLine("Button is null");
            return;
        }

        var reservation = button.BindingContext as ReservationListViewItems;
        if (reservation == null)
        {
            Debug.WriteLine("Reservation is null");
            return;
        }


        var isAccepted = await DisplayAlert("Delete", "Are you sure you want to delete this reservation?", "Yes", "No");

        if (!isAccepted)
        {
            Debug.WriteLine("Deletion cancelled");
            return;
        }

        DeleteReservation(reservation.reservationId);
        await RefreshListView();
    }







    private async void AddBookingButtonClicked(object sender, EventArgs e)
    {

        await Navigation.PushAsync(new Booking());
    }



    private async Task RefreshListView()
    {
        BookingListView.ItemsSource = await ReservationController.GetAllReservationDataAsync();
    }



}