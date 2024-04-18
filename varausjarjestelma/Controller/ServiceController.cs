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
        public async Task<List<ServiceData>> GetAllServiceDataAsync()
        {
            MySqlConnection connection = MySqlController.GetConnection();

            await connection.OpenAsync();

                using (var command = new MySqlCommand("SELECT * FROM palvelu;", connection))
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
