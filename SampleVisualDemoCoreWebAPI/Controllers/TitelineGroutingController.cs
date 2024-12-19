using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using SampleVisualDemoCoreWebAPI.Models.Entities;

namespace SampleVisualDemoCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitelineGroutingController : Controller
    {
        private readonly IConfiguration _configuration;

        public TitelineGroutingController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("AddGrouting")]
        public IActionResult AddGrouting([FromBody] List<Grouting> groutingList)
        {
            if (groutingList == null || groutingList.Count == 0)
            {
                return BadRequest("Grouting list data is empty or invalid.");
            }

            string query = @"
                INSERT INTO dbo.StagingTitelineAppGrouting
                    ([Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], [HoleID], [DrillSite], [BagsUsed], [Metres], [Volume], [DataSource])
                VALUES (@Pid, @PlodDate, @PlodShift, @ContractNo, @RigNo, @HoleID, @DrillSite, @BagsUsed, @Metres, @Volume, @DataSource);";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();

                    foreach (var grouting in groutingList)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Pid", grouting.Pid ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodDate", grouting.PlodDate ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodShift", grouting.PlodShift ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ContractNo", grouting.ContractNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@RigNo", grouting.RigNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@HoleID", grouting.HoleID ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DrillSite", grouting.DrillSite ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@BagsUsed", grouting.BagsUsed ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Metres", grouting.Metres ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Volume", grouting.Volume ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DataSource", grouting.DataSource ?? (object)DBNull.Value);

                            command.ExecuteNonQuery();
                        }
                    }
                }

                return Ok(new { message = "Grouting data added successfully." });
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
        [Route("GetGroutingByPid/{pid}")]
        public IActionResult GetGroutingByPid(int pid)
        {
            string query = @"
                SELECT 
                    [Gid], [Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], [HoleID], 
                    [DrillSite], [BagsUsed], [Metres], [Volume], [DataSource]
                FROM dbo.StagingTitelineAppGrouting
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
                    return NotFound(new { message = $"No grouting records found for Pid: {pid}" });
                }

                var groutingList = new List<Grouting>();
                foreach (DataRow row in table.Rows)
                {
                    groutingList.Add(new Grouting
                    {
                        Gid = Convert.ToInt32(row["Gid"]),
                        Pid = row["Pid"] == DBNull.Value ? null : Convert.ToInt32(row["Pid"]),
                        PlodDate = row["PlodDate"].ToString(),
                        PlodShift = row["PlodShift"].ToString(),
                        ContractNo = row["ContractNo"].ToString(),
                        RigNo = row["RigNo"].ToString(),
                        HoleID = row["HoleID"].ToString(),
                        DrillSite = row["DrillSite"].ToString(),
                        BagsUsed = row["BagsUsed"].ToString(),
                        Metres = row["Metres"].ToString(),
                        Volume = row["Volume"].ToString(),
                        DataSource = row["DataSource"].ToString(),
                    });
                }

                return Ok(groutingList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("DeleteGroutingByGid/{gid}")]
        public IActionResult DeleteGroutingByGid(int gid)
        {
            string query = "DELETE FROM dbo.StagingTitelineAppGrouting WHERE Gid = @Gid;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Gid", gid);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Grouting record with Gid {gid} deleted successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No grouting record found for Gid: {gid}" });
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
        [Route("UpdateGroutingByGid/{gid}")]
        public IActionResult UpdateGroutingByGid(int gid, [FromBody] Grouting updatedData)
        {
            if (updatedData == null)
            {
                return BadRequest(new { message = "Invalid grouting data." });
            }

            string query = @"
                UPDATE dbo.StagingTitelineAppGrouting
                SET 
                    HoleID = @HoleID,
                    DrillSite = @DrillSite,
                    BagsUsed = @BagsUsed,
                    Metres = @Metres,
                    Volume = @Volume
                WHERE Gid = @Gid;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Gid", gid);
                        command.Parameters.AddWithValue("@HoleID", updatedData.HoleID ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@DrillSite", updatedData.DrillSite ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@BagsUsed", updatedData.BagsUsed ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Metres", updatedData.Metres ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Volume", updatedData.Volume ?? (object)DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Grouting record with Gid {gid} updated successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No grouting record found with Gid {gid}." });
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