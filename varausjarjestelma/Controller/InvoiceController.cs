using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.EntityFrameworkCore;


namespace varausjarjestelma.Controller
{
    public class InvoiceController
    {
        public InvoiceController()
        {
        }


        // laskussa:
        // asiakas
        //      id, nimi, osoite, sähköposti, puhelin
        //
        // mökki
        //      nimi, alkupvm, loppupvm, hinta
        //
        // varaus
        //      id
        //
        // lasku
        //      laskunumero, laskunpäivämäärä, eräpäivä, summa, verot
        //      varauksen palvelut
        //          palvelun nimi, hinta, alv



        public async Task<List<InvoiceData>> GetAllInvoicesPreviewAsync()
        {

            MySqlConnection connection = MySqlController.GetConnection();
            await connection.OpenAsync();

            using (var command = new MySqlCommand(
                @"SELECT
                        l.lasku_id,
                        concat(a.sukunimi, ' ', a.etunimi) AS asiakas,
                     l.summa,
                     l.maksettu
                    FROM 
                     lasku l
                    JOIN
                    	varaus v ON l.varaus_id = v.varaus_id
                    JOIN
                    	asiakas a ON v.asiakas_id = a.asiakas_id;", connection))

            using (var reader = await command.ExecuteReaderAsync())
            {
                List<InvoiceData> invoiceDataList = new List<InvoiceData>();

                while (await reader.ReadAsync())
                {
                    InvoiceData invoiceData = new InvoiceData
                    {
                        InvoiceNumber = reader.GetInt32("lasku_id"),
                        CustomerName = reader.GetString("asiakas"),
                        InvoiceAmount = reader.GetDouble("summa"),
                        IsPaid = reader.GetInt32("maksettu")
                    };
                    invoiceDataList.Add(invoiceData);
                }
                await connection.CloseAsync();
                return invoiceDataList;

            }
        }


        public async Task<List<Database.Invoice>> GetInvoiceByNumber(int id)
        {
            using (var context = new Database.AppContext())
            {
                try
                {
                    Debug.WriteLine("Trying to get invoices from database");

                    if (context.Database.CanConnect())
                    {
                        var invoices = await context.lasku.Where(i => i.lasku_id == id).ToListAsync();
                        Debug.WriteLine("Got invoice from database");
                        if (invoices != null)
                        {
                            return invoices;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Database connection is not open.");
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error getting invoices from database:");
                    Debug.WriteLine(e.Message);
                    return null;
                }
            }
        }

    }
    public class InvoiceData
    {
        public int InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public double InvoiceAmount { get; set; }
        public double Vat { get; set; }
        public int IsPaid { get; set; }
    }



    public class InvoiceDetails
        {
            // Customer
            public int CustomerId { get; set; }
            public required string FullName { get; set; }
            public required string Address { get; set; }
            public required string PostalCode { get; set; }
            public required string Email { get; set; }
            public required string Phone { get; set; }

            // Cabin
            public int CabinId { get; set; }
            public required string CabinName { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int Price { get; set; }

            // Invoice
            public int InvoiceId { get; set; }
            public required string InvoiceDate { get; set; }
            public required string DueDate { get; set; }
            public int Total { get; set; }
            public int Tax { get; set; }

            // Services

        }

}