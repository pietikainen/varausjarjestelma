namespace varausjarjestelma;

public partial class Management : ContentPage
{
	public Management()
	{
		InitializeComponent();
	}

    // private string <valittu arvo dropista>;


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

    // search:
    // ottaa vastaan parametrin "drop_value" ja "haku"




    // list area
        // etsint‰ parametrit on: "area" ja "haku"
        //suodattaa listalta ja n‰ytt‰‰ vain ne jotka vastaavat hakua


    // list cabin

    // list service



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