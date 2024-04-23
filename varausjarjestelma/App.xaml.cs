using System.Globalization;

namespace varausjarjestelma
{
    public partial class App : Application
    {

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

            // Change the window Size
            window.Width = 1280; window.Height = 960;

            // BONUS -> Center the window
            var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
            window.X = (displayInfo.Width / displayInfo.Density - window.Width) / 2;
            window.Y = (displayInfo.Height / displayInfo.Density - window.Height) / 2;
            return window;
        }

        public App()
        {
            InitializeComponent();
            CultureInfo.CurrentCulture = new CultureInfo("fi-FI");
            MainPage = new AppShell();
        }
    }
}
