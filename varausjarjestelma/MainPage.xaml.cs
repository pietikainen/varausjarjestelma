using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using System.ComponentModel.DataAnnotations;
using MySql.EntityFrameworkCore;
using System.Diagnostics;
using varausjarjestelma.Database;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using Microsoft.Maui.Controls;


namespace varausjarjestelma
{
    public partial class MainPage : ContentPage
    {
        // Viittaus AppShell-luokan instanssiin

        public MainPage()
        {
            InitializeComponent();
        }

        private async void signInButton_Clicked(object sender, EventArgs e)
        {  
            string username = usernameEntry.Text;
            string password = passwordEntry.Text;
            if (username == "testikayttaja" && password == "salasana")
            {
                MessagingCenter.Send<MainPage>(this, "EnableMenu");
                await DisplayAlert("login successful", "welcome to mökkimaster!", "ok");
                usernameEntry.Text = "";
                passwordEntry.Text = "";
            }
            else
            {
                await DisplayAlert("login failed", "incorrect username or password.", "ok");
            }
        }
    }
}
