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

        public void VerifyAccount(string Id)
        {
            var query = "UPDATE dbo.Users SET IsVerified = 1 WHERE Id = @Id";
            var sqlCommandForQuery = new SqlCommand(query, Connection);

            sqlCommandForQuery.Parameters.AddWithValue("@Id", Id);

            Connection.Open();
            sqlCommandForQuery.ExecuteNonQuery();
            Connection.Close();
        }

        public List<DevUserModel> GetDevUsers() {
            List<DevUserModel> devUsers = new List<DevUserModel>();
            string query = "SELECT u.Name, u.LastName, u.Email, e.Identification, en.Name AS Enterprise " +
               "FROM Users u " +
               "JOIN Entrepreneurs e ON u.Id = e.UserId " +
               "LEFT JOIN Entrepreneurs_Enterprises ee ON e.Id = ee.EntrepreneurId " +
               "LEFT JOIN Enterprises en ON ee.EnterpriseId = en.Id";
            var queryCommand = new SqlCommand(query, Connection);
            SqlDataAdapter tableAdapter = new SqlDataAdapter(queryCommand);
            DataTable resultTable = new DataTable();
            Connection.Open();
            tableAdapter.Fill(resultTable);
            Connection.Close();

            var usersDictionary = new Dictionary<string, DevUserModel>();

            foreach (DataRow row in resultTable.Rows)
            {
                string userEmail = Convert.ToString(row["Email"]);
                string fullName = $"{Convert.ToString(row["Name"])} {Convert.ToString(row["LastName"])}"; // Concatenamos el nombre y apellido

                if (!usersDictionary.ContainsKey(userEmail))
                {
                    // Crear un nuevo DevUserModel si aún no existe en el diccionario
                    usersDictionary[userEmail] = new DevUserModel
                    {
                        Name = fullName,
                        Email = userEmail,
                        Identification = Convert.ToString(row["Identification"]),
                        AssociatedCompanies = new List<string>().ToArray()
                    };
                }

                // Añadir la empresa a la lista de empresas asociadas
                string enterpriseName = Convert.ToString(row["Enterprise"]);
                if (!string.IsNullOrEmpty(enterpriseName))
                {
                    var associatedCompanies = usersDictionary[userEmail].AssociatedCompanies.ToList();
                    associatedCompanies.Add(enterpriseName);
                    usersDictionary[userEmail].AssociatedCompanies = associatedCompanies.ToArray();
                }
            }
            devUsers = usersDictionary.Values.ToList();

            return devUsers;
        }
    }
}
