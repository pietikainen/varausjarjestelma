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
        GetAllCustomersData();
    }

    private async void GetAllCustomersData()
    {
        try
        {
            ActivityIndicator.IsRunning = true;
            ActivityIndicator.IsVisible = true;
            
            var customers = await CustomerController.GetAllCustomerDataAsync();
            CustomerListView.ItemsSource = customers;
            
            ActivityIndicator.IsRunning = false;
            ActivityIndicator.IsVisible = false;
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
    // Search bar to filter customers

    private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        var keyword = SearchCustomerEntry.Text;

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

    // OBS!! WORK IN PROGRESS


    // Sort customers by name

    //private async void SortButton_Clicked(object sender, EventArgs e)
    //{
    //    var allCustomers = await CustomerController.GetAllCustomerDataAsync();
    //    var sortedCustomers = allCustomers.OrderBy(customer => customer.FullName);
    //    CustomerListView.ItemsSource = sortedCustomers;
    //}



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