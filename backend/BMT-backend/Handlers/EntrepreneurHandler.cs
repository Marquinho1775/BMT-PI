using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using BMT_backend.Models;
using System.Data.SqlClient;

namespace BMT_backend.Handlers
{
    public class EntrepreneurHandler
    {
        private SqlConnection _conection;
        private string _conectionPath;

        private string getAllEntrepreneurInfo = "select e.Identification, u.Name, u.LastName, u.UserName, u.Email, u.IsVerified " +
            "from Entrepreneurs e " +
            "join Users u on e.UserId = u.Id;";

        public EntrepreneurHandler()
        {
            var builder = WebApplication.CreateBuilder();
            _conectionPath = builder.Configuration.GetConnectionString("BMTContext");
            _conection = new SqlConnection( _conectionPath );
        }

        private DataTable CreateQueryTable (string query)
        {
            SqlCommand queryCommand = new SqlCommand(query, _conection);
            SqlDataAdapter tableAdapter = new SqlDataAdapter(queryCommand);
            DataTable tableFormatQuery = new DataTable();
            _conection.Open();
            tableAdapter.Fill(tableFormatQuery);
            _conection.Close();
            return tableFormatQuery;
        }

        public bool CreateEntrepreneur(EntrepreneurModel entrepreneur)
        {
            // First we get the user id from the user table using the username inside the fronted provided entrepreneur object
            string getUserIdQuery = "select Id from Users where UserName = @UserName;";
            var getUserIdCommand = new SqlCommand(getUserIdQuery, _conection);
            getUserIdCommand.Parameters.AddWithValue("@UserName", entrepreneur.UserName);
            _conection.Open();
            var userId = getUserIdCommand.ExecuteScalar();
            _conection.Close();

            // Now we create the entrepreneur associated with the user id in the Entrepreneurs table
            string createEntrepreneurQuery = "insert into Entrepreneurs (UserId, Identification) " +
                "values (@UserId, @Identification);";
            var createEntrepreneurCommand = new SqlCommand(createEntrepreneurQuery, _conection);
            createEntrepreneurCommand.Parameters.AddWithValue("@UserId", userId);
            createEntrepreneurCommand.Parameters.AddWithValue("@Identification", entrepreneur.Identification);
            _conection.Open();
            bool exit = createEntrepreneurCommand.ExecuteNonQuery() >= 1;
            _conection.Close();
            return exit;
        }

        public List<EntrepreneurModel> getEntrepreneurs() {
            List<EntrepreneurModel> entrepreneurs = new List<EntrepreneurModel>();
            DataTable table = CreateQueryTable(getAllEntrepreneurInfo);
            foreach (DataRow row in table.Rows)
            {
                entrepreneurs.Add(
                    new EntrepreneurModel
                    {
                        Identification = Convert.ToString(row["Identification"]),
                        Name = Convert.ToString(row["Name"]),
                        LastName = Convert.ToString(row["LastName"]),
                        Username = Convert.ToString(row["Username"]),
                        Email = Convert.ToString(row["Email"]),
                        IsVerified = Convert.ToBoolean(row["IsVerified"]),
                    }
                );
            }
            return entrepreneurs;
        }
    }
}
