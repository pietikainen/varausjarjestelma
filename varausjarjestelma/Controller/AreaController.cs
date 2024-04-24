using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;


namespace varausjarjestelma.Controller
{
    public class AreaController
    {

        public static async Task<List<AreaData>> GetAllAreaDataAsync()
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
        public static async Task<bool> InsertAndModifyAreaAsync(Database.Area area, string option)
        {
            MySqlConnection connection = MySqlController.GetConnection();
            await connection.OpenAsync();
            string optionText;
            try
            {
                switch (option)
                {
                    case "modify":
                        optionText = @"UPDATE alue SET nimi = @nimi where alue_id = @id";
                        break;

                    default:
                        optionText = @"INSERT INTO alue (nimi)
                                        VALUES (@nimi)";
                        break;
                }

                using (var command = new MySqlCommand(optionText, connection))
                {
                    if (option == "modify")
                    {
                        command.Parameters.AddWithValue("@id", area.alue_id);
                    }

                    command.Parameters.AddWithValue("@nimi", area.nimi);                   
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
        public static async Task<bool> DeleteAreaAsync(int id)
        {
            MySqlConnection connection = MySqlController.GetConnection();
            await connection.OpenAsync();

            try
            {
                using (var command = new MySqlCommand(
                    @"DELETE FROM alue WHERE alue_id = @id", connection))
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




    public class AreaData
    {
        public int AreaId { get; set; }
        public string Name { get; set; }
    }

}
