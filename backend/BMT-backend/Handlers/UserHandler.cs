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

        private DataTable CrearTablaConsulta(string query)
        {
            SqlCommand comandoParaConsulta = new SqlCommand(query, Connection);
            SqlDataAdapter adaptadorParaTabla = new SqlDataAdapter(comandoParaConsulta);
            DataTable consultaFormatoTabla = new DataTable();
            Connection.Open();
            adaptadorParaTabla.Fill(consultaFormatoTabla);
            Connection.Close();
            return consultaFormatoTabla;
        }

        public List<UserModel> GetUsers()
        {
            List<UserModel> users = new List<UserModel>();
            string consulta = "SELECT * FROM dbo.Users ";
            DataTable tablaResultado =
            CrearTablaConsulta(consulta);
            foreach (DataRow columna in tablaResultado.Rows)
            {
                users.Add(
                new UserModel
                {
                    Name = Convert.ToString(columna["Name"]),
                    Username = Convert.ToString(columna["Username"]),
                    Email = Convert.ToString(columna["Email"]),
                    IsVerified = Convert.ToBoolean(columna["IsVerified"]),
                    Password = Convert.ToString(columna["Password"]),
                    IsEntrepeneur = Convert.ToBoolean(columna["IsEntrepeneur"]),
                    Identification = Convert.ToString(columna["Identification"])

                });
            }
            return users;
        }

        public bool CreateUser(UserModel user)
        {
            var query = "INSERT INTO dbo.Users (Name, Identification, Username, Email, IsVerified, Password, IsEntrepeneur) VALUES (@Name, @Identification, @Username, @Email, @IsVerified, @Password, @IsEntrepeneur)";
            var comandoParaConsulta = new SqlCommand(query, Connection);

            comandoParaConsulta.Parameters.AddWithValue("@Name", user.Name);
            comandoParaConsulta.Parameters.AddWithValue("@Identification", user.Identification);
            comandoParaConsulta.Parameters.AddWithValue("@Username", user.Username);
            comandoParaConsulta.Parameters.AddWithValue("@Email", user.Email);
            comandoParaConsulta.Parameters.AddWithValue("@IsVerified", user.IsVerified);
            comandoParaConsulta.Parameters.AddWithValue("@Password", user.Password);
            comandoParaConsulta.Parameters.AddWithValue("@IsEntrepeneur", user.IsEntrepeneur);

            Connection.Open();
            bool result = comandoParaConsulta.ExecuteNonQuery() >= 1;
            Connection.Close();

            return result;

        }
    }
}
