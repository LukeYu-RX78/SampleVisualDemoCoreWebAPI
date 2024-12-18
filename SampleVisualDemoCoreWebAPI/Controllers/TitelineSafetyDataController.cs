using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using SampleVisualDemoCoreWebAPI.Models.Entities;

namespace SampleVisualDemoCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitelineSafetyDataController : Controller
    {
        private readonly IConfiguration _configuration;

        public TitelineSafetyDataController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("AddSafetyData")]
        public IActionResult AddSafetyData([FromBody] List<SafetyData> safetyDataList)
        {
            if (safetyDataList == null || safetyDataList.Count == 0)
                return BadRequest("Safety Data list is empty or invalid.");

            string query = @"
                INSERT INTO dbo.StagingTitelineAppSafetyData
                    (Pid, PlodDate, PlodShift, ContractNo, RigNo, JHA, SBO, Hazards, SWMSReviews, Observations, 
                    OperationsIncidents, Incidents, ReportableIncidents, LostTimeInjuries, MedicalTreatedInjuries, 
                    SiteInspections, Comments, DataSource)
                VALUES (@Pid, @PlodDate, @PlodShift, @ContractNo, @RigNo, @JHA, @SBO, @Hazards, @SWMSReviews, @Observations, 
                    @OperationsIncidents, @Incidents, @ReportableIncidents, @LostTimeInjuries, @MedicalTreatedInjuries, 
                    @SiteInspections, @Comments, @DataSource);";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();

                    foreach (var data in safetyDataList)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Pid", data.Pid ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodDate", data.PlodDate ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodShift", data.PlodShift ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ContractNo", data.ContractNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@RigNo", data.RigNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@JHA", data.JHA ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@SBO", data.SBO ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Hazards", data.Hazards ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@SWMSReviews", data.SWMSReviews ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Observations", data.Observations ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@OperationsIncidents", data.OperationsIncidents ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Incidents", data.Incidents ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ReportableIncidents", data.ReportableIncidents ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@LostTimeInjuries", data.LostTimeInjuries ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@MedicalTreatedInjuries", data.MedicalTreatedInjuries ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@SiteInspections", data.SiteInspections ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Comments", data.Comments ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DataSource", data.DataSource ?? (object)DBNull.Value);

                            command.ExecuteNonQuery();
                        }
                    }
                }
                return Ok("Safety Data added successfully.");
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
        [Route("GetSafetyDataByPid/{pid}")]
        public IActionResult GetSafetyDataByPid(int pid)
        {
            string query = @"
                SELECT Sid, Pid, PlodDate, PlodShift, ContractNo, RigNo, JHA, SBO, Hazards, SWMSReviews, Observations, 
                    OperationsIncidents, Incidents, ReportableIncidents, LostTimeInjuries, MedicalTreatedInjuries, 
                    SiteInspections, Comments, DataSource
                FROM dbo.StagingTitelineAppSafetyData
                WHERE Pid = @Pid";

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
                    return NotFound($"No safety data found for Pid: {pid}");

                var safetyDataList = new List<SafetyData>();
                foreach (DataRow row in table.Rows)
                {
                    safetyDataList.Add(new SafetyData
                    {
                        Sid = Convert.ToInt32(row["Sid"]),
                        Pid = row["Pid"] as int?,
                        PlodDate = row["PlodDate"]?.ToString(),
                        PlodShift = row["PlodShift"]?.ToString(),
                        ContractNo = row["ContractNo"]?.ToString(),
                        RigNo = row["RigNo"]?.ToString(),
                        JHA = row["JHA"]?.ToString(),
                        SBO = row["SBO"]?.ToString(),
                        Hazards = row["Hazards"]?.ToString(),
                        SWMSReviews = row["SWMSReviews"]?.ToString(),
                        Observations = row["Observations"]?.ToString(),
                        OperationsIncidents = row["OperationsIncidents"]?.ToString(),
                        Incidents = row["Incidents"]?.ToString(),
                        ReportableIncidents = row["ReportableIncidents"]?.ToString(),
                        LostTimeInjuries = row["LostTimeInjuries"]?.ToString(),
                        MedicalTreatedInjuries = row["MedicalTreatedInjuries"]?.ToString(),
                        SiteInspections = row["SiteInspections"]?.ToString(),
                        Comments = row["Comments"]?.ToString(),
                        DataSource = row["DataSource"]?.ToString(),
                    });
                }

                return Ok(safetyDataList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("DeleteSafetyDataBySid/{sid}")]
        public IActionResult DeleteSafetyDataBySid(int sid)
        {
            string query = "DELETE FROM dbo.StagingTitelineAppSafetyData WHERE Sid = @Sid";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Sid", sid);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Safety data with Sid {sid} deleted successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No safety data found for Sid: {sid}" });
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
        [Route("UpdateSafetyDataBySid/{sid}")]
        public IActionResult UpdateSafetyDataBySid(int sid, [FromBody] SafetyData updatedData)
        {

            if (updatedData == null)
            {
                return BadRequest(new { message = "Invalid safety data." });
            }

            string query = @"
                UPDATE dbo.StagingTitelineAppSafetyData
                SET JHA = @JHA, SBO = @SBO, Hazards = @Hazards, SWMSReviews = @SWMSReviews, 
                    Observations = @Observations, OperationsIncidents = @OperationsIncidents, 
                    Incidents = @Incidents, ReportableIncidents = @ReportableIncidents, 
                    LostTimeInjuries = @LostTimeInjuries, MedicalTreatedInjuries = @MedicalTreatedInjuries,
                    SiteInspections = @SiteInspections, Comments = @Comments
                WHERE Sid = @Sid";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Sid", sid);
                        command.Parameters.AddWithValue("@JHA", updatedData.JHA ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@SBO", updatedData.SBO ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Hazards", updatedData.Hazards ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@SWMSReviews", updatedData.SWMSReviews ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Observations", updatedData.Observations ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@OperationsIncidents", updatedData.OperationsIncidents ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Incidents", updatedData.Incidents ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ReportableIncidents", updatedData.ReportableIncidents ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@LostTimeInjuries", updatedData.LostTimeInjuries ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@MedicalTreatedInjuries", updatedData.MedicalTreatedInjuries ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@SiteInspections", updatedData.SiteInspections ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Comments", updatedData.Comments ?? (object)DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                            return Ok(new { message = $"Safety data with Sid {sid} updated successfully." });
                        else
                            return NotFound(new { message =  $"No safety data found for Sid: {sid}" });
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
