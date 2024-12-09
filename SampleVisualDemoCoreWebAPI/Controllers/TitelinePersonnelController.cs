using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Collections.Generic;
using SampleVisualDemoCoreWebAPI.Models.Entities;
using System.Data;

namespace SampleVisualDemoCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitelinePersonnelController : ControllerBase
    {
        private IConfiguration _configuration;

        public TitelinePersonnelController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("AddPersonnel")]
        public IActionResult AddPersonnel([FromBody] List<Personnel> personnelList)
        {
            if (personnelList == null || personnelList.Count == 0)
            {
                return BadRequest("Personnel data is empty or invalid.");
            }

            string query = @"
                INSERT INTO dbo.StagingTitelineAppPersonnel 
                ([Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], [Position], [Name], [Hours], [TimeStart], [TimeFinish], [TravelDay], [SickDay], [DataSource])
                VALUES (@Pid, @PlodDate, @PlodShift, @ContractNo, @RigNo, @Position, @Name, @Hours, @TimeStart, @TimeFinish, @TravelDay, @SickDay, @DataSource);";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();

                    foreach (var personnel in personnelList)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Pid", personnel.Pid ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodDate", personnel.PlodDate ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodShift", personnel.PlodShift ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ContractNo", personnel.ContractNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@RigNo", personnel.RigNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Position", personnel.Position ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Name", personnel.Name ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Hours", personnel.Hours ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@TimeStart", personnel.TimeStart ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@TimeFinish", personnel.TimeFinish ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@TravelDay", personnel.TravelDay ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@SickDay", personnel.SickDay ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DataSource", personnel.DataSource ?? (object)DBNull.Value);

                            command.ExecuteNonQuery();
                        }
                    }
                }

                return Ok("Personnel data added successfully.");
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
        [Route("GetPersonnelByPid/{pid}")]
        public IActionResult GetPersonnelByPid(int pid)
        {
            string query = @"
            SELECT 
                [PerId], [Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], 
                [Position], [Name], [Hours], [TimeStart], [TimeFinish], 
                [TravelDay], [SickDay], [DataSource]
            FROM dbo.StagingTitelineAppPersonnel
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
                    return NotFound(new { message = $"No personnel records found for Pid: {pid}" });
                }

                var personnelList = new List<Personnel>();
                foreach (DataRow row in table.Rows)
                {
                    personnelList.Add(new Personnel
                    {
                        PerId = Convert.ToInt32(row["PerId"]),
                        Pid = Convert.ToInt32(row["Pid"]),
                        PlodDate = row["PlodDate"].ToString(),
                        PlodShift = row["PlodShift"].ToString(),
                        ContractNo = row["ContractNo"].ToString(),
                        RigNo = row["RigNo"].ToString(),
                        Position = row["Position"].ToString(),
                        Name = row["Name"].ToString(),
                        Hours = row["Hours"].ToString(),
                        TimeStart = row["TimeStart"].ToString(),
                        TimeFinish = row["TimeFinish"].ToString(),
                        TravelDay = row["TravelDay"].ToString(),
                        SickDay = row["SickDay"].ToString(),
                        DataSource = row["DataSource"].ToString()
                    });
                }

                return Ok(personnelList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("DeletePersonnelByPid/{perId}")]
        public IActionResult DeletePersonnelByPid(int perId)
        {
            string query = "DELETE FROM dbo.StagingTitelineAppPersonnel WHERE PerId = @PerId;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@PerId", perId);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Personnel records for PerId {perId} deleted successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No personnel records found for Pid: {perId}" });
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
        [Route("UpdatePersonnelByPid/{perId}")]
        public IActionResult UpdatePersonnelByPid(int perId, [FromBody] Personnel updatedData)
        {
            if (updatedData == null)
            {
                return BadRequest(new { message = "Invalid personnel data." });
            }

            if (string.IsNullOrEmpty(updatedData.Name) || string.IsNullOrEmpty(updatedData.Hours))
            {
                return BadRequest(new { message = "Name and Hours are required fields." });
            }

            string query = @"
            UPDATE dbo.StagingTitelineAppPersonnel 
            SET 
                Position = @Position,
                Name = @Name,
                Hours = @Hours,
                TimeStart = @TimeStart,
                TimeFinish = @TimeFinish,
                TravelDay = @TravelDay,
                SickDay = @SickDay
            WHERE PerId = @PerId;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        // Bind parameters to prevent SQL injection
                        command.Parameters.AddWithValue("@PerId", perId);
                        command.Parameters.AddWithValue("@Position", updatedData.Position ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Name", updatedData.Name ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Hours", updatedData.Hours ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@TimeStart", updatedData.TimeStart ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@TimeFinish", updatedData.TimeFinish ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@TravelDay", updatedData.TravelDay ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@SickDay", updatedData.SickDay ?? (object)DBNull.Value);

                        // Execute the query
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Personnel record with PerId {perId} updated successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No personnel record found with PerId {perId}." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
