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

        var confirmationMessage = $"Nimi: {firstName} {lastName}\nOsoite: {address}\nPostinumero: {postalCode}\nPuhelin: {phone}\nSähköposti: {email}\nHaluatko hyväksyä?";
        var isAccepted = await DisplayAlert("Vahvista tiedot", confirmationMessage, "Kyllä", "Ei");
        Debug.WriteLine("customer: " + firstName + " " + lastName + " " + address + " " + postalCode + " " + phone + " " + email + " " + city);
        Debug.WriteLine("Is accepted: " + isAccepted);
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
                Debug.WriteLine("cityData: " + cityData.postinro + " " + cityData.toimipaikka);

                Debug.WriteLine("Starting controller calls");
                    await PostalCodeController.InsertPostalCodeAsync(cityData);
                    await CustomerController.InsertCustomerAsync(customer);             
                Debug.WriteLine("Controller calls done");

                await RefreshListView();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error inserting customer to database:");
            Debug.WriteLine(ex.Message);
        }
    }

    private async Task RefreshListView()
    {
        CustomerListView.ItemsSource = await CustomerController.GetAllCustomerDataAsync();
    }
}