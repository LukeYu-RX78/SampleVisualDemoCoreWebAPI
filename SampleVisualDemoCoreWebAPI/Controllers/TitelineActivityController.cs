using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using SampleVisualDemoCoreWebAPI.Models.Entities;

namespace SampleVisualDemoCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitelineActivityController : Controller
    {
        private readonly IConfiguration _configuration;

        public TitelineActivityController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("AddActivity")]
        public IActionResult AddActivity([FromBody] List<Activity> activityList)
        {
            if (activityList == null || activityList.Count == 0)
            {
                return BadRequest("Activity list data is empty or invalid.");
            }

            string query = @"
                INSERT INTO dbo.StagingTitelineAppActivities
                    ([Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], [HoleID], [Activity], [TimeStart], [TimeFinish], [Hours], [DataSource])
                VALUES (@Pid, @PlodDate, @PlodShift, @ContractNo, @RigNo, @HoleID, @Activity, @TimeStart, @TimeFinish, @Hours, @DataSource);";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();

                    foreach (var activity in activityList)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Pid", activity.Pid ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodDate", activity.PlodDate ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodShift", activity.PlodShift ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ContractNo", activity.ContractNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@RigNo", activity.RigNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@HoleID", activity.HoleID ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Activity", activity.ActivityName ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@TimeStart", activity.TimeStart ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@TimeFinish", activity.TimeFinish ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Hours", activity.Hours ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DataSource", activity.DataSource ?? (object)DBNull.Value);

                            command.ExecuteNonQuery();
                        }
                    }
                }

                return Ok("Activity data added successfully.");
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
        [Route("GetActivityByPid/{pid}")]
        public IActionResult GetActivityByPid(int pid)
        {
            string query = @"
                SELECT 
                    [Actid], [Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], [HoleID],
                    [Activity], [TimeStart], [TimeFinish], [Hours], [DataSource]
                FROM dbo.StagingTitelineAppActivities
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
                    return NotFound(new { message = $"No activity records found for Pid: {pid}" });
                }

                var activityList = new List<Activity>();
                foreach (DataRow row in table.Rows)
                {
                    activityList.Add(new Activity
                    {
                        Actid = Convert.ToInt32(row["Actid"]),
                        Pid = row["Pid"] == DBNull.Value ? null : Convert.ToInt32(row["Pid"]),
                        PlodDate = row["PlodDate"].ToString(),
                        PlodShift = row["PlodShift"].ToString(),
                        ContractNo = row["ContractNo"].ToString(),
                        RigNo = row["RigNo"].ToString(),
                        HoleID = row["HoleID"].ToString(),
                        ActivityName = row["Activity"].ToString(),
                        TimeStart = row["TimeStart"].ToString(),
                        TimeFinish = row["TimeFinish"].ToString(),
                        Hours = row["Hours"].ToString(),
                        DataSource = row["DataSource"].ToString()
                    });
                }

                return Ok(activityList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("DeleteActivityByActid/{actid}")]
        public IActionResult DeleteActivityByActid(int actid)
        {
            string query = "DELETE FROM dbo.StagingTitelineAppActivities WHERE Actid = @Actid;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Actid", actid);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Activity record with Actid {actid} deleted successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No activity record found for Actid: {actid}" });
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
        [Route("UpdateActivityByActid/{actid}")]
        public IActionResult UpdateActivityByActid(int actid, [FromBody] Activity updatedData)
        {
            if (updatedData == null)
            {
                return BadRequest(new { message = "Invalid activity data." });
            }

            string query = @"
                UPDATE dbo.StagingTitelineAppActivities 
                SET 
                    HoleID = @HoleID,
                    Activity = @Activity,
                    TimeStart = @TimeStart,
                    TimeFinish = @TimeFinish,
                    Hours = @Hours
                WHERE Actid = @Actid;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Actid", actid);
                        command.Parameters.AddWithValue("@HoleID", updatedData.HoleID ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Activity", updatedData.ActivityName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@TimeStart", updatedData.TimeStart ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@TimeFinish", updatedData.TimeFinish ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Hours", updatedData.Hours ?? (object)DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Activity record with Actid {actid} updated successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No activity record found with Actid {actid}." });
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
