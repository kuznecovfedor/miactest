using AutoMapper;
using MIACApi.Data;
using MIACApi.DTO;
using MIACApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MIACApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class MaterialController : ControllerBase
    {
        private readonly MIACContext _context;
        private readonly IMapper _mapper;

        public MaterialController(MIACContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region GET
        /// <summary>
        /// Метод для получения списка материалов
        /// </summary>
        /// <returns>Список материалов</returns>
        /// <response code="200">Успех</response>
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

        /// <summary>
        /// Метод для получения материала по его id
        /// </summary>
        /// <param name="idMaterial">Идентификатор материала</param>
        /// <returns>Материал</returns>
        /// <response code="200">Успех</response>
        /// /// <responce code="404">Материал не найден</responce>
        [HttpGet("{idMaterial}")]
        [ProducesResponseType(typeof(MaterialDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
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
        #endregion

        #region POST
        /// <summary>
        /// Создание материала
        /// </summary>
        /// <param name="materialDTO">Объект материала</param>
        /// <returns>Статус</returns>
        /// <response code="201">Объект успешно создан</response>
        /// <response code="403">Доступ запрещен</response>
        /// <response code="404">Некорректные данные</response>
        [HttpPost]
        [ProducesResponseType(typeof(MaterialDTO), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Post([FromBody] MaterialDTO materialDTO)
        {
            if (materialDTO is null)
                return StatusCode((int)HttpStatusCode.BadRequest);

            try
            {
                Seller? current = await _context.Sellers
                    .FirstOrDefaultAsync(s => s.Login == HttpContext.User.Identity!.Name);

                if(current.IdSeller != materialDTO.IdSeller)
                    return StatusCode((int)HttpStatusCode.Forbidden);

                Material material = _mapper.Map<Material>(materialDTO);

                await _context.Materials.AddAsync(material);
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
        #endregion

        #region PUT
        /// <summary>
        /// Обновление информации о материале
        /// </summary>
        /// <param name="materialDTO">Объект материала</param>
        /// <returns>Статус</returns>
        /// <response code="201">Объект успешно обновлен</response>
        /// <response code="403">Доступ запрещен</response>
        /// <response code="404">Некорректные данные</response>
        [HttpPut]
        [ProducesResponseType(typeof(MaterialDTO), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Put([FromBody] MaterialDTO materialDTO)
        {
            if (materialDTO is null)
                return StatusCode((int)HttpStatusCode.BadRequest);

            try
            {
                Seller? current = await _context.Sellers
                    .FirstOrDefaultAsync(s => s.Login == HttpContext.User.Identity!.Name);

                if (current.IdSeller != materialDTO.IdSeller)
                    return StatusCode((int)HttpStatusCode.Forbidden);

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
        #endregion

        #region DELETE
        /// <summary>
        /// Удаление материала
        /// </summary>
        /// <param name="idMaterial">Идентификатор материала</param>
        /// <returns>Статус</returns>
        /// <response code="204">Объект успешно удален</response>
        /// <response code="403">Доступ запрещен</response>
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
                    Seller? current = await _context.Sellers
                    .FirstOrDefaultAsync(s => s.Login == HttpContext.User.Identity!.Name);

                    if (current.IdSeller != material.IdSeller)
                        return StatusCode((int)HttpStatusCode.Forbidden);

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
        #endregion
    }
}