using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageShare.Data
{
    public class Manager
    {
        private string _connectionString { get; set; }
        public Manager(string connectionString)
        {
            _connectionString = connectionString;
        }
        public IEnumerable<Image> AllImages()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Images";
                List<Image> result = new List<Image>();
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Image image = new Image
                    {
                        Id = (int)reader["Id"],
                        FileName = (string)reader["FileName"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"],
                        ViewCount = (int)reader["ViewCount"],
                        DateUploaded = (DateTime)reader["DateUploaded"]
                    };
                    image.Likes = GetLikesCountForImage(image.Id);
                    result.Add(image);
                }
                return result;
            }
        }
        public IEnumerable<Image> Top5Viewed()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT TOP 5 * FROM Images ORDER BY ViewCount DESC";
                List<Image> result = new List<Image>();
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Image image = new Image
                    {
                        Id = (int)reader["Id"],
                        FileName = (string)reader["FileName"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"],
                        ViewCount = (int)reader["ViewCount"],
                        DateUploaded = (DateTime)reader["DateUploaded"]
                    };
                    image.Likes = GetLikesCountForImage(image.Id);
                    result.Add(image);
                }
                return result;
            }
        }
        public IEnumerable<Image> Top5Date()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT TOP 5 * FROM Images ORDER BY DateUploaded DESC";
                List<Image> result = new List<Image>();
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Image image = new Image
                    {
                        Id = (int)reader["Id"],
                        FileName = (string)reader["FileName"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"],
                        ViewCount = (int)reader["ViewCount"],
                        DateUploaded = (DateTime)reader["DateUploaded"]
                    };
                    image.Likes = GetLikesCountForImage(image.Id);
                    result.Add(image);
                }
                return result;
            }
        }
        public IEnumerable<Image> Top5Liked()
        {
            var allImages = AllImages();
            var result = allImages.OrderByDescending(i => i.Likes);
            return result.Take(5);
        }
        public int GetLikesCountForImage(int ImageId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Count(*) FROM Likes WHERE ImageId = @id";
                command.Parameters.AddWithValue("@id", ImageId);
                connection.Open();
                if(command.ExecuteScalar() == DBNull.Value)
                {
                    return 0;
                }
                return (int)command.ExecuteScalar();
            }
        }
        public User GetByEmail(string email)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Users WHERE Email = @email";
                command.Parameters.AddWithValue("@email", email);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                return (new User
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    Email = (string)reader["Email"],
                    PasswordHash = (string)reader["PasswordHash"],
                    PasswordSalt = (string)reader["PasswordSalt"]
                });
            }
        }
        public int AddImage(Image image)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Images VALUES(@first, @last, @file, @view, @date); SELECT @@IDENTITY";
                command.Parameters.AddWithValue("@first", image.FirstName);
                command.Parameters.AddWithValue("@last", image.LastName);
                command.Parameters.AddWithValue("@file", image.FileName);
                command.Parameters.AddWithValue("@view", image.ViewCount);
                command.Parameters.AddWithValue("@date", image.DateUploaded);
                connection.Open();
                return (int)(decimal)command.ExecuteScalar();
            }
        }
        public void AddUser(User user, string password)
        {
            string salt = PasswordHelper.GenerateSalt();
            string hash = PasswordHelper.HashPassword(password, salt);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Users VALUES(@first, @last, @email, @hash, @salt)";
                command.Parameters.AddWithValue("@first", user.FirstName);
                command.Parameters.AddWithValue("@last", user.LastName);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@hash", hash);
                command.Parameters.AddWithValue("@salt", salt);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public User LogIn(string email, string password)
        {
            User user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }
            bool match = PasswordHelper.PasswordMatch(password, user.PasswordSalt, user.PasswordHash);
            if (!match)
            {
                return null;
            }
            return user;
        }
        public IEnumerable<Image> GetLikesForUser(int UserId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Likes JOIN Images ON Images.Id = Likes.ImageId WHERE UserId = @id";
                command.Parameters.AddWithValue("@id", UserId);
                List<Image> result = new List<Image>();
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Image image = new Image
                    {
                        Id = (int)reader["Id"],
                        FileName = (string)reader["FileName"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"],
                        ViewCount = (int)reader["ViewCount"],
                        DateUploaded = (DateTime)reader["DateUploaded"]
                    };
                    image.Likes = GetLikesCountForImage(image.Id);
                    result.Add(image);
                }
                return result;
            }
        }
        public Image GetImageById(int Id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Images WHERE Id = @id";
                command.Parameters.AddWithValue("@id", Id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                Image image = new Image
                {
                    Id = (int)reader["Id"],
                    FileName = (string)reader["FileName"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    ViewCount = (int)reader["ViewCount"],
                    DateUploaded = (DateTime)reader["DateUploaded"]
                };
                image.Likes = GetLikesCountForImage(image.Id);
                return image;
            }
        }
        public void AddViewCount(int ImageId)
        {
            Image image = GetImageById(ImageId);
            image.ViewCount++;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Images SET ViewCount = @vc WHERE Id = @id";
                command.Parameters.AddWithValue("@vc",image.ViewCount);
                command.Parameters.AddWithValue("@id", ImageId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public void AddLike(Like like)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Likes VALUES(@userId, @imageId)";
                command.Parameters.AddWithValue("@userId", like.UserId);
                command.Parameters.AddWithValue("@imageId", like.ImageId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public bool LikedAlready(Like like)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Likes WHERE UserId = @user AND ImageId = @image";
                command.Parameters.AddWithValue("@user", like.UserId);
                command.Parameters.AddWithValue("@image", like.ImageId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    return false;
                }
                return true;
            }
        }
    }
}
