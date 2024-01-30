
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

    private void Button_Clicked(object sender, EventArgs e)
    {

    }

    async Task MainPageButton_Clicked(object sender, EventArgs e)
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