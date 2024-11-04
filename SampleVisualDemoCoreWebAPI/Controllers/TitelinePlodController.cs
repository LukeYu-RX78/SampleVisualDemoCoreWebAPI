using dors_backend.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SampleVisualDemoCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitelinePlodController : ControllerBase
    {
        private IConfiguration _configuration;

        public TitelinePlodController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetPlods")]
        public JsonResult GetPlods()
        {
            string query = "select top 10 * from dbo.plod where ContractNo = 'CW2262484_2024' order by PlodID desc";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");
            SqlDataReader sqlReader;
            using (SqlConnection conn = new SqlConnection(sqlDatasource))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    sqlReader = command.ExecuteReader();
                    table.Load(sqlReader);
                    sqlReader.Close();
                    conn.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpGet]
        [Route("GetStagingPlods")]
        public JsonResult GetStagingPlods()
        {
            string query = "select * from dbo.StagingTitelineAppPlod where ContractNo = 'CW2262484_2024' order by PlodDate desc";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");
            SqlDataReader sqlReader;
            using (SqlConnection conn = new SqlConnection(sqlDatasource))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    sqlReader = command.ExecuteReader();
                    table.Load(sqlReader);
                    sqlReader.Close();
                    conn.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpGet]
        [Route("GetStagingPlodsByAid")]
        public JsonResult GetStagingPlodsByAuthLv(int aid)
        {
  
            string query = "select * from dbo.StagingTitelineAppPlod where SendTo = '" + aid.ToString() + "' order by ReportState desc, Pid";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");
            SqlDataReader sqlReader;
            using (SqlConnection conn = new SqlConnection(sqlDatasource))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    sqlReader = command.ExecuteReader();
                    table.Load(sqlReader);
                    sqlReader.Close();
                    conn.Close();
                }
            }
            return new JsonResult(table);
        }


        [HttpPost]
        [Route("AddStagingPlod")]
        public JsonResult AddStagingPlod([FromBody] string newPlod)
        {
            string coulums = "([PlodDate], [PlodShift], [ContractNo], [RigNo], [Department], [Client], [DayType], [Location], "
                + "[Comments], [MachineHoursFrom], [MachineHoursTo], [TotalMetres], [DrillingHrs], [MetresperDrillingHr], "
                + "[TotalActivityHrs], [MetresperTotalHr], [VersionNumber], [DataSource], [SourceFrom], [SendTo], [ReportState])";
            string query = "insert into dbo.StagingTitelineAppPlod " + coulums + " Values(" + newPlod + ")";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        int affectedRows = command.ExecuteNonQuery();
                        return new JsonResult("Added Successfully");
                    }
                }
            }
            catch (SqlException ex)
            {
                return new JsonResult($"SQL error occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new JsonResult(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("UpdateStagingPlodDDRState")]
        public async Task<IActionResult> UpdateStagingPlodDDRState(int pid, bool isApprove)
        {
            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    await conn.OpenAsync();

                    string updateQuery;
                    List<SqlParameter> parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@Pid", pid)
                    };

                    if (isApprove)
                    {
                        string getSuperiorQuery = "SELECT Superior FROM AppAccount WHERE Aid = (SELECT SendTo FROM StagingTitelineAppPlod WHERE Pid = @Pid)";
                        object superior;

                        using (SqlCommand getSuperiorCmd = new SqlCommand(getSuperiorQuery, conn))
                        {
                            getSuperiorCmd.Parameters.AddWithValue("@Pid", pid);
                            var result = await getSuperiorCmd.ExecuteScalarAsync();
                            superior = result ?? DBNull.Value;
                        }
                   
                        updateQuery = @"
                            UPDATE dbo.StagingTitelineAppPlod 
                            SET 
                                ReportState = ReportState + 2, 
                                SourceFrom = SendTo, 
                                SendTo = @SuperiorAid
                            WHERE 
                                Pid = @Pid";
                        parameters.Add(new SqlParameter("@SuperiorAid", superior));
                    }
                    else
                    {
                        updateQuery = @"
                            UPDATE dbo.StagingTitelineAppPlod 
                            SET 
                                ReportState = ReportState + 1, 
                                SourceFrom = SendTo, 
                                SendTo = SourceFrom
                            WHERE 
                                Pid = @Pid";
                    }
             
                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddRange(parameters.ToArray());
                        int rowsAffected = await updateCmd.ExecuteNonQueryAsync();
                        return new JsonResult($"Updated Successfully, Rows affected: {rowsAffected}");
                    }
                }
            }
            catch (SqlException ex)
            {
                return new JsonResult($"SQL error occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new JsonResult(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("UpdateStagingPlod")]
        public async Task<IActionResult> UpdateStagingPlod([FromBody] StagingAppPlod updatedPlod)
        {
            if (updatedPlod == null || updatedPlod.Pid <= 0)
            {
                return BadRequest("Invalid data.");
            }

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    await conn.OpenAsync();

                    string selectQuery = "SELECT * FROM dbo.StagingTitelineAppPlod WHERE Pid = @Pid";
                    using (SqlCommand selectCmd = new SqlCommand(selectQuery, conn))
                    {
                        selectCmd.Parameters.AddWithValue("@Pid", updatedPlod.Pid);
                        using (SqlDataReader reader = await selectCmd.ExecuteReaderAsync())
                        {
                            if (!reader.HasRows)
                            {
                                return NotFound("Record not found.");
                            }

                            await reader.ReadAsync();

                            updatedPlod.PlodDate = updatedPlod.PlodDate ?? reader["PlodDate"].ToString();
                            updatedPlod.PlodShift = updatedPlod.PlodShift ?? reader["PlodShift"].ToString();
                            updatedPlod.ContractNo = updatedPlod.ContractNo ?? reader["ContractNo"].ToString();
                            updatedPlod.RigNo = updatedPlod.RigNo ?? reader["RigNo"].ToString();
                            updatedPlod.Department = updatedPlod.Department ?? reader["Department"].ToString();
                            updatedPlod.Client = updatedPlod.Client ?? reader["Client"].ToString();
                            updatedPlod.DayType = updatedPlod.DayType ?? reader["DayType"].ToString();
                            updatedPlod.Location = updatedPlod.Location ?? reader["Location"].ToString();
                            updatedPlod.Comments = updatedPlod.Comments ?? reader["Comments"].ToString();
                            updatedPlod.MachineHoursFrom = updatedPlod.MachineHoursFrom ?? reader["MachineHoursFrom"].ToString();
                            updatedPlod.MachineHoursTo = updatedPlod.MachineHoursTo ?? reader["MachineHoursTo"].ToString();
                            updatedPlod.TotalMetres = updatedPlod.TotalMetres ?? reader["TotalMetres"].ToString();
                            updatedPlod.DrillingHrs = updatedPlod.DrillingHrs ?? reader["DrillingHrs"].ToString();
                            updatedPlod.MetresperDrillingHr = updatedPlod.MetresperDrillingHr ?? reader["MetresperDrillingHr"].ToString();
                            updatedPlod.TotalActivityHrs = updatedPlod.TotalActivityHrs ?? reader["TotalActivityHrs"].ToString();
                            updatedPlod.MetresperTotalHr = updatedPlod.MetresperTotalHr ?? reader["MetresperTotalHr"].ToString();
                            updatedPlod.VersionNumber = updatedPlod.VersionNumber ?? reader["VersionNumber"].ToString();
                            updatedPlod.DataSource = updatedPlod.DataSource ?? reader["DataSource"].ToString();
                            updatedPlod.SourceFrom = updatedPlod.SourceFrom ?? reader["SourceFrom"].ToString();
                            updatedPlod.SendTo = updatedPlod.SendTo ?? reader["SendTo"].ToString();
                            updatedPlod.ReportState = updatedPlod.ReportState ?? reader["ReportState"].ToString();
                        }
                    }

                    List<string> setClauses = new List<string>();
                    List<SqlParameter> parameters = new List<SqlParameter>();

                    void AddParameter(string fieldName, object? fieldValue)
                    {
                        // Use DBNull.Value for NULL or empty strings
                        if (fieldValue == null || string.IsNullOrEmpty(fieldValue.ToString()))
                        {
                            parameters.Add(new SqlParameter($"@{fieldName}", DBNull.Value));
                        }
                        else
                        {
                            parameters.Add(new SqlParameter($"@{fieldName}", fieldValue));
                        }
                        setClauses.Add($"{fieldName} = @{fieldName}");
                    }

                    AddParameter("PlodDate", updatedPlod.PlodDate);
                    AddParameter("PlodShift", updatedPlod.PlodShift);
                    AddParameter("ContractNo", updatedPlod.ContractNo);
                    AddParameter("RigNo", updatedPlod.RigNo);
                    AddParameter("Department", updatedPlod.Department);
                    AddParameter("Client", updatedPlod.Client);
                    AddParameter("DayType", updatedPlod.DayType);
                    AddParameter("Location", updatedPlod.Location);
                    AddParameter("Comments", updatedPlod.Comments);
                    AddParameter("MachineHoursFrom", updatedPlod.MachineHoursFrom);
                    AddParameter("MachineHoursTo", updatedPlod.MachineHoursTo);
                    AddParameter("TotalMetres", updatedPlod.TotalMetres);
                    AddParameter("DrillingHrs", updatedPlod.DrillingHrs);
                    AddParameter("MetresperDrillingHr", updatedPlod.MetresperDrillingHr);
                    AddParameter("TotalActivityHrs", updatedPlod.TotalActivityHrs);
                    AddParameter("MetresperTotalHr", updatedPlod.MetresperTotalHr);
                    AddParameter("VersionNumber", updatedPlod.VersionNumber);
                    AddParameter("DataSource", updatedPlod.DataSource);
                    AddParameter("SourceFrom", updatedPlod.SourceFrom);
                    AddParameter("SendTo", updatedPlod.SendTo);
                    AddParameter("ReportState", updatedPlod.ReportState);

                    string setClause = string.Join(", ", setClauses);
                    string updateQuery = $"UPDATE dbo.StagingTitelineAppPlod SET {setClause} WHERE Pid = @Pid";
                    parameters.Add(new SqlParameter("@Pid", updatedPlod.Pid));

                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddRange(parameters.ToArray());
                        int rowsAffected = await updateCmd.ExecuteNonQueryAsync();
                        return new JsonResult($"Updated Successfully, Rows affected: {rowsAffected}");
                    }
                }
            }
            catch (SqlException ex)
            {
                return new JsonResult($"SQL error occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new JsonResult(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }


        [HttpDelete]
        [Route("DeleteStagingPlod/{pid}")]
        public async Task<IActionResult> DeleteStagingPlod(int pid)
        {
            if (pid <= 0)
            {
                return BadRequest("Invalid Pid.");
            }

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    await conn.OpenAsync();
                    string deleteQuery = "DELETE FROM dbo.StagingTitelineAppPlod WHERE Pid = @Pid";

                    using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@Pid", pid);
                        int rowsAffected = await deleteCmd.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
                        {
                            return NotFound("No record found with the specified Pid.");
                        }

                        return new JsonResult($"Deleted Successfully, Rows affected: {rowsAffected}");
                    }
                }
            }
            catch (SqlException ex)
            {
                return new JsonResult($"SQL error occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new JsonResult(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

    }
}
