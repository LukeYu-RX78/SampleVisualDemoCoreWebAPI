using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using SampleVisualDemoCoreWebAPI.Models.Entities;

namespace SampleVisualDemoCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitelineBitsController : Controller
    {
        private readonly IConfiguration _configuration;

        public TitelineBitsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("AddBits")]
        public IActionResult AddBits([FromBody] List<Bit> bitsList)
        {
            if (bitsList == null || bitsList.Count == 0)
            {
                return BadRequest("Bits list data is empty or invalid.");
            }

            string query = @"
                INSERT INTO dbo.StagingTitelineAppBits
                    ([Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], [BitOrReamer], 
                     [SerialNo], [Size], [Type], [HoleID], [DepthFrom], [DepthTo], [TotalMetres], [DataSource])
                VALUES (@Pid, @PlodDate, @PlodShift, @ContractNo, @RigNo, @BitOrReamer, 
                        @SerialNo, @Size, @Type, @HoleID, @DepthFrom, @DepthTo, @TotalMetres, @DataSource);";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();

                    foreach (var bit in bitsList)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Pid", bit.Pid ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodDate", bit.PlodDate ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodShift", bit.PlodShift ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ContractNo", bit.ContractNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@RigNo", bit.RigNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@BitOrReamer", bit.BitOrReamer ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@SerialNo", bit.SerialNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Size", bit.Size ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Type", bit.Type ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@HoleID", bit.HoleID ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DepthFrom", bit.DepthFrom ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DepthTo", bit.DepthTo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@TotalMetres", bit.TotalMetres ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DataSource", bit.DataSource ?? (object)DBNull.Value);

                            command.ExecuteNonQuery();
                        }
                    }
                }

                return Ok("Bits data added successfully.");
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
        [Route("GetBitsByPid/{pid}")]
        public IActionResult GetBitsByPid(int pid)
        {
            string query = @"
                SELECT 
                    [BitId], [Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], 
                    [BitOrReamer], [SerialNo], [Size], [Type], [HoleID], 
                    [DepthFrom], [DepthTo], [TotalMetres], [DataSource]
                FROM dbo.StagingTitelineAppBits
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
                    return NotFound(new { message = $"No bits records found for Pid: {pid}" });
                }

                var bitsList = new List<Bit>();
                foreach (DataRow row in table.Rows)
                {
                    bitsList.Add(new Bit
                    {
                        BitId = Convert.ToInt32(row["BitId"]),
                        Pid = row["Pid"] == DBNull.Value ? null : Convert.ToInt32(row["Pid"]),
                        PlodDate = row["PlodDate"].ToString(),
                        PlodShift = row["PlodShift"].ToString(),
                        ContractNo = row["ContractNo"].ToString(),
                        RigNo = row["RigNo"].ToString(),
                        BitOrReamer = row["BitOrReamer"].ToString(),
                        SerialNo = row["SerialNo"].ToString(),
                        Size = row["Size"].ToString(),
                        Type = row["Type"].ToString(),
                        HoleID = row["HoleID"].ToString(),
                        DepthFrom = row["DepthFrom"].ToString(),
                        DepthTo = row["DepthTo"].ToString(),
                        TotalMetres = row["TotalMetres"].ToString(),
                        DataSource = row["DataSource"].ToString()
                    });
                }

                return Ok(bitsList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("DeleteBitByBitId/{bitId}")]
        public IActionResult DeleteBitByBitId(int bitId)
        {
            string query = "DELETE FROM dbo.StagingTitelineAppBits WHERE BitId = @BitId;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@BitId", bitId);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Bit record with BitId {bitId} deleted successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No bit record found for BitId: {bitId}" });
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
        [Route("UpdateBitByBitId/{bitId}")]
        public IActionResult UpdateBitByBitId(int bitId, [FromBody] Bit updatedData)
        {
            if (updatedData == null)
            {
                return BadRequest(new { message = "Invalid bit data." });
            }

            string query = @"
                UPDATE dbo.StagingTitelineAppBits 
                SET 
                    BitOrReamer = @BitOrReamer,
                    SerialNo = @SerialNo,
                    Size = @Size,
                    Type = @Type,
                    HoleID = @HoleID,
                    DepthFrom = @DepthFrom,
                    DepthTo = @DepthTo,
                    TotalMetres = @TotalMetres
                WHERE BitId = @BitId;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@BitId", bitId);
                        command.Parameters.AddWithValue("@BitOrReamer", updatedData.BitOrReamer ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@SerialNo", updatedData.SerialNo ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Size", updatedData.Size ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Type", updatedData.Type ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@HoleID", updatedData.HoleID ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@DepthFrom", updatedData.DepthFrom ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@DepthTo", updatedData.DepthTo ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@TotalMetres", updatedData.TotalMetres ?? (object)DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Bit record with BitId {bitId} updated successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No bit record found with BitId: {bitId}." });
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
