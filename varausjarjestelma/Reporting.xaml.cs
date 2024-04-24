using System.Diagnostics;
using varausjarjestelma.Controller;
namespace varausjarjestelma;

public partial class Reporting : ContentPage
{
    List<AreaData>? areas;

    public Reporting()
    {
        InitializeComponent();
        ClearPage();
        InitializeAreaPicker();

    }



    private async void InitializeAreaPicker()
    {
        try
        {
            areas = await AreaController.GetAllAreaDataAsync();

            AreaPicker.Items.Add("All");

            foreach (AreaData area in areas)
            {
                AreaPicker.Items.Add(area.Name);
            }

            AreaPicker.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    private async void GenerateReportButtonClicked(object sender, EventArgs eventArgs)
    {
        // Enable loading indicators
        CabinReportingActivityIndicator.IsRunning = true;
        CabinReportingActivityIndicator.IsVisible = true;
        ServiceReportingActivityIndicator.IsRunning = true;
        ServiceReportingActivityIndicator.IsVisible = true;

        // Do the work
        var area = AreaPicker.SelectedItem.ToString();
        var cabinReports = await ReportingController.GetCabinReportingDataByAreaAsync(area, StartDate.Date, EndDate.Date);
        var serviceReports = await ReportingController.GetServiceReportingDataByAreaAsync(area, StartDate.Date, EndDate.Date);

        double cabinTotalRevenueSum = 0;
        if (cabinReports != null)
        {
            CabinReportingListViewResult.ItemsSource = cabinReports;

            foreach (CabinReporting cabinReport in cabinReports)
            {
                cabinTotalRevenueSum += cabinReport.CabinTotalRevenue;
            }
        }

        double serviceTotalRevenueSum = 0;
        if (serviceReports != null)
        {
            ServiceReportingListView.ItemsSource = serviceReports;

            foreach (ServiceReporting serviceReporting in serviceReports)
            {
                serviceTotalRevenueSum += serviceReporting.ServiceTotalRevenue;
            }
        }

        CabinTotalRevenueLabel.Text = "Total revenue: " + cabinTotalRevenueSum + "€";
        ServiceTotalRevenueLabel.Text = "Total revenue: " + serviceTotalRevenueSum + "€";


        // Disable loading indicators
        CabinReportingActivityIndicator.IsRunning = false;
        CabinReportingActivityIndicator.IsVisible = false;
        ServiceReportingActivityIndicator.IsRunning = false;
        ServiceReportingActivityIndicator.IsVisible = false;


    }

    private async void PopulateCabinReport(string area, DateTime start, DateTime end)
    {
        CabinReportingActivityIndicator.IsRunning = true;
        CabinReportingActivityIndicator.IsVisible = true;

        var cabinReports = await ReportingController.GetCabinReportingDataByAreaAsync(area, start, end);
        CabinReportingListViewResult.ItemsSource = cabinReports;

        CabinReportingActivityIndicator.IsRunning = false;
        CabinReportingActivityIndicator.IsVisible = false;

    }

    private void ClearReportsButtonClicked(object sender, EventArgs eventArgs) {
        ClearPage();
    }
    private void ClearPage()
    {
        AreaPicker.SelectedIndex = 0;
        CabinReportingListViewResult.ItemsSource = null;
        ServiceReportingListView.ItemsSource = null;

        EndDate.Date = DateTime.Now;
        StartDate.Date = DateTime.Now;

        CabinTotalRevenueLabel.Text = "";
        ServiceTotalRevenueLabel.Text = "";
    }
}