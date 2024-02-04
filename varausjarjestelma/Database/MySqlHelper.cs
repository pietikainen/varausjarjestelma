﻿using System;
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


namespace varausjarjestelma.Database
{
    public class MySqlHelper
    {
        private readonly MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder
        {
            Server = "ohjelmisto1-sql-pietikainen-6a40.a.aivencloud.com",
            UserID = "kayttaja",
            Password = "AVNS_DpLuKO1gwxqxgTFe1dy",
            Database = "vn",
            Port = 11244
        };

        public MySqlHelper()
        {        
        }

        public async Task<bool> TestConnectionAsync()
        {
            MySql.Data.MySqlClient.MySqlConnection conn;

            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = connectionStringBuilder.ConnectionString;
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

        public async Task<List<InvoiceData>> GetAllInvoicesAsync()
        {
            using (var connection = new MySqlConnection(connectionStringBuilder.ConnectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand(
                    @"SELECT
                        l.lasku_id,
                        concat(a.sukunimi, ' ', a.etunimi) AS asiakas,
	                    l.summa,
	                    l.maksettu
                    FROM 
	                    lasku l
                    JOIN
                    	varaus v ON l.varaus_id = v.varaus_id
                    JOIN
                    	asiakas a ON v.asiakas_id = a.asiakas_id;", connection))

                using (var reader = await command.ExecuteReaderAsync())
                {
                    List<InvoiceData> invoiceDataList = new List<InvoiceData>();

                    while (await reader.ReadAsync())
                    {
                        InvoiceData invoiceData = new InvoiceData
                        {
                            InvoiceNumber = reader.GetInt32("lasku_id"),
                            CustomerName = reader.GetString("asiakas"),
                            InvoiceAmount = reader.GetDouble("summa"),
                            IsPaid = reader.GetInt32("maksettu")
                        };
                        invoiceDataList.Add(invoiceData);
                        Debug.WriteLine(invoiceData.InvoiceNumber + " " + invoiceData.CustomerName + " " + invoiceData.InvoiceAmount + " " + invoiceData.IsPaid);
                    }
                    return invoiceDataList;
                }
            }
        }

        public class AlueData
        {
            public int AlueId { get; set; }
            public required string Nimi { get; set; }
        }

        public class InvoiceData
        {
            public int InvoiceNumber { get; set; }
            public string CustomerName { get; set; }
            public double InvoiceAmount { get; set; }
            public int IsPaid { get; set; }
        }   
    }
}
