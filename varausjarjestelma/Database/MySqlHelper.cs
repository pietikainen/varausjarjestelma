using System;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace varausjarjestelma.Database
{
    public class MySqlHelper
    {
        private readonly string connString = "server=ohjelmisto1-sql-pietikainen-6a40.a.aivencloud.com; port=12244; database=vn; user=kayttaja; password=AVNS_DpLuKO1gwxqxgTFe1dy";
        
        public MySqlHelper( string connString )
        {
            this.connString = connString;
        }
        public MySqlHelper()
        {
        }

        public bool TestConnection()
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            {
                try
                {
                    conn = new MySql.Data.MySqlClient.MySqlConnection(connString);
                    conn.Open();
                    return true;
                }
                catch (MySqlException ex)
                {
                    Debug.WriteLine(ex.Message);
                    return false;
                }

            }
        }

    }
}
