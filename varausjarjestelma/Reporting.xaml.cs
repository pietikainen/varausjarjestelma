using System.Diagnostics;
using varausjarjestelma.Controller;
namespace varausjarjestelma;

public partial class Reporting : ContentPage
{

    AreaController areaController = new AreaController();
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
            areas = await areaController.GetAllAreaDataAsync();

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


        var area = AreaPicker.SelectedItem.ToString();


        Debug.WriteLine("Area: " + area);

        var cabinReports = await ReportingController.GetCabinReportingDataByAreaAsync(area, StartDate.Date, EndDate.Date);


        double CabinTotalRevenueSum = 0;
        if (cabinReports != null)
        {
            CabinReportingListViewResult.ItemsSource = cabinReports;

            foreach (CabinReporting cabinReport in cabinReports)
            {
                CabinTotalRevenueSum += cabinReport.CabinTotalRevenue;
            }
        }

        CabinTotalRevenueLabel.Text = "Total revenue: " + CabinTotalRevenueSum + "€";
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

    private async void ClearReportsButtonClicked(object sender, EventArgs eventArgs) { }
    private async void ClearPage()
    {
        AreaPicker.SelectedIndex = 0;
        CabinReportingListViewResult.ItemsSource = null;
        ServiceReportingListView.ItemsSource = null;
    }
}