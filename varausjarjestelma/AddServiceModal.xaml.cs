using System.Diagnostics;
using varausjarjestelma.Controller;
namespace varausjarjestelma;

public partial class AddServiceModal : ContentPage
{
    public AddServiceModal()
    {
        InitializeComponent();
    }
    public AddServiceModal(ServiceData service)
    {
        InitializeComponent();
        serviceIdEntry.Text = service.ServiceId.ToString();
        serviceIdEntry.Text = service.AreaId.ToString();
        serviceNameEntry.Text = service.Name;
        serviceTypeEntry.Text = service.Type.ToString();
        serviceDescriptionEntry.Text = service.Description;
        servicePriceEntry.Text = service.Price.ToString();
        serviceIdEntry.IsVisible = true;
        AreaIdEntry.IsVisible = true;
        AreaIdLabel.IsVisible = true;
        serviceIdLabel.IsVisible = true;
    }
    private async void SaveServiceButton_Clicked(object sender, EventArgs e)
    {
        Debug.WriteLine("ServiceSubmitButton_Clicked");
        var formDataisValid = await ValidateFormData();
        if (formDataisValid)
        {
            ServiceSubmitButtonClicked(sender, e);
        }
        else
        {
            Debug.WriteLine("Form data is not valid.");
        }
    }
    private async Task<bool> ValidateFormData()
    {
        Debug.WriteLine("Validating form data");
        List<string> errorString = new List<string>();
        bool isValid = true;

        try
        {
            Debug.WriteLine("Catching null errors!");
            if (serviceNameEntry.Text == null || serviceNameEntry.Text.Length == 0)
            {
                errorString.Add("Name cannot be empty.");
            }

            if (serviceTypeEntry.Text == null || serviceTypeEntry.Text.Length == 0)
            {
                errorString.Add("Service type cannot be empty.");
            }

            if (servicePriceEntry.Text == null || servicePriceEntry.Text.Length == 0)
            {
                errorString.Add("Price cannot be empty.");
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
    private async void ServiceModifyButtonClicked(object sender, EventArgs e)
    {

        var serviceId = serviceIdEntry.Text;
        var name = serviceNameEntry.Text;
        var type = serviceTypeEntry.Text;
        var price = servicePriceEntry.Text;
        var description = serviceDescriptionEntry.Text;

        var confirmationMessage = $"Service Id: {serviceId}\nName: {name}\ntype: {type}\n" +
            $"Price: {price}\ndescription: {description}";
        var isAccepted = await DisplayAlert("Confirm modification", confirmationMessage, "Yes", "No");

        if (isAccepted)
        {
            var service = new varausjarjestelma.Database.Service
            {
                palvelu_id = int.Parse(serviceIdEntry.Text),
                nimi = serviceNameEntry.Text,
                tyyppi = int.Parse(servicePriceEntry.Text),
                kuvaus = serviceDescriptionEntry.Text,
            };

    
            await ServiceController.InsertAndModifyServiceAsync(service, "modify");
            ResetServiceForm();
            await Navigation.PopModalAsync();
        }
    }
    private async void ServiceSubmitButtonClicked(object sender, EventArgs e)
    {
        
        var name = serviceNameEntry.Text;
        var type = serviceTypeEntry.Text;
        var price = servicePriceEntry.Text;
        var description = serviceDescriptionEntry.Text;
        var confirmationMessage = $"Name: {name}\nType: {type}\nPrice: {price}\nDescription: {description}";
        var isAccepted = await DisplayAlert("Confirm service information", confirmationMessage, "Yes", "No");

        try
        {
            Debug.WriteLine("Inside try: Trying to insert service to database");
            if (isAccepted)
            {
                var service = new varausjarjestelma.Database.Service
                {
                    
                    nimi = name,
                    tyyppi = int.Parse(type),
                    hinta = int.Parse(price),
                    kuvaus = description
                };
                //var serviceData = new varausjarjestelma.Database.PostalCode
                //{
                //    postinro = postalCode,
                //    toimipaikka = city
                //};
                //await PostalCodeController.InsertPostalCodeAsync(cityData);
                await ServiceController.InsertAndModifyServiceAsync(service, "add");
                Debug.WriteLine("Service inserted to database. Closing modal.");
                ResetServiceForm(); 
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            await DisplayAlert("Error", "Failed to save the service: " + ex.Message, "OK");
        }
        await Navigation.PopModalAsync();
    }
    private void ResetServiceForm()
    {
        serviceNameEntry.Text = "";
        serviceTypeEntry.Text = "";
        servicePriceEntry.Text = "";
        serviceDescriptionEntry.Text = "";
    }

    private async void CancelServiceButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}