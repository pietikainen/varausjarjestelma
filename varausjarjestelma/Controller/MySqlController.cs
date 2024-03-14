using System;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient.Authentication;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using varausjarjestelma.Database;
using System.ComponentModel;


using AppContext = varausjarjestelma.Database.AppContext;
using System.Configuration;


// This class is used to connect to the MySQL database and retrieve data from it.
// Still contains obsolete mysql.data library, which should be replaced with the EF Core library.
// Will remove later and create controller classes to each table class to avoid confusion.



namespace varausjarjestelma.Controller
{
    public class MySqlController
    {
        public MySqlController()
        {
        }

        public static MySqlConnection GetConnection()
        {
            var connectionString = ConfigurationManager.AppSettings["DatabaseConnection"];
            Debug.WriteLine("connectionstring: " + connectionString);

            return new MySqlConnection(connectionString);
        }



        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                MySqlConnection connection = MySqlController.GetConnection();

                if (connection != null)
                {
                    await connection.OpenAsync();
                    return true;
                }
                else
                {
                    return false;
                    Debug.WriteLine("Connection failed: Connection string is null");
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }


        public class FullInvoiceData
        {
            public int InvoiceNumber { get; set; }
            public List<CustomerData> Customers { get; set; }
            public double InvoiceAmount { get; set; }
            public double Vat { get; set; }
            public int IsPaid { get; set; }
            public List<ReservationData> Reservations { get; set; }
            public List<ServicesOnReservationData> ServicesOnReservations { get; set; }




            public class ReservationData
            {
                public int ReservationId { get; set; }
                public int CustomerId { get; set; }
                public int CabinId { get; set; }
                public DateTime ReservationDate { get; set; }
                public DateTime ConfirmationDate { get; set; }
                public DateTime StartDate { get; set; }
                public DateTime EndDate { get; set; }
            }



            public class ServicesOnReservationData
            {
                public int ReservationId { get; set; }
                public int ServiceId { get; set; }
                public int Amount { get; set; }
            }


            public class PostalCodeData
            {
                public int PostalCode { get; set; }
                public string City { get; set; }
            }


        }
    }
}
