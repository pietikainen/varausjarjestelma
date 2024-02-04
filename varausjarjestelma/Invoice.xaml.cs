using System.Diagnostics;

namespace varausjarjestelma;

public partial class Invoice : ContentPage
{
	public Invoice()
	{
        // Asetetaan sivun BindingContext InvoiceViewModel-olioon.
        // T‰m‰ mahdollistaa XAML-tiedostossa m‰‰riteltyjen elementtien datan sitomisen
        // InvoiceViewModelin ominaisuuksiin k‰ytt‰m‰ll‰ {Binding}-syntaksia.

        InitializeComponent();
        BindingContext = new InvoiceViewModel();
    }

    public async void OnSearchButtonClicked(object sender, EventArgs e)
    {
        try
        {
            var helper = new Database.MySqlHelper();
        
        // Tarkasta hakuehdot
        // Jos tyhj‰, aja GetAllInvoicesAsync
        if (string.IsNullOrEmpty(CustomerNameEntry.Text))
        {
            var invoices = await helper.GetAllInvoicesAsync();
            InvoicesListView.ItemsSource = invoices;
            }
        }
        // TODO: Muuten hakukenttien arvot ja aja GetInvoicesBySearchAsync
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

    async void MainMenuButtonClicked(object sender, EventArgs e)
    {
       await Navigation.PushAsync(new MainPage());
    }

    private void PrintInvoiceButton_Clicked(object sender, EventArgs e)
    {

    }

    private void SendEmailButton_Clicked(object sender, EventArgs e)
    {

    }
}