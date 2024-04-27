using System.Diagnostics;
using varausjarjestelma.Controller;

namespace varausjarjestelma;

public partial class AddAreaModal : ContentPage
{
	public AddAreaModal()
	{
		InitializeComponent();
	}
    public AddAreaModal(AreaData area)
    {
        InitializeComponent();
        areaId.IsVisible = true;

        areaIdEntry.Text = area.AreaId.ToString();
        areaIdEntry.IsVisible = true;

        areaNameLabel.IsVisible = true;
        areaNameEntry.Text = area.Name;

    }
    private async void addAreaButton_Clicked(object sender, EventArgs e)
    {
        var areaName = areaNameEntry.Text;
        if (string.IsNullOrEmpty(areaName) || areaName.Length > 50) // Päivitetty ehto
        {
            await DisplayAlert("Error", "Area name cannot be null, empty or longer than 50 characters.", "Close");
            return;
        }

        var confirmationResult = await DisplayAlert("Confirm area information", $"Name: {areaName}", "Yes", "No");
        if (!confirmationResult) // Jos käyttäjä valitsee "No", ei jatketa eteenpäin.
        {
            return;
        }

        try
        {
            var area = new varausjarjestelma.Database.Area
            {
                nimi = areaName
            };

            if (!string.IsNullOrEmpty(areaIdEntry.Text))
            {
                area.alue_id = int.Parse(areaIdEntry.Text); // Tarkista myös, että tämä muunnos onnistuu oikein
                await AreaController.InsertAndModifyAreaAsync(area, "modify");
            }
            else
            {
                await AreaController.InsertAndModifyAreaAsync(area, "add");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Failed to save the service: " + ex.Message, "OK");
        }
        finally
        {
            ResetAreaForm();
            await Navigation.PopModalAsync();
        }
    }

    private async void cancelAreaButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private void ResetAreaForm()
    {
        areaNameEntry.Text = "";
        areaIdEntry.Text = "";
    }
}