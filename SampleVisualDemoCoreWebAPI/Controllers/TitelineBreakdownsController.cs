using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using SampleVisualDemoCoreWebAPI.Models.Entities;

namespace SampleVisualDemoCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitelineBreakdownsController : Controller
    {
        private readonly IConfiguration _configuration;

        public TitelineBreakdownsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("AddBreakdowns")]
        public IActionResult AddBreakdowns([FromBody] List<Breakdowns> breakdownsList)
        {
            if (breakdownsList == null || breakdownsList.Count == 0)
            {
                return BadRequest("Breakdowns list data is empty or invalid.");
            }

            string query = @"
                INSERT INTO dbo.StagingTitelineAppBreakdowns
                    ([Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], [Type], [Hours], [DataSource])
                VALUES (@Pid, @PlodDate, @PlodShift, @ContractNo, @RigNo, @Type, @Hours, @DataSource);";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();

                    foreach (var breakdown in breakdownsList)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Pid", breakdown.Pid ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodDate", breakdown.PlodDate ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodShift", breakdown.PlodShift ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ContractNo", breakdown.ContractNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@RigNo", breakdown.RigNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Type", breakdown.Type ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Hours", breakdown.Hours ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DataSource", breakdown.DataSource ?? (object)DBNull.Value);

                            command.ExecuteNonQuery();
                        }
                    }
                }

                return Ok("Breakdowns data added successfully.");
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
        [Route("GetBreakdownsByPid/{pid}")]
        public IActionResult GetBreakdownsByPid(int pid)
        {
            string query = @"
                SELECT 
                    [Bid], [Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], [Type], [Hours], [DataSource]
                FROM dbo.StagingTitelineAppBreakdowns
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
                    return NotFound(new { message = $"No breakdowns records found for Pid: {pid}" });
                }

                var breakdownsList = new List<Breakdowns>();
                foreach (DataRow row in table.Rows)
                {
                    breakdownsList.Add(new Breakdowns
                    {
                        Bid = Convert.ToInt32(row["Bid"]),
                        Pid = row["Pid"] == DBNull.Value ? null : Convert.ToInt32(row["Pid"]),
                        PlodDate = row["PlodDate"].ToString(),
                        PlodShift = row["PlodShift"].ToString(),
                        ContractNo = row["ContractNo"].ToString(),
                        RigNo = row["RigNo"].ToString(),
                        Type = row["Type"].ToString(),
                        Hours = row["Hours"].ToString(),
                        DataSource = row["DataSource"].ToString()
                    });
                }

                return Ok(breakdownsList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("DeleteBreakdownsByBid/{bid}")]
        public IActionResult DeleteBreakdownsByBid(int bid)
        {
            string query = "DELETE FROM dbo.StagingTitelineAppBreakdowns WHERE Bid = @Bid;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Bid", bid);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Breakdowns record with Bid {bid} deleted successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No breakdowns record found for Bid: {bid}" });
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
        [Route("UpdateBreakdownsByBid/{bid}")]
        public IActionResult UpdateBreakdownsByBid(int bid, [FromBody] Breakdowns updatedData)
        {
            if (updatedData == null)
            {
                return BadRequest(new { message = "Invalid breakdowns data." });
            }

            string query = @"
                UPDATE dbo.StagingTitelineAppBreakdowns
                SET 
                    Type = @Type,
                    Hours = @Hours
                WHERE Bid = @Bid;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Bid", bid);
                        command.Parameters.AddWithValue("@Type", updatedData.Type ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Hours", updatedData.Hours ?? (object)DBNull.Value);
                     
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Breakdowns record with Bid {bid} updated successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No Breakdowns record found with Bid {bid}." });
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
