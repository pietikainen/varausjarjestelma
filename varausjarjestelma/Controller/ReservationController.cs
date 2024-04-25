using MySql.Data.MySqlClient;
using varausjarjestelma.Database;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data;

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

        // get all reservation data
        public static async Task<List<ReservationListViewItems>> GetAllReservationDataAsync()
        {
            MySqlConnection connection = MySqlController.GetConnection();

            try
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand(@"
                    SELECT v.*, concat(m.mokkinimi, ', ', al.nimi) AS cabinName, concat(a.sukunimi, ' ', a.etunimi) AS customerName
                        FROM varaus v
                        INNER JOIN asiakas a ON v.asiakas_id = a.asiakas_id
                        INNER JOIN mokki m ON v.mokki_mokki_id = m.mokki_id
                        INNER JOIN alue al ON m.alue_id = al.alue_id;
                        ", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        List<ReservationListViewItems> reservations = new List<ReservationListViewItems>();

                        while (await reader.ReadAsync())
                        {
                            ReservationListViewItems reservation = new ReservationListViewItems()
                            {
                                reservationId = reader.GetInt32("varaus_id"),
                                customerId = reader.GetInt32("asiakas_id"),
                                cabinId = reader.GetInt32("mokki_mokki_id"),
                                reservedDate = reader.GetDateTime("varattu_pvm"),
                                confirmationDate = reader.GetDateTime("vahvistus_pvm"),
                                startDate = reader.GetDateTime("varattu_alkupvm"),
                                endDate = reader.GetDateTime("varattu_loppupvm"),
                                customerName = reader.GetString("customerName"),
                                cabinName = reader.GetString("cabinName")
                                //areaName = readerGetString("areaName")
                            };

                            reservations.Add(reservation);
                        }

                        return reservations;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        // Delete reservation

        public static async Task<bool> DeleteReservationAsync(int reservationId)
        {
            MySqlConnection connection = MySqlController.GetConnection();

            try
            {
                await connection.OpenAsync();

                // insert check to see if reservation has services in "varauksen_palvelut" table
                // if it does, delete them first

                using (var command = new MySqlCommand("DELETE FROM varauksen_palvelut WHERE varaus_id = @reservationId;", connection))
                {
                    command.Parameters.AddWithValue("@reservationId", reservationId);
                    await command.ExecuteNonQueryAsync();
                }

                using (var command = new MySqlCommand("DELETE FROM varaus WHERE varaus_id = @reservationId;", connection))
                {
                    command.Parameters.AddWithValue("@reservationId", reservationId);
                    await command.ExecuteNonQueryAsync();
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        // insert new reservation to database
        public static async Task<int> InsertReservationAsync(Reservation reservation)
        {
            MySqlConnection connection = MySqlController.GetConnection();

            try
            {
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;

            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        // Find all available cabins on given dates

        public static async Task<List<CabinData>> GetAllAvailableCabinsOnDatesAsync(int id, DateTime start, DateTime end)
        {
            MySqlConnection connection = MySqlController.GetConnection();

            try
            {
                Debug.WriteLine("Connection opened");
                Debug.WriteLine("area: " + id + " start: " + start + " end: " + end);
                await connection.OpenAsync();

                using (var command = new MySqlCommand(@"
                 SELECT
                    m.mokki_id,
                    m.alue_id,
                    m.hinta,
                    m.mokkinimi,
                    m.katuosoite,
                    m.postinro,
                    p.toimipaikka,
                    m.henkilomaara,
                    m.kuvaus,
                    m.varustelu
                 FROM 
                    mokki m
                 JOIN 
                    posti p on m.postinro = p.postinro
                 WHERE 
                    m.alue_id = @id
                 AND 
                    m.mokki_id NOT IN (
                        SELECT DISTINCT v.mokki_mokki_id
                        FROM varaus v
                        WHERE (v.varattu_alkupvm BETWEEN @start AND @end)
                               OR(v.varattu_loppupvm BETWEEN @start AND @end)
                        )", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@start", start);
                    command.Parameters.AddWithValue("@end", end);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        List<CabinData> cabinDataList = new List<CabinData>();

                        while (await reader.ReadAsync())
                        {
                            CabinData cabinData = new CabinData
                            {
                                CabinId = reader.GetInt32("mokki_id"),
                                AreaId = reader.GetInt32("alue_id"),
                                PostalCode = reader.GetString("postinro"),
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
            catch (Exception e)
            {
                Debug.WriteLine("ERROR: " + e.Message);
                return null;
            }
            finally
            {
                await connection.CloseAsync();
            }

        }




        // set reservation as confirmed

        public static async Task<bool> SetReservationConfirmedAsync(int id)
        {
            MySqlConnection connection = MySqlController.GetConnection();

            try
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand(@"UPDATE varaus SET vahvistus_pvm = @confirmationDate WHERE varaus_id = @id;", connection))
                {
                    command.Parameters.AddWithValue("@confirmationDate", DateTime.Now);
                    command.Parameters.AddWithValue("@id", id);
                    await command.ExecuteNonQueryAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                await connection.CloseAsync();
            }


        }


    }

    public class ReservationListViewItems
    {
        public int reservationId { get; set; }
        public int customerId { get; set; }
        public string customerName { get; set; }
        public int cabinId { get; set; }
        public string AreaName { get; set; }
        public string cabinName { get; set; }
        public DateTime reservedDate { get; set; }
        public DateTime confirmationDate { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }

    }

    public class ReservationInfo
    {
        public int CustomerId { get; set; }
        public int CabinId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Dictionary<int, int> Services { get; set; }

    }
}
