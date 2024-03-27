using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Net.Cache;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


namespace varausjarjestelma.Controller
{
    public class PostalCodeController
    {
        public PostalCodeController() { }

        public static async Task<bool> InsertPostalCodeAsync(Database.PostalCode postalCode)
        {
            MySqlConnection connection = MySqlController.GetConnection();
            await connection.OpenAsync();

            try
            {
                // Check if postal code already exists in database
                Debug.WriteLine("Inside insertpostalcodeasync try");
                using (var checkCommand = new MySqlCommand(
                    @"SELECT * FROM posti WHERE postinro = @postinro", connection))
                {
                    checkCommand.Parameters.AddWithValue("@postinro", postalCode.postinro);
                    using (var reader = await checkCommand.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            Debug.WriteLine("Postal code already exists in database");
                            return false;
                        }
                    }
                }


                // Insert postal code to database

                using (var command = new MySqlCommand(
                    @"INSERT INTO posti (postinro, toimipaikka)
                    VALUES (@postinro, @toimipaikka)", connection))
                {
                    command.Parameters.AddWithValue("@postinro", postalCode.postinro);
                    command.Parameters.AddWithValue("@toimipaikka", postalCode.toimipaikka);

                    await command.ExecuteNonQueryAsync();

                    Debug.WriteLine("Postal code inserted to database");
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error inserting postal code to database:");
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public static async Task<string> FetchPostalCodeFromApi(string postalCode)
        {
            var apiToken = ConfigurationManager.AppSettings["PostalCodeAPIToken"];

            string url = "https://sbxgw.ecosystem.posti.fi/location/v3/find-by-address?postcode=" + postalCode + "&limit=1&filter=%7B%0A%20%20%22parcelLocker%22%3A%20true%2C%0A%20%20%22siteAccess%22%3A%20%22PUBLIC%22%0A%7D";
            string cityName = "";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

                    request.Headers.Add("accept", "application/json");
                    request.Headers.Add("Accept-Language", "et, en;q=0.9");
                    request.Headers.Add("Authorization", "Bearer " + apiToken);

                    HttpResponseMessage response = await client.SendAsync(request);

                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();

                    Debug.WriteLine("Response from Posti API: " + responseBody);

                    // Parse the JSON response

                    JObject jsonObject = JsonConvert.DeserializeObject<JObject>(responseBody);

                    if (jsonObject.Count <= 0)
                    {
                        return "Postal Code doesn't exist";
                    }
                    else
                    {
                        cityName = (string)jsonObject["servicePoints"][0]["addresses"][0]["city"];
                        return cityName;
                    }



                    
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error fetching postal code from Posti API");
                    Debug.WriteLine(e.Message);
                    return "Cannot fetch postal code";
                }
            }

        }

        internal class PostalCodeApiResponse
        {
            public string postalCode { get; set; }
            public string city { get; set; }
        }

    }
}
