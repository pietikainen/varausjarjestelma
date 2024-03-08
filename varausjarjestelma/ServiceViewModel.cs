using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace varausjarjestelma
{
    public class ServiceViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int serviceId;
        public int ServiceId
        {   get => serviceId;   set => SetProperty(ref serviceId, value);   }

        private int areaId;
        public int AreaId
        {   get => areaId;      set => SetProperty(ref areaId, value);      }

        private string name;
        public string Name
        {   get => name;        set => SetProperty(ref name, value);        }

        private int type;
        public int Type
        { get => type; set => SetProperty(ref type, value); }

        private string description;
        public string Description
        { get => description; set => SetProperty(ref description, value); }

        private double price;
        public double Price
        { get => price; set => SetProperty(ref price, value); }

        private double vat;
        public double Vat
        { get => vat; set => SetProperty(ref vat, value); }
        // Tässä voit lisätä lisää ominaisuuksia, kuten Reservation, Amount, jne.

        // Apumetodi, joka laukaisee PropertyChanged-tapahtuman.
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Apumetodi, jota käytetään ominaisuuksien arvojen asettamiseen.
        // Tarkistaa, onko arvo muuttunut, ja jos on, asettaa uuden arvon ja laukaisee PropertyChanged-tapahtuman.
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false; // Arvo ei muuttunut, joten ei tehdä mitään
            }

            storage = value; // Asetetaan uusi arvo
            OnPropertyChanged(propertyName); // Laukaistaan PropertyChanged-tapahtuma
            return true; // Ilmoitetaan, että arvo muuttui
        }
    }
}
