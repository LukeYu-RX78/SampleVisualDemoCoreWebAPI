using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using SampleVisualDemoCoreWebAPI.Models.Entities;

namespace SampleVisualDemoCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitelinePowerPackController : Controller
    {
        private readonly IConfiguration _configuration;

        public TitelinePowerPackController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("AddPowerPack")]
        public IActionResult AddPowerPack([FromBody] List<PowerPack> powerPackList)
        {
            if (powerPackList == null || powerPackList.Count == 0)
            {
                return BadRequest("PowerPack list data is empty or invalid.");
            }

            string query = @"
                INSERT INTO dbo.StagingTitelineAppPowerPack
                    ([Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], [TimeFrom], [TimeTo], [DataSource])
                VALUES (@Pid, @PlodDate, @PlodShift, @ContractNo, @RigNo, @TimeFrom, @TimeTo, @DataSource);";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();

                    foreach (var powerPack in powerPackList)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Pid", powerPack.Pid ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodDate", powerPack.PlodDate ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodShift", powerPack.PlodShift ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ContractNo", powerPack.ContractNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@RigNo", powerPack.RigNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@TimeFrom", powerPack.TimeFrom ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@TimeTo", powerPack.TimeTo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DataSource", powerPack.DataSource ?? (object)DBNull.Value);

                            command.ExecuteNonQuery();
                        }
                    }
                }

                return Ok("PowerPack data added successfully.");
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"SQL error occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("GetPowerPackByPid/{pid}")]
        public IActionResult GetPowerPackByPid(int pid)
        {
            string query = @"
                SELECT 
                    [PowId], [Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], [TimeFrom], [TimeTo], [DataSource]
                FROM dbo.StagingTitelineAppPowerPack
                WHERE Pid = @Pid;";

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
                    return NotFound(new { message = $"No PowerPack records found for Pid: {pid}" });
                }

                var powerPackList = new List<PowerPack>();
                foreach (DataRow row in table.Rows)
                {
                    powerPackList.Add(new PowerPack
                    {
                        PowId = Convert.ToInt32(row["PowId"]),
                        Pid = row["Pid"] == DBNull.Value ? null : Convert.ToInt32(row["Pid"]),
                        PlodDate = row["PlodDate"].ToString(),
                        PlodShift = row["PlodShift"].ToString(),
                        ContractNo = row["ContractNo"].ToString(),
                        RigNo = row["RigNo"].ToString(),
                        TimeFrom = row["TimeFrom"].ToString(),
                        TimeTo = row["TimeTo"].ToString(),
                        DataSource = row["DataSource"].ToString()
                    });
                }

                return Ok(powerPackList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("DeletePowerPackByPowId/{powId}")]
        public IActionResult DeletePowerPackByPowId(int powId)
        {
            string query = "DELETE FROM dbo.StagingTitelineAppPowerPack WHERE PowId = @PowId;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@PowId", powId);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"PowerPack record with PowId {powId} deleted successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No PowerPack record found for PowId: {powId}" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("UpdatePowerPackByPowId/{powId}")]
        public IActionResult UpdatePowerPackByPowId(int powId, [FromBody] PowerPack updatedData)
        {
            if (updatedData == null)
            {
                return BadRequest(new { message = "Invalid PowerPack data." });
            }

            string query = @"
                UPDATE dbo.StagingTitelineAppPowerPack
                SET 
                    TimeFrom = @TimeFrom,
                    TimeTo = @TimeTo
                WHERE PowId = @PowId;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@PowId", powId);
                        command.Parameters.AddWithValue("@TimeFrom", updatedData.TimeFrom ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@TimeTo", updatedData.TimeTo ?? (object)DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"PowerPack record with PowId {powId} updated successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No PowerPack record found with PowId {powId}." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}