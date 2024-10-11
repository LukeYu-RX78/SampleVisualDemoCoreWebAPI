using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

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
            string query = "select * from dbo.Plod";
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
            string query = "insert into dbo.StagingTitelinePlod " + coulums + " Values(" + newPlod + ")";
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
    }
}
