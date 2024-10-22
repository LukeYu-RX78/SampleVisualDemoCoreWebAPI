using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

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
            string query = "select * from [lookup].[v_DDRValues] where ContractNo = 'CW2262484_2024' order by Category, Value;";
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


            var lookupList = new List<dynamic>();

            foreach (DataRow row in table.Rows)
            {
                lookupList.Add(new
                {
                    Type = row["Type"].ToString(),
                    ContractNo = row["ContractNo"].ToString(),
                    Category = row["Category"].ToString(),
                    Value = row["Value"].ToString()
                });
            }

            var groupedLookups = lookupList
                .GroupBy(l => l.Category)
                .Select(g => new
                {
                        Category = g.Key,
                        Values = g.Select(x => x.Value).ToList()
                })
                .ToList();

            return new JsonResult(groupedLookups);
        }
    }


}
