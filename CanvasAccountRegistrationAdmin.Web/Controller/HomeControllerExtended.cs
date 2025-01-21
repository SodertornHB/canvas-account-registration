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

        public async Task<IActionResult> Approve(int id)
        {
            if (id == default)
            {
                return BadRequest("User ID is required.");
            }

            var account = await accountService.Get(id); 
            if (account == null)
            {
                return NotFound($"Account with ID {id} not found.");
            }

            account.VerifiedOn = System.DateTime.UtcNow;
            await accountService.Update(account);
            TempData["SuccessMessage"] = "User {0} has been approved successfully.";
            TempData["AccountDisplayName"] = account.DisplayName;
            return RedirectToAction("List"); 
        }

    }
}
