using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using MySql.Data.MySqlClient;

namespace varausjarjestelma.Controller
{
    public class PostalCodeController
    {
        public PostalCodeController() { }

        public static async Task<bool> InsertPostalCodeAsync(Database.PostalCode postalCode)
        {
            MySqlConnection connection = MySqlController.GetConnection();
            await connection.OpenAsync();

            try
            {
                // Check if postal code already exists in database
                Debug.WriteLine("Inside insertpostalcodeasync try");
                using (var checkCommand = new MySqlCommand(
                    @"SELECT * FROM posti WHERE postinro = @postinro", connection))
                {
                    checkCommand.Parameters.AddWithValue("@postinro", postalCode.postinro);
                    using (var reader = await checkCommand.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            Debug.WriteLine("Postal code already exists in database");
                            return false;
                        }
                    }
                }


                // Insert postal code to database

                using (var command = new MySqlCommand(
                    @"INSERT INTO posti (postinro, toimipaikka)
                    VALUES (@postinro, @toimipaikka)", connection))
                {
                    command.Parameters.AddWithValue("@postinro", postalCode.postinro);
                    command.Parameters.AddWithValue("@toimipaikka", postalCode.toimipaikka);

                    await command.ExecuteNonQueryAsync();

                    Debug.WriteLine("Postal code inserted to database");
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error inserting postal code to database:");
                Debug.WriteLine(e.Message);
                return false;
            }
        }

    }
}
