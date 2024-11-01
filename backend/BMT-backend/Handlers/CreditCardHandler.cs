using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using BMT_backend.Models;
using BMT_backend.Controllers;
using System.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data.Common;

namespace BMT_backend.Handlers
{
    public class CreditCardHandler
    {
        private SqlConnection _conection;
        private string _conectionPath;

        public SqlConnection Connection { get => _conection; set => _conection = value; }

        public CreditCardHandler(IConfiguration configuration)
        {
            _conectionPath = configuration.GetConnectionString("BMTContext");
            _conection = new SqlConnection(_conectionPath);
        }


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


        public bool CreateCreditCard(CreditCardModel creditCard)
        {
            try
            {
                var query = "INSERT INTO dbo.CreditCard (UserID, Name, Number, MagicNumber, DateVenc) VALUES (@UserID, @Name, @Number, @MagicNumber, @DateVenc)";
                using (SqlCommand querryCommando = new SqlCommand(query, Connection))
                {
                    querryCommando.Parameters.AddWithValue("@UserID", creditCard.UserID);
                    querryCommando.Parameters.AddWithValue("@Name", creditCard.Name);
                    querryCommando.Parameters.AddWithValue("@Number", creditCard.Number);
                    querryCommando.Parameters.AddWithValue("@MagicNumber", creditCard.MagicNumber);
                    querryCommando.Parameters.AddWithValue("@DateVenc", creditCard.DateVenc);

                    Connection.Open();
                    bool result = querryCommando.ExecuteNonQuery() >= 1;
                    Connection.Close();

                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateCreditCard: {ex.Message}");
                return false;
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
            }
        }


        public List<CreditCardModel> GetCreditCards()
        {
            List<CreditCardModel> creditCards = new List<CreditCardModel>();
            string query = "SELECT * FROM dbo.CreditCard";
            DataTable resultTable = CreateQueryTable(query);
            foreach (DataRow row in resultTable.Rows)
            {
                creditCards.Add(
                new CreditCardModel
                {
                    Id = row["Id"].ToString(),
                    UserID = row["UserID"].ToString(),
                    Name = row["Name"].ToString(),
                    Number = row["Number"].ToString(),
                    MagicNumber = row["MagicNumber"].ToString(),
                    DateVenc = row["DateVenc"].ToString(),

                });
            }
            return creditCards;
        }

        public List<CreditCardModel> GetCreditCardsByUser(string userId)
        {
            List<CreditCardModel> creditCards = new List<CreditCardModel>();
            string query = "SELECT * FROM dbo.CreditCard WHERE UserID = @UserID";
            SqlParameter[] parameters = new SqlParameter[1];
            parameters[0] = new SqlParameter("@UserID", userId);
            DataTable resultTable = CreateQueryTable(query, parameters);
            foreach (DataRow row in resultTable.Rows)
            {
                creditCards.Add(
                new CreditCardModel
                {
                    Id = row["Id"].ToString(),
                    UserID = row["UserID"].ToString(),
                    Name = row["Name"].ToString(),
                    Number = row["Number"].ToString(),
                    MagicNumber = row["MagicNumber"].ToString(),
                    DateVenc = row["DateVenc"].ToString(),

                });
            }
            return creditCards;
        }
    }
}
