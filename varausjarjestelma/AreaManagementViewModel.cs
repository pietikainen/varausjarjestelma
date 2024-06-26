﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace varausjarjestelma
{
    public class AreaManagementViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string invoiceId;
        public string InvoiceId
        {
            get => invoiceId;
            set => SetProperty(ref invoiceId, value);
        }

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
