using Mysqlx;
using Org.BouncyCastle.Tls;

namespace varausjarjestelma
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private void OnDatabaseTestButtonClicked(object sender, EventArgs e)
        {
            var helper = new Database.MySqlHelper();
            var isConnected = helper.TestConnection();

            MySqlTestLabel.Text = isConnected ? "Connected successfully" : "Connection failed"; 
        }
    }

}
