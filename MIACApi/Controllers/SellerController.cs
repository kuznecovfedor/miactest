using AutoMapper;
using MIACApi.Data;
using MIACApi.DTO;
using MIACApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace MIACApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class SellerController : ControllerBase
    {
        private readonly MIACContext _context;
        private readonly IMapper _mapper;

        public SellerController(MIACContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region GET
        /// <summary>
        /// Метод для получения списка продавцов
        /// </summary>
        /// <returns>Список продавцов</returns>
        /// <response code="200">Успех</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<SellerDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get()
        {
            List<Seller> sellersList =
                await _context.Sellers
                .AsNoTracking()
                .ToListAsync();

            return StatusCode((int)HttpStatusCode.OK, _mapper.Map<List<SellerDTO>>(sellersList));
        }

        /// <summary>
        /// Метод для получения продавца по его id
        /// </summary>
        /// <param name="idSeller">Идентификатор продавца</param>
        /// <returns>Продавец</returns>
        /// <response code="200">Успех</response>
        /// <responce code="404">Продавец не найден</responce>
        [HttpGet("{idSeller}")]
        [ProducesResponseType(typeof(SellerDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get(int idSeller)
        {
            Seller? seller =
                await _context.Sellers
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.IdSeller == idSeller);

            if (seller is null)
            {
                return NotFound();
            }

            return StatusCode((int)HttpStatusCode.OK, _mapper.Map<SellerDTO>(seller));
        }
        #endregion

        #region Post
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ModifySellerDTO), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Post([FromBody] RegisterSellerDTO registerSellerDTO)
        {
            if (registerSellerDTO is null)
                return StatusCode((int)HttpStatusCode.BadRequest);

            try
            {
                Seller seller = _mapper.Map<Seller>(registerSellerDTO);
                seller.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerSellerDTO.Password);

                await _context.Sellers.AddAsync(seller);
                await _context.SaveChangesAsync();
                return StatusCode((int)HttpStatusCode.Created, _mapper.Map<SellerDTO>(seller));
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


        [HttpPost("auth/{login}/{password}")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> Authorization(string login, string password)
        {
            Seller seller;
            ClaimsIdentity identity;
            try
            {
                (seller, identity) = GetUserData(login, password);

                DateTime now = DateTime.UtcNow;
                JwtSecurityToken jwt = new JwtSecurityToken(
                    issuer: AuthenticationOptions.ISSUER,
                    audience: AuthenticationOptions.AUDIENCE,
                    claims: identity.Claims,
                    notBefore: now,
                    expires: now.Add(TimeSpan.FromMinutes(AuthenticationOptions.LIFTTIME)),
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthenticationOptions.KEY)),
                        SecurityAlgorithms.HmacSha256)
                );

                var responce = new
                {
                    jwt = new JwtSecurityTokenHandler().WriteToken(jwt),
                    login = seller.Login
                };

                return new JsonResult(responce);
            }
            catch (ArgumentException)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, "Пользователь не найден");
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

        #region PUT
        [HttpPut]
        [ProducesResponseType(typeof(SellerDTO), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Put([FromBody] ModifySellerDTO modifySellerDTO)
        {
            try
            {
                Seller? seller = await _context.Sellers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.IdSeller == modifySellerDTO.IdSeller);

                if (seller is null)
                {
                    await _context.AddAsync(_mapper.Map<Seller>(modifySellerDTO));
                }
                else
                {
                    _context.Update(_mapper.Map<Seller>(modifySellerDTO));
                }

                await _context.SaveChangesAsync();
                return StatusCode((int)HttpStatusCode.Created, _mapper.Map<SellerDTO>(seller));
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
        [HttpDelete("{idSeller}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete(int idSeller)
        {
            try
            {
                Seller? seller = await _context.Sellers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.IdSeller == idSeller);

                if (seller is null)
                {
                    return StatusCode((int)HttpStatusCode.NotFound);
                }
                else
                {
                    _context.Remove(seller);
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

        private (Seller seller, ClaimsIdentity claims) GetUserData(string login, string password)
        {
            Seller? seller = _context.Sellers
                .AsNoTracking()
                .FirstOrDefault(u => u.Login == login);

            if (seller is null)
                throw new ArgumentException("seller not found");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, seller.PasswordHash);

            if (!isPasswordValid)
                throw new ArgumentException("password invalid");

            var claims = new List<Claim>()
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, seller.Login)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Authorization", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            return (seller, claimsIdentity);
        }
    }
}