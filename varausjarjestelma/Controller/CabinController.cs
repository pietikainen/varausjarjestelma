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
    public class CabinController
    {
        public async Task<List<CabinData>> GetAllCabinDataAsync()
        {
            MySqlConnection connection = MySqlController.GetConnection();

            await connection.OpenAsync();

            using (var command = new MySqlCommand("SELECT * FROM mokki;", connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                List<CabinData> cabinDataList = new List<CabinData>();

                while (await reader.ReadAsync())
                {
                    CabinData cabinData = new CabinData
                    {
                        CabinId = reader.GetInt32("mokki_id"),
                        AreaId = reader.GetInt32("alue_id"),
                        PostalCode = reader.GetInt32("postinro"),
                        CabinName = reader.GetString("mokkinimi"),
                        Address = reader.GetString("katuosoite"),
                        Price = reader.GetDouble("hinta"),
                        Description = reader.GetString("kuvaus"),
                        Beds = reader.GetInt32("henkilomaara"),
                        Features = reader.GetString("varustelu")
                    };
                    cabinDataList.Add(cabinData);
                }
                await connection.CloseAsync();
                return cabinDataList;
            }
        }

        public async Task<List<CabinData>> GetCabinsByAreaIdAsync(int id)
        {
            MySqlConnection connection = MySqlController.GetConnection();
            await connection.OpenAsync();

            try
            {
                using (var command = new MySqlCommand(
                    @"SELECT * FROM mokki WHERE alue_id = @id;", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        List<CabinData> cabinDataList = new List<CabinData>();

                        while (await reader.ReadAsync())
                        {
                            CabinData cabinData = new CabinData
                            {
                                CabinId = reader.GetInt32("mokki_id"),
                                AreaId = reader.GetInt32("alue_id"),
                                PostalCode = reader.GetInt32("postinro"),
                                CabinName = reader.GetString("mokkinimi"),
                                Address = reader.GetString("katuosoite"),
                                Price = reader.GetDouble("hinta"),
                                Description = reader.GetString("kuvaus"),
                                Beds = reader.GetInt32("henkilomaara"),
                                Features = reader.GetString("varustelu")
                            };
                            cabinDataList.Add(cabinData);
                        }
                        await connection.CloseAsync();
                        return cabinDataList;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return null;
            }
        }

    }


    public class CabinData
    {
        public int CabinId { get; set; }
        public int AreaId { get; set; }
        public int PostalCode { get; set; }
        public string CabinName { get; set; }
        public string Address { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public int Beds { get; set; }
        public string Features { get; set; }
    }
}
