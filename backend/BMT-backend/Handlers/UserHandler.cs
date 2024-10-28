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

        private DataTable CreateQueryTable(string query, SqlParameter[] parameters = null)
        {
            DataTable tableFormatQuery = new DataTable();

            try
            {
                using (SqlCommand queryCommand = new SqlCommand(query, Connection))
                {
                    if (parameters != null)
                    {
                        queryCommand.Parameters.AddRange(parameters);
                    }

                    using (SqlDataAdapter tableAdapter = new SqlDataAdapter(queryCommand))
                    {
                        Connection.Open();
                        tableAdapter.Fill(tableFormatQuery);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateQueryTable: {ex.Message}");
            }
            finally
            {
                Connection.Close();
            }

            return tableFormatQuery;
        }


        public List<UserModel> GetUsers()
        {
            List<UserModel> users = new List<UserModel>();
            string query = "SELECT * FROM dbo.Users ";
            DataTable resultTable =
            CreateQueryTable(query);
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
                    Password = Convert.ToString(column["Password"]),
                    Role = Convert.ToString(column["Role"]),
                    ProfilePictureURL = Convert.ToString(column["ProfilePictureURL"])
                });
            }
            return users;
        }

        public UserModel GetUserById(string userId)
        {
            string query = "SELECT * FROM dbo.Users WHERE Id = @Id";
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@Id", userId)
            };

            DataTable resultTable = CreateQueryTable(query, parameters);

            if (resultTable.Rows.Count > 0)
            {
                DataRow row = resultTable.Rows[0];
                return new UserModel
                {
                    Id = Convert.ToString(row["Id"]),
                    Name = Convert.ToString(row["Name"]),
                    LastName = Convert.ToString(row["LastName"]),
                    Username = Convert.ToString(row["Username"]),
                    Email = Convert.ToString(row["Email"]),
                    IsVerified = Convert.ToBoolean(row["IsVerified"]),
                    Password = Convert.ToString(row["Password"]),
                    Role = Convert.ToString(row["Role"])
                };
            }
            return null; 
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

        public bool UpdateRole(string Id, string Role)
        {
            var query = "UPDATE dbo.Users SET Role = @Role WHERE Id = @Id";
            var sqlCommandForQuery = new SqlCommand(query, Connection);

            sqlCommandForQuery.Parameters.AddWithValue("@Role", Role);
            sqlCommandForQuery.Parameters.AddWithValue("@Id", Id);

            Connection.Open();
            bool result = sqlCommandForQuery.ExecuteNonQuery() >= 1;
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

        public List<DevUserModel> GetDevUsers()
        {
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
                    usersDictionary[userEmail] = new DevUserModel
                    {
                        Name = fullName,
                        Email = userEmail,
                        Identification = Convert.ToString(row["Identification"]),
                        AssociatedCompanies = new List<string>().ToArray()
                    };
                }

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

        public bool UpdateUserProfile(UpdateUserProfileModel updatedUser)
        {
            var fieldsToUpdate = new List<string>();

            if (!string.IsNullOrEmpty(updatedUser.Username))
            {
                fieldsToUpdate.Add("Username = @Username");
            }
            if (!string.IsNullOrEmpty(updatedUser.Name))
            {
                fieldsToUpdate.Add("Name = @Name");
            }
            if (!string.IsNullOrEmpty(updatedUser.LastName))
            {
                fieldsToUpdate.Add("LastName = @LastName");
            }
            if (!string.IsNullOrEmpty(updatedUser.Password))
            {
                fieldsToUpdate.Add("Password = @Password");
            }

            if (fieldsToUpdate.Count == 0)
            {
                throw new ArgumentException("No hay campos válidos para actualizar.");
            }

            var updateQuery = $"UPDATE dbo.Users SET {string.Join(", ", fieldsToUpdate)} WHERE Id = @Id";

            using (var updateCommand = new SqlCommand(updateQuery, Connection))
            {
                updateCommand.Parameters.AddWithValue("@Id", updatedUser.Id);

                if (!string.IsNullOrEmpty(updatedUser.Username))
                    updateCommand.Parameters.AddWithValue("@Username", updatedUser.Username);

                if (!string.IsNullOrEmpty(updatedUser.Name))
                    updateCommand.Parameters.AddWithValue("@Name", updatedUser.Name);

                if (!string.IsNullOrEmpty(updatedUser.LastName))
                    updateCommand.Parameters.AddWithValue("@LastName", updatedUser.LastName);

                if (!string.IsNullOrEmpty(updatedUser.Password))
                    updateCommand.Parameters.AddWithValue("@Password", updatedUser.Password);

                Connection.Open();
                bool result = updateCommand.ExecuteNonQuery() >= 1;
                Connection.Close();

                return result;
            }
        }


    }
}
