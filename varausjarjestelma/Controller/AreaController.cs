using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;


namespace varausjarjestelma.Controller
{
    public class AreaController
    {
        private readonly MySqlConnectionStringBuilder connectionStringBuilder;
        private readonly String connectionString = ConfigurationManager.AppSettings["connectionString"];

        public async Task<List<AreaData>> GetAllAreaDataAsync()
        {
            MySqlConnection connection = MySqlController.GetConnection();
            await connection.OpenAsync();

                using (var command = new MySqlCommand("SELECT * FROM alue", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    List<AreaData> areaDataList = new List<AreaData>();

                    while (await reader.ReadAsync())
                    {
                        AreaData areaData = new AreaData
                        {
                            AreaId = reader.GetInt32("alue_id"),
                            Name = reader.GetString("nimi")
                        };
                        areaDataList.Add(areaData);
                    }
                await connection.CloseAsync();
                    return areaDataList;
                }
            }
        

        public async Task<bool> AddAreaAsync(string newArea)
        {
            MySqlConnection connection = MySqlController.GetConnection();

            try
            {
                Console.WriteLine("Connecting to MySQL...");
                await connection.OpenAsync();

                string query = "INSERT INTO alue(nimi) VALUES(" + newArea + ");";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

            await connection.CloseAsync();
            Console.WriteLine("Done.");
            return true;
        }

        public async Task<AreaData> GetAreaAsync(int areaID)
        {
            string query = "SELECT * FROM alue WHERE alue_id=" + areaID + ");";

            MySqlConnection connection = MySqlController.GetConnection();

            await connection.OpenAsync();

                using (var command = new MySqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    AreaData alueData = new AreaData();

                    await reader.ReadAsync();
                    alueData.AreaId = reader.GetInt32("alue_id");
                    alueData.Name = reader.GetString("nimi");
                    
                    await connection.CloseAsync();
                    return alueData;
                }
            }
        


    }


    public class AreaData
    {
        public int AreaId { get; set; }
        public string Name { get; set; }
    }

}
