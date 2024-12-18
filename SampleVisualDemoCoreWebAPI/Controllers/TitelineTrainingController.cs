using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using SampleVisualDemoCoreWebAPI.Models.Entities;

namespace SampleVisualDemoCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitelineTrainingController : Controller
    {
        private readonly IConfiguration _configuration;

        public TitelineTrainingController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("AddTraining")]
        public IActionResult AddTraining([FromBody] List<Training> trainingList)
        {
            if (trainingList == null || trainingList.Count == 0)
            {
                return BadRequest("Training list data is empty or invalid.");
            }

            string query = @"
                INSERT INTO dbo.StagingTitelineAppTraining
                    ([Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], [Details], [Instructor], [Trainee], [Time], [DataSource])
                VALUES (@Pid, @PlodDate, @PlodShift, @ContractNo, @RigNo, @Details, @Instructor, @Trainee, @Time, @DataSource);";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();

                    foreach (var training in trainingList)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Pid", training.Pid ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodDate", training.PlodDate ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodShift", training.PlodShift ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ContractNo", training.ContractNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@RigNo", training.RigNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Details", training.Details ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Instructor", training.Instructor ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Trainee", training.Trainee ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Time", training.Time ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DataSource", training.DataSource ?? (object)DBNull.Value);

                            command.ExecuteNonQuery();
                        }
                    }
                }

                return Ok("Training data added successfully.");
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
        [Route("GetTrainingByPid/{pid}")]
        public IActionResult GetTrainingByPid(int pid)
        {
            string query = @"
                SELECT 
                    [TrId], [Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], 
                    [Details], [Instructor], [Trainee], [Time], [DataSource]
                FROM dbo.StagingTitelineAppTraining
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
                    return NotFound(new { message = $"No training records found for Pid: {pid}" });
                }

                var trainingList = new List<Training>();
                foreach (DataRow row in table.Rows)
                {
                    trainingList.Add(new Training
                    {
                        TrId = Convert.ToInt32(row["TrId"]),
                        Pid = row["Pid"] == DBNull.Value ? null : Convert.ToInt32(row["Pid"]),
                        PlodDate = row["PlodDate"].ToString(),
                        PlodShift = row["PlodShift"].ToString(),
                        ContractNo = row["ContractNo"].ToString(),
                        RigNo = row["RigNo"].ToString(),
                        Details = row["Details"].ToString(),
                        Instructor = row["Instructor"].ToString(),
                        Trainee = row["Trainee"].ToString(),
                        Time = row["Time"].ToString(),
                        DataSource = row["DataSource"].ToString(),
                    });
                }

                return Ok(trainingList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("DeleteTrainingByTrId/{trid}")]
        public IActionResult DeleteTrainingByTrId(int trid)
        {
            string query = "DELETE FROM dbo.StagingTitelineAppTraining WHERE TrId = @TrId;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@TrId", trid);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Training record with TrId {trid} deleted successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No training record found for TrId: {trid}" });
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
        [Route("UpdateTrainingByTrId/{trid}")]
        public IActionResult UpdateTrainingByTrId(int trid, [FromBody] Training updatedData)
        {
            if (updatedData == null)
            {
                return BadRequest(new { message = "Invalid training data." });
            }

            string query = @"
                UPDATE dbo.StagingTitelineAppTraining
                SET 
                    Details = @Details,
                    Instructor = @Instructor,
                    Trainee = @Trainee,
                    Time = @Time
                WHERE TrId = @TrId;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@TrId", trid);
                        command.Parameters.AddWithValue("@Details", updatedData.Details ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Instructor", updatedData.Instructor ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Trainee", updatedData.Trainee ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Time", updatedData.Time ?? (object)DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Training record with TrId {trid} updated successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No training record found with TrId {trid}." });
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
