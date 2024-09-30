using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using BMT_backend.Models;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace BMT_backend.Handlers
{
    public class ImageUploadHandler
    {
        private SqlConnection _conection;
        private string _conectionPath;

        public ImageUploadHandler()
        {
            var builder = WebApplication.CreateBuilder();
            _conectionPath = builder.Configuration.GetConnectionString("BMTContext");
            _conection = new SqlConnection(_conectionPath);
        }

        public void SaveImage(string ownerId, string relativePath)
        {
            string createImageQuery = "insert into Images (OwnerId, URL) " +
                "values(@OwnerId, @URL);";
            var createImageCommand = new SqlCommand(createImageQuery, _conection);
            createImageCommand.Parameters.AddWithValue("@OwnerId", ownerId);
            createImageCommand.Parameters.AddWithValue("@URL", relativePath);
            _conection.Open();
            createImageCommand.ExecuteNonQuery();
            _conection.Close();
        }
    }
}
