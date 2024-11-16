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


    }
}