using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.AccessControl;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;

namespace SampleVisualDemoCoreWebAPI.Controllers
{

    //All the api functions could be check with Postman, with the url 'https://{IP of current}:7057/api/SampleVisual/{Method Name in Route}'
    //After deploy the porject with a certain domain name, the url would like 'https://earthsql.domainname/api/SampleVisual/{Method Name in Route}'
    [Route("api/[controller]")]
    [ApiController]
    public class SampleVisualController : ControllerBase
    {
        private IConfiguration _configuration;
        
        public SampleVisualController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //Select all data from the StagingRCSamples table
        [HttpGet]
        [Route("GetStagingSamples")]
        public JsonResult GetStagingSamples()
        {
            string query = "Select * from StagingRCSamples";
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

        //Adding new rows to the StagingRCSamples table
        [HttpPost]
        [Route("AddNewSample")]
        public JsonResult AddNewSample([FromBody] string newSample)
        {
            string coulums = "([SourceFile], [HoleID], [mFrom], [mTo], " +
                "[Width], [CheckType], [SampleType], [SampleMethod], " +
                "[SampleID], [ParentSampleID], [ParentCheckType], [StandardID], " +
                "[SampleQuality], [SampleCondition], [PreSplitMass_kg], [SplitMass_kg], " +
                "[Priority], [Exclude], [SamplingComments], [DateLoaded], [LoadedBy])";
            string query = "insert into dbo.StagingRCSamples " + coulums + " Values(" + newSample + ")";
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

        //Delet the data base on SampleID
        [HttpDelete]
        [Route("DeleteBySampleID")]
        public JsonResult DelBySampleID([FromQuery]String id)
        {
            string query = "delete from dbo.StagingRCSamples where SampleID=@id";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");
            SqlDataReader sqlReader;
            using (SqlConnection conn = new SqlConnection(sqlDatasource))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@id", id);
                    sqlReader = command.ExecuteReader();
                    table.Load(sqlReader);
                    sqlReader.Close();
                    conn.Close();
                }
            }

            return new JsonResult("Deleted Successfully");
        }

        //Updata the data base on Interage PK, input an json body with key-value pairs
        [HttpPut]
        [Route("UpdateSample")]
        public JsonResult UpdateSample(String id, [FromBody] Dictionary<string, object> updatedData)
        {
            if (updatedData == null || updatedData.Count == 0)
            {
                return new JsonResult("No data provided");
            }

            StringBuilder queryBuilder = new StringBuilder("Update dbo.StagingRCSamples Set ");
            List<SqlParameter> parameters = new List<SqlParameter>();

            foreach (var key in updatedData.Keys)
            {
                queryBuilder.Append($"{key} = @{key}, ");
                parameters.Add(new SqlParameter($"@{key}", updatedData[key]));
            }

            //Remove the trailing comma and space
            queryBuilder.Length -= 2; 
            queryBuilder.Append(" Where SampleID = @SampleID");
            parameters.Add(new SqlParameter("@SampleID", id));

            string query = queryBuilder.ToString();
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            using (SqlConnection conn = new SqlConnection(sqlDatasource))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddRange(parameters.ToArray());
                    SqlDataReader sqlReader = command.ExecuteReader();
                    table.Load(sqlReader);
                    sqlReader.Close();
                    conn.Close();
                }
            }

            return new JsonResult("Updated Successfully");
        }

        //This method would excute the string input sql in database, and  if it's a select sql, return the select result as json
        //Sould be very careful with this method, althought has edge case protection 
        [HttpPost]
        [Route("ExecuteSql")]
        public IActionResult ExecuteSql([FromBody] string sqlQuery)
        {
            if (string.IsNullOrWhiteSpace(sqlQuery))
            {
                return BadRequest("SQL query cannot be null or empty.");
            }

            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");
            DataTable resultTable = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(sqlQuery, conn))
                    {
                        // Determine if the query is a SELECT statement
                        if (Regex.IsMatch(sqlQuery.Trim(), @"^SELECT", RegexOptions.IgnoreCase))
                        {
                            SqlDataReader sqlReader = command.ExecuteReader();
                            resultTable.Load(sqlReader);
                            sqlReader.Close();
                            return Ok(resultTable);
                        }
                        else
                        {
                            int affectedRows = command.ExecuteNonQuery();
                            return Ok(new { Message = "Execution Successful", AffectedRows = affectedRows });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return BadRequest($"SQL error occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        //Could have more methods for database operation
    }
}
