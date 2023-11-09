using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using recipe_in_home.Models;
using System.Data;
using K4os.Compression.LZ4.Internal;

namespace recipe_in_home
{
	public class member_management
	{
		public string ConnectionString { get; set; }
		public member_management(string connectionString)
		{
			ConnectionString = connectionString;
		}
		private MySqlConnection GetConnection()
		{
			return new MySqlConnection(ConnectionString);
		}
		public List<members> Getmember()
		{
			List<members> list = new List<members>();
			string SQL = "SELECT * FROM members";
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand(SQL, conn);
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						list.Add(new members()
						{
							member_name = reader["member_name"].ToString(),
                            member_birth = reader["member_birth"].ToString(),
                            member_id = reader["member_id"].ToString(),
							member_pw = reader["member_pw"].ToString(),
							member_gender = Convert.ToChar(reader["member_gender"]),
							member_job = reader["member_job"].ToString()
						});
					}
				}
				conn.Close();
			}
			return list;
		}
        public members Selectmem(string id)
        {
            members mem = new members();
            string SQL = "SELECT * FROM members WHERE member_id = '" + id + "'";
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(SQL, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        mem.member_id = reader["member_id"].ToString();
                        mem.member_pw = reader["member_pw"].ToString();
                        mem.member_name = reader["member_name"].ToString();
                        mem.member_birth = reader["member_birth"].ToString();
                        mem.member_gender = Convert.ToChar(reader["member_gender"]);
                        mem.member_job = reader["member_job"].ToString();
                    }
                }
                conn.Close();
            }
            return mem;
        }
        public int Updatemem(string member_id, string member_pw,string member_job)
        {
            string SQL = "UPDATE members SET member_pw='" + member_pw + 
                                          "', member_job ='" + member_job +
                                        "'WHERE member_id = '" + @member_id + "'";
            using (MySqlConnection conn = GetConnection())
            {

                try
                { 
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(@SQL, conn);
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        Console.WriteLine("update success!");
                        return 1;

                    }
                    else
                    {
                        Console.WriteLine("update fail");
                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("database 접속 실패");
                    Console.WriteLine(ex.Message);
                }
                conn.Close();
            }
            return 0;
        }
        public int Insertmem(string member_name, string member_birth,string member_id, string member_pw,char member_gender,string member_job)
        {
            string SQL = "INSERT INTO members(member_name, member_birth, member_id, member_pw, member_gender,member_job) " +
                         "VALUES('" + member_name + "','"
                                    + member_birth + "','"
                                    + member_id + "','"
                                    + member_pw + "','"
                                    + member_gender + "','"
                                    + member_job + "')";
            using (MySqlConnection conn = GetConnection())
            {

                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(@SQL, conn);
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        Console.WriteLine("insert success!");
                        return 1;
                    }
                    else
                    {
                        Console.WriteLine("insert fail");
                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("database 접속 실패");
                    Console.WriteLine(ex.Message);
                }
                conn.Close();
            }
            return 0;
        }
        
        public int Deletemem(string member_id)
        {
            string SQL = "delete from members where member_id= '" + member_id + "'";
            using (MySqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(SQL, conn);
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        Console.WriteLine(SQL);
                        Console.WriteLine("삭제 성공");
                        return 1;
                    }
                    else
                    {
                        Console.WriteLine(SQL);
                        Console.WriteLine("삭제 실패");
                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("데이터 베이스 접속 실패");
                    Console.WriteLine(ex.Message);
                }
                conn.Close();
            }
            return 0;
        }

        public int Loginmem(string id, string pw)
        {
            string SQL = "select member_id,member_pw from members where member_id= '" + id + "'";
            using (MySqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(SQL, conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string login_pw = reader.GetString("password");
                            if (login_pw == pw)
                            {
                                Console.WriteLine("로그인 성공");
                                return 1;
                            }
                            else
                            {
                                Console.WriteLine("로그인 실패");
                                return 0;
                            }
                        }
                    }

                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        Console.WriteLine(SQL);
                        Console.WriteLine("삭제 성공");
                        return 1;
                    }
                    else
                    {
                        Console.WriteLine(SQL);
                        Console.WriteLine("삭제 실패");
                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("데이터 베이스 접속 실패");
                    Console.WriteLine(ex.Message);
                }
                conn.Close();
            }
            return 0;
        }
    }
}
