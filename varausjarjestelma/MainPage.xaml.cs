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

        private async void OnDatabaseTestButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var helper = new Database.MySqlController();
                var isConnected = await helper.TestConnectionAsync();
                MySqlTestLabel.Text = isConnected ? "Connected successfully" : "Connection failed";
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

        private async void MySqlReadBtn_Clicked(object sender, EventArgs e)
        {
            var customers = await Database.MySqlController.GetCustomersAsync();
            List<Customer> customerDataList = new List<Customer>();

            if (customers != null)
            {
                foreach (var customer in customers)
                {
                    customerDataList.Add(new Customer
                    {
                        asiakas_id = customer.asiakas_id,
                        postinro = customer.postinro,
                        etunimi = customer.etunimi,
                        sukunimi = customer.sukunimi,
                        lahiosoite = customer.lahiosoite,
                        email = customer.email,
                        puhelinnro = customer.puhelinnro
                    });
                }
                MySqlListView.ItemsSource = customerDataList;
            }
        }


    }
}
