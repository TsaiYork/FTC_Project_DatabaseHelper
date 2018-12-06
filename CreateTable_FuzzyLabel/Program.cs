using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Text;

namespace CreateTable_FuzzyLabel
{
    public partial class fuzzy_label_created
    {
        [JsonProperty(PropertyName = "BeaconMinor")]
        public int BeaconMinor { get; set; }

        [JsonProperty(PropertyName = "Position")]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "Label")]
        public string Label { get; set; }
    }

    public partial class fuzzy_membership_created
    {

        [JsonProperty(PropertyName = "BeaconMinor")]
        public int BeaconMinor { get; set; }

        [JsonProperty(PropertyName = "Label")]
        public string Label { get; set; }

        [JsonProperty(PropertyName = "Samples")]
        public int Samples { get; set; }

        [JsonProperty(PropertyName = "Max")]
        public int Max { get; set; }

        [JsonProperty(PropertyName = "Min")]
        public int Min { get; set; }

        [JsonProperty(PropertyName = "Mean")]
        public double Mean { get; set; }

        [JsonProperty(PropertyName = "SD")]
        public double SD { get; set; }
    }

    class Program
    {
        //設定AP、RP之數量與編號
        static List<int> APs = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        static List<int> RPs = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

        //Database 之連接字串
        static string BaseUrl = "140.122.105.187:8080/";

        static void Main(string[] args)
        {

            InitTable_FuzzyLabel();
            InitTable_FuzzyMembership();

            Console.ReadKey();
        }

        static void InitTable_FuzzyLabel()
        {
            if (IsFuzzy_LabelsNull())
            {
                foreach (int AP in APs)
                {
                    foreach (int RP in RPs)
                    {
                        string res = Post_fuzzu_label(new fuzzy_label_created()
                        {
                            BeaconMinor = AP,
                            Position = RP,
                            Label = "U"
                        });

                        //Console.WriteLine(res);
                    }
                }
            }
            Console.WriteLine("InitTable_FuzzyLabel Finished");
        }

        static void InitTable_FuzzyMembership()
        {
            if (IsFuzzy_MembershipNull())
            {
                foreach (int AP in APs)
                {
                    string res = Post_fuzzu_membership(new fuzzy_membership_created()
                    {
                        BeaconMinor = AP,
                        Label = "N"
                    });
                    res = Post_fuzzu_membership(new fuzzy_membership_created()
                    {
                        BeaconMinor = AP,
                        Label = "M"
                    });
                    res = Post_fuzzu_membership(new fuzzy_membership_created()
                    {
                        BeaconMinor = AP,
                        Label = "F"
                    });

                }
            }

            //try
            //{
            //    Console.WriteLine("Connecting to MySQL...");
            //    Console.WriteLine();
            //    conn.Open();

            //    string SqlQuery = "SELECT * FROM `ftc_office_fuzzy_membership`";
            //    cmd = new MySqlCommand(SqlQuery, conn);
            //    MySqlDataReader rdr = cmd.ExecuteReader();
            //    if (rdr.HasRows != true)
            //    {
            //        rdr.Close();
            //        foreach (int AP in APs)
            //        {
            //            SqlQuery = "INSERT INTO `ftc_office_fuzzy_membership` (`ID`, `AP`, `Label`, `Samples`, `Max`, `Min`, `Mean`, `S.D.`) VALUES (NULL, '" + AP + "', 'N', '', '', '', '', '');";
            //            cmd = new MySqlCommand(SqlQuery, conn);
            //            cmd.ExecuteNonQuery();
            //            SqlQuery = "INSERT INTO `ftc_office_fuzzy_membership` (`ID`, `AP`, `Label`, `Samples`, `Max`, `Min`, `Mean`, `S.D.`) VALUES (NULL, '" + AP + "', 'M', '', '', '', '', '');";
            //            cmd = new MySqlCommand(SqlQuery, conn);
            //            cmd.ExecuteNonQuery();
            //            SqlQuery = "INSERT INTO `ftc_office_fuzzy_membership` (`ID`, `AP`, `Label`, `Samples`, `Max`, `Min`, `Mean`, `S.D.`) VALUES (NULL, '" + AP + "', 'F', '', '', '', '', '');";
            //            cmd = new MySqlCommand(SqlQuery, conn);
            //            cmd.ExecuteNonQuery();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //    Console.ReadKey();
            //}
            Console.WriteLine("InitTable_FuzzyMembership Finished");
            //conn.Close();
        }

        static bool IsFuzzy_LabelsNull()
        {
            try
            {
                WebRequest URI = WebRequest.Create("http://" + BaseUrl + "api/fuzzy_label");
                URI.Method = "GET";
                using (HttpWebResponse response = (HttpWebResponse)URI.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        // Get the stream containing content returned by the server.  
                        Stream dataStream = response.GetResponseStream();
                        // Open the stream using a StreamReader for easy access.  
                        StreamReader reader = new StreamReader(dataStream);
                        // Read the content.  
                        string responseFromServer = reader.ReadToEnd();
                        if (responseFromServer == "[]")
                        {
                            reader.Close();
                            response.Close();
                            return true;
                        }   
                        else
                        {
                            reader.Close();
                            response.Close();
                            return false;
                        }
                    }
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        static string Post_fuzzu_label(fuzzy_label_created newData) //(4)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    fuzzy_label_created data = new fuzzy_label_created();
                    data.Position = newData.Position;
                    data.BeaconMinor = newData.BeaconMinor;
                    data.Label = newData.Label;
                    string json = JsonConvert.SerializeObject(data);
                    HttpContent contentPost = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage res = client.PostAsync("http://" + BaseUrl + "api/fuzzy_label", contentPost).Result;

                    return res.Content.ReadAsStringAsync().Result.ToString();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        static bool IsFuzzy_MembershipNull()
        {
            try
            {
                WebRequest URI = WebRequest.Create("http://" + BaseUrl + "api/fuzzy_membership");
                URI.Method = "GET";
                using (HttpWebResponse response = (HttpWebResponse)URI.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        // Get the stream containing content returned by the server.  
                        Stream dataStream = response.GetResponseStream();
                        // Open the stream using a StreamReader for easy access.  
                        StreamReader reader = new StreamReader(dataStream);
                        // Read the content.  
                        string responseFromServer = reader.ReadToEnd();
                        if (responseFromServer == "[]")
                        {
                            reader.Close();
                            response.Close();
                            return true;
                        }
                        else
                        {
                            reader.Close();
                            response.Close();
                            return false;
                        }
                    }
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        static string Post_fuzzu_membership(fuzzy_membership_created newData) 
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    fuzzy_membership_created data = new fuzzy_membership_created();
                    data.BeaconMinor = newData.BeaconMinor;
                    data.Label = newData.Label;
                    data.Samples = newData.Samples;
                    data.Max = newData.Min;
                    data.Min = newData.Min;
                    data.Mean = newData.Mean;             
                    data.SD = newData.SD;
                    string json = JsonConvert.SerializeObject(data);
                    HttpContent contentPost = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage res = client.PostAsync("http://" + BaseUrl + "api/fuzzy_membership", contentPost).Result;

                    return res.Content.ReadAsStringAsync().Result.ToString();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
