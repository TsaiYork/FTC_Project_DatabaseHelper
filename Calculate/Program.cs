using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace Calculate
{
    public partial class fuzzy_label
    {
        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "BeaconMinor")]
        public int BeaconMinor { get; set; }

        [JsonProperty(PropertyName = "Position")]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "Label")]
        public string Label { get; set; }
    }

    public class data_all
    {
        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Position")]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "BeaconMinor")]
        public int BeaconMinor { get; set; }

        [JsonProperty(PropertyName = "RSS")]
        public double RSS { get; set; }
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
        static List<int> APs = new List<int> { 1, 2 };

        //暫存資料
        static List<double> allData = new List<double>();
        static double mean, entropy, variance, sd;
        static double Max, Min;

        //Database 之連接字串
        //static string connStr = "server=140.122.105.178;user=user;database=ftc_project;port=3306;password=0000;SslMode=none;";
        static string BaseUrl = "localhost:51346/";

        static void Main(string[] args)
        {
            List<List<int>> RPs = new List<List<int>>();
            List<int> N = new List<int> { };
            List<int> M = new List<int> { };
            List<int> F = new List<int> { };
            RPs.Add(N);     //RPs[0] is Near Set
            RPs.Add(M);     //RPs[1] is Medium Set
            RPs.Add(F);     //RPs[2] is Far Set

            List<data_all> DataRes = new List<data_all>();
            List<fuzzy_label> LabelRes = new List<fuzzy_label>();
            fuzzy_membership_created uploadData = new fuzzy_membership_created();

            foreach (int AP in APs)
            {
                //N
                LabelRes.Clear();
                LabelRes = Get_fuzzy_label_N(AP);
                foreach (var data in LabelRes)
                {
                    RPs[0].Add(data.Position);
                }
                //RPs[0].ForEach(Console.Write);

                foreach (int Position in RPs[0])
                {
                    DataRes.Clear();
                    DataRes = Get_data_all_PosMinor(Position, AP);
                    foreach (var data in DataRes)
                    {
                        if (data.RSS <= 0)
                            allData.Add(data.RSS);
                    }
                }
                updateValue();
                Console.WriteLine("AP:" + AP.ToString() + "   N");
                Console.WriteLine("  S: " + allData.Count + "  S.D: " + sd + "  Mean: " + mean);

                uploadData.BeaconMinor = AP;
                uploadData.Label = "N";
                uploadData.Samples = allData.Count;
                uploadData.Max = (int)Max;
                uploadData.Min = (int)Min;
                uploadData.Mean = mean;
                uploadData.SD = sd;

                UploadMembership(uploadData);
              
                dataClear();
                RPs[0].Clear();

                Console.WriteLine();

                //M
                LabelRes.Clear();
                LabelRes = Get_fuzzy_label_M(AP);
                foreach (var data in LabelRes)
                {
                    RPs[1].Add(data.Position);
                }
                //RPs[1].ForEach(Console.Write);

                foreach (int Position in RPs[1])
                {
                    DataRes.Clear();
                    DataRes = Get_data_all_PosMinor(Position, AP);
                    foreach (var data in DataRes)
                    {
                        if (data.RSS <= 0)
                            allData.Add(data.RSS);
                    }
                }
                updateValue();
                Console.WriteLine("AP:" + AP.ToString() + "   M");
                Console.WriteLine("  S: " + allData.Count + "  S.D: " + sd + "  Mean: " + mean);

                uploadData.BeaconMinor = AP;
                uploadData.Label = "M";
                uploadData.Samples = allData.Count;
                uploadData.Max = (int)Max;
                uploadData.Min = (int)Min;
                uploadData.Mean = mean;
                uploadData.SD = sd;

                UploadMembership(uploadData);

                dataClear();
                RPs[1].Clear();

                Console.WriteLine();

                //F
                LabelRes.Clear();
                LabelRes = Get_fuzzy_label_F(AP);
                foreach (var data in LabelRes)
                {
                    RPs[2].Add(data.Position);
                }
                RPs[2].ForEach(Console.Write);

                foreach (int Position in RPs[2])
                {
                    DataRes.Clear();
                    DataRes = Get_data_all_PosMinor(Position, AP);
                    foreach (var data in DataRes)
                    {
                        if(data.RSS<=0)
                            allData.Add(data.RSS);
                    }
                }
                updateValue();
                Console.WriteLine("AP:" + AP.ToString() + "   F");
                Console.WriteLine("  S: " + allData.Count + "  S.D: " + sd + "  Mean: " + mean);

                uploadData.BeaconMinor = AP;
                uploadData.Label = "F";
                uploadData.Samples = allData.Count;
                uploadData.Max = (int)Max;
                uploadData.Min = (int)Min;
                uploadData.Mean = mean;
                uploadData.SD = sd;

                UploadMembership(uploadData);

                dataClear();
                RPs[2].Clear();

                Console.WriteLine();
            }

            Console.WriteLine("Finished, Press Any Key to stop the program");
            Console.ReadKey();  
        }

        static void updateValue()
        {
            if (allData.Count != 0)
            {
                //Max,Min
                Max = allData.Max();
                Min = allData.Min();

                //Mean

                mean = allData.Average();

                //variance
                double total = 0;
                foreach (var rss in allData)
                {
                    total += Math.Pow(((double)rss - mean), 2);
                    variance = total / allData.Count;
                }

                //Standard Deviation
                sd = Math.Sqrt(variance);

                ////entropy
                double P = 0;
                int count = 0;
                for (var x = Min; x <= Max; x++)
                {
                    P = 0;
                    count = allData.Count(k => k == x);
                    if (count > 0)
                    {
                        P = Convert.ToDouble(count) / Convert.ToDouble(allData.Count);
                        entropy = entropy + (-1) * P * Math.Log(P, 2);
                    }
                }
            }
        }

        static void uploadData(MySqlCommand cmd, MySqlConnection conn, int AP, string label)
        {
            string UpdateQuery = "UPDATE `fuzzy_membership_function` SET `Samples`=@sample, `Max` = @max, `Min` = @min, `Mean` = @mean, `S.D.` = @sd WHERE `fuzzy_membership_function`.`AP` =" + AP + " AND `fuzzy_membership_function`.`Label` = '" + label + "'";

            cmd = new MySqlCommand(UpdateQuery, conn);
            cmd.Parameters.AddWithValue("@sample", allData.Count);
            cmd.Parameters.AddWithValue("@max", Max);
            cmd.Parameters.AddWithValue("@min", Min);
            cmd.Parameters.AddWithValue("@mean", Math.Round(mean, 3));
            cmd.Parameters.AddWithValue("@sd", Math.Round(sd, 3));
            cmd.ExecuteNonQuery();
        }

        static void dataClear()
        {
            allData.Clear();
            
            mean = 0;
            sd = 0;
            Max = 0;
            Min = 0;
        }

        static List<fuzzy_label> Get_fuzzy_label_N(int BeaconMinor)
        {
            List<fuzzy_label> fuzzy_label = new List<fuzzy_label>();
            string url = "http://" + BaseUrl + "api/fuzzy_label_N?BeaconMinor=" + BeaconMinor.ToString();
            WebRequest URI = WebRequest.Create(url);
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

                    fuzzy_label = Newtonsoft.Json.JsonConvert.DeserializeObject<List<fuzzy_label>>(responseFromServer);

                    // Clean up the streams and the response.  
                    reader.Close();
                    response.Close();
                    return fuzzy_label;
                }
                else
                {
                    return null;
                }
            }
        }

        static List<fuzzy_label> Get_fuzzy_label_M(int BeaconMinor)
        {
            List<fuzzy_label> fuzzy_label = new List<fuzzy_label>();
            string url = "http://" + BaseUrl + "api/fuzzy_label_M?BeaconMinor=" + BeaconMinor.ToString();
            WebRequest URI = WebRequest.Create(url);
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

                    fuzzy_label = Newtonsoft.Json.JsonConvert.DeserializeObject<List<fuzzy_label>>(responseFromServer);

                    // Clean up the streams and the response.  
                    reader.Close();
                    response.Close();
                    return fuzzy_label;
                }
                else
                {
                    return null;
                }
            }
        }

        static List<fuzzy_label> Get_fuzzy_label_F(int BeaconMinor)
        {
            List<fuzzy_label> fuzzy_label = new List<fuzzy_label>();
            string url = "http://" + BaseUrl + "api/fuzzy_label_F?BeaconMinor=" + BeaconMinor.ToString();
            WebRequest URI = WebRequest.Create(url);
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

                    fuzzy_label = Newtonsoft.Json.JsonConvert.DeserializeObject<List<fuzzy_label>>(responseFromServer);

                    // Clean up the streams and the response.  
                    reader.Close();
                    response.Close();
                    return fuzzy_label;
                }
                else
                {
                    return null;
                }
            }
        }

        static List<data_all> Get_data_all_PosMinor(int Position,int BeaconMinor)
        {
            List<data_all> fuzzy_label = new List<data_all>();
            string url = "http://" + BaseUrl + "api/data_all?Position="+Position.ToString()+"&&BeaconMinor=" + BeaconMinor.ToString();
            WebRequest URI = WebRequest.Create(url);
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

                    fuzzy_label = Newtonsoft.Json.JsonConvert.DeserializeObject<List<data_all>>(responseFromServer);

                    // Clean up the streams and the response.  
                    reader.Close();
                    response.Close();
                    return fuzzy_label;
                }
                else
                {
                    return null;
                }
            }
        }

        static string UploadMembership(fuzzy_membership_created newData)
        {

            try
            {
                using (HttpClient client = new HttpClient())
                {

                    string json = JsonConvert.SerializeObject(newData);
                    HttpContent contentPost = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage res = client.PutAsync("http://" + BaseUrl + "api/fuzzy_membership", contentPost).Result;

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
