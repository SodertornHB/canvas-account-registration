// This is an organization specific file 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public partial class HomeController
    {

        [AllowAnonymous]
        [HttpGet("instructions")]
        public IActionResult Instructions()
        {
            return View();
        }
    }
}
