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
using varausjarjestelma.Database;

namespace varausjarjestelma.Controller
{
    public class CustomerController
    {
        public static async Task<List<CustomerData>> GetAllCustomerDataAsync()
        {
            MySqlConnection connection = MySqlController.GetConnection();
            await connection.OpenAsync();

            using (var command = new MySqlCommand(
                @"SELECT a.asiakas_id, CONCAT(a.sukunimi, ' ', a.etunimi) as kokonimi, a.lahiosoite,
		                a.postinro, p.toimipaikka, a.puhelinnro, a.email 
                    FROM asiakas a
                    JOIN posti p on a.postinro = p.postinro
                    ORDER BY asiakas_id DESC;", connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                List<CustomerData> customerDataList = new List<CustomerData>();

                while (await reader.ReadAsync())
                {
                    CustomerData customerData = new CustomerData
                    {
                        CustomerId = reader.GetInt32("asiakas_id"),
                        PostalCode = reader.GetString("postinro"),
                        City = reader.GetString("toimipaikka"),
                        FullName = reader.GetString("kokonimi"),
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

        public static async Task<bool> InsertCustomerAsync(Database.Customer customer)
        {
            MySqlConnection connection = MySqlController.GetConnection();
            await connection.OpenAsync();

            try
            {
                Debug.WriteLine("Inside insertcustomerasync try");
                using (var command = new MySqlCommand(
                    @"INSERT INTO asiakas (etunimi, sukunimi, lahiosoite, postinro, email, puhelinnro)
                    VALUES (@etunimi, @sukunimi, @lahiosoite, @postinro, @email, @puhelinnro)", connection))
                {
                    command.Parameters.AddWithValue("@etunimi", customer.etunimi);
                    command.Parameters.AddWithValue("@sukunimi", customer.sukunimi);
                    command.Parameters.AddWithValue("@lahiosoite", customer.lahiosoite);
                    command.Parameters.AddWithValue("@postinro", customer.postinro);
                    command.Parameters.AddWithValue("@email", customer.email);
                    command.Parameters.AddWithValue("@puhelinnro", customer.puhelinnro);

                    await command.ExecuteNonQueryAsync();

                    Debug.WriteLine("Customer inserted to database");
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error inserting customer to database:");
                Debug.WriteLine(e.Message);
                return false;
            }
        }


    }

    public class CustomerData
    {
        public int CustomerId { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

    }
}
