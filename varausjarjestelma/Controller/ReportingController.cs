using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using varausjarjestelma.Database;


namespace varausjarjestelma.Controller
{
    public class ReportingController
    {

        public async Task<int> GetCountOfReservationsWithinTimeFrame(DateTime start, DateTime end)
        {
            try
            {
                MySqlConnection connection = MySqlController.GetConnection();
                await connection.OpenAsync();

                int count = 0;

                using (var command = new MySqlCommand("SELECT COUNT(*) FROM varaus WHERE varattu_alkupvm >= @start AND varattu_loppupvm <= @end;", connection))
                {
                    command.Parameters.AddWithValue("@start", start);
                    command.Parameters.AddWithValue("@end", end);
                    count = Convert.ToInt32(await command.ExecuteScalarAsync());
                }

                await connection.CloseAsync();
                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 0;
            }
        }

        public static async Task<List<CabinReporting>> GetCabinReportingDataByAreaAsync(string area, DateTime start, DateTime end)
        {
            MySqlConnection connection = MySqlController.GetConnection();
            try
            {
                Debug.WriteLine("Opening MYSQL connection: ");
                await connection.OpenAsync();

                List<CabinReporting> cabinReports = new List<CabinReporting>();
                var query = "";

                if (area == "All")
                {
                    //query = @"SELECT 
                    //m.mokki_id, m.mokkinimi, m.hinta, 
                    //COUNT(*) AS amount, 
                    //ROUND(COUNT(*) * 100 / SUM(COUNT(*)) OVER (), 2) AS percentage,
                    //ROUND((SUM(DATEDIFF(varattu_loppupvm, varattu_alkupvm)) * 100.0) / SUM(SUM(DATEDIFF(@end, @start))) OVER (), 2) AS usepercentage,
                    //SUM(m.hinta * DATEDIFF(varattu_loppupvm, varattu_alkupvm)) AS totalrevenue
                    //FROM varaus v
                    //JOIN mokki m ON v.mokki_mokki_id = m.mokki_id
                    //WHERE v.mokki_mokki_id IN (
                    //	SELECT m.mokki_id
                    //    FROM mokki m
                    //    JOIN alue a ON a.alue_id = m.alue_id
                    //    )
                    //    AND v.varattu_alkupvm >= @start
                    //    AND v.varattu_loppupvm <= @end
                    //GROUP BY m.mokki_id
                    //";

                    query = @"SELECT 
                        m.mokki_id,
                        m.mokkinimi,
                        m.hinta,
                        COUNT(*) AS reservationCount,
                        SUM(DATEDIFF(varattu_loppupvm, varattu_alkupvm)) AS totalDaysReserved,
                        ROUND(COUNT(*) * 100 / SUM(COUNT(*)) OVER (), 2) / 100 AS percentage,
                        ROUND((SUM(DATEDIFF(varattu_loppupvm, varattu_alkupvm)) * 100.0) / SUM(SUM(DATEDIFF(@end, @start))) OVER (), 2) / 100 AS usePercentage,
                        SUM(m.hinta * DATEDIFF(varattu_loppupvm, varattu_alkupvm)) AS totalRevenue
                    FROM varaus v
                    JOIN mokki m ON v.mokki_mokki_id = m.mokki_id
                    WHERE v.mokki_mokki_id IN (
                        SELECT m.mokki_id
                        FROM mokki m
                        JOIN alue a ON a.alue_id = m.alue_id
                    )
                    AND v.varattu_alkupvm >= @start
                    AND v.varattu_loppupvm <= @end
                    GROUP BY m.mokki_id, m.mokkinimi, m.hinta;";
                }

                else
                {
                    query = @"SELECT 
                        m.mokki_id,
                        m.mokkinimi,
                        m.hinta,
                        COUNT(*) AS reservationCount,
                        SUM(DATEDIFF(varattu_loppupvm, varattu_alkupvm)) AS totalDaysReserved,
                        ROUND(COUNT(*) * 100 / SUM(COUNT(*)) OVER (), 2) / 100 AS percentage,
                        ROUND((SUM(DATEDIFF(varattu_loppupvm, varattu_alkupvm)) * 100.0) / SUM(SUM(DATEDIFF(@end, @start))) OVER (), 2) / 100 AS usePercentage,
                        SUM(m.hinta * DATEDIFF(varattu_loppupvm, varattu_alkupvm)) AS totalRevenue
                    FROM varaus v
                    JOIN mokki m ON v.mokki_mokki_id = m.mokki_id
                    WHERE v.mokki_mokki_id IN (
                        SELECT m.mokki_id
                        FROM mokki m
                        JOIN alue a ON a.alue_id = m.alue_id
                        WHERE a.nimi = @area
                    )
                    AND v.varattu_alkupvm >= @start
                    AND v.varattu_loppupvm <= @end
                    GROUP BY m.mokki_id, m.mokkinimi, m.hinta;";
                }


                using (var command = new MySqlCommand(query, connection))
                {
                    Debug.WriteLine("Adding MYSQL parameters");
                    command.Parameters.AddWithValue("@area", area);
                    command.Parameters.AddWithValue("@start", start);
                    command.Parameters.AddWithValue("@end", end);
                    Debug.WriteLine("Parameters added");
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Debug.WriteLine("Reading data from MYSQL");
                            CabinReporting cabinReport = new CabinReporting
                            {
                                CabinId = reader.GetInt32("mokki_id"),
                                CabinName = reader.GetString("mokkinimi"),
                                CabinPrice = reader.GetDouble("hinta"),
                                ReservationCount = reader.GetInt32("reservationCount"),
                                TotalDaysReserved = reader.GetInt32("totalDaysReserved"),
                                ReservationPercentage = reader.GetDouble("percentage"),
                                CabinUsePercentage = reader.GetDouble("usePercentage"),
                                CabinTotalRevenue = reader.GetDouble("totalRevenue") 
                            };
                            Debug.WriteLine("Data read from MYSQL");
                            cabinReports.Add(cabinReport);
                            Debug.WriteLine("Adding data to cabinReport");
                        }

                    }
                }

                await connection.CloseAsync();
                return cabinReports;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }




    }

    public class CabinReporting
    {
        public int CabinId { get; set; }
        public string CabinName { get; set; }
        public double CabinPrice { get; set; }
        public int ReservationCount { get; set; }
        public int TotalDaysReserved { get; set; }
        public double ReservationPercentage { get; set; }
        public double CabinUsePercentage { get; set; }
        public double CabinTotalRevenue { get; set; }
    }
}
