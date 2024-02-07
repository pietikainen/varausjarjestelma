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


namespace varausjarjestelma.Database
{
    public class MySqlHelper
    {
        private readonly MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder
        {
            Server = "ohjelmisto1-sql-pietikainen-6a40.a.aivencloud.com",
            UserID = "kayttaja",
            Password = "AVNS_DpLuKO1gwxqxgTFe1dy",
            Database = "vn",
            Port = 11244
        };

        public MySqlHelper()
        {        
        }

        public async Task<bool> TestConnectionAsync()
        {
            MySql.Data.MySqlClient.MySqlConnection conn;

            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = connectionStringBuilder.ConnectionString;
                await conn.OpenAsync();
                return true;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<List<CustomerData>> GetAllCustomerDataAsync()
        {
            using (var connection = new MySqlConnection(connectionStringBuilder.ConnectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand("SELECT * FROM asiakas JOIN posti ON asiakas.postinro = posti.postinro;", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    List<CustomerData> customerDataList = new List<CustomerData>();

                    while (await reader.ReadAsync())
                    {
                        CustomerData customerData = new CustomerData
                        {
                            CustomerId = reader.GetInt32("asiakas_id"),
                            PostalCode = reader.GetInt32("postinro"),
                            FirstName = reader.GetString("etunimi"),
                            LastName = reader.GetString("sukunimi"),
                            Address = reader.GetString("lahiosoite"),
                            Email = reader.GetString("email"),
                            Phone = reader.GetString("puhelinnro")
                        };
                        customerDataList.Add(customerData);
                    }
                    return customerDataList;
                }
            }
        }

        public async Task<List<ServiceData>> GetAllServiceDataAsync()
        {
            using (var connection = new MySqlConnection(connectionStringBuilder.ConnectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand("SELECT * FROM asiakas JOIN posti ON asiakas.postinro = posti.postinro;", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    List<ServiceData> serviceDataList = new List<ServiceData>();

                    while (await reader.ReadAsync())
                    {
                            ServiceData serviceData = new ServiceData
                            {
                            ServiceId = reader.GetInt32("palvelu_id"),
                            AreaId = reader.GetString("alue_id"),
                            Name = reader.GetString("nimi"),
                            Type = reader.GetString("tyyppi"),
                            Description = reader.GetString("kuvaus"),
                            Price = reader.GetDouble("hinta"),
                            Vat = reader.GetDouble("alv")
                        };
                            serviceDataList.Add(serviceData);
                    }
                    return serviceDataList;
                }
            }
        }

        public async Task<List<CabinData>> GetAllCabinDataAsync()
        {
            using (var connection = new MySqlConnection(connectionStringBuilder.ConnectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand("SELECT * FROM mokki;", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    List<CabinData> cabinDataList = new List<CabinData>();

                    while (await reader.ReadAsync())
                    {
                        CabinData cabinData = new CabinData
                        {
                            CabinId = reader.GetInt32("mokki_id"),
                            AreaId = reader.GetInt32("alue_id"),
                            PostalCode = reader.GetInt32("postinro"),
                            CabinName = reader.GetString("mokkinimi"),
                            Address = reader.GetString("katuosoite"),
                            Price = reader.GetDouble("hinta"),
                            Description = reader.GetString("kuvaus"),
                            Beds = reader.GetInt32("henkilomaara"),
                            Features = reader.GetString("varustelu")
                        };
                        cabinDataList.Add(cabinData);
                    }
                    return cabinDataList;
                }
            }
        }

        public async Task<List<AreaData>> GetAllAreaDataAsync()
        {
            using (var connection = new MySqlConnection(connectionStringBuilder.ConnectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand("SELECT * FROM alue", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    List<AreaData> areaDataList = new List<AreaData>();

                    while (await reader.ReadAsync())
                    {
                        AreaData areaData = new AreaData
                        {
                            AreaId = reader.GetInt32("alue_id"),
                            Name = reader.GetString("nimi")
                        };
                        areaDataList.Add(areaData);
                    }
                    return areaDataList;
                }
            }
        }

        public async Task<bool> AddAreaAsync(string newArea)
        {
            MySqlConnection connection = new MySqlConnection(connectionStringBuilder.ConnectionString);

            try
            {
                Console.WriteLine("Connecting to MySQL...");
                await connection.OpenAsync();

                string query = "INSERT INTO alue(nimi) VALUES(" + newArea + ");";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

            connection.Close();
            Console.WriteLine("Done.");
            return true;
        }

        public async Task<AreaData> GetAreaAsync(int areaID)
        {
            string query = "SELECT * FROM alue WHERE alue_id=" + areaID + ");";

            using (var connection = new MySqlConnection(connectionStringBuilder.ConnectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    AreaData alueData = new AreaData();

                    await reader.ReadAsync();
                    alueData.AreaId = reader.GetInt32("alue_id");
                    alueData.Name = reader.GetString("nimi");
                    return alueData;
                }
            }
        }

        public async Task<List<InvoiceData>> GetAllInvoicesAsync()
        {
            using (var connection = new MySqlConnection(connectionStringBuilder.ConnectionString))
            {
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
                    return invoiceDataList;
                }
            }
        }

        public class AreaData
        {
            public int AreaId { get; set; }
            public string Name { get; set; }
        }

        public class InvoiceData
        {
            public int InvoiceNumber { get; set; }
            public string CustomerName { get; set; }
            public double InvoiceAmount { get; set; }
            public double Vat { get; set; }
            public int IsPaid { get; set; }
        }   

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

        public class CustomerData
        {
            public int CustomerId { get; set; }
            public int PostalCode { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Address { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }

        }

        public class ServiceData
        {
            public int ServiceId { get; set; }
            public string AreaId { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Description { get; set; }
            public double Price { get; set; }
            public double Vat { get; set; }
        }

        public class ServicesOnReservationData
        {
            public int ReservationId { get; set; }
            public int ServiceId { get; set; }
            public int Amount { get; set; }
        }
        public class CabinData
        {
            public int CabinId { get; set; }
            public int AreaId { get; set; }
            public int PostalCode { get; set; }
            public string CabinName { get; set; }
            public string Address { get; set; }
            public double Price { get; set; }
            public string Description { get; set; }
            public int Beds { get; set; }
            public string Features { get; set; }
        }

        public class PostalCodeData
        {
            public int PostalCode { get; set; }
            public string City { get; set; }
        }


    }
}
