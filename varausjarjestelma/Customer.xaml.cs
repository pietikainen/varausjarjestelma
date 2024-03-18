using varausjarjestelma.Controller;
using System.Diagnostics;
using Org.BouncyCastle.Crypto.Signers;
using Microsoft.Maui.Graphics.Text;
using Google.Protobuf.Reflection;

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
    private async void CustomerSubmitButtonClicked(object sender, EventArgs e)
    {
        var firstName = firstNameEntry.Text;
        var lastName = lastNameEntry.Text;
        var address = addressEntry.Text;
        var postalCode = postalCodeEntry.Text;
        var phone = phoneNumberEntry.Text;
        var email = emailEntry.Text;
        var city = cityEntry.Text;

        var confirmationMessage = $"Name: {firstName} {lastName}\nAddress: {address}\nPostal code: {postalCode}\nPhone: {phone}\nEmail: {email}\n";
        var isAccepted = await DisplayAlert("Vahvista tiedot", confirmationMessage, "Kyllä", "Ei");

        try
        {
            Debug.WriteLine("Inside try: Trying to insert customer to database");
            if (isAccepted)
            {
                var customer = new varausjarjestelma.Database.Customer
                {
                    etunimi = firstName,
                    sukunimi = lastName,
                    lahiosoite = address,
                    postinro = postalCode,
                    puhelinnro = phone,
                    email = email
                };
                Debug.WriteLine("customer: " + customer.etunimi + " " + customer.sukunimi + " " + customer.lahiosoite + " " + customer.postinro + " " + customer.puhelinnro + " " + customer.email);

                var cityData = new varausjarjestelma.Database.PostalCode
                {
                    postinro = postalCode,
                    toimipaikka = city
                };
                await PostalCodeController.InsertPostalCodeAsync(cityData);
                await CustomerController.InsertAndModifyCustomerAsync(customer, "add");
                ResetCustomerForm();
                await RefreshListView();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    private async void CustomerModifyButton_Clicked(object sender, EventArgs e)
    {

        var customerId = customerIdEntry.Text;
        var firstName = firstNameEntry.Text;
        var lastName = lastNameEntry.Text;
        var address = addressEntry.Text;
        var postalCode = postalCodeEntry.Text;
        var phone = phoneNumberEntry.Text;
        var email = emailEntry.Text;
        var city = cityEntry.Text;

        var confirmationMessage = $"Customer Id: {customerId}\nName: {firstName} {lastName}\nAddress: {address}\n" +
            $"Postal code: {postalCode}\nPhone: {phone}\nEmail: {email}\n";
        var isAccepted = await DisplayAlert("Confirm modification", confirmationMessage, "Yes", "No");

        if (isAccepted)
        {
            var customer = new varausjarjestelma.Database.Customer
            {
                asiakas_id = int.Parse(customerIdEntry.Text),
                etunimi = firstNameEntry.Text,
                sukunimi = lastNameEntry.Text,
                lahiosoite = addressEntry.Text,
                postinro = postalCodeEntry.Text,
                puhelinnro = phoneNumberEntry.Text,
                email = emailEntry.Text
            };

            var cityData = new varausjarjestelma.Database.PostalCode
            {
                postinro = postalCodeEntry.Text,
                toimipaikka = cityEntry.Text
            };           

            await PostalCodeController.InsertPostalCodeAsync(cityData);
            await CustomerController.InsertAndModifyCustomerAsync(customer, "modify");
            ResetCustomerForm();
            await RefreshListView();
        }
    }


    private async void CustomerListViewItemTapped(object sender, ItemTappedEventArgs e)
    {
        string action = await DisplayActionSheet("Choose action for customer: ", "Cancel", null, "Modify", "Delete");
        var customer = e.Item as varausjarjestelma.Controller.CustomerData;

        if (action == "Modify" && customer != null)
        {          
            customerIdEntry.Text = customer.CustomerId.ToString();
            firstNameEntry.Text = customer.FirstName;
            lastNameEntry.Text = customer.LastName;
            addressEntry.Text = customer.Address;
            postalCodeEntry.Text = customer.PostalCode;
            cityEntry.Text = customer.City;
            phoneNumberEntry.Text = customer.Phone;
            emailEntry.Text = customer.Email;

            customerIdLabel.IsVisible = true;
            customerIdEntry.IsVisible = true;
            CustomerSubmitButton.IsVisible = false;
            CustomerModifyButton.IsVisible = true;
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

    private void ResetCustomerForm()
    {
        customerIdEntry.Text = "";
        firstNameEntry.Text = "";
        lastNameEntry.Text = "";
        addressEntry.Text = "";
        postalCodeEntry.Text = "";
        cityEntry.Text = "";
        phoneNumberEntry.Text = "";
        emailEntry.Text = "";

        CustomerSubmitButton.IsVisible = true;
        CustomerModifyButton.IsVisible = false;

    }


    private async Task RefreshListView()
    {
        CustomerListView.ItemsSource = await CustomerController.GetAllCustomerDataAsync();
    }


}