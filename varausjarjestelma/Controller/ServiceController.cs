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

                using (var command = new MySqlCommand(@"SELECT * FROM palvelu;", connection))
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
        public static async Task<bool> InsertAndModifyServiceAsync(Database.Service service, string option)
        {
            MySqlConnection connection = MySqlController.GetConnection();
            await connection.OpenAsync();
            string optionText;
            try
            {
                switch (option)
                {
                    case "modify":
                        optionText = @"UPDATE palvelu SET nimi = @nimi, tyyppi = @tyyppi, 
                                        hinta = @hinta, kuvaus = @kuvaus,
                                       WHERE palvelu_id = @id";
                        break;
                    default: //add 
                        optionText = @"INSERT INTO palvelu (nimi, tyyppi, hinta, kuvaus)
                                        VALUES (@nimi, @tyyppi, @hinta, @kuvaus);";
                        break;
                }
                using (var command = new MySqlCommand(optionText, connection))
                {
                    if (option == "modify")
                    {
                        command.Parameters.AddWithValue("@id", service.palvelu_id);
                    }
                    command.Parameters.AddWithValue("@nimi", service.nimi);
                    command.Parameters.AddWithValue("@tyyppi", service.tyyppi);
                    command.Parameters.AddWithValue("@hinta", service.hinta);
                    command.Parameters.AddWithValue("@kuvaus", service.kuvaus);
                    
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
        public string Name { get; set; }
        public int Type { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public double Vat { get; set; }
    }

}
