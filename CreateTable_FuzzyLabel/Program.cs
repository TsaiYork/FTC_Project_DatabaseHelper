using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace CreateTable_FuzzyLabel
{
    class Program
    {
        //設定AP、RP之數量與編號
        static List<int> APs = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        static List<int> RPs = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

        //Database 之連接字串
        static string connStr = "server=140.122.105.178;user=user;database=ftc_project;port=3306;password=0000;SslMode=none;";

        static void Main(string[] args)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            MySqlCommand command = conn.CreateCommand();

            InitTable_FuzzyLabel(command, conn);
            InitTable_FuzzyMembership(command, conn);
            Console.ReadKey();
        }

        static void InitTable_FuzzyLabel(MySqlCommand cmd, MySqlConnection conn)
        {
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                Console.WriteLine();
                conn.Open();
                string SqlQuery = "SELECT * FROM `ftc_office_fuzzy_label`";
                cmd = new MySqlCommand(SqlQuery, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (!rdr.HasRows)
                {
                    foreach (int AP in APs)
                    {
                        foreach (int RP in RPs)
                        {
                            //Console.WriteLine(AP.ToString() + ",   " + RP.ToString());
                            SqlQuery = "INSERT INTO `ftc_office_fuzzy_label` (`ID`, `AP`, `RP`, `Label`) VALUES (NULL, '" + AP + "', '" + RP + "', 'U')";
                            cmd = new MySqlCommand(SqlQuery, conn);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
            Console.WriteLine("InitTable_FuzzyLabel Finished");
            conn.Close();
        }

        static void InitTable_FuzzyMembership(MySqlCommand cmd, MySqlConnection conn)
        {
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                Console.WriteLine();
                conn.Open();

                string SqlQuery = "SELECT * FROM `ftc_office_fuzzy_membership`";
                cmd = new MySqlCommand(SqlQuery, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows != true)
                {
                    rdr.Close();
                    foreach (int AP in APs)
                    {
                        SqlQuery = "INSERT INTO `ftc_office_fuzzy_membership` (`ID`, `AP`, `Label`, `Samples`, `Max`, `Min`, `Mean`, `S.D.`) VALUES (NULL, '" + AP + "', 'N', '', '', '', '', '');";
                        cmd = new MySqlCommand(SqlQuery, conn);
                        cmd.ExecuteNonQuery();
                        SqlQuery = "INSERT INTO `ftc_office_fuzzy_membership` (`ID`, `AP`, `Label`, `Samples`, `Max`, `Min`, `Mean`, `S.D.`) VALUES (NULL, '" + AP + "', 'M', '', '', '', '', '');";
                        cmd = new MySqlCommand(SqlQuery, conn);
                        cmd.ExecuteNonQuery();
                        SqlQuery = "INSERT INTO `ftc_office_fuzzy_membership` (`ID`, `AP`, `Label`, `Samples`, `Max`, `Min`, `Mean`, `S.D.`) VALUES (NULL, '" + AP + "', 'F', '', '', '', '', '');";
                        cmd = new MySqlCommand(SqlQuery, conn);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
            Console.WriteLine("InitTable_FuzzyMembership Finished");
            conn.Close();
        }
    }
}
