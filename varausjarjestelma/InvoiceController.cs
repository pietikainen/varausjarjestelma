using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace varausjarjestelma
{
    internal class InvoiceController
    {


        // laskussa:
        // asiakas
        //      nimi,osoite, sähköposti, puhelin
        // mökki
        //      nimi, alkupvm, loppupvm, hinta

        // lasku
        //      laskunumero, laskunpäivämäärä, eräpäivä, summa, verot
        //      varauksen palvelut
        //          palvelun nimi, hinta, alv



        public class Customer
        {
            public int CustomerId { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
        }

        public class Reservation
        {
            public int ReservationId { get; set; }
            public int CustomerId { get; set; }
            public int RoomId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        public class Cabin
        {
            public int RoomId { get; set; }
            public string RoomType { get; set; }
            public int RoomNumber { get; set; }
            public int Price { get; set; }
        }
    }

}