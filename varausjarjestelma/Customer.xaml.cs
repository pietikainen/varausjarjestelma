namespace varausjarjestelma;

public partial class Customer : ContentPage
{
	public Customer()
	{
		InitializeComponent();
	}


    private async void submitButton_Clicked(object sender, EventArgs e)
    {
            var firstName = firstNameEntry.Text;
            var surName = surNameEntry.Text;
            var address = addressEntry.Text;
            var postalCode = postalCodeEntry.Text;
            var phone = phoneEntry.Text;
            var email = emailEntry.Text;

            var confirmationMessage = $"Nimi: {firstName} {surName}\nOsoite: {address}\nPostinumero: {postalCode}\nPuhelin: {phone}\nSähköposti: {email}\nHaluatko hyväksyä?";
            var isAccepted = await DisplayAlert("Vahvista tiedot", confirmationMessage, "Kyllä", "Ei");

            if (isAccepted)
            {
                var customer = new varausjarjestelma.Database.Customer
                {
                    etunimi = firstName,
                    sukunimi = surName,
                    lahiosoite = address,
                    postinro = postalCode,
                    puhelinnro = phone,
                    email = email
                };

                approvedDataList.ItemsSource ??= new List<varausjarjestelma.Database.Customer>();
                var currentData = (List<varausjarjestelma.Database.Customer>)approvedDataList.ItemsSource;
                currentData.Add(customer);

                approvedDataList.ItemsSource = null;
                approvedDataList.ItemsSource = currentData;
            } 
        
    }
}