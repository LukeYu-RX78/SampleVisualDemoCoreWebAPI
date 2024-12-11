using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using SampleVisualDemoCoreWebAPI.Models.Entities;

namespace SampleVisualDemoCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitelineDHSurveyController : Controller
    {
        private readonly IConfiguration _configuration;

        public TitelineDHSurveyController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("AddDHSurvey")]
        public IActionResult AddDHSurvey([FromBody] List<DHSurvey> dhSurveyList)
        {
            if (dhSurveyList == null || dhSurveyList.Count == 0)
            {
                return BadRequest("DHSurvey list data is empty or invalid.");
            }

            string query = @"
                INSERT INTO dbo.StagingTitelineAppDHSurvey
                    ([Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], [HoleID], [Depth], 
                    [ToolType], [ToolNo], [Dip], [Azimuth], [AziType], [MagInt], [MagDip], 
                    [GravRoll], [Temp], [DataSource])
                VALUES (@Pid, @PlodDate, @PlodShift, @ContractNo, @RigNo, @HoleID, @Depth, 
                        @ToolType, @ToolNo, @Dip, @Azimuth, @AziType, @MagInt, @MagDip, 
                        @GravRoll, @Temp, @DataSource);";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();

                    foreach (var survey in dhSurveyList)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Pid", survey.Pid ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodDate", survey.PlodDate ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodShift", survey.PlodShift ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ContractNo", survey.ContractNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@RigNo", survey.RigNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@HoleID", survey.HoleID ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Depth", survey.Depth ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ToolType", survey.ToolType ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ToolNo", survey.ToolNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Dip", survey.Dip ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Azimuth", survey.Azimuth ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@AziType", survey.AziType ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@MagInt", survey.MagInt ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@MagDip", survey.MagDip ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@GravRoll", survey.GravRoll ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Temp", survey.Temp ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DataSource", survey.DataSource ?? (object)DBNull.Value);

                            command.ExecuteNonQuery();
                        }
                    }
                }

                return Ok("DHSurvey data added successfully.");
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
        [Route("GetDHSurveyByPid/{pid}")]
        public IActionResult GetDHSurveyByPid(int pid)
        {
            string query = @"
                SELECT * FROM dbo.StagingTitelineAppDHSurvey
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
                    return NotFound(new { message = $"No DHSurvey records found for Pid: {pid}" });
                }

                var dhSurveyList = new List<DHSurvey>();
                foreach (DataRow row in table.Rows)
                {
                    dhSurveyList.Add(new DHSurvey
                    {
                        DhsId = Convert.ToInt32(row["DhsId"]),
                        Pid = row["Pid"] == DBNull.Value ? null : Convert.ToInt32(row["Pid"]),
                        PlodDate = row["PlodDate"].ToString(),
                        PlodShift = row["PlodShift"].ToString(),
                        ContractNo = row["ContractNo"].ToString(),
                        RigNo = row["RigNo"].ToString(),
                        HoleID = row["HoleID"].ToString(),
                        Depth = row["Depth"].ToString(),
                        ToolType = row["ToolType"].ToString(),
                        ToolNo = row["ToolNo"].ToString(),
                        Dip = row["Dip"].ToString(),
                        Azimuth = row["Azimuth"].ToString(),
                        AziType = row["AziType"].ToString(),
                        MagInt = row["MagInt"].ToString(),
                        MagDip = row["MagDip"].ToString(),
                        GravRoll = row["GravRoll"].ToString(),
                        Temp = row["Temp"].ToString(),
                        DataSource = row["DataSource"].ToString()
                    });
                }

                return Ok(dhSurveyList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("DeleteDHSurveyByDhsId/{dhsId}")]
        public IActionResult DeleteDHSurveyByDhsId(int dhsId)
        {
            string query = "DELETE FROM dbo.StagingTitelineAppDHSurvey WHERE DhsId = @DhsId;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@DhsId", dhsId);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"DHSurvey record with DhsId {dhsId} deleted successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No DHSurvey record found for DhsId: {dhsId}" });
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
        [Route("UpdateDHSurveyByDhsId/{dhsId}")]
        public IActionResult UpdateDHSurveyByDhsId(int dhsId, [FromBody] DHSurvey updatedData)
        {
            if (updatedData == null)
            {
                return BadRequest(new { message = "Invalid DHSurvey data." });
            }

            string query = @"
                UPDATE dbo.StagingTitelineAppDHSurvey 
                SET 
                    HoleID = @HoleID,
                    Depth = @Depth,
                    ToolType = @ToolType,
                    ToolNo = @ToolNo,
                    Dip = @Dip,
                    Azimuth = @Azimuth,
                    AziType = @AziType,
                    MagInt = @MagInt,
                    MagDip = @MagDip,
                    GravRoll = @GravRoll,
                    Temp = @Temp
                WHERE DhsId = @DhsId;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@DhsId", dhsId);
                        command.Parameters.AddWithValue("@HoleID", updatedData.HoleID ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Depth", updatedData.Depth ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ToolType", updatedData.ToolType ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ToolNo", updatedData.ToolNo ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Dip", updatedData.Dip ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Azimuth", updatedData.Azimuth ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@AziType", updatedData.AziType ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@MagInt", updatedData.MagInt ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@MagDip", updatedData.MagDip ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@GravRoll", updatedData.GravRoll ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Temp", updatedData.Temp ?? (object)DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"DHSurvey record with DhsId {dhsId} updated successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No DHSurvey record found with DhsId {dhsId}." });
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