using System.Data;
using System.Data.SqlClient;

namespace BMT_backend.Handlers
{
    public class ImageFileHandler
    {
        private SqlConnection _conection;
        private string _conectionPath;

        public ImageFileHandler()
        {
            var builder = WebApplication.CreateBuilder();
            _conectionPath = builder.Configuration.GetConnectionString("BMTContext");
            _conection = new SqlConnection(_conectionPath);
        }

        private DataTable CreateQueryTable(string query)
        {
            SqlCommand queryCommand = new SqlCommand(query, _conection);
            SqlDataAdapter tableAdapter = new SqlDataAdapter(queryCommand);
            DataTable tableFormatQuery = new DataTable();
            _conection.Open();
            tableAdapter.Fill(tableFormatQuery);
            _conection.Close();
            return tableFormatQuery;
        }

        public void SaveImage(string ownerId, string ownerType, string relativePath)
        {
            string createImageQuery = "";
            string parameterName = "";
            if (ownerType == "Product")
            {
                createImageQuery = "INSERT INTO ProductImages(ProductId, URL) VALUES(@OwnerId, @URL);";
                parameterName = "@OwnerId";
            }
            else if (ownerType == "User")
            {
                createImageQuery = "UPDATE Users SET ProfilePictureURL = @URL WHERE Id = @OwnerId;";
                parameterName = "@OwnerId";
            }
            var createImageCommand = new SqlCommand(createImageQuery, _conection);
            createImageCommand.Parameters.Add(parameterName, SqlDbType.UniqueIdentifier).Value = Guid.Parse(ownerId);
            createImageCommand.Parameters.Add("@URL", SqlDbType.VarChar, 255).Value = relativePath;
            _conection.Open();
            createImageCommand.ExecuteNonQuery();
            _conection.Close();
        }

        public List<string> GetImage(string ownerId)
        {
            List<string> images = new List<string>();
            string getImageQuery = "select URL from Images where OwnerId = @OwnerId";
            var getImageCommand = new SqlCommand(getImageQuery, _conection);
            getImageCommand.Parameters.AddWithValue("@OwnerId", ownerId);
            _conection.Open();
            DataTable dataTable = CreateQueryTable(getImageQuery);
            _conection.Close();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                images.Add(dataTable.Rows[i]["URL"].ToString());
            }
            return images;
        }
    }
}
