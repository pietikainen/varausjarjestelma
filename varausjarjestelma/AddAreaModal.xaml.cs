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
        var confirmationMessage = $"Name: {areaName}";
        try
        {
            if(areaName == null || areaNameEntry.Text.Length == 0)
            {
                throw new ArgumentNullException("areaNameEntry.Text", "Area name cannot be null or empty.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
        await DisplayAlert("Confirm area information", confirmationMessage, "Yes", "No");

        try 
        {
            var area = new varausjarjestelma.Database.Area
            {
               nimi = areaName
            };

            if (areaIdEntry.IsVisible)
            {
                area.alue_id = int.Parse(areaIdEntry.Text);
                await AreaController.InsertAndModifyAreaAsync(area, "modify");
            }
            else
            await AreaController.InsertAndModifyAreaAsync(area, "add");
        }
        catch (Exception ex) 
        {
            await DisplayAlert("Error", "Failed to save the service: " + ex.Message, "OK");
        }
        ResetAreaForm();
        await Navigation.PopModalAsync();
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