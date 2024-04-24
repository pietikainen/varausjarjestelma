using System.Diagnostics;
using varausjarjestelma.Controller;
namespace varausjarjestelma;

public partial class AddServiceModal : ContentPage
{
    AreaController areaController = new AreaController();
    List<AreaData>? areas;

    public AddServiceModal()
    {
        InitializeComponent();
        InitializeAreaPicker();
    }
    public AddServiceModal(ServiceData service)
    {
        InitializeComponent();
        serviceIdEntry.Text = service.ServiceId.ToString();

        serviceNameEntry.Text = service.Name;
        serviceTypeEntry.Text = service.Type.ToString();
        serviceDescriptionEntry.Text = service.Description;
        servicePriceEntry.Text = service.Price.ToString();
        serviceVatEntry.Text = service.Vat.ToString();
        AreaIdEntry.Text = service.AreaId.ToString();
        AreaIdLabelHidden.Text = service.AreaId.ToString();
        InitializeAreaPicker();
        AreaPicker.SelectedIndex = service.AreaId - 1;
        Debug.WriteLine("Areaidtexthidden: " + AreaIdLabelHidden.Text);

        areaLabel.IsVisible = true;
        AreaPicker.IsVisible = true;
        serviceIdEntry.IsVisible = true;
        AreaIdEntry.IsVisible = true;
        AreaIdLabel.IsVisible = true;
        serviceIdLabel.IsVisible = true;






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

                if (areas != null)
                {
                    foreach (AreaData area in areas)
                    {
                        if (area.Name == selectedArea)
                        {
                            selectedAreaId = area.AreaId;
                            AreaIdLabelHidden.Text = selectedAreaId.ToString();
                            break;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }


    }



    private async void SaveServiceButton_Clicked(object sender, EventArgs e)
    { 
        if (await ValidateFormData())
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

    private async void ServiceSubmitButtonClicked(object sender, EventArgs e)
    {
        var areaId = AreaIdLabelHidden.Text;
        var name = serviceNameEntry.Text;
        var type = serviceTypeEntry.Text;
        var price = servicePriceEntry.Text;
        var vat = serviceVatEntry.Text;
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
                    alue_id = int.Parse(areaId),
                    nimi = name,
                    tyyppi = int.Parse(type),
                    hinta = int.Parse(price),
                    alv = int.Parse(vat),
                    kuvaus = description
                };


                if (serviceIdEntry.IsVisible)
                {
                    service.palvelu_id = int.Parse(serviceIdEntry.Text);
                    try
                    {
                        await ServiceController.InsertAndModifyServiceAsync(service, "modify");
                    }
                    catch
                    {

                    Debug.WriteLine("Service not modded to database. Closing modal.");
                    }
                    finally
                    {
                        Debug.WriteLine("SErvice modded successfully. Closing modal.");
                    }
                }
                else
                {
                    await ServiceController.InsertAndModifyServiceAsync(service, "add");
                    Debug.WriteLine("Service inserted to database. Closing modal.");
                }
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