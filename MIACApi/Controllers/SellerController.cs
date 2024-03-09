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

            return StatusCode(
                (int)HttpStatusCode.OK,
                _mapper.Map<List<SellerDTO>>(sellersList)
                );
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

            return StatusCode(
                (int)HttpStatusCode.OK,
                _mapper.Map<SellerDTO>(seller)
                );
        }
    }
}