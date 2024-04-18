namespace varausjarjestelma
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }


        // Navigation for menubar items

        private async void RequestMainMenuPage(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        private async void RequestCustomerPage(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Customer");
        }

        private async void RequestAreaPage(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//AreaManagement");
        }

        private async void RequestBookingPage(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Booking");
        }

        private async void RequestManagementPage(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Management");
        }
        private async void RequestInvoicePage(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Invoice");
        }

        private async void RequestAboutPage(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//About");
        }

        private void RequestQuit(object sender, EventArgs e)
        {
            Application.Current.Quit();
        }
        private async void RequestAreaPageNew(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Area");
        }

        private async void RequestCabinPage(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Cabin");
        }

        private async void RequestServicePage(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Services");
        }
    }
}



//    < ShellContent ContentTemplate = "{DataTemplate local:MainPage}" Route = "MainPage" />
//    < ShellContent ContentTemplate = "{DataTemplate local:Customer}" Route = "Customer" />
//    < ShellContent ContentTemplate = "{DataTemplate local:Cabin}" Route = "Cabin" />
//    < ShellContent ContentTemplate = "{DataTemplate local:Booking}" Route = "Booking" />
//    < ShellContent ContentTemplate = "{DataTemplate local:Service}" Route = "Service" />
//    < ShellContent ContentTemplate = "{DataTemplate local:Invoice}" Route = "Invoice" />
//    < ShellContent ContentTemplate = "{DataTemplate local:About}" Route = "About" />

//</ Shell >