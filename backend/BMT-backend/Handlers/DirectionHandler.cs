using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using BMT_backend.Models;
using System.Data.SqlClient;

namespace BMT_backend.Handlers
{
    public class DirectionHandler
    {
        private SqlConnection _conection;
        private string _conectionString;

        public DirectionHandler()
        {
            var builder = WebApplication.CreateBuilder();
            _conectionString = builder.Configuration.GetConnectionString("BMTContext");
            _conection = new SqlConnection(_conectionString);
        }

        public SqlConnection Connection { get => _conection; set => _conection = value; }

        private DataTable CreateQueryTable(string query)
        {
            SqlCommand queryCommand = new SqlCommand(query, Connection);
            SqlDataAdapter tableAdapter = new SqlDataAdapter(queryCommand);
            DataTable tableFormatQuery = new DataTable();
            Connection.Open();
            tableAdapter.Fill(tableFormatQuery);
            Connection.Close();
            return tableFormatQuery;
        }

        public List<DirectionModel> GetDirectionsFromUser(UserModel user)
        {
            string query = "SELECT * FROM Directions WHERE Username = @Username";
            List<DirectionModel> directions = new List<DirectionModel>();

            var queryCommand = new SqlCommand(query, _conection);
            SqlDataAdapter tableAdapter = new SqlDataAdapter(queryCommand);
            DataTable tableFormatQuery = new DataTable();
            queryCommand.Parameters.AddWithValue("@Username", user.Username);
            _conection.Open();
            tableAdapter.Fill(tableFormatQuery);
            _conection.Close();

            if (tableFormatQuery.Rows.Count == 0)
            {
                return directions;
            }

            foreach (DataRow row in tableFormatQuery.Rows)
            {
                directions.Add(
                    new DirectionModel
                    {
                        Username = Convert.ToString(row["Username"]),
                        NumDirection = Convert.ToString(row["NumDirection"]),
                        Province = Convert.ToString(row["Province"]),
                        Canton = Convert.ToString(row["Canton"]),
                        District = Convert.ToString(row["District"]),
                        OtherSigns = Convert.ToString(row["OtherSigns"]),
                        Coordinates = Convert.ToString(row["Coordinates"])
                    });
            }

            return directions;
        }

        public bool CreateDirection(DirectionModel direction)
        {
            var query = "INSERT INTO dbo.Directions (Username, NumDirection, Province, Canton, District, OtherSigns, Coordinates) VALUES (@Username, @NumDirection, @Province, @Canton, @District, @OtherSigns, @Coordinates)";
            var querryCommand = new SqlCommand(query, Connection);

            querryCommand.Parameters.AddWithValue("@Username", direction.Username);
            querryCommand.Parameters.AddWithValue("@NumDirection", direction.NumDirection);
            querryCommand.Parameters.AddWithValue("@Province", direction.Province);
            querryCommand.Parameters.AddWithValue("@Canton", direction.Canton);
            querryCommand.Parameters.AddWithValue("@District", direction.District);
            querryCommand.Parameters.AddWithValue("@OtherSigns", direction.OtherSigns);
            querryCommand.Parameters.AddWithValue("@Coordinates", direction.Coordinates);

            Connection.Open();
            var result = querryCommand.ExecuteNonQuery();
            Connection.Close();
            return result > 0;
        }
    }
}