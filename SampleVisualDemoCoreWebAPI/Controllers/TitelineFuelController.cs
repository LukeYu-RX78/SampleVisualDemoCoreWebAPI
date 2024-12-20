using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using SampleVisualDemoCoreWebAPI.Models.Entities;

namespace SampleVisualDemoCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitelineFuelController : Controller
    {
        private readonly IConfiguration _configuration;

        public TitelineFuelController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("AddFuel")]
        public IActionResult AddFuel([FromBody] List<Fuel> fuelList)
        {
            if (fuelList == null || fuelList.Count == 0)
            {
                return BadRequest("Fuel list data is empty or invalid.");
            }

            string query = @"
                INSERT INTO dbo.StagingTitelineAppFuel
                    ([Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], [Loads], [Kilometres], [Litres], [Hours], [Driver], [DataSource])
                VALUES (@Pid, @PlodDate, @PlodShift, @ContractNo, @RigNo, @Loads, @Kilometres, @Litres, @Hours, @Driver, @DataSource);";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();

                    foreach (var fuel in fuelList)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Pid", fuel.Pid ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodDate", fuel.PlodDate ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PlodShift", fuel.PlodShift ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ContractNo", fuel.ContractNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@RigNo", fuel.RigNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Loads", fuel.Loads ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Kilometres", fuel.Kilometres ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Litres", fuel.Litres ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Hours", fuel.Hours ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Driver", fuel.Driver ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@DataSource", fuel.DataSource ?? (object)DBNull.Value);

                            command.ExecuteNonQuery();
                        }
                    }
                }

                return Ok("Fuel data added successfully.");
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
        [Route("GetFuelByPid/{pid}")]
        public IActionResult GetFuelByPid(int pid)
        {
            string query = @"
                SELECT 
                    [Fid], [Pid], [PlodDate], [PlodShift], [ContractNo], [RigNo], [Loads], [Kilometres], [Litres], [Hours], [Driver], [DataSource]
                FROM dbo.StagingTitelineAppFuel
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
                    return NotFound(new { message = $"No fuel records found for Pid: {pid}" });
                }

                var fuelList = new List<Fuel>();
                foreach (DataRow row in table.Rows)
                {
                    fuelList.Add(new Fuel
                    {
                        Fid = Convert.ToInt32(row["Fid"]),
                        Pid = row["Pid"] == DBNull.Value ? null : Convert.ToInt32(row["Pid"]),
                        PlodDate = row["PlodDate"].ToString(),
                        PlodShift = row["PlodShift"].ToString(),
                        ContractNo = row["ContractNo"].ToString(),
                        RigNo = row["RigNo"].ToString(),
                        Loads = row["Loads"].ToString(),
                        Kilometres = row["Kilometres"].ToString(),
                        Litres = row["Litres"].ToString(),
                        Hours = row["Hours"].ToString(),
                        Driver = row["Driver"].ToString(),
                        DataSource = row["DataSource"].ToString()
                    });
                }

                return Ok(fuelList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("DeleteFuelByFid/{fid}")]
        public IActionResult DeleteFuelByFid(int fid)
        {
            string query = "DELETE FROM dbo.StagingTitelineAppFuel WHERE Fid = @Fid;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Fid", fid);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Fuel record with Fid {fid} deleted successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No fuel record found for Fid: {fid}" });
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
        [Route("UpdateFuelByFid/{fid}")]
        public IActionResult UpdateFuelByFid(int fid, [FromBody] Fuel updatedData)
        {
            if (updatedData == null)
            {
                return BadRequest(new { message = "Invalid fuel data." });
            }

            string query = @"
                UPDATE dbo.StagingTitelineAppFuel
                SET 
                    Loads = @Loads,
                    Kilometres = @Kilometres,
                    Litres = @Litres,
                    Hours = @Hours,
                    Driver = @Driver
                WHERE Fid = @Fid;";

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Fid", fid);
                        command.Parameters.AddWithValue("@Loads", updatedData.Loads ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Kilometres", updatedData.Kilometres ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Litres", updatedData.Litres ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Hours", updatedData.Hours ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Driver", updatedData.Driver ?? (object)DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Fuel record with Fid {fid} updated successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = $"No fuel record found with Fid {fid}." });
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