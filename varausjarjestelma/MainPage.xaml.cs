using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using System.ComponentModel.DataAnnotations;
using MySql.EntityFrameworkCore;
using System.Diagnostics;
using varausjarjestelma.Database;
using Microsoft.Extensions.Options;
using System.ComponentModel;


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

        private async void NavigateToCustomerButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Customer());
        }

        private async void NavigateToReportingButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Reporting());
        }

    }
}
