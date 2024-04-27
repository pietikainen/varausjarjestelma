namespace varausjarjestelma;

public partial class About : ContentPage
{

	private readonly List<string> _authors = new() { "Simo Pietikäinen", "Niko Järvelä" };
	private readonly string _version = "1.0.0";
	private List<string> _version100Description = new()
	{
		"Support for management of...",
		"- Customers",
		"- Booking",
		"- Cabins",
		"- Areas",
		"- Services",
		"- Invoicing",
		"Page for viewing statistics"
	};

	public About()
	{
		InitializeComponent();
		CreateVersion();
		CreateVersionDescription();
	}

	private void CreateVersion()
	{
        // create new label for version
        var version = new Label
		{
            Text = _version,
            FontSize = 15,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromRgba(0,0,0,255),
			Margin = new Thickness(5,0,0,0),
            HorizontalOptions = LayoutOptions.Start
        };

        // add version to stacklayout
        versionStackLayout.Children.Add(version);
    }

	// create new listview for version description
	private void CreateVersionDescription()
	{
        var versionDescription = new ListView
		{
            ItemsSource = _version100Description,
            BackgroundColor = Color.FromRgba(250,250,250,255),
            HasUnevenRows = true,
            IsEnabled = false,
            IsVisible = true,
            HeightRequest = 200
        };

        // add version description to stacklayout
        descriptionStackLayout.Children.Add(versionDescription);
    }


}