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
        public static async Task<List<CabinData>> GetAllCabinDataAsync()
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
                await connection.CloseAsync();
                return cabinDataList;
            }
        }

        public static async Task<CabinData> GetCabinDataAsync(int id)
        {
            MySqlConnection connection = MySqlController.GetConnection();

            try
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand(@"
                    SELECT * FROM mokki WHERE mokki_id = @id;", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
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

                            return cabinData;
                        }
                        else
                        {
                            return null;
                        }
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
        public static async Task<bool> InsertAndModifyCabinAsync(Database.Cabin cabin, string option)
        {
            MySqlConnection connection = MySqlController.GetConnection();
            await connection.OpenAsync();
            string optionText;
            try
            {

                switch (option)
                {
                    case "modify":
                        optionText = @"UPDATE mokki SET mokkinimi = @mokkinimi, henkilomaara = @henkilomaara, 
                                        katuosoite = @katuosoite, postinro = @postinro,
                                        varustelu = @varustelu, kuvaus = @kuvaus, hinta = @hinta WHERE mokki_id = @id";
                        break;


                    default: // add
                        optionText = @"INSERT INTO asiakas (mokkinimi, henkilomaara, katuosoite, postinro, varustelu, kuvaus, hinta)
                                        VALUES (@mokkinimi, @henkilomaara, @katuosoite, @postinro, @varsutelu,@kuvaus, @hinta)";
                        break;
                }

                using (var command = new MySqlCommand(optionText, connection))
                {
                    if (option == "modify")
                    {
                        command.Parameters.AddWithValue("@id", cabin.mokki_id);
                    }
                    command.Parameters.AddWithValue("@mokkinimi", cabin.mokkinimi);
                    command.Parameters.AddWithValue("@henkilomaara", cabin.henkilomaara);
                    command.Parameters.AddWithValue("@katuosoite", cabin.katuosoite);
                    command.Parameters.AddWithValue("@postinro", cabin.postinro);
                    command.Parameters.AddWithValue("@varustelu", cabin.varustelu);
                    command.Parameters.AddWithValue("@kuvaus", cabin.kuvaus);
                    command.Parameters.AddWithValue("@hinta", cabin.hinta);
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

        public static async Task<bool> DeleteCabinAsync(int id)
        {
            MySqlConnection connection = MySqlController.GetConnection();
            await connection.OpenAsync();

            try
            {
                using (var command = new MySqlCommand(
                    @"DELETE FROM mokki WHERE mokki_id = @id", connection))
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



    public class CabinData
    {
        public int CabinId { get; set; }
        public int AreaId { get; set; }
        public string PostalCode { get; set; }
        public string CabinName { get; set; }
        public string Address { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public int Beds { get; set; }
        public string Features { get; set; }
    }
}
