using System;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient.Authentication;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using varausjarjestelma.Database;


// This class is used to connect to the MySQL database and retrieve data from it.
// Still contains obsolete mysql.data library, which should be replaced with the EF Core library.
// Will remove later and create controller classes to each table class to avoid confusion.



namespace varausjarjestelma.Database 
{ 
    public class MySqlController
    {
        private readonly MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder
        {
            Server = "ohjelmisto1-sql-pietikainen-6a40.a.aivencloud.com",
            UserID = "kayttaja",
            Password = "AVNS_DpLuKO1gwxqxgTFe1dy",
            Database = "vn",
            Port = 11244
        };

        public MySqlController()
        {        
        }

        public static async Task<List<Customer>> GetCustomersAsync()
        {
            using (var context = new AppContext())
            {
                try
                {
                    Debug.WriteLine("Trying to get customers from database");

                    if (context.Database.CanConnect())
                    {
                        var customers = await context.asiakas.ToListAsync();
                        Debug.WriteLine("Got customers from database");
                        return customers;
                    }
                    else
                    {
                        Debug.WriteLine("Database connection is not open.");
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error getting customers from database:");
                    Debug.WriteLine(e.Message);
                    return null;
                }
            }
        }


        public async Task<bool> TestConnectionAsync()
        {
            MySql.Data.MySqlClient.MySqlConnection conn;

            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = connectionStringBuilder.ConnectionString;
                Debug.WriteLine(connectionStringBuilder.ConnectionString);
                await conn.OpenAsync();
                return true;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<List<AlueData>> GetAllAlueDataAsync()
        {
            using (var connection = new MySqlConnection(connectionStringBuilder.ConnectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand("SELECT * FROM alue", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    List<AlueData> alueDataList = new List<AlueData>();

                    while (await reader.ReadAsync())
                    {
                        AlueData alueData = new AlueData
                        {
                            AlueId = reader.GetInt32("alue_id"),
                            Nimi = reader.GetString("nimi")
                        };
                        alueDataList.Add(alueData);
                    }
                    return alueDataList;
                }
            }
        }

        public class AlueData
        {
            public int AlueId { get; set; }
            public string Nimi { get; set; }
        }
    }
}
