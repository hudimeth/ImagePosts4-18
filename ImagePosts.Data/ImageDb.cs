using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagePosts.Data
{
    public class ImageDb
    {
        private string _connectionString;
        public ImageDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int AddImage(string imageName, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Images (ImageName, Password, Views)" +
                                    "VALUES (@imageName, @password,0) SELECT SCOPE_IDENTITY()";
            command.Parameters.AddWithValue("@imageName", imageName);
            command.Parameters.AddWithValue("@password", password);
            connection.Open();
            return (int)(decimal)command.ExecuteScalar();
        }
        public Image GetImageById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Images WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }
            return new Image
            {
                Id = (int)reader["Id"],
                ImageName = (string)reader["ImageName"],
                Password = (string)reader["Password"],
                Views = (int)reader["Views"]
            };
        }
        public void UpdateViews(Image image)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "UPDATE Images SET Views = Views + 1 WHERE Id = @id";
            command.Parameters.AddWithValue("@id", image.Id);
            connection.Open();
            var reader = command.ExecuteReader();
        }
    }
}
