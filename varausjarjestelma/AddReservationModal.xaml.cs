using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using varausjarjestelma.Controller;
using varausjarjestelma.Database;

namespace varausjarjestelma;

public partial class AddReservationModal : ContentPage
{
    private ReservationInfo reservationInfo;

    public AddReservationModal(ReservationInfo reservationInfo)
    {
        InitializeComponent();
        this.reservationInfo = reservationInfo;
        PopulateDetails(reservationInfo);
    }

    private async void PopulateDetails(ReservationInfo reservationInfo)
    {
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;

        // store the grand total
        double grandTotal = 0;

        // Populate details with reservationInfo data
        if (reservationInfo != null)
        {
            Debug.WriteLine("customerid in populatedetails: " + reservationInfo.CustomerId);
            Debug.WriteLine("Reservation info on init:: " + reservationInfo.CustomerId + " " + reservationInfo.CabinId + " " + reservationInfo.StartDate + " " + reservationInfo.EndDate);

            var customerDetails = await CustomerController.GetCustomerDataAsync(reservationInfo.CustomerId);
            var cabinDetails = await CabinController.GetCabinDataAsync(reservationInfo.CabinId);

            CustomerIDLabel.Text = "Loading...";
            FullNameLabel.Text = "Loading...";
            EmailLabel.Text = "Loading...";
            PhoneLabel.Text = "Loading...";
            AddressLabel.Text = "Loading...";
            PostalCodeLabel.Text = "Loading...";
            CityLabel.Text = "Loading..."; 
            CabinLabel.Text = "Loading...";
            PriceLabel.Text = "Loading...";
            StartDateLabel.Text = "Loading...";
            EndDateLabel.Text = "Loading...";

            if (customerDetails != null)
            {
                var customerCityName = await PostalCodeController.GetCityNameAsync(customerDetails.PostalCode);

                CustomerIDLabel.Text = customerDetails.CustomerId.ToString();
                FullNameLabel.Text = customerDetails.FullName;
                EmailLabel.Text = customerDetails.Email;
                PhoneLabel.Text = customerDetails.Phone;
                AddressLabel.Text = customerDetails.Address;
                PostalCodeLabel.Text = customerDetails.PostalCode;
                CityLabel.Text = customerCityName;
            }
            else
            {
                Debug.WriteLine("Customer details are null");
            }
            if (cabinDetails != null)
            {
                CabinLabel.Text = cabinDetails.CabinName;
                PriceLabel.Text = $"{cabinDetails.Price.ToString()} €";

            }
            else
            {
                Debug.WriteLine("Cabin details are null");
            }

            // Handle dates and do calculations:
            StartDateLabel.Text = reservationInfo.StartDate.ToString("dd.MM.yyyy");
            EndDateLabel.Text = reservationInfo.EndDate.ToString("dd.MM.yyyy");

            TimeSpan duration = reservationInfo.EndDate - reservationInfo.StartDate;
            int nights = duration.Days;

            DatesTotalLabel2.Text = $"x {nights}";


            // Handle money and do calculations:
            if (cabinDetails != null)
            {
                double total = nights * cabinDetails.Price;
                grandTotal += total;
                CabinTotalLabel.Text = $"{total} €";
            }

            // Handle services and do calculations:
            Dictionary<int, int> servicesOrdered = new Dictionary<int, int>();
            double serviceGrandTotal = 0;

            if (reservationInfo.Services.IsNullOrEmpty())
            {
                Label servicesNullLabel = new Label
                {
                    Text = "No services on order",
                    HorizontalOptions = LayoutOptions.Start
                };

                StackLayout serviceNullLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children = { servicesNullLabel }
                };

                ServicesOnReservationList.Children.Add(serviceNullLayout);
            }

            foreach (KeyValuePair<int, int> service in reservationInfo.Services)
            {
                Debug.WriteLine("Service id: " + service.Key);
                Debug.WriteLine("Service amount: " + service.Value);




                if (service.Value != 0)
                {
                    var serviceDetails = await ServiceController.GetServiceDataById(service.Key);

                    if (serviceDetails != null)
                    {
                        Debug.WriteLine("Service name: " + serviceDetails.Name);
                        Debug.WriteLine("Service price: " + serviceDetails.Price);
                        Debug.WriteLine("Service type: " + serviceDetails.Type);
                        Debug.WriteLine("Service area id: " + serviceDetails.AreaId);
                        Debug.WriteLine("Service description: " + serviceDetails.Description);

                        double serviceTotal = serviceDetails.Price * service.Value;
                        servicesOrdered.Add(service.Key, service.Value);
                        serviceGrandTotal += serviceTotal;

                        // create a new label for each service and price

                        Label serviceNameLabel = new Label
                        {
                            Text = serviceDetails.Name,
                        };

                        Label servicePriceLabel = new Label
                        {
                            Text = serviceDetails.Price + " €",
                            HorizontalOptions = LayoutOptions.End
                        };

                        Label serviceAmountLabel = new Label
                        {
                            Text = "x" + service.Value.ToString(),
                            HorizontalOptions = LayoutOptions.Start
                        };

                        Label serviceTotalLabel = new Label
                        {
                            Text = serviceTotal + " €",
                            FontAttributes = FontAttributes.Bold,
                            HorizontalOptions = LayoutOptions.End
                        };

                        StackLayout serviceListStackLayout = new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            Children = { serviceNameLabel }
                        };

                        StackLayout servicePriceStackLayout = new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            Children = { serviceAmountLabel, servicePriceLabel }
                        };

                        StackLayout servicetotalStackLayout = new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            Children = { serviceTotalLabel }
                        };

                        ServicesOnReservationList.Children.Add(serviceListStackLayout);
                        ServicesOnReservationList2.Children.Add(servicePriceStackLayout);
                        ServicesOnReservationList3.Children.Add(servicetotalStackLayout);
                    }
                    else
                    {
                        Label serviceNullLabel = new Label
                        {
                            Text = "No services on reservation"
                        };

                        StackLayout servicesNullStackLayout = new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            Children = { serviceNullLabel }
                        };

