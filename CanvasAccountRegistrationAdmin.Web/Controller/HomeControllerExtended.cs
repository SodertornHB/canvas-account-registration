using AutoMapper;
using CanvasAccountRegistration.Logic.Services;
using CanvasAccountRegistration.Web.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public partial class HomeController
    {

        [HttpGet("list")]
        public async Task<IActionResult> List(
            [FromServices] IAccountServiceExtended accountService,
            [FromServices] IMapper mapper)
        {
            var accounts = await accountService.GetAll();
            var viewModels = mapper.Map<IEnumerable<RegistrationViewModel>>(accounts);
            return View(viewModels.OrderByDescending(x => x.CreatedOn));
        }
    }
}
