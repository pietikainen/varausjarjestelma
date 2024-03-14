using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.EntityFrameworkCore;

namespace varausjarjestelma.Controller
{
    public class CustomerController
    {
        private readonly MySqlConnectionStringBuilder connectionStringBuilder;
        private readonly String connectionString = ConfigurationManager.AppSettings["connectionString"];

        public static async Task<List<Database.Customer>> GetCustomersAsync()
        {
            using (var context = new Database.AppContext())
            {
                try
                {
                    Debug.WriteLine("Trying to get customers from database");

                    if (context.Database.CanConnect())
                    {
                        var customers = await context.asiakas.ToListAsync();
                        Debug.WriteLine("Got customers from database");
                        return customers;
                    }
                    else
                    {
                        Debug.WriteLine("Database connection is not open.");
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error getting customers from database:");
                    Debug.WriteLine(e.Message);
                    return null;
                }
            }
        }

        public async Task<List<CustomerData>> GetAllCustomerDataAsync()
        {
            MySqlConnection connection = MySqlController.GetConnection();
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
                await connection.CloseAsync();
                    return customerDataList;
                }
            }
        


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
}
