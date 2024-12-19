using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using SampleVisualDemoCoreWebAPI.Models.Entities;

namespace SampleVisualDemoCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitelineRigServiceController : Controller
    {
        private readonly IConfiguration _configuration;

        public TitelineRigServiceController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("AddRigService")]
        public IActionResult AddRigService([FromBody] List<RigService> rigServiceList)
        {
            if (rigServiceList == null || rigServiceList.Count == 0)
            {
                return BadRequest("Rig Service list is empty or invalid.");
            }

            string query = @"
                INSERT INTO dbo.StagingTitelineAppRigService
                    ([Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], [ServiceType], [Daily], [Weekly], [ThousandHour], [DataSource])
                VALUES 
                    (@Pid, @PlodDate, @PlodShift, @ContractNo, @RigNo, @ServiceType, @Daily, @Weekly, @ThousandHour, @DataSource);
            ";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();

                    foreach (var rigService in rigServiceList)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Pid", rigService.Pid ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodDate", rigService.PlodDate ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodShift", rigService.PlodShift ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ContractNo", rigService.ContractNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@RigNo", rigService.RigNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ServiceType", rigService.ServiceType ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Daily", rigService.Daily ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Weekly", rigService.Weekly ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ThousandHour", rigService.ThousandHour ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DataSource", rigService.DataSource ?? (object)DBNull.Value);

                            command.ExecuteNonQuery();
                        }
                    }
                }

                return Ok(new { message = "Rig Service data added successfully." });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { message = $"SQL error occurred: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet]
        [Route("GetRigServiceByPid/{pid}")]
        public IActionResult GetRigServiceByPid(int pid)
        {
            string query = @"
                SELECT 
                    [RsId], [Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], 
                    [ServiceType], [Daily], [Weekly], [ThousandHour], [DataSource]
                FROM dbo.StagingTitelineAppRigService
                WHERE Pid = @Pid;
            ";

            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Pid", pid);
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(table);
                    }
                }

                if (table.Rows.Count == 0)
                {
                    return NotFound(new { message = $"No Rig Service records found for Pid: {pid}" });
                }

                var rigServiceList = new List<RigService>();
                foreach (DataRow row in table.Rows)
                {
                    rigServiceList.Add(new RigService
                    {
                        RsId = Convert.ToInt32(row["RsId"]),
                        Pid = row["Pid"] == DBNull.Value ? null : Convert.ToInt32(row["Pid"]),
                        PlodDate = row["PlodDate"].ToString(),
                        PlodShift = row["PlodShift"].ToString(),
                        ContractNo = row["ContractNo"].ToString(),
                        RigNo = row["RigNo"].ToString(),
                        ServiceType = row["ServiceType"].ToString(),
                        Daily = row["Daily"].ToString(),
                        Weekly = row["Weekly"].ToString(),
                        ThousandHour = row["ThousandHour"].ToString(),
                        DataSource = row["DataSource"].ToString(),
                    });
                }

                return Ok(rigServiceList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpDelete]
        [Route("DeleteRigServiceByRsId/{rsid}")]
        public IActionResult DeleteRigServiceByRsId(int rsid)
        {
            string query = "DELETE FROM dbo.StagingTitelineAppRigService WHERE RsId = @RsId;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@RsId", rsid);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Rig Service record with RsId {rsid} deleted successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No Rig Service record found for RsId: {rsid}" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPut]
        [Route("UpdateRigServiceByRsId/{rsid}")]
        public IActionResult UpdateRigServiceByRsId(int rsid, [FromBody] RigService updatedData)
        {
            if (updatedData == null)
            {
                return BadRequest(new { message = "Invalid Rig Service data." });
            }

            string query = @"
                UPDATE dbo.StagingTitelineAppRigService
                SET 
                    ServiceType = @ServiceType,
                    Daily = @Daily,
                    Weekly = @Weekly,
                    ThousandHour = @ThousandHour
                WHERE RsId = @RsId;
            ";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@RsId", rsid);
                        command.Parameters.AddWithValue("@ServiceType", updatedData.ServiceType ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Daily", updatedData.Daily ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Weekly", updatedData.Weekly ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ThousandHour", updatedData.ThousandHour ?? (object)DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Rig Service record with RsId {rsid} updated successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No Rig Service record found with RsId {rsid}." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}