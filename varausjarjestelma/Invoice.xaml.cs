using Microsoft.Maui.Controls.Platform;
using System.Diagnostics;
using System.Drawing;
using varausjarjestelma.Controller;

namespace varausjarjestelma;

public partial class Invoice : ContentPage
{
    public List<InvoiceData> invoices;

    public Invoice()
    {
        InitializeComponent();
        GetInvoicesPreviewData();
    }

    private async void GetInvoicesPreviewData()
    // Ladataan kaikki laskut tietokannasta ja näytetään ne InvoicesListView:ssä.
    {
        try
        {
            ActivityIndicator.IsRunning = true;
            ActivityIndicator.IsVisible = true;

            InvoicesListView.ItemsSource = null;
            invoices = await InvoiceController.GetAllInvoicesPreviewAsync();

            foreach (InvoiceData invoice in invoices)
            {
                if (invoice.IsPaid == 1)
                {
                    invoice.IsPaidString = "Paid";
                }
                else
                {
                    invoice.IsPaidString = "";
                }
            }
            InvoicesListView.ItemsSource = invoices;

            ActivityIndicator.IsRunning = false;
            ActivityIndicator.IsVisible = false;
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

            // Populate invoice details page with selected invoice data
            BindingContext = await InvoiceController.GetFullInvoiceAsync(selectedInvoice.InvoiceNumber);

            // Bind the grid
            var InvoiceGrid_ContentPart = this.FindByName<Grid>("InvoiceGrid_ContentPart");

            // Clear the InvoiceGrid_ContentPart
            InvoiceGrid_ContentPart.Children.Clear();
            InvoiceGrid_ContentPart.RowDefinitions.Clear();
            InvoiceGrid_ContentPart.ColumnDefinitions.Clear();

            // Generate new column definitions x4

            for (int i = 0; i < 5; i++)
            {
                var columnDefinition = new ColumnDefinition();
                columnDefinition.Width = new GridLength(128);
                InvoiceGrid_ContentPart.ColumnDefinitions.Add(columnDefinition);
            }

            // Populate Invoice contents with selected invoice data
            var cabins = await InvoiceController.GetCabinsOnReservationAsync(selectedInvoice.InvoiceNumber);

            foreach (var cabin in cabins)
            {
                int row = 0;

                var cabinNameLabel = new Label { Text = "Mökki: " + cabin.CabinName, FontSize = 11 };
                var cabinAmountLabel = new Label { Text = cabin.CabinAmount.ToString() + " vrk", FontSize = 11 };
                var cabinPriceLabel = new Label { Text = cabin.CabinPrice.ToString("C"), FontSize = 11 };
                var cabinVatLabel = new Label { Text = (cabin.CabinPrice * 0.24).ToString("C"), FontSize = 11 };
                var cabinTotal = (double)cabin.CabinAmount * (double)cabin.CabinPrice;
                var cabinTotalLabel = new Label { Text = cabinTotal.ToString("C"), FontSize = 11 };

                // Generate new row to the grid
                var rowDefinition = new RowDefinition();
                rowDefinition.Height = GridLength.Auto;
                InvoiceGrid_ContentPart.RowDefinitions.Add(rowDefinition);

                // Add labels to grid and set their location
                InvoiceGrid_ContentPart.Children.Add(cabinNameLabel);
                Grid.SetRow(cabinNameLabel, InvoiceGrid_ContentPart.RowDefinitions.Count - 1);
                Grid.SetColumn(cabinNameLabel, 0);

                InvoiceGrid_ContentPart.Children.Add(cabinAmountLabel);
                Grid.SetRow(cabinAmountLabel, InvoiceGrid_ContentPart.RowDefinitions.Count - 1);
                Grid.SetColumn(cabinAmountLabel, 1);

                InvoiceGrid_ContentPart.Children.Add(cabinPriceLabel);
                Grid.SetRow(cabinPriceLabel, InvoiceGrid_ContentPart.RowDefinitions.Count - 1);
                Grid.SetColumn(cabinPriceLabel, 2);

                InvoiceGrid_ContentPart.Children.Add(cabinVatLabel);
                Grid.SetRow(cabinVatLabel, InvoiceGrid_ContentPart.RowDefinitions.Count - 1);
                Grid.SetColumn(cabinVatLabel, 3);

                InvoiceGrid_ContentPart.Children.Add(cabinTotalLabel);
                Grid.SetRow(cabinTotalLabel, InvoiceGrid_ContentPart.RowDefinitions.Count - 1);
                Grid.SetColumn(cabinTotalLabel, 4);

                // Generate empty row with no columns and height of 20px
                var emptyRowDefinition = new RowDefinition();
                emptyRowDefinition.Height = new GridLength(20);
                InvoiceGrid_ContentPart.RowDefinitions.Add(emptyRowDefinition);
            }


            var services = await InvoiceController.GetServicesOnReservationAsync(selectedInvoice.InvoiceNumber);

            foreach (var service in services)
            {
                int row = InvoiceGrid_ContentPart.RowDefinitions.Count - 1; // Seuraava rivi

                var serviceNameLabel = new Label { Text = service.ServiceName, FontSize = 11 };
                var serviceAmountLabel = new Label { Text = service.ServiceAmount.ToString() + " kpl", FontSize = 11 };
                var servicePriceLabel = new Label { Text = service.ServicePrice.ToString("C"), FontSize = 11 };
                var serviceVatLabel = new Label { Text = (service.ServicePrice * 0.24).ToString("C"), FontSize = 11 };
                var serviceTotal = (double)service.ServiceAmount * (double)service.ServicePrice;
                var serviceTotalLabel = new Label { Text = serviceTotal.ToString("C"), FontSize = 11 };

                // Generate new row to the grid
                var rowDefinition = new RowDefinition();
                rowDefinition.Height = GridLength.Auto;
                InvoiceGrid_ContentPart.RowDefinitions.Add(rowDefinition);

                // Add labels to grid and set their location

                InvoiceGrid_ContentPart.Children.Add(serviceNameLabel);
                Grid.SetRow(serviceNameLabel, InvoiceGrid_ContentPart.RowDefinitions.Count - 1);
                Grid.SetColumn(serviceNameLabel, 0);

                InvoiceGrid_ContentPart.Children.Add(serviceAmountLabel);
                Grid.SetRow(serviceAmountLabel, InvoiceGrid_ContentPart.RowDefinitions.Count - 1);
                Grid.SetColumn(serviceAmountLabel, 1);

                InvoiceGrid_ContentPart.Children.Add(servicePriceLabel);
                Grid.SetRow(servicePriceLabel, InvoiceGrid_ContentPart.RowDefinitions.Count - 1);
                Grid.SetColumn(servicePriceLabel, 2);

                InvoiceGrid_ContentPart.Children.Add(serviceVatLabel);
                Grid.SetRow(serviceVatLabel, InvoiceGrid_ContentPart.RowDefinitions.Count - 1);
                Grid.SetColumn(serviceVatLabel, 3);

                InvoiceGrid_ContentPart.Children.Add(serviceTotalLabel);
                Grid.SetRow(serviceTotalLabel, InvoiceGrid_ContentPart.RowDefinitions.Count - 1);
                Grid.SetColumn(serviceTotalLabel, 4);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An error occurred: {ex.Message}");
        }
    }


    private async void SearchInvoiceByCustomerNameEntryChanged(object sender, TextChangedEventArgs e)
    {
        var keyword = SearchInvoiceByCustomerNameEntry.Text;

        var allInvoices = await InvoiceController.GetAllInvoicesPreviewAsync();

        if (string.IsNullOrEmpty(keyword))
        {
            InvoicesListView.ItemsSource = allInvoices;
        }
        else
        {
            var filteredInvoices = allInvoices.Where(i => i.CustomerName.Contains(keyword)).ToList();
            InvoicesListView.ItemsSource = filteredInvoices;
        }
    }

    private async void SetIsPaidButtonClicked(object sender, EventArgs e)
    {
        try
        {
            var button = sender as ImageButton;
            if (button == null)
            {
                Debug.WriteLine("Button is null");
                return;
            }
            var invoice = button.BindingContext as InvoiceData;
            if (invoice == null)
            {
                Debug.WriteLine("Invoice is null");
                return;
            }

            var isAccepted = await DisplayAlert("Confirm", "Are you sure you want to mark this invoice as paid?", "Yes", "No");

            if (!isAccepted)
            {
                Debug.WriteLine("Confirmation cancelled");
                return;
            }

            await InvoiceController.SetInvoicePaidAsync(invoice.InvoiceNumber);
           
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An error occurred: {ex.Message}");
        }
        finally
        {
            await RefreshListView();
        }
    }

    private async void RemoveInvoiceButtonClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        if (button == null)
        {
            Debug.WriteLine("Button is null");
            return;
        }
        var invoice = button.BindingContext as InvoiceData;
        if (invoice == null)
        {
            Debug.WriteLine("Invoice is null");
            return;
        }

        var isAccepted = await DisplayAlert("Confirm", "Are you sure you want to delete this invoice?", "Yes", "No");

        if (!isAccepted)
        {
            Debug.WriteLine("Deletion cancelled");
            return;
        }

        await InvoiceController.DeleteInvoiceAsync(invoice.InvoiceNumber);
        await RefreshListView();
    }

    public async Task RefreshListView()
    {
        Debug.WriteLine("Inside Invoice.RefreshListView()");
        GetInvoicesPreviewData();
        Debug.WriteLine("InvoicesListView.ItemsSource refreshed");
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