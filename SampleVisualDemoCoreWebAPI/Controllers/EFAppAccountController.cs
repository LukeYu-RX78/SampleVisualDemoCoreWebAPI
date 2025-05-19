using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleVisualDemoCoreWebAPI.Models.Entities;

namespace SampleVisualDemoCoreWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EFAppAccountController : ControllerBase
    {
        private readonly DorsDbContext _context;

        public EFAppAccountController(DorsDbContext context)
        {
            _context = context;
        }

        [HttpGet("by-aid/{aid}")]
        public async Task<ActionResult<AppAccount>> GetByAid(int aid)
        {
            var account = await _context.AppAccounts.FindAsync(aid);
            if (account == null)
                return NotFound();
            return account;
        }
    }
}
