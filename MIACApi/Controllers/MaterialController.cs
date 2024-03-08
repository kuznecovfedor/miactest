using AutoMapper;
using MIACApi.Data;
using MIACApi.DTO;
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
        private readonly MIACContext _context;
        private readonly IMapper _mapper;
        public MaterialController(MIACContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [ProducesResponseType(typeof(IEnumerable<MaterialDTO>), (int)HttpStatusCode.OK)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<Material> materialsList =
                await _context.Materials
                .AsNoTracking()
                .ToListAsync();
            return StatusCode(200, _mapper.Map<List<MaterialDTO>>(materialsList));
        }


        [ProducesResponseType(typeof(MaterialDTO), (int)HttpStatusCode.OK)]
        [HttpGet("{idMaterial}")]
        public async Task<IActionResult> Get(int idMaterial)
        {
            Material? material =
                await _context.Materials
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdMaterial == idMaterial);

            if( material is null )
            {
                return StatusCode(404, null);
            }

            return StatusCode(200, _mapper.Map<MaterialDTO>(material));
        }
    }
}
