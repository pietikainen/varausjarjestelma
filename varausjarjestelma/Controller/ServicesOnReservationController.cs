using MySql.Data.MySqlClient;
using varausjarjestelma.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace varausjarjestelma.Controller
{
    public class ServicesOnReservationController
    {

        // get services on reservation by reservation id

        public static async Task<List<ServiceOnReservation>> GetServicesOnReservationAsync(int reservationId)
        {
            List<ServiceOnReservation> servicesOnReservations = new List<ServiceOnReservation>();
            try
            {
                MySqlConnection connection = MySqlController.GetConnection();
                await connection.OpenAsync();

                using (var command = new MySqlCommand("SELECT * FROM varauksen_palvelut WHERE varaus_id = @reservationId;", connection))
                {
                    command.Parameters.AddWithValue("@reservationId", reservationId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            servicesOnReservations.Add(new ServiceOnReservation()
                            {
                                varaus_id = reader.GetInt32("varaus_id"),
                                palvelu_id = reader.GetInt32("palvelu_id"),
                                lkm = reader.GetInt32("lkm")
                            });
                        }
                    }
                }
                await connection.CloseAsync();
                return servicesOnReservations;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }



        // Add one service on Reservation

        public static async Task<bool> AddOneServiceOnReservationAsync(int reservationId, int serviceId, int amount)
        {
            try
            {
                MySqlConnection connection = MySqlController.GetConnection();
                await connection.OpenAsync();

                using (var command = new MySqlCommand("INSERT INTO varauksen_palvelut (varaus_id, palvelu_id, lkm) VALUES (@reservationId, @serviceId, @amount);", connection))
                {
                    command.Parameters.AddWithValue("@reservationId", reservationId);
                    command.Parameters.AddWithValue("@serviceId", serviceId);
                    command.Parameters.AddWithValue("@amount", amount);
                    await command.ExecuteNonQueryAsync();
                }
                await connection.CloseAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        // Add multiple services on reservation
        public static async Task<bool> AddServicesOnReservation(int reservationId, List<int> serviceIds, List<int> amounts)
        {
            try
            {
                MySqlConnection connection = MySqlController.GetConnection();
                await connection.OpenAsync();

                for (int i = 0; i < serviceIds.Count; i++)
                {
                    using (var command = new MySqlCommand("INSERT INTO varauksen_palvelut (varaus_id, palvelu_id, lkm) VALUES (@reservationId, @serviceId, @amount);", connection))
                    {
                        command.Parameters.AddWithValue("@reservationId", reservationId);
                        command.Parameters.AddWithValue("@serviceId", serviceIds[i]);
                        command.Parameters.AddWithValue("@amount", amounts[i]);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                await connection.CloseAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }



        // Delete all SoR's by reservation id

        public static async Task<bool> DeleteAllServicesOnReservationByReservationIdAsync(int reservationId)
        {
            try
            {
                MySqlConnection connection = MySqlController.GetConnection();
                await connection.OpenAsync();

                using (var command = new MySqlCommand("DELETE FROM varauksen_palvelut WHERE varaus_id = @reservationId;", connection))
                {
                    command.Parameters.AddWithValue("@reservationId", reservationId);
                    await command.ExecuteNonQueryAsync();
                }
                await connection.CloseAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        // Delete one service from reservation by reservation id and service id

        public static async Task<bool> DeleteOneServiceOnReservationByReservationIdAndServiceIdAsync(int reservationId, int serviceId)
        {
            try
            {
                MySqlConnection connection = MySqlController.GetConnection();
                await connection.OpenAsync();

                using (var command = new MySqlCommand("DELETE FROM varauksen_palvelut WHERE varaus_id = @reservationId AND palvelu_id = @serviceId;", connection))
                {
                    command.Parameters.AddWithValue("@reservationId", reservationId);
                    command.Parameters.AddWithValue("@serviceId", serviceId);
                    await command.ExecuteNonQueryAsync();
                }
                await connection.CloseAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }




    }
}
