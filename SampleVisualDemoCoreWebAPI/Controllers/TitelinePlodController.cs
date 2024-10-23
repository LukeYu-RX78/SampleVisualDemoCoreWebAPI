using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SampleVisualDemoCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitelinePlodController : ControllerBase
    {
        private IConfiguration _configuration;

        public TitelinePlodController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetPlods")]
        public JsonResult GetPlods()
        {
            string query = "select top 10 * from dbo.plod where ContractNo = 'CW2262484_2024' order by PlodID desc";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");
            SqlDataReader sqlReader;
            using (SqlConnection conn = new SqlConnection(sqlDatasource))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    sqlReader = command.ExecuteReader();
                    table.Load(sqlReader);
                    sqlReader.Close();
                    conn.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpGet]
        [Route("GetStagingPlods")]
        public JsonResult GetStagingPlods()
        {
            string query = "select * from dbo.StagingTitelineAppPlod where ContractNo = 'CW2262484_2024' order by PlodDate desc";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");
            SqlDataReader sqlReader;
            using (SqlConnection conn = new SqlConnection(sqlDatasource))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    sqlReader = command.ExecuteReader();
                    table.Load(sqlReader);
                    sqlReader.Close();
                    conn.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpGet]
        [Route("GetStagingPlodsByAuthLv")]
        public JsonResult GetStagingPlodsByAuthLv(int authLv)
        {
            int nextAuthLv = authLv + 1;
            string ddrApproving = "%DORS-APP-" + authLv.ToString() + "-%";
            string ddrApproved = "%DORS-APP-" + nextAuthLv.ToString() + "-%";
            string query = "select * from dbo.StagingTitelinePlod where ContractNo = 'CW2262484_2024' and DataSource like '"+ ddrApproving + "' or DataSource like '" + ddrApproved + "' order by PlodDate desc";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");
            SqlDataReader sqlReader;
            using (SqlConnection conn = new SqlConnection(sqlDatasource))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    sqlReader = command.ExecuteReader();
                    table.Load(sqlReader);
                    sqlReader.Close();
                    conn.Close();
                }
            }
            return new JsonResult(table);
        }


        [HttpPost]
        [Route("AddStagingPlod")]
        public JsonResult AddStagingPlod([FromBody] string newPlod)
        {
            string coulums = "([PlodDate], [PlodShift], [ContractNo], [RigNo], [Department], [Client], [DayType], [Location], "
                + "[Comments], [MachineHoursFrom], [MachineHoursTo], [TotalMetres], [DrillingHrs], "
                + "[MetresperDrillingHr], [TotalActivityHrs], [MetresperTotalHr], [VersionNumber], [DataSource])";
            string query = "insert into dbo.StagingTitelineAppPlod " + coulums + " Values(" + newPlod + ")";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        int affectedRows = command.ExecuteNonQuery();
                        return new JsonResult("Added Successfully");
                    }
                }
            }
            catch (SqlException ex)
            {
                return new JsonResult($"SQL error occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new JsonResult(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("UpdateStagingPlodDDRState")]
        public JsonResult UpdateStagingPlod(String oldDataSource, String newDataSource)
        {
            if (string.IsNullOrWhiteSpace(oldDataSource) || string.IsNullOrWhiteSpace(newDataSource))
            {
                return new JsonResult("Old or new DataSource cannot be empty");
            }

            string query = "UPDATE dbo.StagingTitelineAppPlod SET DataSource = @NewDataSource WHERE DataSource = @OldDataSource";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@NewDataSource", newDataSource),
                new SqlParameter("@OldDataSource", oldDataSource)
            };

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            using (SqlConnection conn = new SqlConnection(sqlDatasource))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddRange(parameters.ToArray());
                    int rowsAffected = command.ExecuteNonQuery();
                    return new JsonResult($"Updated Successfully, Rows affected: {rowsAffected}");
                }
            }
        }
    }
}
