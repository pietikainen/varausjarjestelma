using System.Diagnostics;
using varausjarjestelma.Controller;

namespace varausjarjestelma;

public partial class AddCabinModal : ContentPage
{

    AreaController areaController = new AreaController();
    List<AreaData>? areas;

    public AddCabinModal()
    {
        InitializeComponent();
        InitializeAreaPicker();
    }
    public AddCabinModal(CabinData cabin)
    {
        InitializeComponent();
        cabinIdEntry.Text = cabin.CabinId.ToString();
        cabinNameEntry.Text = cabin.CabinName;
        cabinAddressEntry.Text = cabin.Address;
        cabinFeaturesEntry.Text = cabin.Features;
        cabinDescriptionEntry.Text = cabin.Description;
        cabinBedsEntry.Text = cabin.Beds.ToString();
        cabinPostalCodeEntry.Text = cabin.PostalCode;
        cabinPriceEntry.Text = cabin.Price.ToString();
        AreaIdLabelHidden.Text = cabin.AreaId.ToString();
        
        AreaPicker.IsVisible = false;

        cabinIdEntry.IsVisible = true;
        cabinIdLabel.IsVisible = true;
        areaIdEntry.IsVisible = true;
        areaIdLabel.IsVisible = true;
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

    private async void CabinSaveButton_Clicked(object sender, EventArgs e)
    {
        Debug.WriteLine("CustomerModifyButtonClicked");

        var cabinAreaId = AreaIdLabelHidden.Text;
        Debug.WriteLine("AreaIDLabelHGidden: " + cabinAreaId);
        var cabinName = cabinNameEntry.Text;
        var cabinAddress = cabinAddressEntry.Text;
        var cabinCity = cabinCityEntry.Text;
        var cabinPostalCode = cabinPostalCodeEntry.Text;
        var cabinBed = cabinBedsEntry.Text;
        var cabinFeatures = cabinFeaturesEntry.Text;
        var cabinDescription = cabinDescriptionEntry.Text;
        var cabinPrice = cabinPriceEntry.Text;
        Debug.WriteLine("Populated vars in modal");


        var confirmationMessage = $"Cabin name: {cabinName} {cabinAddress}\nAddress: {cabinAddress}\n" +
            $"Postal code: {cabinPostalCode}\nCity: {cabinCity}\nBeds: {cabinBed}\nFeatures: {cabinFeatures}\n" +
           $"Description: {cabinDescription}\nPrice: {cabinPrice}";
        var isAccepted = await DisplayAlert("Confirm cabin information", confirmationMessage, "Yes", "No");

        try
        {
            Debug.WriteLine("Inside try: Trying to insert cabin to database");

            if (isAccepted)
            {

                Debug.WriteLine("Inside if isAccepted: Trying to insert cabin to database");
                var cabin = new varausjarjestelma.Database.Cabin
                {                  

                    alue_id = int.Parse(cabinAreaId),
                    mokkinimi = cabinName,
                    katuosoite = cabinAddress,
                    postinro = cabinPostalCode,
                    henkilomaara = int.Parse(cabinBed),
                    varustelu = cabinFeatures,
                    kuvaus = cabinDescription,
                    hinta = int.Parse(cabinPrice)

                };
                Debug.WriteLine("class instance created");

                if (cabinIdEntry.IsVisible)
                {
                    cabin.mokki_id = int.Parse(cabinIdEntry.Text);
                }
                Debug.WriteLine("After if id entry statement");

                Debug.WriteLine("await PostalCodeController.InsertPostalCodeAsync(cityData)");

                var cityData = new varausjarjestelma.Database.PostalCode
                {
                    postinro = cabinPostalCode,
                    toimipaikka = cabinCity
                };
                await PostalCodeController.InsertPostalCodeAsync(cityData);


                if (cabinIdEntry.IsVisible)
                {
                    cabin.mokki_id = int.Parse(cabinIdEntry.Text);
                    Debug.WriteLine("await CabinController.InsertAndModifyCabinAsync(cabin, modify)");

                    await CabinController.InsertAndModifyCabinAsync(cabin, "modify");
                }
                else
                {
                    await CabinController.InsertAndModifyCabinAsync(cabin, "add");
                }
                ResetCabinForm();
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
        Debug.WriteLine("CustomerModifyButtonClicked");
        var cabinAreaId = AreaIdLabelHidden.Text;
        var cabinId = cabinIdEntry.Text;
        var cabinName = cabinNameEntry.Text;
        var cabinBeds  = cabinBedsEntry.Text;
        var cabinAddress = cabinAddressEntry.Text;
        var cabinpostalCode = cabinPostalCodeEntry.Text;
        var cabinFeatures = cabinFeaturesEntry.Text;
        var cabinDescription = cabinDescriptionEntry.Text;
        var cabinCity = cabinCityEntry.Text;
        var cabinPrice = cabinPriceEntry.Text;
        Debug.WriteLine("Populated vars in modal");
        var confirmationMessage = $"Cabin Id: {cabinId}\nCabin name: {cabinName}\nAddress {cabinAddress}\nPostal Code: {cabinpostalCode}\n" +
            $"City: {cabinCity}\nBeds: {cabinBeds}\nFeatures: {cabinFeatures}\nDescription: {cabinDescription}\nPrice: {cabinPrice}";
        var isAccepted = await DisplayAlert("Confirm modification", confirmationMessage, "Yes", "No");

        if (isAccepted)
        {
            Debug.WriteLine("Inside try: Trying to insert cabin to database");
            var cabin = new varausjarjestelma.Database.Cabin
            {
                alue_id = int.Parse(cabinAreaId),
                mokki_id = int.Parse(cabinId),
                mokkinimi = cabinNameEntry.Text,
                henkilomaara = int.Parse(cabinBedsEntry.Text),
                katuosoite = cabinAddressEntry.Text,
                postinro = cabinPostalCodeEntry.Text,
                varustelu = cabinFeaturesEntry.Text,
                kuvaus = cabinDescriptionEntry.Text,
                hinta = int.Parse(cabinPriceEntry.Text),
                
            };
            Debug.WriteLine("await CabinController.InsertAndModifyCabinAsync(cabin, modify)");
            await CabinController.InsertAndModifyCabinAsync(cabin, "modify");
            ResetCabinForm();
            await Navigation.PopModalAsync();
        }
    }
    private async Task<bool> ValidateFormData()
    {
        Debug.WriteLine("Validating form data");
        List<string> errorString = new List<string>();
        bool isValid = true;

        foreach (var entry in this.FindByName<VerticalStackLayout>("AreaDetails").Children)
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
            if (cabinNameEntry.Text == null || cabinNameEntry.Text.Length == 0)
            {
                errorString.Add("cabin name cannot be empty.");
            }

            if (cabinAddressEntry.Text == null || cabinAddressEntry.Text.Length == 0)
            {
                errorString.Add(" Address cannot be empty.");
            }

            if (cabinDescriptionEntry.Text == null || cabinDescriptionEntry.Text.Length == 0)
            {
                errorString.Add("Description cannot be empty.");
            }

            if (!cabinPostalCodeEntry.Text.All(char.IsDigit) || cabinPostalCodeEntry.Text.Length != 5)
            {
                errorString.Add("Postal code must be numeric and 5 characters long.");
            }

            if (!cabinBedsEntry.Text.All(char.IsDigit))
            {
                errorString.Add("Beds must be in numeric form.");
            }

            if (!cabinPriceEntry.Text.All(char.IsDigit))
            {
                errorString.Add("Price must be in numeric form.");
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
    private async void cabinPostalCodeEntryUnfocused(object sender, FocusEventArgs e)
    {
        var postalCode = cabinPostalCodeEntry.Text;

        if (postalCode.Length == 5)
        {
            var city = await PostalCodeController.FetchPostalCodeFromApi(postalCode);
            if (city != null)
            {
                cabinCityEntry.IsReadOnly = true;
                cabinCityEntry.BackgroundColor = Color.FromArgb("#f7f7f7");
                cabinCityEntry.Text = city;

            }
            else
            {
                cabinCityEntry.IsReadOnly = false;
                cabinCityEntry.BackgroundColor = Color.FromArgb("#ffffff");
                cabinCityEntry.Text = "";
            }
        }
        else
        {
            cabinCityEntry.IsReadOnly = false;
            cabinCityEntry.BackgroundColor = Color.FromArgb("#ffffff");
            cabinCityEntry.Text = "";
        }
    }
    private void ResetCabinForm()
    {
        cabinNameEntry.Text = "";
        cabinAddressEntry.Text = "";
        cabinCityEntry.Text = "";
        cabinPostalCodeEntry.Text = "";
        cabinBedsEntry.Text = "";
        cabinFeaturesEntry.Text = "";

    }

    private async void CloseCabinButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private async void cabinCloseButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}