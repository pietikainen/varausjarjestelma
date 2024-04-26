using varausjarjestelma.Controller;
using varausjarjestelma.Database;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.IdentityModel.Tokens;


namespace varausjarjestelma;

public partial class BookingView : ContentPage
{

    public BookingView()
    {
        InitializeComponent();

    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        BookingListView.ItemsSource = null;
        GetAllBookingData();
    }

    private async void GetAllBookingData()
    {
        try
        {
            BookingListActivityIndicator.IsRunning = true;
            BookingListActivityIndicator.IsVisible = true;
            var bookings = await ReservationController.GetAllReservationDataAsync();

            DateTime nullDate = new DateTime(1900, 1, 1);

            if (bookings != null)
            {
                // tiedon muokkaus:
                foreach (ReservationListViewItems r in bookings)
                {
                    //if (r.confirmationDate == nullDate)
                    //{
                    //    r.confirmationDateString = "";
                    //}
                    //else
                    //{
                    //    r.confirmationDateString = r.confirmationDate.ToString("dd.MM.yyyy");
                    //}

                    if (!r.AreaName.IsNullOrEmpty() && !r.cabinName.IsNullOrEmpty())
                    {
                        r.cabinName = r.cabinName + ", " + r.AreaName;
                    }
                }
                Debug.WriteLine("Bookings: ");
                foreach (var booking in bookings)
                {
                    Debug.WriteLine($"Reservation ID: {booking.reservationId}, Customer: {booking.customerName}, Start date: {booking.startDate}, End date: {booking.endDate}, Cabin: {booking.cabinName}, Area: {booking.AreaName}, Confirmed: {booking.confirmationDate}");
                }
                BookingListView.ItemsSource = bookings;


                BookingListActivityIndicator.IsRunning = false;
                BookingListActivityIndicator.IsVisible = false;
                //return customers;

            } 
            else
            {
                Debug.WriteLine("Bookings is null");
            }
        }

        catch (AggregateException ae)
        {
            foreach (var innerException in ae.InnerExceptions)
            {
                Debug.WriteLine($"Inner Exception: {innerException.Message}");
            }
        }
        catch (COMException ce)
        {
            Debug.WriteLine("ERROR: " + ce.Message);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An error occurred in GetAllBookingData(): {ex.Message}");
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
            var filteredCustomers = allCustomers.Where(reservation => reservation.customerName.ToLower().Contains(keyword.ToLower()));
            BookingListView.ItemsSource = filteredCustomers;
        }
    }

    private async void ReservationIdSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        var keyword = SearchReservationIdEntry.Text;

        var allCustomers = await ReservationController.GetAllReservationDataAsync();

