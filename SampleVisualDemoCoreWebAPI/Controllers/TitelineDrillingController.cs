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
    public class TitelineDrillingController : Controller
    {
        private IConfiguration _configuration;

        public TitelineDrillingController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpPost]
        [Route("AddDrilling")]
        public IActionResult AddDrilling([FromBody] List<Drilling> drillingList)
        {
            if (drillingList == null || drillingList.Count == 0)
            {
                return BadRequest("DrillingList data is empty or invalid.");
            }

            string query = @"
                INSERT INTO dbo.StagingTitelineAppDrilling 
                    ([Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], [HoleID], [Angle], [DrillType], [Size], [DepthFrom], [DepthTo], 
                     [TotalMetres], [Barrel], [RecoveredMetres], [DCIMetres], [NonChargeableMetres], [DataSource])
                VALUES  (@Pid, @PlodDate, @PlodShift, @ContractNo, @RigNo, @HoleID, @Angle, @DrillType, @Size, @DepthFrom, @DepthTo, 
                     @TotalMetres, @Barrel, @RecoveredMetres, @DCIMetres, @NonChargeableMetres, @DataSource);";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();

                    foreach (var drilling in drillingList)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Pid", drilling.Pid ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodDate", drilling.PlodDate ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodShift", drilling.PlodShift ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ContractNo", drilling.ContractNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@RigNo", drilling.RigNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@HoleID", drilling.HoleID ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Angle", drilling.Angle ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DrillType", drilling.DrillType ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Size", drilling.Size ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DepthFrom", drilling.DepthFrom ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DepthTo", drilling.DepthTo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@TotalMetres", drilling.TotalMetres ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Barrel", drilling.Barrel ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@RecoveredMetres", drilling.RecoveredMetres ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DCIMetres", drilling.DCIMetres ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@NonChargeableMetres", drilling.NonChargeableMetres ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DataSource", drilling.DataSource ?? (object)DBNull.Value);

                            command.ExecuteNonQuery();
                        }
                    }
                }

                return Ok("Drilling data added successfully.");
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
        [Route("GetDrillingByPid/{pid}")]
        public IActionResult GetDrillingByPid(int pid)
        {
            string query = @"
                SELECT 
                    [Did], [Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], [HoleID], [Angle], [DrillType], [Size], [DepthFrom], 
                    [DepthTo], [TotalMetres], [Barrel], [RecoveredMetres], [DCIMetres], [NonChargeableMetres], [DataSource]
                FROM dbo.StagingTitelineAppDrilling
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
                    return NotFound(new { message = $"No drilling records found for Pid: {pid}" });
                }

                var drillingList = new List<Drilling>();
                foreach (DataRow row in table.Rows)
                {
                    drillingList.Add(new Drilling
                    {
                        Did = Convert.ToInt32(row["Did"]),
                        Pid = Convert.ToInt32(row["Pid"]),
                        PlodDate = row["PlodDate"].ToString(),
                        PlodShift = row["PlodShift"].ToString(),
                        ContractNo = row["ContractNo"].ToString(),
                        RigNo = row["RigNo"].ToString(),
                        HoleID = row["HoleID"].ToString(),
                        Angle = row["Angle"].ToString(),
                        DrillType = row["DrillType"].ToString(),
                        Size = row["Size"].ToString(),
                        DepthFrom = row["DepthFrom"].ToString(),
                        DepthTo = row["DepthTo"].ToString(),
                        TotalMetres = row["TotalMetres"].ToString(),
                        Barrel = row["Barrel"].ToString(),
                        RecoveredMetres = row["RecoveredMetres"].ToString(),
                        DCIMetres = row["DCIMetres"].ToString(),
                        NonChargeableMetres = row["NonChargeableMetres"].ToString(),
                        DataSource = row["DataSource"].ToString()
                    });
                }

                return Ok(drillingList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("DeleteDrillingByDid/{did}")]
        public IActionResult DeleteDrillingByDid(int did)
        {
            string query = "DELETE FROM dbo.StagingTitelineAppDrilling WHERE Did = @Did;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Did", did);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Drilling record with Did {did} deleted successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No drilling record found for Did: {did}" });
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
        [Route("UpdateDrillingByDid/{did}")]
        public IActionResult UpdateDrillingByDid(int did, [FromBody] Drilling updatedData)
        {
            if (updatedData == null)
            {
                return BadRequest(new { message = "Invalid drilling data." });
            }

            string query = @"
                UPDATE dbo.StagingTitelineAppDrilling 
                SET 
                    HoleID = @HoleID,
                    Angle = @Angle,
                    DrillType = @DrillType,
                    Size = @Size,
                    DepthFrom = @DepthFrom,
                    DepthTo = @DepthTo,
                    TotalMetres = @TotalMetres,
                    Barrel = @Barrel,
                    RecoveredMetres = @RecoveredMetres,
                    DCIMetres = @DCIMetres,
                    NonChargeableMetres = @NonChargeableMetres
                WHERE Did = @Did;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Did", did);
                        command.Parameters.AddWithValue("@HoleID", updatedData.HoleID ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Angle", updatedData.Angle ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@DrillType", updatedData.DrillType ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Size", updatedData.Size ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@DepthFrom", updatedData.DepthFrom ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@DepthTo", updatedData.DepthTo ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@TotalMetres", updatedData.TotalMetres ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Barrel", updatedData.Barrel ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@RecoveredMetres", updatedData.RecoveredMetres ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@DCIMetres", updatedData.DCIMetres ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@NonChargeableMetres", updatedData.NonChargeableMetres ?? (object)DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Drilling record with Did {did} updated successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No drilling record found with Did {did}." });
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