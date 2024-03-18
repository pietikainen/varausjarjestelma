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
                @"SELECT a.asiakas_id, a.sukunimi, a.etunimi, a.lahiosoite,
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
                        FirstName = reader.GetString("etunimi"),
                        LastName = reader.GetString("sukunimi"),
                        FullName = reader.GetString("sukunimi") + " " + reader.GetString("etunimi"),
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

        public static async Task<bool> InsertAndModifyCustomerAsync(Database.Customer customer, string option)
        {
            MySqlConnection connection = MySqlController.GetConnection();
            await connection.OpenAsync();
            string optionText;
            try
            {

                switch (option)
                {
                    case "modify":
                        optionText = @"UPDATE asiakas SET etunimi = @etunimi, sukunimi = @sukunimi, 
                                        lahiosoite = @lahiosoite, postinro = @postinro,
                                        email = @email, puhelinnro = @puhelinnro WHERE asiakas_id = @id";
                        break;

                    default: // add
                        optionText = @"INSERT INTO asiakas (etunimi, sukunimi, lahiosoite, postinro, email, puhelinnro)
                                        VALUES (@etunimi, @sukunimi, @lahiosoite, @postinro, @email, @puhelinnro)";
                        break;
                }

                using (var command = new MySqlCommand(optionText, connection))
                {
                    if (option == "modify")
                    {
                        command.Parameters.AddWithValue("@id", customer.asiakas_id);
                    }
                    command.Parameters.AddWithValue("@etunimi", customer.etunimi);
                    command.Parameters.AddWithValue("@sukunimi", customer.sukunimi);
                    command.Parameters.AddWithValue("@lahiosoite", customer.lahiosoite);
                    command.Parameters.AddWithValue("@postinro", customer.postinro);
                    command.Parameters.AddWithValue("@email", customer.email);
                    command.Parameters.AddWithValue("@puhelinnro", customer.puhelinnro);
                    await command.ExecuteNonQueryAsync();
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public static async Task<bool> DeleteCustomerAsync(int id)
        {
            MySqlConnection connection = MySqlController.GetConnection();
            await connection.OpenAsync();

            try
            {
                using (var command = new MySqlCommand(
                    @"DELETE FROM asiakas WHERE asiakas_id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    await command.ExecuteNonQueryAsync();

                    return true;
                }
            }
            catch (Exception e)
            {
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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

    }
}
