using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using BMT_backend.Models;
using System.Data.SqlClient;



namespace BMT_backend.Handlers
{
    public class EnterpriseHandler
    {
        private SqlConnection _conection;
        private string _conectionPath;

        private const string createEnterpriseQuery = "INSERT INTO [dbo].[Entrepeneurship] (" +
            "[IdentificationType], [IdentificationNumber], [Name], [Description]) " +
            "VALUES (@IdentificationType, @IdentificationNumber, @Name, @Description);";

        private const string getEnterprisesQuery = "SELECT * FROM dbo.Enterprises;";

        private const string getEnterpriseStaffQuery = "SELECT e.Identification, " +
            "u.Name, u.LastName, u.UserName, u.Email, u.IsVerified, u.Password " +
            "FROM Entrepreneurs_Enterprises ee " +
            "JOIN Entrepreneurs e ON ee.EntrepreneurId = e.Id " +
            "JOIN Users u ON e.UserId = u.Id " +
            "WHERE ee.EnterpriseId = @enterpriseId;";

        private const string getAdminEntrpreneurQuery = "select e.Identification " +
            "from Entrepreneurs_Enterprises ee " +
            "join Entrepreneurs e on ee.EntrepreneurId = e.Id " +
            "where ee.Administrator = 1 and ee.EnterpriseId = @enterpriseId;";

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
            tableAdapter.Fill( tableFormatQuery);
            _conection.Close();
            return tableFormatQuery;
        }
        public List<EnterpriseModel> GetEnterprises()
        {
            List<EnterpriseModel> enterprises = new List<EnterpriseModel>();
            DataTable resultTable = CreateQueryTable(getEnterprisesQuery);
            foreach (DataRow row in resultTable.Rows)
            {
                enterprises.Add(
                new EnterpriseModel
                {
                    IdentificationType = Convert.ToInt32(row["IdentificationType"]),
                    IdentificationNumber = Convert.ToString(row["IdentificationNumber"]),
                    Name = Convert.ToString(row["Name"]),
                    Description = Convert.ToString(row["Description"]),
                    Admininstrator = GetEnterpriseAdministrator(Convert.ToString(row["Id"])),
                    Staff = GetEnterpriseStaff(Convert.ToString(row["Id"])),
                });
            }
            return enterprises;
        }
        public EntrepreneurModel GetEnterpriseAdministrator(string enterpriseId)
        {
            var queryCommand = new SqlCommand(getAdminEntrpreneurQuery, _conection);
            queryCommand.Parameters.AddWithValue("@enterpriseId", enterpriseId);
            DataTable resultTable = CreateQueryTable(getAdminEntrpreneurQuery);
            DataRow row = resultTable.Rows[0];
            return new EntrepreneurModel
            {
                IdentificationNumber = Convert.ToString(row["Identification"])
            };
        }
        public List<EntrepreneurModel> GetEnterpriseStaff(string enterpriseId)
        {
            List<EntrepreneurModel> staff = new List<EntrepreneurModel>();
            var queryCommand = new SqlCommand(getEnterpriseStaffQuery, _conection);
            queryCommand.Parameters.AddWithValue("@enterpriseId", enterpriseId);
            DataTable resultTable = CreateQueryTable(getEnterpriseStaffQuery);
            foreach (DataRow row in resultTable.Rows)
            {   
                staff.Add(
                new EntrepreneurModel
                {
                    IdentificationNumber = Convert.ToString(row["Identification"]),
                    Name = Convert.ToString(row["Name"]),
                    LastName = Convert.ToString(row["LastName"]),
                    Username = Convert.ToString(row["Username"]),
                    Email = Convert.ToString(row["Email"]),
                    IsVerified = Convert.ToBoolean(row["IsVerified"]),
                    Password = Convert.ToString(row["Password"])
                });
            }
            return staff;
        }
        public bool CreateEnterprise(EnterpriseModel entrepeneurship)
        {
            var queryCommand = new SqlCommand(createEnterpriseQuery, _conection);

            queryCommand.Parameters.AddWithValue("@IddentificationType", entrepeneurship.IdentificationType);
            queryCommand.Parameters.AddWithValue("@IdentificationNumber", entrepeneurship.IdentificationNumber);
            queryCommand.Parameters.AddWithValue("@Name", entrepeneurship.Name);
            queryCommand.Parameters.AddWithValue("@Description", entrepeneurship.Description);

            _conection.Open();
            bool exit = queryCommand.ExecuteNonQuery() >= 1;
            _conection.Close();

            return exit;
        }
    }
}