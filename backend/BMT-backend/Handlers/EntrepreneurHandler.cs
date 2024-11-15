using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;
using BMT_backend.Domain.Entities;
using BMT_backend.Presentation.Requests;

namespace BMT_backend.Handlers
{
    public class EntrepreneurHandler
    {
        private SqlConnection _conection;
        private string _conectionPath;

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
        private bool CheckIfEntryInTable (string tableName, string columnName, string columnValue)
        {
            string query = "select " + columnName + " from " + tableName + " where " + columnName + " = '" + columnValue + "'";
            DataTable table = CreateQueryTable(query);
            if (table.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        public bool CheckIfEntrepreneurExists(string identification)
        {
            if (CheckIfEntryInTable("Entrepreneurs", "Identification", identification))
            {
                return true;
            }
            return false;
        }

        public bool CreateEntrepreneur(Entrepreneur entrepreneur)
        {
            string createEntrepreneurQuery = "insert into Entrepreneurs (UserId, Identification) " +
                "values ((select Id from Users where UserName = @UserName), " +
                        "@Identification);";

            var createEntrepreneurCommand = new SqlCommand(createEntrepreneurQuery, _conection);
            createEntrepreneurCommand.Parameters.AddWithValue("@UserName", entrepreneur.Username);
            createEntrepreneurCommand.Parameters.AddWithValue("@Identification", entrepreneur.Identification);
            _conection.Open();
            createEntrepreneurCommand.ExecuteNonQuery();
            _conection.Close();
            return true;
        }

        public bool AddEntrepreneurToEnterprise(AddEntrepreneurToEnterpriseRequest request)
        {
            string addEntrepreneurToEnterpriseQuery = "insert into Entrepreneurs_Enterprises (EntrepreneurId, EnterpriseId, Administrator) " +
                "values ((select Id from Entrepreneurs where Identification = @EntrepreneurIdentification), " +
                        "(select Id from Enterprises where IdentificationNumber = @EnterpriseIdentification), " +
                        "@IsAdmin);";

            var addEntrepreneurToEnterpriseCommand = new SqlCommand(addEntrepreneurToEnterpriseQuery, _conection);
            addEntrepreneurToEnterpriseCommand.Parameters.AddWithValue("@EntrepreneurIdentification", request.EntrepreneurIdentification);
            addEntrepreneurToEnterpriseCommand.Parameters.AddWithValue("@EnterpriseIdentification", request.EnterpriseIdentification);
            addEntrepreneurToEnterpriseCommand.Parameters.AddWithValue("@IsAdmin", request.IsAdmin ? 1 : 0);

            _conection.Open();
            bool exit = addEntrepreneurToEnterpriseCommand.ExecuteNonQuery() >= 1;
            _conection.Close();
            return exit;
        }

        public List<Entrepreneur> GetEntrepreneurs() {
            List<Entrepreneur> entrepreneurs = new List<Entrepreneur>();
            DataTable table = CreateQueryTable("select e.Id, e.Identification, u.Name, u.LastName, u.UserName, u.Email, u.IsVerified " +
                        "from Entrepreneurs e " +
                        "join Users u on e.UserId = u.Id;");
            foreach (DataRow row in table.Rows)
            {
                entrepreneurs.Add(
                    new Entrepreneur
                    {
                        Id = Convert.ToString(row["Id"]),
                        Name = Convert.ToString(row["Name"]),
                        LastName = Convert.ToString(row["LastName"]),
                        Username = Convert.ToString(row["Username"]),
                        Email = Convert.ToString(row["Email"]),
                        IsVerified = Convert.ToBoolean(row["IsVerified"]),
                        Identification = Convert.ToString(row["Identification"]),
                    }
                );
            }
            return entrepreneurs;
        }

        public List<Enterprise> GetEnterprisesOfEntrepreneur(string Identification)
        {
            List<Enterprise> enterprises = new List<Enterprise>();
            EnterpriseHandler enterpriseHandler = new EnterpriseHandler();

            string query = $"select en.Id, en.IdentificationNumber, en.Name, en.Description, en.Email, en.PhoneNumber " +
                           $"from Entrepreneurs_Enterprises ee " +
                           $"join Enterprises en on ee.EnterpriseId = en.Id " +
                           $"where ee.EntrepreneurId = (select Id from Entrepreneurs where Identification = '{Identification}');";

            DataTable tableOfEnterprises = CreateQueryTable(query);

            foreach (DataRow row in tableOfEnterprises.Rows)
            {
                var enterprise = new Enterprise
                {
                    Id = Convert.ToString(row["Id"]),
                    IdentificationNumber = Convert.ToString(row["IdentificationNumber"]),
                    Name = Convert.ToString(row["Name"]),
                    Description = Convert.ToString(row["Description"]),
                    Email = Convert.ToString(row["Email"]),
                    PhoneNumber = Convert.ToString(row["PhoneNumber"]),
                    Administrator = enterpriseHandler.GetEnterpriseAdministrator(Convert.ToString(row["Id"]))
                };
                enterprises.Add(enterprise);
                Console.WriteLine(enterprise);
            }
            return enterprises;
        }


        public Entrepreneur GetEntrepreneurByUserId(string id)
        {
            Entrepreneur entrepreneur = new();

            string query = $"SELECT e.Id, e.Identification " +
                            $"FROM Entrepreneurs e " +
                            $"JOIN Users u ON e.UserId = u.Id " +
                            $"WHERE u.Id = '{id}'";
            DataTable table = CreateQueryTable(query);

            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];
                entrepreneur.Id = Convert.ToString(row["Id"]);
                entrepreneur.Identification = Convert.ToString(row["Identification"]);
            }

            return entrepreneur;
        }
    }
}


