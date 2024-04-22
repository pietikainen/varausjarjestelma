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
using varausjarjestelma.Database;

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
                @"SELECT l.lasku_id, a.sukunimi, a.etunimi, l.summa, l.maksettu
                    FROM lasku l
                    JOIN varaus v ON l.varaus_id = v.varaus_id
                    JOIN asiakas a ON v.asiakas_id = a.asiakas_id;", connection))

            using (var reader = await command.ExecuteReaderAsync())
            {
                List<InvoiceData> invoiceDataList = new List<InvoiceData>();

                while (await reader.ReadAsync())
                {
                    InvoiceData invoiceData = new InvoiceData
                    {
                        InvoiceNumber = reader.GetInt32("lasku_id"),
                        FirstName = reader.GetString("etunimi"),
                        LastName = reader.GetString("sukunimi"),
                        CustomerName = reader.GetString("sukunimi") + " " + reader.GetString("etunimi"),
                        InvoiceAmount = reader.GetDouble("summa"),
                        IsPaid = reader.GetInt32("maksettu")
                    };
                    invoiceDataList.Add(invoiceData);
                }
                await connection.CloseAsync();
                return invoiceDataList;

            }
        }

        public static async Task<InvoiceDetails> GetFullInvoiceAsync(int id)
        {
            MySqlConnection connection = MySqlController.GetConnection();
            await connection.OpenAsync();

            using (var command = new MySqlCommand(
                @"SELECT a.etunimi, a.sukunimi, a.lahiosoite, a.postinro, p.toimipaikka, a.email, a.puhelinnro,
                v.varaus_id, v.asiakas_id, v.mokki_mokki_id, v.varattu_alkupvm, v.varattu_loppupvm,
                m.mokki_id, m.mokkinimi, m.hinta as mokkihinta,
                l.lasku_id, l.summa, l.alv as laskualv
                FROM lasku l
                INNER JOIN varaus v ON l.varaus_id = v.varaus_id
                LEFT JOIN asiakas a ON v.asiakas_id = a.asiakas_id
                LEFT JOIN mokki m ON v.mokki_mokki_id = m.mokki_id
                LEFT JOIN posti p ON a.postinro = p.postinro
                WHERE l.lasku_id = @id;", connection))
            {
                command.Parameters.AddWithValue("@id", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        InvoiceDetails fullInvoice = new InvoiceDetails
                        {
                            CustomerId = reader.GetInt32("asiakas_id"),
                            FullName = reader.GetString("etunimi") + " " + reader.GetString("sukunimi"),
                            Address = reader.GetString("lahiosoite"),
                            PostalCode = reader.GetString("postinro"),
                            City = reader.GetString("toimipaikka"),
                            PostalCodeAndCity = reader.GetString("postinro") + " " + reader.GetString("toimipaikka"),
                            Email = reader.GetString("email"),
                            Phone = reader.GetString("puhelinnro"),

                            CabinId = reader.GetInt32("mokki_id"),
                            CabinName = reader.GetString("mokkinimi"),
                            StartDate = reader.GetDateTime("varattu_alkupvm"),
                            EndDate = reader.GetDateTime("varattu_loppupvm"),
                            Days = (reader.GetDateTime("varattu_loppupvm") - reader.GetDateTime("varattu_alkupvm")).Days,
                            CabinPrice = reader.GetDouble("mokkihinta"),
                            TotalCabinPrice = reader.GetDouble("mokkihinta") * (reader.GetDateTime("varattu_loppupvm") - reader.GetDateTime("varattu_alkupvm")).Days,

                            InvoiceId = reader.GetInt32("lasku_id"),
                            InvoiceDate = DateTime.Today.ToString("d"),
                            DueDate = DateTime.Today.AddDays(14).ToString("d"),
                            Total = reader.GetDouble("summa"),
                            Tax = reader.GetInt32("laskualv"),

                        };

                        await connection.CloseAsync();
                        return fullInvoice;
                    }
                }
            }

            await connection.CloseAsync();
            return null; // Palauta null, jos laskua ei löydy
        }

        public static async Task<List<ServicesOnReservation>> GetServicesOnReservationAsync(int id)
        {
            MySqlConnection connection = MySqlController.GetConnection();
            await connection.OpenAsync();
            Debug.WriteLine("Trying to get services on reservation from database");

            using (var command = new MySqlCommand(
                @"SELECT p.nimi, p.hinta, vp.lkm, p.alv
                    FROM palvelu p
                    JOIN varauksen_palvelut vp ON vp.palvelu_id = p.palvelu_id
                    JOIN lasku l ON vp.varaus_id = l.varaus_id
                    WHERE l.lasku_id = @id;", connection))
            {
                command.Parameters.AddWithValue("@id", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    List<ServicesOnReservation> serviceListOnReservation = new List<ServicesOnReservation>();

                    while (await reader.ReadAsync())
                    {
                        ServicesOnReservation servicesOnReservation = new ServicesOnReservation
                        {
                            ServiceName = reader.GetString("nimi"),
                            ServiceAmount = reader.GetInt32("lkm"),
                            ServicePrice = reader.GetDouble("hinta"),
                            ServiceVat = reader.GetInt32("alv")
                        };
                        serviceListOnReservation.Add(servicesOnReservation);
                    }
                    await connection.CloseAsync();
                    if (serviceListOnReservation.Count == 0)
                    {
                        Debug.WriteLine("No services on reservation found from database");
                    }
                    else
                    {
                        Debug.WriteLine("Got services on reservation from database");
                    }
                    return serviceListOnReservation;
                }
            }
        }

        public static async Task<List<InvoiceContentsCabin>> GetCabinsOnReservationAsync(int id)
        {
            MySqlConnection connection = MySqlController.GetConnection();
            await connection.OpenAsync();
            Debug.WriteLine("Trying to get cabins on reservation from database");

            using (var command = new MySqlCommand(
                @"SELECT m.mokki_id, m.mokkinimi, m.hinta, v.varattu_alkupvm, v.varattu_loppupvm, l.alv
                    FROM mokki m
                    JOIN varaus v ON m.mokki_id = v.mokki_mokki_id
                    JOIN lasku l ON v.varaus_id = l.varaus_id
                    WHERE l.lasku_id = @id;", connection))
            {
                command.Parameters.AddWithValue("@id", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    List<InvoiceContentsCabin> listOfCabins = new List<InvoiceContentsCabin>();

                    if (await reader.ReadAsync())
                    {
                        InvoiceContentsCabin contents = new InvoiceContentsCabin
                        {
                            CabinId = reader.GetInt32("mokki_id"),
                            CabinName = reader.GetString("mokkinimi"),
                            CabinAmount = (reader.GetDateTime("varattu_loppupvm") - reader.GetDateTime("varattu_alkupvm")).Days,
                            CabinPrice = reader.GetDouble("hinta"),
                            CabinVat = reader.GetInt32("alv")
                        };
                        listOfCabins.Add(contents);
                    }
                    await connection.CloseAsync();
                    if (listOfCabins.Count == 0)
                    {
                        Debug.WriteLine("No cabins on reservation found from database");
                    }
                    else
                    {
                        Debug.WriteLine("Got cabins on reservation from database");
                    }
                    return listOfCabins;
                }
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
                        var invoice = await context.lasku.Where(i => i.lasku_id == id).ToListAsync();
                        Debug.WriteLine("Got invoice from database");
                        Debug.WriteLine("invoice id: " + invoice.ToString());

                        if (invoice != null)
                        {
                            return invoice;
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

        public async Task<List<InvoiceData>> FilterInvoicesPreviewData(string filter)
        {
            MySqlConnection connection = MySqlController.GetConnection();
            await connection.OpenAsync();

            using (var command = new MySqlCommand(
                               @"SELECT
                        l.lasku_id,
                        a.sukunimi, a.etunimi,
                     l.summa,
                     l.maksettu
                    FROM 
                     lasku l
                    JOIN
                    	varaus v ON l.varaus_id = v.varaus_id
                    JOIN
                    	asiakas a ON v.asiakas_id = a.asiakas_id
                    WHERE
                    	a.sukunimi LIKE @filter OR a.etunimi LIKE @filter; OR l.lasku_id LIKE @filter", connection))
            {
                command.Parameters.AddWithValue("@filter", "%" + filter + "%");

                using (var reader = await command.ExecuteReaderAsync())
                {
                    List<InvoiceData> invoiceDataList = new List<InvoiceData>();

                    while (await reader.ReadAsync())
                    {
                        InvoiceData invoiceData = new InvoiceData
                        {
                            InvoiceNumber = reader.GetInt32("lasku_id"),
                            FirstName = reader.GetString("a.etunimi"),
                            LastName = reader.GetString("a.sukunimi"),
                            CustomerName = reader.GetString("a.sukunimi") + " " + reader.GetString("a.etunimi"),
                            InvoiceAmount = reader.GetDouble("summa"),
                            IsPaid = reader.GetInt32("maksettu")
                        };
                        invoiceDataList.Add(invoiceData);
                    }
                    await connection.CloseAsync();
                    return invoiceDataList;
                }
            }
        }

        public async Task<bool> AlterInvoicePaidStatus(int id)
        {
            using (var context = new Database.AppContext())
            {
                try
                {
                    Debug.WriteLine("Trying to get invoices from database");

                    if (context.Database.CanConnect())
                    {
                        var invoice = await context.lasku.Where(i => i.lasku_id == id).FirstOrDefaultAsync();
                        Debug.WriteLine("Got invoice from database");
                        Debug.WriteLine("invoice id: " + invoice.ToString());

                        if (invoice != null)
                        {
                            invoice.maksettu = 1;
                            await context.SaveChangesAsync();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Database connection is not open.");
                        return false;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error getting invoices from database:");
                    Debug.WriteLine(e.Message);
                    return false;
                }
            }
        }



        // TÄMÄ ON KESKEN
        public static async Task<bool> CreateInvoiceAsync(int id)
        {
            MySqlConnection connection = MySqlController.GetConnection();

            try
            {
                await connection.OpenAsync();
                // kantaan menee ALV euroissa, koska ei ole selvyyttä puhutaanko alv-prosentista vai alv-summasta.
                using (var command = new MySqlCommand(
                    @"INSERT INTO lasku (varaus_id, summa, alv, maksettu)
                    SELECT varaus_id, SUM(total_sum) AS summa, SUM(total_sum * 0.24) AS alv, 0 AS maksettu
                    FROM (
                        SELECT varaus_id, SUM(mokki_summa) AS total_sum
                        FROM (
                            SELECT v.varaus_id, SUM(m.hinta * DATEDIFF(v.varattu_loppupvm, v.varattu_alkupvm)) AS mokki_summa
                            FROM varaus v
                            JOIN mokki m ON v.mokki_mokki_id = m.mokki_id
                            WHERE v.varaus_id = @id
                            GROUP BY v.varaus_id
                    
                            UNION ALL
                    
                            SELECT vp.varaus_id, SUM(p.hinta * vp.lkm) AS palvelu_summa
                            FROM varauksen_palvelut vp
                            JOIN palvelu p ON vp.palvelu_id = p.palvelu_id
                            WHERE vp.varaus_id = @id
                            GROUP BY vp.varaus_id
                        ) AS subquery
                        GROUP BY varaus_id
                    ) AS laskutiedot
                    GROUP BY varaus_id;", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    await command.ExecuteNonQueryAsync();
                    return true;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
    }



    public class InvoiceData
    {
        public int InvoiceNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CustomerName { get; set; }
        public double InvoiceAmount { get; set; }
        public int Vat { get; set; }
        public int IsPaid { get; set; }
    }

    public class InvoiceContentsCabin
    {
        public int CabinId { get; set; }
        public string CabinName { get; set; }
        public int CabinAmount { get; set; }
        public double CabinPrice { get; set; }
        public double CabinVat { get; set; }

    }

    public class InvoiceDetails
    {

        // Customer
        public int CustomerId { get; set; }
        public required string FullName { get; set; }
        public required string Address { get; set; }
        public required string PostalCode { get; set; }
        public required string City { get; set; }
        public required string PostalCodeAndCity { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }

        // Cabin
        public int CabinId { get; set; }
        public required string CabinName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Days { get; set; }
        public double CabinPrice { get; set; }
        public double TotalCabinPrice { get; set; }

        // Invoice
        public int InvoiceId { get; set; }
        public required string InvoiceDate { get; set; }
        public required string DueDate { get; set; }
        public double Total { get; set; }
        public int Tax { get; set; }

        // Services helper

    }

    public class ServicesOnReservation
    {
        public int ServiceAmount { get; set; }
        public string ServiceName { get; set; }
        public double ServicePrice { get; set; }
        public double ServiceVat { get; set; }


    }

}