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
            //if (username == "testuser" && password == "salasana")
            if (true)
            {
                MessagingCenter.Send<MainPage>(this, "EnableMenu");

                usernameEntry.Text = "";
                passwordEntry.Text = "";
                usernameEntry.IsVisible = false;
                passwordEntry.IsVisible = false;
                logOutButton.IsVisible = true;
                logInLabel.IsVisible = false;
                welcomeLabels.IsVisible = true;
                signInButton.IsVisible = false;
            }
            else
            {
                await DisplayAlert("Login failed", "incorrect username or password.", "ok");
            }
        }
        private void logOutButton_Clicked(object sender, EventArgs e)
        {
            signInButton.IsVisible = false;
            usernameEntry.IsVisible = true;
            passwordEntry.IsVisible = true;
            signInButton.IsVisible = true;
            logOutButton.IsVisible = false;
            welcomeLabels.IsVisible = false;
            logInLabel.IsVisible = true;
            MessagingCenter.Send<MainPage>(this, "LockMenu");
        }
    }
}
