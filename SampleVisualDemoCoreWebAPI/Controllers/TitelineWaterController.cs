using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using SampleVisualDemoCoreWebAPI.Models.Entities;

namespace SampleVisualDemoCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitelineWaterController : Controller
    {
        private readonly IConfiguration _configuration;

        public TitelineWaterController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("AddWater")]
        public IActionResult AddWater([FromBody] List<Water> waterList)
        {
            if (waterList == null || waterList.Count == 0)
            {
                return BadRequest("Water list data is empty or invalid.");
            }

            string query = @"
                INSERT INTO dbo.StagingTitelineAppWater
                    ([Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], [Loads], [Kilometres], [Litres], [Hours], [Driver], [DataSource])
                VALUES (@Pid, @PlodDate, @PlodShift, @ContractNo, @RigNo, @Loads, @Kilometres, @Litres, @Hours, @Driver, @DataSource);";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();

                    foreach (var water in waterList)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Pid", water.Pid ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodDate", water.PlodDate ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodShift", water.PlodShift ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ContractNo", water.ContractNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@RigNo", water.RigNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Loads", water.Loads ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Kilometres", water.Kilometres ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Litres", water.Litres ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Hours", water.Hours ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Driver", water.Driver ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DataSource", water.DataSource ?? (object)DBNull.Value);

                            command.ExecuteNonQuery();
                        }
                    }
                }

                return Ok("Water data added successfully.");
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
        [Route("GetWaterByPid/{pid}")]
        public IActionResult GetWaterByPid(int pid)
        {
            string query = @"
                SELECT 
                    [Wid], [Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], [Loads], [Kilometres], [Litres], [Hours], [Driver], [DataSource]
                FROM dbo.StagingTitelineAppWater
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
                    return NotFound(new { message = $"No water records found for Pid: {pid}" });
                }

                var waterList = new List<Water>();
                foreach (DataRow row in table.Rows)
                {
                    waterList.Add(new Water
                    {
                        Wid = Convert.ToInt32(row["Wid"]),
                        Pid = row["Pid"] == DBNull.Value ? null : Convert.ToInt32(row["Pid"]),
                        PlodDate = row["PlodDate"].ToString(),
                        PlodShift = row["PlodShift"].ToString(),
                        ContractNo = row["ContractNo"].ToString(),
                        RigNo = row["RigNo"].ToString(),
                        Loads = row["Loads"].ToString(),
                        Kilometres = row["Kilometres"].ToString(),
                        Litres = row["Litres"].ToString(),
                        Hours = row["Hours"].ToString(),
                        Driver = row["Driver"].ToString(),
                        DataSource = row["DataSource"].ToString()
                    });
                }

                return Ok(waterList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("DeleteWaterByWid/{wid}")]
        public IActionResult DeleteWaterByWid(int wid)
        {
            string query = "DELETE FROM dbo.StagingTitelineAppWater WHERE Wid = @Wid;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Wid", wid);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Water record with Wid {wid} deleted successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No water record found for Wid: {wid}" });
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
        [Route("UpdateWaterByWid/{wid}")]
        public IActionResult UpdateWaterByWid(int wid, [FromBody] Water updatedData)
        {
            if (updatedData == null)
            {
                return BadRequest(new { message = "Invalid water data." });
            }

            string query = @"
                UPDATE dbo.StagingTitelineAppWater
                SET 
                    Loads = @Loads,
                    Kilometres = @Kilometres,
                    Litres = @Litres,
                    Hours = @Hours,
                    Driver = @Driver
                WHERE Wid = @Wid;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Wid", wid);
                        command.Parameters.AddWithValue("@Loads", updatedData.Loads ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Kilometres", updatedData.Kilometres ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Litres", updatedData.Litres ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Hours", updatedData.Hours ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Driver", updatedData.Driver ?? (object)DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Water record with Wid {wid} updated successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No water record found with Wid {wid}." });
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