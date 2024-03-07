using MIACApi.Data;
using MIACApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MIACApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private MIACContext _context;
        public MaterialController(MIACContext context)
        {
            _context = context;
        }

        [ProducesResponseType(typeof(IEnumerable<Material>), (int)HttpStatusCode.OK)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<Material> materialsList =
                await _context.Materials
                .AsNoTracking()
                .ToListAsync();
            return StatusCode(200, materialsList);
        }

        [ProducesResponseType(typeof(Material), (int)HttpStatusCode.OK)]
        [HttpGet("{idMaterial}")]
        public async Task<IActionResult> GetById(int idMaterial)
        {
            Material? materialsList =
                await _context.Materials
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdMaterial == idMaterial);
            return materialsList is not null ? StatusCode(200, materialsList) : StatusCode(404, null);
        }
    }
}
