using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using SampleVisualDemoCoreWebAPI.Models.Entities;

namespace SampleVisualDemoCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitelineCasingController : Controller
    {
        private readonly IConfiguration _configuration;

        public TitelineCasingController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("AddCasing")]
        public IActionResult AddCasing([FromBody] List<Casing> casingList)
        {
            if (casingList == null || casingList.Count == 0)
            {
                return BadRequest("Casing list data is empty or invalid.");
            }

            string query = @"
                INSERT INTO dbo.StagingTitelineAppCasing
                    ([Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], [HoleID], [CasingType], [CasingSize], [DepthFrom], [DepthTo], [DataSource])
                VALUES (@Pid, @PlodDate, @PlodShift, @ContractNo, @RigNo, @HoleID, @CasingType, @CasingSize, @DepthFrom, @DepthTo, @DataSource);";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();

                    foreach (var casing in casingList)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Pid", casing.Pid ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodDate", casing.PlodDate ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodShift", casing.PlodShift ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ContractNo", casing.ContractNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@RigNo", casing.RigNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@HoleID", casing.HoleID ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@CasingType", casing.CasingType ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@CasingSize", casing.CasingSize ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DepthFrom", casing.DepthFrom ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DepthTo", casing.DepthTo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DataSource", casing.DataSource ?? (object)DBNull.Value);

                            command.ExecuteNonQuery();
                        }
                    }
                }

                return Ok("Casing data added successfully.");
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
        [Route("GetCasingByPid/{pid}")]
        public IActionResult GetCasingByPid(int pid)
        {
            string query = @"
                SELECT 
                    [CasId], [Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], 
                    [HoleID], [CasingType], [CasingSize], [DepthFrom], [DepthTo], [DataSource]
                FROM dbo.StagingTitelineAppCasing
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
                    return NotFound(new { message = $"No casing records found for Pid: {pid}" });
                }

                var casingList = new List<Casing>();
                foreach (DataRow row in table.Rows)
                {
                    casingList.Add(new Casing
                    {
                        CasId = Convert.ToInt32(row["CasId"]),
                        Pid = row["Pid"] == DBNull.Value ? null : Convert.ToInt32(row["Pid"]),
                        PlodDate = row["PlodDate"].ToString(),
                        PlodShift = row["PlodShift"].ToString(),
                        ContractNo = row["ContractNo"].ToString(),
                        RigNo = row["RigNo"].ToString(),
                        HoleID = row["HoleID"].ToString(),
                        CasingType = row["CasingType"].ToString(),
                        CasingSize = row["CasingSize"].ToString(),
                        DepthFrom = row["DepthFrom"].ToString(),
                        DepthTo = row["DepthTo"].ToString(),
                        DataSource = row["DataSource"].ToString()
                    });
                }

                return Ok(casingList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("DeleteCasingByCasId/{casid}")]
        public IActionResult DeleteCasingByCasId(int casid)
        {
            string query = "DELETE FROM dbo.StagingTitelineAppCasing WHERE CasId = @CasId;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@CasId", casid);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Casing record with CasId {casid} deleted successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No casing record found for CasId: {casid}" });
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
        [Route("UpdateCasingByCasId/{casid}")]
        public IActionResult UpdateCasingByCasId(int casid, [FromBody] Casing updatedData)
        {
            if (updatedData == null)
            {
                return BadRequest(new { message = "Invalid casing data." });
            }

            string query = @"
                UPDATE dbo.StagingTitelineAppCasing
                SET 
                    HoleID = @HoleID,
                    CasingType = @CasingType,
                    CasingSize = @CasingSize,
                    DepthFrom = @DepthFrom,
                    DepthTo = @DepthTo
                WHERE CasId = @CasId;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@CasId", casid);
                        command.Parameters.AddWithValue("@HoleID", updatedData.HoleID ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CasingType", updatedData.CasingType ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CasingSize", updatedData.CasingSize ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@DepthFrom", updatedData.DepthFrom ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@DepthTo", updatedData.DepthTo ?? (object)DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Casing record with CasId {casid} updated successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No casing record found with CasId {casid}." });
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
