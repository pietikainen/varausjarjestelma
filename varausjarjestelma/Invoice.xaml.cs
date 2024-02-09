using Microsoft.Maui.Controls.Platform;
using System.Diagnostics;

namespace varausjarjestelma;

public partial class Invoice : ContentPage
{
	public Invoice()
	{
        // Asetetaan sivun BindingContext InvoiceViewModel-olioon.
        // T�m� mahdollistaa XAML-tiedostossa m��riteltyjen elementtien datan sitomisen
        // InvoiceViewModelin ominaisuuksiin k�ytt�m�ll� {Binding}-syntaksia.

        InitializeComponent();
        BindingContext = new InvoiceViewModel();
        GetInvoicesPreviewData();

    }

    private async void GetInvoicesPreviewData()
    // Ladataan kaikki laskut tietokannasta ja n�ytet��n ne InvoicesListView:ss�.
    {
        try
        {
            var helper = new Database.MySqlController();
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

    private void PrintInvoiceButton_Clicked(object sender, EventArgs e)
    {
        // kirjoita tulostusmetodi t�h�n
    }

    private void SendEmailButton_Clicked(object sender, EventArgs e)
    {
        // kirjoita s�hk�postin l�hetysmetodi t�h�n
    }
}