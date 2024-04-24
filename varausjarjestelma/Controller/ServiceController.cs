using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace varausjarjestelma.Controller
{
    public class ServiceController
    {
        public static async Task<List<ServiceData>> GetAllServiceDataAsync()
        {
            MySqlConnection connection = MySqlController.GetConnection();

            await connection.OpenAsync();

            using (var command = new MySqlCommand(@"SELECT
                        p.palvelu_id,
                        p.alue_id,
                        p.nimi,
                        p.tyyppi,
                        p.kuvaus,
                        p.hinta,
                        p.alv,
                        a.nimi AS alue_nimi
                    FROM
                        palvelu p
                    JOIN
                        alue a ON p.alue_id = a.alue_id
                    ORDER BY
                        p.palvelu_id;
                    ", connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                List<ServiceData> serviceDataList = new List<ServiceData>();

                while (await reader.ReadAsync())
                {
                    ServiceData serviceData = new ServiceData
                    {
                        ServiceId = reader.GetInt32("palvelu_id"),
                        AreaId = reader.GetInt32("alue_id"),
                        AreaName = reader.GetString("alue_nimi"),
                        Name = reader.GetString("nimi"),
                        Type = reader.GetInt32("tyyppi"),
                        Description = reader.GetString("kuvaus"),
                        Price = reader.GetDouble("hinta"),
                        Vat = reader.GetDouble("alv"),
                        Category = reader.GetInt32("tyyppi")

                    };
                    serviceDataList.Add(serviceData);
                }
                await connection.CloseAsync();
                return serviceDataList;
            }
        }


        public async Task<List<ServiceData>> GetServiceDataByAreaId(int id)
        {
            try
            {
                MySqlConnection connection = MySqlController.GetConnection();
                await connection.OpenAsync();

                using (var command = new MySqlCommand("SELECT * FROM palvelu WHERE alue_id = @id;", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        List<ServiceData> serviceDataList = new List<ServiceData>();

                        while (await reader.ReadAsync())
                        {
                            ServiceData serviceData = new ServiceData
                            {
                                ServiceId = reader.GetInt32("palvelu_id"),
                                AreaId = reader.GetInt32("alue_id"),
                                Name = reader.GetString("nimi"),
                                Type = reader.GetInt32("tyyppi"),
                                Description = reader.GetString("kuvaus"),
                                Price = reader.GetDouble("hinta"),
                                Vat = reader.GetDouble("alv")
                            };
                            serviceDataList.Add(serviceData);
                        }
                        await connection.CloseAsync();
                        return serviceDataList;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public static async Task<ServiceData> GetServiceDataById(int id)
        {
            MySqlConnection connection = MySqlController.GetConnection();
            try
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand("SELECT * FROM palvelu WHERE palvelu_id = @id;", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            ServiceData serviceData = new ServiceData
                            {

                                AreaId = reader.GetInt32("alue_id"),
                                Name = reader.GetString("nimi"),
                                Type = reader.GetInt32("tyyppi"),
                                Description = reader.GetString("kuvaus"),
                                Price = reader.GetDouble("hinta"),
                                Vat = reader.GetDouble("alv")
                            };
                            return serviceData;
                        }
                        else
                        {
                            Debug.WriteLine("Great problem in Mysql when fetching service data.");
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
        public static async Task<bool> InsertAndModifyServiceAsync(Database.Service service, string option)
        {
            MySqlConnection connection = MySqlController.GetConnection();
            await connection.OpenAsync();
            string optionText;
            Debug.WriteLine("service contents: " + "palvid: " + service.palvelu_id + " " + "alue_id" + service.alue_id + " nimi:  " + service.nimi + " typpii:  " + service.tyyppi + " hinta:" + service.hinta + " alv: " + service.alv + " kuvaus: " + service.kuvaus);
            Debug.WriteLine("MYSQL optiontext" + option);
            try
            {
                switch (option)
                {
                    case "modify":
                        optionText = @"UPDATE palvelu SET alue_id = @alue_id, nimi = @nimi, tyyppi = @tyyppi, 
                                        hinta = @hinta, alv = @alv, kuvaus = @kuvaus
                                         WHERE palvelu_id = @id";
                        break;

                    default: // add
                        optionText = @"INSERT INTO palvelu (alue_id, nimi, tyyppi, hinta, alv, kuvaus)
                                        VALUES (@alue_id, @nimi, @tyyppi, @hinta, @alv, @kuvaus)";
                        break;
                }

                using (var command = new MySqlCommand(optionText, connection))
                {
                    if (option == "modify")
                    {
                        command.Parameters.AddWithValue("@id", service.palvelu_id);
                    }
                    command.Parameters.AddWithValue("@alue_id", service.alue_id);
                    command.Parameters.AddWithValue("@nimi", service.nimi);
                    command.Parameters.AddWithValue("@tyyppi", service.tyyppi);
                    command.Parameters.AddWithValue("@hinta", service.hinta);
                    command.Parameters.AddWithValue("@alv", service.alv);
                    command.Parameters.AddWithValue("@kuvaus", service.kuvaus);
                    Debug.WriteLine("command: " + command.ToString());
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
        public static async Task<int> GetServiceIdByServiceName(string name)
        {
            MySqlConnection connection = MySqlController.GetConnection();
            try
            {
                await connection.OpenAsync();
                Debug.WriteLine("Service name: " + name);
                using (var command = new MySqlCommand(@"
                    SELECT palvelu_id FROM palvelu WHERE nimi = @name", connection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return reader.GetInt32("palvelu_id");
                        }
                        else
                        {
                            Debug.WriteLine("Great problem in Mysql when fetching service data.");
                            return 0;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
            finally { await connection.CloseAsync(); }
        }

        public static async Task<bool> DeleteServiceAsync(int id)
        {
            MySqlConnection connection = MySqlController.GetConnection();
            await connection.OpenAsync();

            try
            {
                using (var command = new MySqlCommand(
                    @"DELETE FROM palvelu WHERE palvelu_id = @id", connection))
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

    public class ServiceData
    {
        public int ServiceId { get; set; }
        public int AreaId { get; set; }
        public string AreaName { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public double Vat { get; set; }
        public int Category { get; set; }
    }
}
