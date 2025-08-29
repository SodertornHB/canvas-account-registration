using AutoMapper;
using CanvasAccountRegistration.Logic.Services;
using CanvasAccountRegistration.Web.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public partial class HomeController
    {

        [HttpGet("list")]
        public async Task<IActionResult> List(
     [FromServices] IAccountServiceExtended accountService,
     [FromServices] IMapper mapper,
     [FromQuery] string type)
        {
            var accounts = await accountService.GetAll();
            var filteredAccounts = string.IsNullOrWhiteSpace(type) ? accounts : accounts.Where(x => x.AccountType == type);
            var viewModels = mapper.Map<IEnumerable<RegistrationViewModel>>(filteredAccounts);
            return View(viewModels.OrderByDescending(x => x.CreatedOn));
        }

        public async Task<IActionResult> Approve(int id)
        {
            try
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
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = $"An error occurred: {e.Message}";
            }
            return RedirectToAction("List");
        }

        public async Task<IActionResult> Integrate( int id)
        {
            try
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
                var response = await accountService.IntegrateIntoCanvas(account);
                TempData["SuccessMessage"] = "User {0} has been integrated successfully.";
                TempData["AccountDisplayName"] = account.DisplayName;
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Kontot kunde inte l�ggas till. Finns anv�ndaren redan i Canvas?";
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = $"An error occurred: {e.Message}";
            }
            return RedirectToAction("List");
        }

        public async Task<IActionResult> Remove(int id)
        {
            try
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
                await accountService.Delete(id);
                TempData["SuccessMessage"] = "User {0} has been deleted.";
                TempData["AccountDisplayName"] = account.DisplayName;
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = $"An error occurred: {e.Message}";
            }
            return RedirectToAction("List");
        }

    }
}
