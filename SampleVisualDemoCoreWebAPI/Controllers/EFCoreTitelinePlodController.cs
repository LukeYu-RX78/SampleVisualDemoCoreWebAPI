using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleVisualDemoCoreWebAPI.Models.Entities;

namespace SampleVisualDemoCoreWebAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class EFCoreTitelinePlodController : ControllerBase
    {
        private readonly DorsDbContext _context;

        public EFCoreTitelinePlodController(DorsDbContext context)
        {
            _context = context;
        }

        [HttpGet("by-pid/{pid}")]
        public async Task<ActionResult<Plod>> GetByPid(int pid)
        {
            var plod = await _context.Plods.FindAsync(pid);
            if (plod == null)
                return NotFound();
            return plod;
        }
    }
}
