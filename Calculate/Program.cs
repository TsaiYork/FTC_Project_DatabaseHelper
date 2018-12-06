using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

    class Program
    {
        //設定AP、RP之數量與編號
        static List<int> APs = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        //暫存資料
        static List<int> allData = new List<int>();
        static double mean, entropy, variance, sd;
        static int Max, Min;

        //Database 之連接字串
        //static string connStr = "server=140.122.105.178;user=user;database=ftc_project;port=3306;password=0000;SslMode=none;";
        static string BaseUrl = "140.122.105.187:8080/";

        static void Main(string[] args)
        {
            List<List<int>> RPs = new List<List<int>>();
            List<int> N = new List<int> { };
            List<int> M = new List<int> { };
            List<int> F = new List<int> { };
            RPs.Add(N);
            RPs.Add(M);
            RPs.Add(F);

            List<fuzzy_label> res = new List<fuzzy_label>();
            res = Get_fuzzy_label_N(1);

            foreach (var data in res)
            {
                Console.WriteLine(data.Position);
            }
            Console.ReadKey();
            //MySqlConnection conn = new MySqlConnection(connStr);
            //MySqlCommand command = conn.CreateCommand();

            //foreach (int AP in APs)
            //{
            //    Console.WriteLine();
            //    Console.WriteLine("AP=" + AP.ToString());
            //    try
            //    {
            //        Console.WriteLine("Connecting to MySQL...");
            //        Console.WriteLine();
            //        conn.Open();

            //        //由資料庫選取 Fuzzy Label 為 'N' 的資料
            //        string sqlQuery = "SELECT * FROM `ftc_office_fuzzy_label` WHERE `AP` = " + AP + " AND `Label` LIKE 'N'";
            //        MySqlCommand cmd = new MySqlCommand(sqlQuery, conn);
            //        MySqlDataReader rdr = cmd.ExecuteReader();
            //        if (rdr.HasRows)
            //        {
            //            while (rdr.Read())
            //            {
            //                //Console.WriteLine(rdr[1]+"    "+ rdr[2] + "    " + rdr[3] + "    " );
            //                RPs[0].Add(Convert.ToInt16(rdr[2]));
            //            }
            //        }
            //        rdr.Close();
            //        RPs[0].ForEach(Console.Write);
            //        foreach (int i in RPs[0])
            //        {
            //            sqlQuery = "select * from `ftc_office_data` WHERE `Position` = " + i + " AND `BeaconMinor` = " + AP;
            //            cmd = new MySqlCommand(sqlQuery, conn);
            //            rdr = cmd.ExecuteReader();
            //            if (rdr.HasRows)
            //            {
            //                while (rdr.Read())
            //                {
            //                    if (Convert.ToInt16(rdr[6]) != 999)
            //                    {
            //                        allData.Add(Convert.ToInt16(rdr[6]));
            //                    }
            //                }
            //            }
            //            rdr.Close();
            //        }
            //        updateValue();
            //        Console.WriteLine("  S: " + allData.Count + "  S.D: " + sd + "  Mean: " + mean);
            //        uploadData(command, conn, AP, "N");
            //        dataClear();
            //        RPs[0].Clear();

            //        Console.WriteLine();

            //        //M
            //        sqlQuery = "SELECT * FROM `ftc_office_fuzzy_label` WHERE `AP` = " + AP + " AND `Label` LIKE 'M'";
            //        cmd = new MySqlCommand(sqlQuery, conn);
            //        rdr = cmd.ExecuteReader();
            //        if (rdr.HasRows)
            //        {
            //            while (rdr.Read())
            //            {
            //                //Console.WriteLine(rdr[1] + "    " + rdr[2] + "    " + rdr[3] + "    ");
            //                RPs[1].Add(Convert.ToInt16(rdr[2]));
            //            }
            //        }
            //        rdr.Close();
            //        RPs[1].ForEach(Console.Write);
            //        foreach (int i in RPs[1])
            //        {
            //            sqlQuery = "select * from `ftc_office_data` WHERE `Position` = " + i + " AND `BeaconMinor` = " + AP;
            //            cmd = new MySqlCommand(sqlQuery, conn);
            //            rdr = cmd.ExecuteReader();
            //            if (rdr.HasRows)
            //            {
            //                while (rdr.Read())
            //                {
            //                    if (Convert.ToInt16(rdr[6]) != 999)
            //                    {
            //                        allData.Add(Convert.ToInt16(rdr[6]));
            //                    }
            //                }
            //            }
            //            rdr.Close();
            //        }
            //        updateValue();
            //        Console.WriteLine("  S: " + allData.Count + "  S.D: " + sd + "  Mean: " + mean);
            //        uploadData(command, conn, AP, "M");
            //        dataClear();
            //        RPs[1].Clear();


            //        Console.WriteLine();

            //        //F
            //        sqlQuery = "SELECT * FROM `ftc_office_fuzzy_label` WHERE `AP` = " + AP + " AND `Label` LIKE 'F'";
            //        cmd = new MySqlCommand(sqlQuery, conn);
            //        rdr = cmd.ExecuteReader();
            //        if (rdr.HasRows)
            //        {
            //            while (rdr.Read())
            //            {
            //                //Console.WriteLine(rdr[1] + "    " + rdr[2] + "    " + rdr[3] + "    ");
            //                RPs[2].Add(Convert.ToInt16(rdr[2]));
            //            }
            //        }
            //        rdr.Close();
            //        RPs[2].ForEach(Console.Write);
            //        foreach (int i in RPs[2])
            //        {
            //            sqlQuery = "select * from `ftc_office_data` WHERE `Position` = " + i + " AND `BeaconMinor` = " + AP;
            //            cmd = new MySqlCommand(sqlQuery, conn);
            //            rdr = cmd.ExecuteReader();
            //            if (rdr.HasRows)
            //            {
            //                while (rdr.Read())
            //                {
            //                    if (Convert.ToInt16(rdr[6]) != 999)
            //                    {
            //                        allData.Add(Convert.ToInt16(rdr[6]));
            //                    }
            //                }
            //            }
            //            rdr.Close();
            //        }
            //        updateValue();
            //        Console.WriteLine("  S: " + allData.Count + "  S.D: " + sd + "  Mean: " + mean);
            //        uploadData(command, conn, AP, "F");
            //        dataClear();
            //        RPs[2].Clear();

            //        Console.WriteLine();

            //        //___________________________________________________________________________



            //        Console.WriteLine("Finished, Press Any Key to stop the program");
            //        Console.ReadKey();
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.ToString());
            //        Console.ReadKey();
            //    }
            //    conn.Close();
            //}
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
            string aa = "http://" + BaseUrl + "api/fuzzy_label_N?BeaconMinor=" + BeaconMinor.ToString();
            WebRequest URI = WebRequest.Create(aa);
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

    }
}
