using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SampleVisualDemoCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitelineLookupsController : ControllerBase
    {
        private IConfiguration _configuration;

        public TitelineLookupsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetLookups")]
        public JsonResult GetLookups()
        {
            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            //Fetch all active contract numbers
            string getContractsQuery = "SELECT ContractNo FROM Contract WHERE Active = 1;";
            var contractNumbers = new List<string>();

            using (SqlConnection conn = new SqlConnection(sqlDatasource))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(getContractsQuery, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        contractNumbers.Add(reader["ContractNo"].ToString());
                    }
                }
            }

            //Add the global ContractNo group dynamically
            var allLookups = new List<dynamic>
            {
                new
                {
                    ContractNo = "*",
                    Category = "ContractNo",
                    Values = contractNumbers
                }
            };

            //Fetch lookup values for each contract number
            foreach (var contractNo in contractNumbers)
            {
                string lookupQuery = $"SELECT * FROM [lookup].[v_DDRValues] WHERE ContractNo = '{contractNo}' ORDER BY Category, Value;";
                var lookups = new DataTable();

                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(lookupQuery, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        lookups.Load(reader);
                    }
                }

                // Group by category for this contract number
                var groupedLookups = lookups.AsEnumerable()
                    .GroupBy(row => row["Category"].ToString())
                    .Select(group => new
                    {
                        ContractNo = contractNo,
                        Category = group.Key,
                        Values = group.Select(row => row["Value"].ToString()).ToList()
                    });

                allLookups.AddRange(groupedLookups);
            }

            // Step 4: Return the result as JSON
            return new JsonResult(allLookups);
        }

        [HttpGet]
        [Route("GetLookupValues/{contractNo}/{category}")]
        public IActionResult GetLookupValues(string contractNo, string category)
        {
            string query = @"
                SELECT Value 
                FROM [lookup].[v_DDRValues] 
                WHERE ContractNo = @ContractNo 
                AND Category = @Category
                ORDER BY Value;";

            List<string> lookupValues = new List<string>();
            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            try
            {
                using (SqlConnection conn = new SqlConnection(sqlDatasource))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@ContractNo", contractNo);
                        command.Parameters.AddWithValue("@Category", category);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lookupValues.Add(reader["Value"].ToString());
                            }
                        }
                    }
                }

                if (lookupValues.Count == 0)
                {
                    return NotFound(new { message = $"No lookup values found for ContractNo: {contractNo}, Category: {category}" });
                }

                return Ok(new { ContractNo = contractNo, Category = category, Values = lookupValues });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }
    }
}
