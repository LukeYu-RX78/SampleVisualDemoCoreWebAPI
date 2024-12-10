using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using SampleVisualDemoCoreWebAPI.Models.Entities;

namespace SampleVisualDemoCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitelineRigMovesController : Controller
    {
        private readonly IConfiguration _configuration;

        public TitelineRigMovesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("AddRigMove")]
        public IActionResult AddRigMove([FromBody] List<RigMove> rigMoveList)
        {
            if (rigMoveList == null || rigMoveList.Count == 0)
            {
                return BadRequest("RigMove list data is empty or invalid.");
            }

            string query = @"
                INSERT INTO dbo.StagingTitelineAppRigMoves
                    ([Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], [HoleID], [MoveType], [DataSource])
                VALUES (@Pid, @PlodDate, @PlodShift, @ContractNo, @RigNo, @HoleID, @MoveType, @DataSource);";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();

                    foreach (var rigMove in rigMoveList)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Pid", rigMove.Pid ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodDate", rigMove.PlodDate ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodShift", rigMove.PlodShift ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ContractNo", rigMove.ContractNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@RigNo", rigMove.RigNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@HoleID", rigMove.HoleID ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@MoveType", rigMove.MoveType ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DataSource", rigMove.DataSource ?? (object)DBNull.Value);

                            command.ExecuteNonQuery();
                        }
                    }
                }

                return Ok("RigMove data added successfully.");
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
        [Route("GetRigMoveByPid/{pid}")]
        public IActionResult GetRigMoveByPid(int pid)
        {
            string query = @"
                SELECT 
                    [RmId], [Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], 
                    [HoleID], [MoveType], [DataSource]
                FROM dbo.StagingTitelineAppRigMoves
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
                    return NotFound(new { message = $"No RigMove records found for Pid: {pid}" });
                }

                var rigMoveList = new List<RigMove>();
                foreach (DataRow row in table.Rows)
                {
                    rigMoveList.Add(new RigMove
                    {
                        RmId = Convert.ToInt32(row["RmId"]),
                        Pid = row["Pid"] == DBNull.Value ? null : Convert.ToInt32(row["Pid"]),
                        PlodDate = row["PlodDate"].ToString(),
                        PlodShift = row["PlodShift"].ToString(),
                        ContractNo = row["ContractNo"].ToString(),
                        RigNo = row["RigNo"].ToString(),
                        HoleID = row["HoleID"].ToString(),
                        MoveType = row["MoveType"].ToString(),
                        DataSource = row["DataSource"].ToString()
                    });
                }

                return Ok(rigMoveList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("DeleteRigMoveByRmid/{rmid}")]
        public IActionResult DeleteRigMoveByRmid(int rmid)
        {
            string query = "DELETE FROM dbo.StagingTitelineAppRigMoves WHERE RmId = @RmId;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@RmId", rmid);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"RigMove record with RmId {rmid} deleted successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No RigMove record found for RmId: {rmid}" });
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
        [Route("UpdateRigMoveByRmid/{rmid}")]
        public IActionResult UpdateRigMoveByRmid(int rmid, [FromBody] RigMove updatedData)
        {
            if (updatedData == null)
            {
                return BadRequest(new { message = "Invalid RigMove data." });
            }

            string query = @"
                UPDATE dbo.StagingTitelineAppRigMoves 
                SET 
                    HoleID = @HoleID,
                    MoveType = @MoveType
                WHERE RmId = @RmId;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@RmId", rmid);
                        command.Parameters.AddWithValue("@HoleID", updatedData.HoleID ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@MoveType", updatedData.MoveType ?? (object)DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"RigMove record with RmId {rmid} updated successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No RigMove record found with RmId {rmid}." });
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
