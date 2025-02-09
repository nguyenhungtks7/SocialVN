using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialVN.API.Data;
using SocialVN.API.Models;

namespace SocialVN.API.Controllers
{
    //https:localhost:1234/api/Users
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SocialVNDbContext dbContext;
        public UsersController(SocialVNDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        //GET ALL users
        // GET: http:localhost:portnumber/api/Users
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = dbContext.Users.ToList();   
            return Ok(users);

        }
    }
}
