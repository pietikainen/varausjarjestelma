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
                    query = @"SELECT 
                                m.mokki_id,
                                m.mokkinimi,
                                m.hinta,
                                COUNT(*) AS reservationCount,
                                SUM(DATEDIFF(varattu_loppupvm, varattu_alkupvm)) AS totalDaysReserved,
                                ROUND(COUNT(*) * 100 / SUM(COUNT(*)) OVER (), 2) / 100 AS percentage,
                                ROUND((SUM(DATEDIFF(varattu_loppupvm, varattu_alkupvm))) / DATEDIFF(@end, @start), 2) AS usePercentage,
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
                                ROUND((SUM(DATEDIFF(varattu_loppupvm, varattu_alkupvm))) / DATEDIFF(@end, @start), 2) AS usePercentage,
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

        public static async Task<List<ServiceReporting>> GetServiceReportingDataByAreaAsync(string area, DateTime start, DateTime end)
        {
            MySqlConnection connection = MySqlController.GetConnection();
            try
            {
                Debug.WriteLine("Opening MYSQL connection: ");
                await connection.OpenAsync();

                List<ServiceReporting> serviceReports = new List<ServiceReporting>();
                var query = "";

                if (area == "All")
                {
                    query = @"SELECT 
                        p.palvelu_id as ServiceId,
                        p.nimi AS ServiceName,
                        p.hinta AS ServicePrice,
                        SUM(vp.lkm) AS ServiceCount,
						ROUND((SUM(vp.lkm) / SUM(SUM(vp.lkm)) OVER ()), 2) AS ServicePercentageCountOfAllServices,
                        AVG(vp.lkm) AS ServiceAverageCountIfSelected,
                        SUM(vp.lkm * p.hinta) as totalRevenue
                    FROM varauksen_palvelut vp
                    JOIN palvelu p ON vp.palvelu_id = p.palvelu_id
                    JOIN varaus v ON vp.varaus_id = v.varaus_id
                    WHERE v.varattu_alkupvm >= @start
                    AND v.varattu_loppupvm <= @end
                    GROUP BY p.palvelu_id, p.nimi, p.hinta;";
                }

                else
                {
                    query = @"SELECT 
                        p.palvelu_id as ServiceId,
                        p.nimi AS ServiceName,
                        p.hinta AS ServicePrice,
                        SUM(vp.lkm) AS ServiceCount,
						ROUND((SUM(vp.lkm)/ SUM(SUM(vp.lkm)) OVER ()), 2) AS ServicePercentageCountOfAllServices,
                        AVG(vp.lkm) AS ServiceAverageCountIfSelected,
                        SUM(vp.lkm * p.hinta) as totalRevenue
                    FROM varauksen_palvelut vp
                    JOIN palvelu p ON vp.palvelu_id = p.palvelu_id
                    JOIN varaus v ON vp.varaus_id = v.varaus_id
                    WHERE v.varattu_alkupvm >= @start
                    AND v.varattu_loppupvm <= @end
                    AND a.nimi = @name
                    GROUP BY p.palvelu_id, p.nimi, p.hinta;";
                }

                using (var command = new MySqlCommand(query, connection))
                {
                    Debug.WriteLine("Adding MYSQL parameters");
                    command.Parameters.AddWithValue("@start", start);
                    command.Parameters.AddWithValue("@end", end);
                    command.Parameters.AddWithValue("@name", area);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Debug.WriteLine("Reading data from MYSQL");
                            ServiceReporting serviceReport = new ServiceReporting
                            {
                                ServiceId = reader.GetInt32("ServiceId"),
                                ServiceName = reader.GetString("ServiceName"),
                                ServicePrice = reader.GetDouble("ServicePrice"),
                                ServiceCount = reader.GetInt32("ServiceCount"),
                                ServicePercentageCountOfAllServices = reader.GetDouble("ServicePercentageCountOfAllServices"),
                                ServiceAverageCountIfSelected = reader.GetDouble("ServiceAverageCountIfSelected"),
                                ServiceTotalRevenue = reader.GetDouble("totalRevenue")
                            };
                            Debug.WriteLine("Data read from MYSQL");
                            serviceReports.Add(serviceReport);
                            Debug.WriteLine("Adding data to serviceReport");
                        }
                    }
                }

                await connection.CloseAsync();
                return serviceReports;
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


        // AVG = prosenttia varauksista joissa palveluita, jossa palvelu x on mukana
        // averageservicecountperreservation = palveluiden keskimääräinen määrä varauksessa, jossa ko. palvelu
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

    public class ServiceReporting
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public double ServicePrice { get; set; }
        public int ServiceCount { get; set; }
        
        public double ServiceAverageCountIfSelected { get; set; }
        public double ServicePercentageCountOfAllServices { get; set; }

        public double ServiceTotalRevenue { get; set; }

    }
}
