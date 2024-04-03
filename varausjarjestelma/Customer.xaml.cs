using varausjarjestelma.Controller;
using System.Diagnostics;


namespace varausjarjestelma;

public partial class Customer : ContentPage
{

    public Customer()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ListViewPlaceholder();
        GetAllCustomersData();
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

    private async void CustomerListViewItemTapped(object sender, ItemTappedEventArgs e)
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
        if (!CustomerListView.IsVisible)
        {
            listViewSpinner.IsVisible = true;
        }
        else
        {
            listViewSpinner.IsVisible = false;
        }
    }


    // Modal for adding customer

    private async void AddCustomerButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new AddCustomerModal());
    }



    private async Task RefreshListView()
    {
        CustomerListView.ItemsSource = await CustomerController.GetAllCustomerDataAsync();
    }



}