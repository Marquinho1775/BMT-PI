using BMT_backend.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;

namespace BMT_backend.Handlers
{
    public class UserHandler
    {
        private SqlConnection _connection;

        private string _connectionString;

        public UserHandler()
        {
            var builder = WebApplication.CreateBuilder();
            _connectionString = builder.Configuration.GetConnectionString("BMTContext");
            Connection = new SqlConnection(_connectionString);
        }

        public SqlConnection Connection { get => _connection; set => _connection = value; }

        private DataTable CreateQuerryTable(string query)
        {
            SqlCommand queryCommand = new SqlCommand(query, Connection);
            SqlDataAdapter tableAdapter = new SqlDataAdapter(queryCommand);
            DataTable tableFormatQuery = new DataTable();
            Connection.Open();
            tableAdapter.Fill(tableFormatQuery);
            Connection.Close();
            return tableFormatQuery;
        }

        public List<UserModel> GetUsers()
        {
            List<UserModel> users = new List<UserModel>();
            string query = "SELECT * FROM dbo.Users ";
            DataTable resultTable =
            CreateQuerryTable(query);
            foreach (DataRow column in resultTable.Rows)
            {
                users.Add(
                new UserModel
                {
                    Id = Convert.ToString(column["Id"]),
                    Name = Convert.ToString(column["Name"]),
                    LastName = Convert.ToString(column["LastName"]),
                    Username = Convert.ToString(column["Username"]),
                    Email = Convert.ToString(column["Email"]),
                    IsVerified = Convert.ToBoolean(column["IsVerified"]),
                    Password = Convert.ToString(column["Password"])

                });
            }
            return users;
        }

        public bool CreateUser(UserModel user)
        {
            var query = "INSERT INTO dbo.Users (Name, LastName, Username, Email, IsVerified, Password) VALUES (@Name, @LastName, @Username, @Email, @IsVerified, @Password)";
            var querryCommand = new SqlCommand(query, Connection);

            querryCommand.Parameters.AddWithValue("@Name", user.Name);
            querryCommand.Parameters.AddWithValue("@LastName", user.LastName);
            querryCommand.Parameters.AddWithValue("@Username", user.Username);
            querryCommand.Parameters.AddWithValue("@Email", user.Email);
            querryCommand.Parameters.AddWithValue("@IsVerified", user.IsVerified);
            querryCommand.Parameters.AddWithValue("@Password", user.Password);

            Connection.Open();
            bool result = querryCommand.ExecuteNonQuery() >= 1;
            Connection.Close();

            return result;
        }
    }
}