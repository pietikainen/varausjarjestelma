using Mysqlx;
using Org.BouncyCastle.Tls;
using System.Diagnostics;

namespace varausjarjestelma
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

        }

       private async void NavigateToAreaManagementButtonClicked(object sender, EventArgs e)
        {
              await Navigation.PushAsync(new AreaManagement());
        }
        private async void NavigateToBookingButtonClicked(object sender, EventArgs e)
        {
             await Navigation.PushAsync(new Booking());
        }
        private async void NavigateToInvoiceButtonClicked(object sender, EventArgs e)
        {
            await  Navigation.PushAsync(new Invoice());
        }
    }
}
