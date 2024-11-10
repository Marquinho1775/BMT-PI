using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BMT_backend.Domain.Entities;

namespace BMT_backend.Handlers
{
    public class TagHandler
    {
        private readonly SqlConnection _connection;
        private readonly string _connectionString;

        public TagHandler(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("BMTContext");
            _connection = new SqlConnection(_connectionString);
        }

        private DataTable CreateQueryTable(string query)
        {
            SqlCommand queryCommand = new SqlCommand(query, _connection);
            SqlDataAdapter tableAdapter = new SqlDataAdapter(queryCommand);
            DataTable tableFormatQuery = new DataTable();
            _connection.Open();
            tableAdapter.Fill(tableFormatQuery);
            _connection.Close();
            return tableFormatQuery;
        }

        public List<Tag> GetTags()
        {
            List<Tag> tags = new List<Tag>();
            string query = "SELECT * FROM dbo.Tags ";
            DataTable resultTable =
            CreateQueryTable(query);
            foreach (DataRow column in resultTable.Rows)
            {
                tags.Add(
                new Tag
                {
                    Id = Convert.ToString(column["Id"]),
                    Name = Convert.ToString(column["Name"])
                });
            }
            return tags;
        }
        
        public string CreateTag(string tag)
        {
            var query = "INSERT INTO Tags (Name) OUTPUT Inserted.Id VALUES (@Name)";
            using (var command = new SqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@Name", tag);
                _connection.Open();
                var result = command.ExecuteScalar()?.ToString();
                _connection.Close();
                return result;
            }
        }
    }
}
