using recipe_in_home.Models;
using Google.Protobuf.Collections;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Transactions;

namespace recipe_in_home
{
    public class Csharp_Post_services
    {
        public string ConnectionString { get; set; }

        public Csharp_Post_services(string connectionString)
        {
            ConnectionString = connectionString;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public List<Post> Getpost()
        {
            List<Post> list = new List<Post>();
            string SQL = "SELECT * FROM posts";
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(SQL, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Post post = new Post()
                        {
                            Postid = Convert.ToInt32(reader["member_id"]),
                            // member_id = Convert.ToInt32(reader["member_id"]),
                            member_name = reader["member_name"].ToString(),
                            Title = reader["Title"].ToString(),
                            Content = reader["Content"].ToString(),
                            CreatedAt = reader["CreatedAt"].ToString()

                        };

                        if (!DBNull.Value.Equals(reader["ImagePath"])) // 이미지 경로 필드로 변경
                        {
                            post.ImagePath = reader["ImagePath"].ToString(); // ImagePath를 읽어옴
                        }


                        list.Add(post);
                    }
                }
                conn.Close();
            }
            return list;
        }
        public Post SelectPost(int Postid)
        {
            Post post = null;

            string SQL = "SELECT * FROM posts WHERE member_id = @Postid";
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(SQL, conn);
                cmd.Parameters.AddWithValue("@Postid", Postid);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        post = new Post()
                        {
                            Postid = Convert.ToInt32(reader["member_id"]),
                            // member_id = Convert.ToInt32(reader["member_id"]),
                            member_name = reader["member_name"].ToString(),
                            Title = reader["Title"].ToString(),
                            Content = reader["Content"].ToString(),
                            CreatedAt = reader["CreatedAt"].ToString()
                        };

                        // ImagePath 필드 값을 가져옴
                        if (!reader.IsDBNull(reader.GetOrdinal("ImagePath")))
                        {
                            post.ImagePath = reader["ImagePath"].ToString();
                        }
                    }
                }
            }

            return post;
        }


        public int InsertPost(string member_name, string Title, string Content, string ImagePath, byte[] ImageData)
        {
            string SQL = "INSERT INTO posts (member_name, Title, Content, ImagePath, ImageData,CreatedAt) VALUES (@member_name, @Title, @Content, @ImagePath, @ImageData,NOW())";
            using (MySqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(SQL, conn);
                    cmd.Parameters.AddWithValue("@member_name", member_name);
                    cmd.Parameters.AddWithValue("@Title", Title);
                    cmd.Parameters.AddWithValue("@Content", Content);
                    cmd.Parameters.AddWithValue("@ImagePath", ImagePath);
                    cmd.Parameters.AddWithValue("@ImageData", ImageData);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));

                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        Console.WriteLine("삽입 성공");
                        return 1;
                    }
                    else
                    {
                        Console.WriteLine("실패");
                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DB 연결 실패");
                    Console.WriteLine(ex.Message);
                }
                conn.Close();
            }
            return 0;
        }


        public int UpdatePost(int Postid, string member_name, string Title, string Content, string ImagePath)
        {
            string SQL = "UPDATE posts SET member_name = @member_name, Title = @Title, Content = @Content, ImagePath = @ImagePath, CreatedAt = NOW() WHERE Postid = @Postid";
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(SQL, conn);
                        cmd.Transaction = transaction;
                        cmd.Parameters.AddWithValue("@Postid", Postid);
                        cmd.Parameters.AddWithValue("@member_name", member_name);
                        cmd.Parameters.AddWithValue("@Title", Title);
                        cmd.Parameters.AddWithValue("@Content", Content);
                        cmd.Parameters.AddWithValue("@ImagePath", ImagePath);
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));

                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            Console.WriteLine("수정 성공");
                            transaction.Commit();
                            return 1;
                        }
                        else
                        {
                            Console.WriteLine("수정 실패");
                            transaction.Rollback();
                            return 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("데이터베이스 작업 실패");
                        Console.WriteLine(ex.ToString());
                        transaction.Rollback();
                    }
                }
                conn.Close();
            }
            return 0;
        }


        public void DeletePost(int Postid)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string query = "DELETE FROM posts WHERE member_id = @Postid";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Postid", Postid);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        
                        Console.WriteLine("삭제 중 오류 발생: " + ex.Message);
                       
                        throw; 
                    }
                }
            }
        }
        public Post GetPostById(int Postid)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                string SQL = "SELECT * FROM posts WHERE member_id = @Postid";
                MySqlCommand cmd = new MySqlCommand(SQL, conn);
                cmd.Parameters.AddWithValue("@Postid", Postid);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Post post = new Post
                        {
                            member_name = reader.IsDBNull(reader.GetOrdinal("member_name")) ? string.Empty : reader["member_name"].ToString(),
                            Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? string.Empty : reader["Title"].ToString(),
                            Content = reader.IsDBNull(reader.GetOrdinal("Content")) ? string.Empty : reader["Content"].ToString(),
                            Postid = Convert.ToInt32(reader["member_id"]), // "member_id" 컬럼으로 수정
                        };
                        return post;
                    }
                }
            }
            return null;
        }
        public int UpdatePostWithImagePathOrNoImage(int postId, string memberName, string title, string content, string imagePath, byte[] imageData)
        {
            string SQL;
            MySqlConnection conn = GetConnection();

            try
            {
                conn.Open();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    if (imagePath != null)
                    {
                        // 이미지 경로와 이미지 데이터를 모두 업데이트
                        SQL = "UPDATE post SET member_name = @memberName, title = @title, content = @content, image_path = @imagePath, image_data = @imageData WHERE Postid = @postId";
                        cmd.Parameters.AddWithValue("@imagePath", imagePath);
                        cmd.Parameters.AddWithValue("@imageData", imageData);
                    }
                    else
                    {
                        // 이미지 데이터를 업데이트하지 않음
                        SQL = "UPDATE post SET member_name = @memberName, title = @title, content = @content WHERE Postid = @postId";
                    }

                    cmd.CommandText = SQL;
                    cmd.Parameters.AddWithValue("@postId", postId);
                    cmd.Parameters.AddWithValue("@memberName", memberName);
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@content", content);

                    int result = cmd.ExecuteNonQuery();
                    return result;
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("데이터베이스 업데이트 실패");
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                conn.Close();
            }

            return 0;
        }



    }

}


