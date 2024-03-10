using AutoMapper;
using MIACApi.Data;
using MIACApi.DTO;
using MIACApi.Models;
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


        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MaterialDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get()
        {
            try
            {
                List<Material> materialsList =
                await _context.Materials
                .AsNoTracking()
                .ToListAsync();

                return StatusCode((int)HttpStatusCode.OK, _mapper.Map<List<MaterialDTO>>(materialsList));
            }
            catch (InvalidOperationException ex)
            {
                var statusInfo = DBExceptionMatcher.GetByExceptionMessage($"{ex.Message}{ex.InnerException?.Message}");
                return StatusCode(statusInfo.status, statusInfo.message);
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }


        [HttpGet("{idMaterial}")]
        [ProducesResponseType(typeof(MaterialDTO), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(int idMaterial)
        {
            try
            {
                Material? material =
                await _context.Materials
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdMaterial == idMaterial);

                if (material is null)
                {
                    return StatusCode((int)HttpStatusCode.NotFound); //404
                }

                return StatusCode((int)HttpStatusCode.OK, _mapper.Map<MaterialDTO>(material));
            }
            catch (InvalidOperationException ex)
            {
                var statusInfo = DBExceptionMatcher.GetByExceptionMessage($"{ex.Message}{ex.InnerException?.Message}");
                return StatusCode(statusInfo.status, statusInfo.message);
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        
        [HttpPost]
        [ProducesResponseType(typeof(MaterialDTO), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Post([FromBody] MaterialDTO materialDTO)
        {
            if (materialDTO is null)
                return StatusCode((int)HttpStatusCode.BadRequest); //400

            try
            {
                Material material = _mapper.Map<Material>(materialDTO);

                await _context.Materials.AddAsync(material);
                await _context.SaveChangesAsync();
                return StatusCode((int)HttpStatusCode.Created, _mapper.Map<MaterialDTO>(material)); //201
            }
            catch (DbUpdateException ex)
            {
                var statusInfo = DBExceptionMatcher.GetByExceptionMessage($"{ex.Message}{ex.InnerException?.Message}");
                return StatusCode(statusInfo.status, statusInfo.message);
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }


        [HttpPut]
        [ProducesResponseType(typeof(MaterialDTO), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Put([FromBody] MaterialDTO materialDTO)
        {
            if (materialDTO is null)
                return StatusCode((int)HttpStatusCode.BadRequest);

            try
            {
                Material? material = await _context.Materials
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.IdMaterial == materialDTO.IdMaterial);
                
                if (material is null)
                {
                    await _context.AddAsync(_mapper.Map<Material>(materialDTO));
                }
                else
                {
                    _context.Update(_mapper.Map<Material>(materialDTO));
                }

                await _context.SaveChangesAsync();
                return StatusCode((int)HttpStatusCode.Created, _mapper.Map<MaterialDTO>(material));
            }
            catch (DbUpdateException ex)
            {
                var statusInfo = DBExceptionMatcher.GetByExceptionMessage($"{ex.Message}{ex.InnerException?.Message}");
                return StatusCode(statusInfo.status, statusInfo.message);
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }


        [HttpDelete("{idMaterial}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete(int idMaterial)
        {
            try
            {
                Material? material = await _context.Materials
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.IdMaterial == idMaterial);

                if (material is null)
                {
                    return StatusCode((int)HttpStatusCode.NotFound);
                }
                else
                {
                    _context.Remove(material);
                    await _context.SaveChangesAsync();
                    return StatusCode((int)HttpStatusCode.NoContent);
                }
            }
            catch (DbUpdateException ex)
            {
                var statusInfo = DBExceptionMatcher.GetByExceptionMessage($"{ex.Message}{ex.InnerException?.Message}");
                return StatusCode(statusInfo.status, statusInfo.message);
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}