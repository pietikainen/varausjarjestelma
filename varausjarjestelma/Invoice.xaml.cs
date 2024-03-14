using Microsoft.Maui.Controls.Platform;
using System.Diagnostics;
using varausjarjestelma.Controller;

namespace varausjarjestelma;

public partial class Invoice : ContentPage
{
    public Invoice()
	{
        // Asetetaan sivun BindingContext InvoiceViewModel-olioon.
        // Tämä mahdollistaa XAML-tiedostossa määriteltyjen elementtien datan sitomisen
        // InvoiceViewModelin ominaisuuksiin käyttämällä {Binding}-syntaksia.

        InitializeComponent();
        BindingContext = new InvoiceViewModel();

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        GetInvoicesPreviewData();
    }

    private async void GetInvoicesPreviewData()
    // Ladataan kaikki laskut tietokannasta ja näytetään ne InvoicesListView:ssä.
    {
        try
        {
            var helper = new InvoiceController();
            var invoices = await helper.GetAllInvoicesPreviewAsync();
            InvoicesListView.ItemsSource = invoices;

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

    private async void OnInvoiceTapped(object sender, ItemTappedEventArgs e)
    {
        try
        {
            if (e.Item == null || !(e.Item is InvoiceData selectedInvoice))
            {
                Debug.WriteLine("e.Item is null or not a Invoice object.");
                Debug.WriteLine("e.Item: " + e.Item);
                return;
            }
                

            Debug.WriteLine("Selected invoice: " + selectedInvoice);

            var invoiceId = selectedInvoice.InvoiceNumber;
            var popupPage = new InvoiceDetails(invoiceId);

            Console.WriteLine("Selected invoice: " + selectedInvoice);

            await Navigation.PushAsync(popupPage);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An error occurred: {ex.Message}");
        }
    }



    private void FilterInvoicesByNumber(object sender, EventArgs e)
    {


    }

    private void FilterInvoicesByName(object sender, EventArgs e)
    {

    }


    // Button clicked event handlers start here

    public void OnSearchButtonClicked(object sender, EventArgs e)
    {
        if (InvoiceNumberEntry.Text != null)
        {
            FilterInvoicesByNumber(sender, e);
        }
        else if (CustomerNameEntry.Text != null)
        {
            FilterInvoicesByName(sender, e);
        }
        else
        {
            InvoicesListView.ItemsSource = null;
            GetInvoicesPreviewData();
            // =?=
        }

    }

    async void MainMenuButtonClicked(object sender, EventArgs e)
    {
       await Navigation.PushAsync(new MainPage());
    }

    private void PrintInvoiceButtonClicked(object sender, EventArgs e)
    {
        // kirjoita tulostusmetodi tähän
    }

    private void SendEmailButtonClicked(object sender, EventArgs e)
    {
        // kirjoita sähköpostin lähetysmetodi tähän
    }
}