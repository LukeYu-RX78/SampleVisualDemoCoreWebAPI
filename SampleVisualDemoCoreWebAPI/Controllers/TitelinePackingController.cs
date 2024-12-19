using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using SampleVisualDemoCoreWebAPI.Models.Entities;

namespace SampleVisualDemoCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitelinePackingController : Controller
    {
        private readonly IConfiguration _configuration;

        public TitelinePackingController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("AddPacking")]
        public IActionResult AddPacking([FromBody] List<Packing> packingList)
        {
            if (packingList == null || packingList.Count == 0)
            {
                return BadRequest("Packing list data is empty or invalid.");
            }

            string query = @"
                INSERT INTO dbo.StagingTitelineAppPacking
                    ([Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], [HoleID], [Depth], [Plugged], [Comment], [DataSource])
                VALUES (@Pid, @PlodDate, @PlodShift, @ContractNo, @RigNo, @HoleID, @Depth, @Plugged, @Comment, @DataSource);";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();

                    foreach (var packing in packingList)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Pid", packing.Pid ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodDate", packing.PlodDate ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodShift", packing.PlodShift ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ContractNo", packing.ContractNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@RigNo", packing.RigNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@HoleID", packing.HoleID ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Depth", packing.Depth ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Plugged", packing.Plugged ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Comment", packing.Comment ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DataSource", packing.DataSource ?? (object)DBNull.Value);

                            command.ExecuteNonQuery();
                        }
                    }
                }

                return Ok("Packing data added successfully.");
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
        [Route("GetPackingByPid/{pid}")]
        public IActionResult GetPackingByPid(int pid)
        {
            string query = @"
                SELECT 
                    [PackId], [Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], 
                    [HoleID], [Depth], [Plugged], [Comment], [DataSource]
                FROM dbo.StagingTitelineAppPacking
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
                    return NotFound(new { message = $"No packing records found for Pid: {pid}" });
                }

                var packingList = new List<Packing>();
                foreach (DataRow row in table.Rows)
                {
                    packingList.Add(new Packing
                    {
                        PackId = Convert.ToInt32(row["PackId"]),
                        Pid = row["Pid"] == DBNull.Value ? null : Convert.ToInt32(row["Pid"]),
                        PlodDate = row["PlodDate"].ToString(),
                        PlodShift = row["PlodShift"].ToString(),
                        ContractNo = row["ContractNo"].ToString(),
                        RigNo = row["RigNo"].ToString(),
                        HoleID = row["HoleID"].ToString(),
                        Depth = row["Depth"].ToString(),
                        Plugged = row["Plugged"].ToString(),
                        Comment = row["Comment"].ToString(),
                        DataSource = row["DataSource"].ToString()
                    });
                }

                return Ok(packingList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("DeletePackingByPackId/{packId}")]
        public IActionResult DeletePackingByPackId(int packId)
        {
            string query = "DELETE FROM dbo.StagingTitelineAppPacking WHERE PackId = @PackId;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@PackId", packId);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Packing record with PackId {packId} deleted successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No packing record found for PackId: {packId}" });
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
        [Route("UpdatePackingByPackId/{packId}")]
        public IActionResult UpdatePackingByPackId(int packId, [FromBody] Packing updatedData)
        {
            if (updatedData == null)
            {
                return BadRequest(new { message = "Invalid packing data." });
            }

            string query = @"
                UPDATE dbo.StagingTitelineAppPacking
                SET 
                    HoleID = @HoleID,
                    Depth = @Depth,
                    Plugged = @Plugged,
                    Comment = @Comment
                WHERE PackId = @PackId;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@PackId", packId);
                        command.Parameters.AddWithValue("@HoleID", updatedData.HoleID ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Depth", updatedData.Depth ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Plugged", updatedData.Plugged ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Comment", updatedData.Comment ?? (object)DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Packing record with PackId {packId} updated successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No packing record found with PackId {packId}." });
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