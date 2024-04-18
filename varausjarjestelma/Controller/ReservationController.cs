using MySql.Data.MySqlClient;
using varausjarjestelma.Database;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace varausjarjestelma.Controller
{
    public class ReservationController
    {

        public static async Task<int> GetAmountOfReservations()
        {
            MySqlConnection connection = MySqlController.GetConnection();

            await connection.OpenAsync();

            using (var command = new MySqlCommand("SELECT COUNT(*) FROM varaus;", connection))
            {
                int amount = Convert.ToInt32(await command.ExecuteScalarAsync());
                await connection.CloseAsync();
                return amount;
            }
        }

        public async Task<int> InsertReservationAsync(Reservation reservation)
        {
            MySqlConnection connection = MySqlController.GetConnection();

            try {
                await connection.OpenAsync();

                using (var command = new MySqlCommand(@"INSERT INTO varaus 
                        (asiakas_id, mokki_mokki_id, varattu_pvm, vahvistus_pvm, varattu_alkupvm, varattu_loppupvm) 
                        VALUES 
                        (@customerId, @cabinId, @reservedDate, @confirmationDate, @startDate, @endDate);",
                        connection))
                {
                    command.Parameters.AddWithValue("@customerId", reservation.asiakas_id);
                    command.Parameters.AddWithValue("@cabinId", reservation.mokki_mokki_id);
                    command.Parameters.AddWithValue("@reservedDate", reservation.varattu_pvm);
                    command.Parameters.AddWithValue("@confirmationDate", reservation.vahvistus_pvm);
                    command.Parameters.AddWithValue("@startDate", reservation.varattu_alkupvm);
                    command.Parameters.AddWithValue("@endDate", reservation.varattu_loppupvm);

                    await command.ExecuteNonQueryAsync();
                    Debug.WriteLine("Reservation inserted");
                    Debug.WriteLine("Customer ID: " + reservation.asiakas_id);
                    Debug.WriteLine("Cabin ID: " + reservation.mokki_mokki_id);
                    Debug.WriteLine("Reserved date: " + reservation.varattu_pvm);
                    Debug.WriteLine("Confirmation date: " + reservation.vahvistus_pvm);
                    Debug.WriteLine("Start date: " + reservation.varattu_alkupvm);
                    Debug.WriteLine("End date: " + reservation.varattu_loppupvm);

                    // get the id of the inserted reservation

                    using (var command2 = new MySqlCommand("SELECT LAST_INSERT_ID();", connection))
                    {
                        int id = Convert.ToInt32(await command2.ExecuteScalarAsync());
                        Debug.WriteLine("Inserted reservation id: " + id);
                        return id;
                    }
                }
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;

            } finally
            {
                await connection.CloseAsync();
            }
        }

        // Inserrt service on reservation



    }
}
