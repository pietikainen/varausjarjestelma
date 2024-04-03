namespace varausjarjestelma;

public partial class Management : ContentPage
{
	public Management()
	{
		InitializeComponent();
	}

    private void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
		var picker = sender as Picker;
		var selectedItem = picker.SelectedItem.ToString();

		addAreaButton.IsVisible = false;
		addCabinButton.IsVisible = false;
		addServiceButton.IsVisible = false;

		switch (selectedItem)
		{
            case "Cabins":
                addCabinButton.IsVisible = true;
                break;
            case "Services":
                addServiceButton.IsVisible = true;
                break;
            case "Areas":
                addAreaButton.IsVisible = true;
                break;
        }
    }

    private void addCabinButton_Clicked(object sender, EventArgs e)
    {

    }

    private void addServiceButton_Clicked(object sender, EventArgs e)
    {

    }

    private void addAreaButton_Clicked(object sender, EventArgs e)
    {

    }
}