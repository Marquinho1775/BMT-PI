﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using BMT_backend.Models;
using System.Data.SqlClient;



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
            tableAdapter.Fill( tableFormatQuery);
            _conection.Close();
            return tableFormatQuery;
        }

        public bool CreateEnterprise(EnterpriseModel enterprise)
        {
            string createEnterpriseQuery = "insert into Enterprises (" +
            "IdentificationType, IdentificationNumber, Name, Description) " +
            "values (@IdentificationType, @IdentificationNumber, @Name, @Description);";

            var queryCommand = new SqlCommand(createEnterpriseQuery, _conection);
            queryCommand.Parameters.AddWithValue("@IdentificationType", enterprise.IdentificationType);
            queryCommand.Parameters.AddWithValue("@IdentificationNumber", enterprise.IdentificationNumber);
            queryCommand.Parameters.AddWithValue("@Name", enterprise.Name);
            queryCommand.Parameters.AddWithValue("@Description", enterprise.Description);

            Console.WriteLine("llego antes open");
            _conection.Open();
            Console.WriteLine("llego despues open");
            bool exit = queryCommand.ExecuteNonQuery() >= 1;
            Console.WriteLine("llego antes open");
            _conection.Close();
            Console.WriteLine("llego despues open");
            return exit;
        }

        public List<EnterpriseModel> GetEnterprises()
        {
            List<EnterpriseModel> enterprises = new List<EnterpriseModel>();
            DataTable resultTable = CreateQueryTable("select * FROM Enterprises;");
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

        public List<EntrepreneurViewModel> GetEnterpriseStaff(string enterpriseId)
        {   
            List<EntrepreneurViewModel> staff = new List<EntrepreneurViewModel>();
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
                new EntrepreneurViewModel
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

        public EntrepreneurViewModel GetEnterpriseAdministrator(string enterpriseId)
        {
            List<EntrepreneurViewModel> staff = new List<EntrepreneurViewModel>();
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
            EntrepreneurViewModel administrator = new EntrepreneurViewModel
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
    }
}