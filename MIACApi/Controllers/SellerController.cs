using AutoMapper;
using MIACApi.Data;
using MIACApi.DTO;
using MIACApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace MIACApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SellerController : ControllerBase
    {
        private readonly MIACContext _context;
        private readonly IMapper _mapper;

        public SellerController(MIACContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [ProducesResponseType(typeof(List<SellerDTO>), (int)HttpStatusCode.OK)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<Seller> sellersList =
                await _context.Sellers
                .AsNoTracking()
                .ToListAsync();

            return StatusCode((int)HttpStatusCode.OK, _mapper.Map<List<SellerDTO>>(sellersList));
        }


        [ProducesResponseType(typeof(SellerDTO), (int)HttpStatusCode.OK)]
        [HttpGet("{idSeller}")]
        public async Task<IActionResult> Get(int idSeller)
        {
            Seller? seller =
                await _context.Sellers
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.IdSeller == idSeller);

            if( seller is null)
            {
                return NotFound();
            }

            return StatusCode((int)HttpStatusCode.OK, _mapper.Map<SellerDTO>(seller));
        }


        [HttpPost]
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
        public async Task<IActionResult> Authorization(string login, string password)
        {
            Seller seller;
            try
            {
                seller = GetUserData(login, password);

                DateTime now = DateTime.UtcNow;
                JwtSecurityToken jwt = new JwtSecurityToken(
                    issuer: AuthenticationOptions.ISSUER,
                    audience: AuthenticationOptions.AUDIENCE,
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

        private Seller GetUserData(string login, string password)
        {
            Seller? seller = _context.Sellers
                .AsNoTracking()
                .FirstOrDefault(u => u.Login == login);

            if (seller is null)
                throw new ArgumentException("seller not found");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, seller.PasswordHash);

            if (!isPasswordValid)
                throw new ArgumentException("password invalid");

            return seller;
        }
    }
}