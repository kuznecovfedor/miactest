using MIACApi.Data;
using Microsoft.AspNetCore.Mvc;

namespace MIACApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MIACContext _context;

        public UserController(MIACContext context)
        {
            _context = context;
        }
    }
}
