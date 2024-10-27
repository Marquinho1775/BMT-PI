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
            string query = "SELECT Username, NumDirection, OtherSigns, Coordinates FROM Directions WHERE Username = @Username";
            List<DirectionModel> directions = new List<DirectionModel>();

            using (var queryCommand = new SqlCommand(query, _conection))
            {
                queryCommand.Parameters.AddWithValue("@Username", user.Username);
                var tableAdapter = new SqlDataAdapter(queryCommand);
                var tableFormatQuery = new DataTable();
                _conection.Open();
                tableAdapter.Fill(tableFormatQuery);
                _conection.Close();

                foreach (DataRow row in tableFormatQuery.Rows)
                {
                    directions.Add(new DirectionModel
                    {
                        Username = Convert.ToString(row["Username"]),
                        NumDirection = Convert.ToString(row["NumDirection"]),
                        OtherSigns = row["OtherSigns"] == DBNull.Value ? null : Convert.ToString(row["OtherSigns"]),
                        Coordinates = Convert.ToString(row["Coordinates"])
                    });
                }
            }

            return directions;
        }

        public bool CreateDirection(DirectionModel direction)
        {
            var query = "INSERT INTO dbo.Directions (Username, NumDirection, OtherSigns, Coordinates) VALUES (@Username, @NumDirection, @OtherSigns, @Coordinates)";
            using (var queryCommand = new SqlCommand(query, Connection))
            {
                queryCommand.Parameters.AddWithValue("@Username", direction.Username);
                queryCommand.Parameters.AddWithValue("@NumDirection", direction.NumDirection);
                queryCommand.Parameters.AddWithValue("@OtherSigns", (object)direction.OtherSigns ?? DBNull.Value);
                queryCommand.Parameters.AddWithValue("@Coordinates", direction.Coordinates);

                Connection.Open();
                var result = queryCommand.ExecuteNonQuery();
                Connection.Close();
                return result > 0;
            }
        }
    }
}