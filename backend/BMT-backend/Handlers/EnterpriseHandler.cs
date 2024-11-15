using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;
using BMT_backend.Domain.Views;
using BMT_backend.Domain.Entities;
using BMT_backend.Presentation.Requests;

namespace BMT_backend.Handlers
{
    public class EnterpriseHandler
    {
        private SqlConnection _conection;
        private string _conectionPath;

        private const string getEnterpriseStaffQuery = "select e.Identification, " +
            "u.Name, u.LastName, u.UserName, u.Email, u.IsVerified " +
            "FROM Entrepreneurs_Enterprises ee " +
            "JOIN Entrepreneurs e ON ee.EntrepreneurId = e.Id " +
            "JOIN Users u ON e.UserId = u.Id " +
            "WHERE ee.EnterpriseId = @enterpriseId";

        public EnterpriseHandler()
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

        private bool CheckIfEntryInTable(string tableName, string columnName, string columnValue)
        {
            string query = "select " + columnName + " from " + tableName + " where " + columnName + " = '" + columnValue + "'";
            DataTable table = CreateQueryTable(query);
            if (table.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        public bool CheckIfEnterpriseExists(string identification)
        {
            if (CheckIfEntryInTable("Enterprises", "IdentificationNumber", identification)
                || CheckIfEntryInTable("Enterprises", "Name", identification)
                || CheckIfEntryInTable("Enterprises", "Email", identification)
                || CheckIfEntryInTable("Enterprises", "PhoneNumber", identification))
            {
                return true;
            }
            return false;
        }

        public bool CreateEnterprise(Enterprise enterprise)
        {
            string createEnterpriseQuery = "insert into Enterprises (" +
            "IdentificationType, IdentificationNumber, Name, Description, Email, PhoneNumber) " +
            "values (@IdentificationType, @IdentificationNumber, @Name, @Description, @Email, @PhoneNumber);";

            var queryCommand = new SqlCommand(createEnterpriseQuery, _conection);
            queryCommand.Parameters.AddWithValue("@IdentificationType", enterprise.IdentificationType);
            queryCommand.Parameters.AddWithValue("@IdentificationNumber", enterprise.IdentificationNumber);
            queryCommand.Parameters.AddWithValue("@Name", enterprise.Name);
            queryCommand.Parameters.AddWithValue("@Description", enterprise.Description);
            queryCommand.Parameters.AddWithValue("@Email", enterprise.Email);
            queryCommand.Parameters.AddWithValue("@PhoneNumber", enterprise.PhoneNumber);

            _conection.Open();
            queryCommand.ExecuteNonQuery();
            _conection.Close();
            return true;
        }

        public List<Enterprise> GetEnterprises()
        {
            List<Enterprise> enterprises = new List<Enterprise>();
            DataTable resultTable = CreateQueryTable("select * FROM Enterprises;");
            foreach (DataRow row in resultTable.Rows)
            {
                enterprises.Add(
                new Enterprise
                {
                    IdentificationType = Convert.ToInt32(row["IdentificationType"]),
                    IdentificationNumber = Convert.ToString(row["IdentificationNumber"]),
                    Name = Convert.ToString(row["Name"]),
                    Description = Convert.ToString(row["Description"]),
                    Email = Convert.ToString(row["Email"]),
                    PhoneNumber = Convert.ToString(row["PhoneNumber"]),
                    Administrator = GetEnterpriseAdministrator(Convert.ToString(row["Id"])),
                    Staff = GetEnterpriseStaff(Convert.ToString(row["Id"])),
                });
            }
            return enterprises;
        }

        public List<Entrepreneur> GetEnterpriseStaff(string enterpriseId)
        {
            List<Entrepreneur> staff = new List<Entrepreneur>();
            var queryCommand = new SqlCommand(getEnterpriseStaffQuery, _conection);
            SqlDataAdapter tableAdapter = new SqlDataAdapter(queryCommand);
            DataTable tableFormatQuery = new DataTable();
            queryCommand.Parameters.AddWithValue("@enterpriseId", enterpriseId);
            _conection.Open();
            tableAdapter.Fill(tableFormatQuery);
            _conection.Close();

            if (tableFormatQuery.Rows.Count == 0)
            {
                return staff;
            }

            foreach (DataRow row in tableFormatQuery.Rows)
            {
                staff.Add(
                new Entrepreneur
                {
                    Identification = Convert.ToString(row["Identification"]),
                    Name = Convert.ToString(row["Name"]),
                    LastName = Convert.ToString(row["LastName"]),
                    Username = Convert.ToString(row["Username"]),
                    Email = Convert.ToString(row["Email"]),
                    IsVerified = Convert.ToBoolean(row["IsVerified"]),
                });
            }
            return staff;
        }

        public Entrepreneur GetEnterpriseAdministrator(string enterpriseId)
        {
            List<Entrepreneur> staff = new List<Entrepreneur>();
            string getAdminEntrpreneurQuery = getEnterpriseStaffQuery + " and ee.Administrator = 1;";
            var queryCommand = new SqlCommand(getEnterpriseStaffQuery, _conection);
            SqlDataAdapter tableAdapter = new SqlDataAdapter(queryCommand);
            DataTable tableFormatQuery = new DataTable();
            queryCommand.Parameters.AddWithValue("@enterpriseId", enterpriseId);
            _conection.Open();
            tableAdapter.Fill(tableFormatQuery);
            _conection.Close();

            if (tableFormatQuery.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = tableFormatQuery.Rows[0];
            Entrepreneur administrator = new Entrepreneur
            {
                Identification = Convert.ToString(row["Identification"]),
                Name = Convert.ToString(row["Name"]),
                LastName = Convert.ToString(row["LastName"]),
                Username = Convert.ToString(row["Username"]),
                Email = Convert.ToString(row["Email"]),
                IsVerified = Convert.ToBoolean(row["IsVerified"]),
            };
            return administrator;
        }

        public List<DeveloperEnterpriseView> GetDevEnterprises()
        {
            List<DeveloperEnterpriseView> devEnterprises = new List<DeveloperEnterpriseView>();
            string query = "SELECT e.Name, " +
                           "(SELECT COUNT(*) FROM Entrepreneurs_Enterprises ee WHERE ee.EnterpriseId = e.Id) AS EmployeeQuantity, " +
                           "(SELECT COUNT(*) FROM Products p WHERE p.EnterpriseId = e.Id) AS ProductQuantity " +
                           "FROM Enterprises e";

            var queryCommand = new SqlCommand(query, _conection);
            SqlDataAdapter tableAdapter = new SqlDataAdapter(queryCommand);
            DataTable resultTable = new DataTable();
            _conection.Open();
            tableAdapter.Fill(resultTable);
            _conection.Close();
            foreach (DataRow row in resultTable.Rows)
            {
                devEnterprises.Add(
                    new DeveloperEnterpriseView
                    {
                        Name = Convert.ToString(row["Name"]),
                        EmployeeQuantity = Convert.ToString(row["EmployeeQuantity"]),
                        ProductQuantity = Convert.ToString(row["ProductQuantity"]),
                    });
            }
            return devEnterprises;
        }

        public Enterprise? GetEnterpriseById(string enterpriseId)
        {
            var query = "SELECT * FROM Enterprises WHERE Id = @enterpriseId";
            SqlCommand queryCommand = new SqlCommand(query, _conection);
            queryCommand.Parameters.AddWithValue("@enterpriseId", enterpriseId);

            SqlDataAdapter tableAdapter = new SqlDataAdapter(queryCommand);
            DataTable resultTable = new DataTable();
            _conection.Open();
            tableAdapter.Fill(resultTable);
            _conection.Close();

            if (resultTable.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = resultTable.Rows[0];
            var enterprise = new Enterprise
            {
                Id = enterpriseId,
                IdentificationType = Convert.ToInt32(row["IdentificationType"]),
                IdentificationNumber = Convert.ToString(row["IdentificationNumber"]),
                Name = Convert.ToString(row["Name"]),
                Description = Convert.ToString(row["Description"]),
                Email = Convert.ToString(row["Email"]),
                PhoneNumber = Convert.ToString(row["PhoneNumber"]),
                Administrator = GetEnterpriseAdministrator(enterpriseId),
                Staff = GetEnterpriseStaff(enterpriseId)
            };

            return enterprise;
        }

        public bool UpdateEnterpriseProfile(UpdateEnterpriseRequest updatedEnterprise)
        {
            var fieldsToUpdate = new List<string>();
            
            if (!string.IsNullOrEmpty(updatedEnterprise.Name))
            {
                fieldsToUpdate.Add("Name = @Name");
            }
            if (!string.IsNullOrEmpty(updatedEnterprise.Description))
            {
                fieldsToUpdate.Add("Description = @Description");
            }
            if (!string.IsNullOrEmpty(updatedEnterprise.Email))
            {
                fieldsToUpdate.Add("Email = @Email");
            }
            if (!string.IsNullOrEmpty(updatedEnterprise.PhoneNumber))
            {
                fieldsToUpdate.Add("PhoneNumber = @PhoneNumber");
            }

            if (fieldsToUpdate.Count == 0)
            {
                throw new ArgumentException("No hay campos válidos para actualizar.");
            }

            var updateQuery = $"UPDATE Enterprises SET {string.Join(", ", fieldsToUpdate)} WHERE Id = @Id";

            using (var updateCommand = new SqlCommand(updateQuery, _conection))
            {
                updateCommand.Parameters.AddWithValue("@Id", updatedEnterprise.Id);

                if (!string.IsNullOrEmpty(updatedEnterprise.Name))
                    updateCommand.Parameters.AddWithValue("@Name", updatedEnterprise.Name);

                if (!string.IsNullOrEmpty(updatedEnterprise.Description))
                    updateCommand.Parameters.AddWithValue("@Description", updatedEnterprise.Description);

                if (!string.IsNullOrEmpty(updatedEnterprise.Email))
                    updateCommand.Parameters.AddWithValue("@Email", updatedEnterprise.Email);

                if (!string.IsNullOrEmpty(updatedEnterprise.PhoneNumber))
                    updateCommand.Parameters.AddWithValue("@PhoneNumber", updatedEnterprise.PhoneNumber);

                _conection.Open();
                bool result = updateCommand.ExecuteNonQuery() >= 1;
                _conection.Close();

                return result;
            }
        }


    }
}
