namespace varausjarjestelma;
using varausjarjestelma.Controller;

using System.Diagnostics;

public partial class AddCustomerModal : ContentPage
{
    public AddCustomerModal()
    {
        InitializeComponent();
    }

    public AddCustomerModal(CustomerData customer)
    {
        InitializeComponent();
        customerIdEntry.Text = customer.CustomerId.ToString();
        firstNameEntry.Text = customer.FirstName;
        lastNameEntry.Text = customer.LastName;
        addressEntry.Text = customer.Address;
        postalCodeEntry.Text = customer.PostalCode;
        cityEntry.Text = customer.City;
        phoneNumberEntry.Text = customer.Phone;
        emailEntry.Text = customer.Email;

        customerIdEntry.IsVisible = true;
        customerIdLabel.IsVisible = true;
    }

   private async void CustomerFormSubmitButton_Clicked(object sender, EventArgs e)
    {
        Debug.WriteLine("CustomerFormSubmitButton_Clicked");
        var formDataisValid = await ValidateFormData();
        if (formDataisValid)
        {
            if (customerIdEntry.IsVisible)
            {
                CustomerModifyButtonClicked(sender, e);
            }
            else
            {
                CustomerSubmitButtonClicked(sender, e);
            }
        }
        else
        {
            Debug.WriteLine("Form data is not valid.");
        }
    }

    private async void CustomerSubmitButtonClicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Inside CustomerSubmitButtonClicked");
        var firstName = firstNameEntry.Text;
        var lastName = lastNameEntry.Text;
        var address = addressEntry.Text;
        var postalCode = postalCodeEntry.Text;
        var phone = phoneNumberEntry.Text;
        var email = emailEntry.Text;
        var city = cityEntry.Text;

        var confirmationMessage = $"Name: {firstName} {lastName}\nAddress: {address}\nPostal code: {postalCode}\nCity: {city}\nPhone: {phone}\nEmail: {email}\n";
        var isAccepted = await DisplayAlert("Confirm customer information", confirmationMessage, "Yes", "No");

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
                Debug.WriteLine("Customer inserted to database. Closing modal.");
                await Navigation.PopModalAsync();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    private async void CustomerModifyButtonClicked(object sender, EventArgs e)
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
            $"Postal code: {postalCode}\nCity: {city}\nPhone: {phone}\nEmail: {email}\n";
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
            await Navigation.PopModalAsync();
        }
    }
    private async Task<bool> ValidateFormData()
    {
        Debug.WriteLine("Validating form data");
        List<string> errorString = new List<string>();
        bool isValid = true;

        foreach (var entry in this.FindByName<VerticalStackLayout>("CustomerDetails").Children)
        {
            Debug.WriteLine("inside entry foreach");
            if (entry is Entry)
            {
                var entryValue = (Entry)entry;
                Debug.WriteLine("entryValue: " + entryValue.Text);
                if (entryValue.Text == null || entryValue.Text.Length == 0)
                {
                    entryValue.Placeholder = "Field is required.";
                    return false;
                }
            }
            
        }

        // Tried to do this with foreach loop but it didn't work if user
        // previously modified customer data and then tried to add new customer
        // Because of isvisible property of customerIdEntry
        // Sorry.

        // Try/catch for handling for null errors
        try
        {
            Debug.WriteLine("Catching null errors!");
            if (firstNameEntry.Text == null || firstNameEntry.Text.Length == 0 || firstNameEntry.Text.Length > 35)
            {
                errorString.Add("First name cannot be empty.");
            }

            if (lastNameEntry.Text == null || lastNameEntry.Text.Length == 0 ||lastNameEntry.Text.Length > 35)
            {
                errorString.Add("Last name cannot be empty.");
            }

            if (addressEntry.Text == null || addressEntry.Text.Length == 0 || addressEntry.Text.Length > 35)
            {
                errorString.Add("Address cannot be empty.");
            }

            if (!postalCodeEntry.Text.All(char.IsDigit) || postalCodeEntry.Text.Length != 5)
            {
                errorString.Add("Postal code must be numeric and 5 characters long.");
            }

            if (!phoneNumberEntry.Text.All(char.IsDigit) || phoneNumberEntry.Text.Length > 15)
            {
                errorString.Add("Phone number must be in numeric form.");
            }

            if (!IsEmailValid(emailEntry.Text) || emailEntry.Text > 50)
            {
                errorString.Add("Email is not valid.");
            }

            if (errorString.Count > 0)
            {
                isValid = false;
                await DisplayAlert("Error", string.Join("\n", errorString), "OK");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
        Debug.WriteLine("isvalid?: " + isValid);
        return isValid;
    }

    private bool IsEmailValid(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
        }
        catch
        {
            return false;
        }
        return true;
    }

    private async void PostalCodeEntryUnfocused(object sender, FocusEventArgs e)
    {
        var postalCode = postalCodeEntry.Text;
        try
        {

            if (postalCode != null && postalCode.Length == 5)
            {
                var databaseCity = await PostalCodeController.GetCityNameAsync(postalCode);

                if (databaseCity == null)
                {
                    var city = await PostalCodeController.FetchPostalCodeFromApi(postalCode);
                    if (city != null)
                    {
                        cityEntry.IsReadOnly = true;
                        cityEntry.BackgroundColor = Color.FromArgb("#f7f7f7");
                        cityEntry.Text = city.ToUpper();

                    }
                    else
                    {
                        cityEntry.IsReadOnly = false;
                        cityEntry.BackgroundColor = Color.FromArgb("#ffffff");
                        cityEntry.Text = "";
                    }
                }
                else
                {
                    cityEntry.IsReadOnly = true;
                    cityEntry.BackgroundColor = Color.FromArgb("#f7f7f7");
                    cityEntry.Text = databaseCity.ToUpper();
                }
            }
            else
            {
                cityEntry.IsReadOnly = false;
                cityEntry.BackgroundColor = Color.FromArgb("#ffffff");
                cityEntry.Text = "";
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    private void ResetCustomerForm()
    {

        var entries = new List<Entry> {
            firstNameEntry,
                lastNameEntry,
                addressEntry,
                postalCodeEntry,
                phoneNumberEntry,
                emailEntry
        };

        foreach (var entry in entries)
        {
            entry.Text = "";
        }

        customerIdEntry.IsVisible = false;
        customerIdLabel.IsVisible = false;
    }

    private async void CloseModalButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}