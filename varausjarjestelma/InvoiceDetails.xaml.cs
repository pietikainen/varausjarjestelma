using System.Diagnostics;
using varausjarjestelma.Controller;
namespace varausjarjestelma;

public partial class InvoiceDetails : ContentPage
{
    private int invoiceNumber;

    public InvoiceDetails(int invoiceNumber)
    {
        InitializeComponent();
        this.invoiceNumber = invoiceNumber;

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadInvoiceDetails();
    }


    private async void LoadInvoiceDetails()
    // Ladataan lasku tietokannasta ja n‰ytet‰‰n..
    {
        try
        {
            var controller = new InvoiceController();
            var invoice = await controller.GetInvoiceByNumber(invoiceNumber); // KORJATAAN

            if (invoice != null)
            {
                BindingContext = invoice;
            }
            else
            {
                Debug.WriteLine("Invoice not found");
            }

        }
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
    private async void TestPrintOfDataToConsole(object sender, EventArgs e)
    {

    }

    private async void MainMenuButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}