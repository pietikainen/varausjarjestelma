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

        private async void OnDatabaseTestButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var helper = new Database.MySqlHelper();
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

        private async void OnDatabaseReadButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var helper = new Database.MySqlHelper();
                var alueDataList = await helper.GetAllAlueDataAsync();
                MySqlTestLabel.Text = $"Found {alueDataList.Count} rows";
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


        async void NavigateToAreaManagementButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AreaManagement());
        }
    }
}