                        ServicesOnReservationList.Children.Add(servicesNullStackLayout);
                        Debug.WriteLine("Service details are null");
                    }
                }
            }
            if (serviceGrandTotal > 0)
            {

                grandTotal += serviceGrandTotal;

                Label serviceTotalsCombined = new Label
                {
                    Text = serviceGrandTotal + " €",
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End
                };

                StackLayout serviceTotalCombinedLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children = { serviceTotalsCombined }
                };

                ServicesTotalCell.Children.Add(serviceTotalCombinedLayout);
            }
        }
        else
        {
            Debug.WriteLine("reservationInfo is null");
        }

        // Calculate and present total sum of the reservation

        Label grandTotalLabel = new Label
        {
            Text = grandTotal + " €",
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.End
        };

        StackLayout grandTotalStackLayout = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Children = { grandTotalLabel }
        };

        GrandTotalCell.Children.Add(grandTotalStackLayout);

        LoadingIndicator.IsRunning = false;
        LoadingIndicator.IsVisible = false;
    }

    private async void SaveButtonClicked(object sender, EventArgs e)
    {
        // Varmista, että reservationInfo on käytettävissä tässä vaiheessa
        if (reservationInfo != null)
        {
            // Tallenna tiedot
            await SaveReservation(reservationInfo);
        }
        else
        {
            Debug.WriteLine("reservationInfo is null");
        }
    }

    private async Task SaveReservation(ReservationInfo reservationInfo)
    {
        // get the reservation info from the modal

        Reservation newReservation = new Reservation
        {
            asiakas_id = reservationInfo.CustomerId,
            mokki_mokki_id = reservationInfo.CabinId,
            varattu_pvm = DateTime.Now,
            vahvistus_pvm = new DateTime(1900, 1, 1),
            varattu_alkupvm = reservationInfo.StartDate,
            varattu_loppupvm = reservationInfo.EndDate
        };

        // get the list of services from the reservation info

        List<int> serviceIds = new List<int>();
        List<int> amounts = new List<int>();

        foreach (KeyValuePair<int, int> service in reservationInfo.Services)
        {
            if (service.Value != 0)
            {
                serviceIds.Add(service.Key);
                amounts.Add(service.Value);
            }
        }

        // call the reservation and services database insert methods

        int reservationId = await ReservationController.InsertReservationAsync(newReservation);

        if (reservationId != 0)
        {
            bool servicesAdded = await ServicesOnReservationController.AddServicesOnReservation(reservationId, serviceIds, amounts);

            if (servicesAdded)
            {
                await DisplayAlert("Success", $"Reservation #{reservationId} added successfully", "OK");
                await Navigation.PopModalAsync();

            }
            else
            {
                await DisplayAlert("Error", "Failed to add services on reservation", "OK");
                return;
            }
        }
        else
        {
            
            await DisplayAlert("Error", "Failed to add reservation", "OK");
            return;
        }

    }

    private async void CancelButtonClicked(object sender, EventArgs e)
    {
        // Close the modal

        await Navigation.PopModalAsync();
    }


}