        if (string.IsNullOrEmpty(keyword))
        {
            BookingListView.ItemsSource = allCustomers;
        }
        else
        {
            if (int.TryParse(keyword, out int reservationNumber))
            {
                var filteredCustomers = allCustomers.Where(reservation => reservation.reservationId == reservationNumber);
                BookingListView.ItemsSource = filteredCustomers;
            }
            else
            {
                // Jos syöte ei ole kelvollinen numero, voit esimerkiksi tyhjentää tulokset
                BookingListView.ItemsSource = null;
            }
        }
    }


    //

    // OBS!! WORK IN PROGRESS


    // Sort customers by name

    //private async void SortButton_Clicked(object sender, EventArgs e)
    //{
    //    var allCustomers = await CustomerController.GetAllCustomerDataAsync();
    //    var sortedCustomers = allCustomers.OrderBy(customer => customer.FullName);
    //    CustomerListView.ItemsSource = sortedCustomers;
    //}

    private async void BookingListViewItemTapped(object sender, ItemTappedEventArgs e)
    {
        var booking = e.Item as varausjarjestelma.Controller.ReservationListViewItems;
        if (booking == null)
        {
            Debug.WriteLine("Booking is null");
            return;
        }
        else
        {
            string action = await DisplayActionSheet($"Select an action for Res. # {booking.reservationId}", "Cancel", null, "Modify", "Delete", "Create invoice", "Confirm");

            if (action == "Modify" && booking != null)
            {
                await Navigation.PushAsync(new Booking(booking));
            }
            else if (action == "Delete" && booking != null)
            {
                var isAccepted = await DisplayAlert("Confirm deletion", "This deletion is permanent.", "Yes", "No");
                if (isAccepted && booking != null)
                {
                    DeleteReservation(booking.reservationId);
                    await RefreshListView();
                }
                else
                {
                    await DisplayAlert("Error", "Reservation is null. Cannot delete.", "OK");
                }
            }
            else if (action == "Create invoice" && booking != null)
            {
                var isCreated = await InvoiceController.CreateInvoiceAsync(booking.reservationId);
                if (!isCreated)
                {
                    await DisplayAlert("Error", "An error occurred while creating invoice", "OK");
                }
                else
                {
                    await DisplayAlert("Confirmation", $"Invoice for reservation # {booking.reservationId} has been created.", "OK");
                }
            }
            else if (action == "Confirm" && booking != null)
            {
                var isAccepted = await DisplayAlert("Confirm", "Are you sure you want to confirm this reservation?", "Yes", "No");
                if (isAccepted)
                {
                    await ReservationController.SetReservationConfirmedAsync(booking.reservationId);
                    await RefreshListView();
                }
            }
        }
    }




    //private async void SetConfirmedButtonClicked(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        var button = sender as ImageButton;
    //        if (button == null)
    //        {
    //            Debug.WriteLine("Button is null");
    //            return;
    //        }

    //        var reservation = button.BindingContext as ReservationListViewItems;
    //        if (reservation == null)
    //        {
    //            Debug.WriteLine("Reservation is null");
    //            return;
    //        }

    //        var isAccepted = await DisplayAlert("Confirm", "Are you sure you want to confirm this reservation?", "Yes", "No");

    //        if (!isAccepted)
    //        {
    //            Debug.WriteLine("Confirmation cancelled");
    //            return;
    //        }

    //        await ReservationController.SetReservationConfirmedAsync(reservation.reservationId);
    //        await RefreshListView();
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine($"An error occurred in ConfirmationButton: {ex.Message}");
    //    }
    //}


    //private async void ModifyReservationButtonClicked(object sender, EventArgs e)
    //{

    //    try
    //    {
    //        var button = sender as ImageButton;
    //        if (button == null)
    //        {
    //            Debug.WriteLine("Button is null");
    //            return;
    //        }

    //        var reservation = button.BindingContext as ReservationListViewItems;
    //        if (reservation == null)
    //        {
    //            Debug.WriteLine("Reservation is null");
    //            return;
    //        }
    //        Debug.WriteLine("Modify reservation button clicked + sent id: " + reservation.reservationId);
    //        await Navigation.PushAsync(new Booking(reservation));
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine($"An error occurred in Modifybutton: {ex.Message}");
    //    }
    //}

    //private async void CreateInvoiceButtonClicked(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        var button = sender as ImageButton;
    //        if (button == null)
    //        {
    //            Debug.WriteLine("Button is null");
    //            return;
    //        }

    //        var reservation = button.BindingContext as ReservationListViewItems;
    //        if (reservation == null)
    //        {
    //            Debug.WriteLine("Reservation is null");
    //            return;
    //        }

    //        //TÄMÄ ON KESKEN

    //        var isCreated = await InvoiceController.CreateInvoiceAsync(reservation.reservationId);
    //        if (!isCreated)
    //        {
    //            await DisplayAlert("Error", "An error occurred while creating invoice", "OK");
    //        }
    //        else
    //        {
    //            await DisplayAlert("Confirmation", $"Invoice for reservation # {reservation.reservationId} has been created.", "OK");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine($"An error occurred in CreateInvoiceButtonClicked: {ex.Message}");
    //    }

    //}

    //private async void RemoveReservationButtonClicked(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        var button = sender as ImageButton;
    //        if (button == null)
    //        {
    //            Debug.WriteLine("Button is null");
    //            return;
    //        }

    //        var reservation = button.BindingContext as ReservationListViewItems;
    //        if (reservation == null)
    //        {
    //            Debug.WriteLine("Reservation is null");
    //            return;
    //        }


    //        var isAccepted = await DisplayAlert("Delete", "Are you sure you want to delete this reservation?", "Yes", "No");

    //        if (!isAccepted)
    //        {
    //            Debug.WriteLine("Deletion cancelled");
    //            return;
    //        }

    //        DeleteReservation(reservation.reservationId);
    //        await RefreshListView();
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine($"An error occurred in RemoveReservationButtonClicked: {ex.Message}");
    //    }
    //}







    private async void AddBookingButtonClicked(object sender, EventArgs e)
    {

        await Navigation.PushAsync(new Booking());
    }



    private async Task RefreshListView()
    {
        try
        {
            GetAllBookingData();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An error occurred in RefreshListView(): {ex.Message}");
        }
    }



}