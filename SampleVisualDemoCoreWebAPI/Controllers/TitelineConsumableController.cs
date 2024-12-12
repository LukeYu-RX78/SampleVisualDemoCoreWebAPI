using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using SampleVisualDemoCoreWebAPI.Models.Entities;

namespace SampleVisualDemoCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitelineConsumableController : Controller
    {
        private readonly IConfiguration _configuration;

        public TitelineConsumableController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("AddConsumable")]
        public IActionResult AddConsumable([FromBody] List<Consumable> consumableList)
        {
            if (consumableList == null || consumableList.Count == 0)
            {
                return BadRequest("Consumable list data is empty or invalid.");
            }

            string query = @"
                INSERT INTO dbo.StagingTitelineAppConsumables
                    ([Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], [HoleID], [Item], [Quantity], [DataSource])
                VALUES (@Pid, @PlodDate, @PlodShift, @ContractNo, @RigNo, @HoleID, @Item, @Quantity, @DataSource);";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();

                    foreach (var consumable in consumableList)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Pid", consumable.Pid ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodDate", consumable.PlodDate ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodShift", consumable.PlodShift ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ContractNo", consumable.ContractNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@RigNo", consumable.RigNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@HoleID", consumable.HoleID ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Item", consumable.Item ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Quantity", consumable.Quantity ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DataSource", consumable.DataSource ?? (object)DBNull.Value);

                            command.ExecuteNonQuery();
                        }
                    }
                }

                return Ok("Consumable data added successfully.");
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
        [Route("GetConsumableByPid/{pid}")]
        public IActionResult GetConsumableByPid(int pid)
        {
            string query = @"
                SELECT 
                    [ConsId], [Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], 
                    [HoleID], [Item], [Quantity], [DataSource]
                FROM dbo.StagingTitelineAppConsumables
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
                    return NotFound(new { message = $"No consumable records found for Pid: {pid}" });
                }

                var consumableList = new List<Consumable>();
                foreach (DataRow row in table.Rows)
                {
                    consumableList.Add(new Consumable
                    {
                        ConsId = Convert.ToInt32(row["ConsId"]),
                        Pid = row["Pid"] == DBNull.Value ? null : Convert.ToInt32(row["Pid"]),
                        PlodDate = row["PlodDate"].ToString(),
                        PlodShift = row["PlodShift"].ToString(),
                        ContractNo = row["ContractNo"].ToString(),
                        RigNo = row["RigNo"].ToString(),
                        HoleID = row["HoleID"].ToString(),
                        Item = row["Item"].ToString(),
                        Quantity = row["Quantity"].ToString(),
                        DataSource = row["DataSource"].ToString()
                    });
                }

                return Ok(consumableList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("DeleteConsumableByConsid/{consid}")]
        public IActionResult DeleteConsumableByConsid(int consid)
        {
            string query = "DELETE FROM dbo.StagingTitelineAppConsumables WHERE ConsId = @ConsId;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@ConsId", consid);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Consumable record with ConsId {consid} deleted successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No consumable record found for ConsId: {consid}" });
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
        [Route("UpdateConsumableByConsid/{consid}")]
        public IActionResult UpdateConsumableByConsid(int consid, [FromBody] Consumable updatedData)
        {
            if (updatedData == null)
            {
                return BadRequest(new { message = "Invalid consumable data." });
            }

            string query = @"
                UPDATE dbo.StagingTitelineAppConsumables 
                SET 
                    HoleID = @HoleID,
                    Item = @Item,
                    Quantity = @Quantity
                WHERE ConsId = @ConsId;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@ConsId", consid);
                        command.Parameters.AddWithValue("@HoleID", updatedData.HoleID ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Item", updatedData.Item ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Quantity", updatedData.Quantity ?? (object)DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Consumable record with ConsId {consid} updated successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No consumable record found with ConsId {consid}." });
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
