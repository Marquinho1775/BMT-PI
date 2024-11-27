using BMT_backend.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;

namespace BMT_backend.Handlers
{
    public class CodeHandler
    {
        private SqlConnection _connection;

        private string _connectionString;

        public CodeHandler()
        {
            var builder = WebApplication.CreateBuilder();
            _connectionString = builder.Configuration.GetConnectionString("BMTContext");
            Connection = new SqlConnection(_connectionString);
        }
        public SqlConnection Connection { get => _connection; set => _connection = value; }

        public string CreateCode(string userId)
        {
            Random random = new Random();
            int code = random.Next(000001, 999999);
            string formattedCode = code.ToString("D6");
            string query = @"
            MERGE INTO Codes AS target
            USING (SELECT @Id AS Id, @Code AS Code) AS source
            ON target.Id = source.Id
            WHEN MATCHED THEN
                UPDATE SET target.Code = source.Code
            WHEN NOT MATCHED THEN
                INSERT (Id, Code)
                VALUES (source.Id, source.Code);
            ";
            var sqlCommandForQuery = new SqlCommand(query, Connection);

            sqlCommandForQuery.Parameters.AddWithValue("@Id", userId);
            sqlCommandForQuery.Parameters.AddWithValue("@Code", formattedCode);

            Connection.Open();
            sqlCommandForQuery.ExecuteNonQuery();
            Connection.Close();

            return formattedCode;
        }

        public string GetCode(string userId)
        {
            string query = "SELECT Code FROM dbo.Codes WHERE Id = @Id";
            var sqlCommandForQuery = new SqlCommand(query, Connection);

            sqlCommandForQuery.Parameters.AddWithValue("@Id", userId);

            Connection.Open();
            var code = sqlCommandForQuery.ExecuteScalar();
            Connection.Close();

            return code.ToString();
        }
    }
}