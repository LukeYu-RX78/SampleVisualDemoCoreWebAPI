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

            //Fetch lookup values for each contract number
            var allLookups = new List<dynamic>();
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

            return new JsonResult(allLookups);
        }
    }
}
