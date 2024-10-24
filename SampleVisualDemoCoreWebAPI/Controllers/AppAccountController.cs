using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using SampleVisualDemoCoreWebAPI.Models.Entities;

namespace SampleVisualDemoCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Invalid login request.");
            }

            string query;
            bool isEmailCheck = !string.IsNullOrEmpty(loginRequest.EmailAddress);
            if (isEmailCheck)
            {
                query = "SELECT TOP 1 * FROM AppAccount WHERE EmailAddress = @EmailAddress";
            }
            else
            {
                query = "SELECT TOP 1 * FROM AppAccount WHERE PhoneNumber = @PhoneNumber";
            }

            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("SampleVisualDemoDBConn");

            using (SqlConnection conn = new SqlConnection(sqlDatasource))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    if (isEmailCheck)
                    {
                        command.Parameters.AddWithValue("@EmailAddress", loginRequest.EmailAddress);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@PhoneNumber", loginRequest.PhoneNumber);
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(table);
                }
            }

            if (table.Rows.Count == 0)
            {
                if (isEmailCheck)
                {
                    return NotFound(new { message = "Email address does not exist." });
                }
                else
                {
                    return NotFound(new { message = "Phone number does not exist." });
                }
            }

            DataRow userRow = table.Rows[0];
            var user = new AppAccount
            {
                Aid = Convert.ToInt32(userRow["Aid"]),
                Uid = userRow["Uid"] != DBNull.Value ? Convert.ToInt32(userRow["Uid"]) : (int?)null,
                FirstName = userRow["FirstName"].ToString(),
                MiddleName = userRow["MiddleName"].ToString(),
                Lastname = userRow["Lastname"].ToString(),
                EmailAddress = userRow["EmailAddress"].ToString(),
                PhoneNumber = userRow["PhoneNumber"].ToString(),
                Password = userRow["Password"].ToString(),
                ContractNo = userRow["ContractNo"].ToString(),
                Organization = userRow["Organization"].ToString(),
                Position = userRow["Position"].ToString(),
                AuthorityLv = userRow["AuthorityLv"].ToString()
            };

            if (user.Password != loginRequest.Password)
            {
                return Unauthorized(new { message = "Password is incorrect." });
            }

            user.Password = null;

            return Ok(user);
        }
    }
}

