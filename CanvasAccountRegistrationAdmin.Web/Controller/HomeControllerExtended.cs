using AutoMapper;
using CanvasAccountRegistration.Logic.Services;
using CanvasAccountRegistration.Web.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Sh.Library.MailSender;
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
            [FromServices] IMapper mapper)
        {
            var accounts = await accountService.GetAll();
            var viewModels = mapper.Map<IEnumerable<RegistrationViewModel>>(accounts);
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

        public async Task<IActionResult> Integrate([FromServices] IMailService mailer, int id)
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
                await mailer.Send("biblioteket@sh.se", account.Email, "V�lkommen till S�dert�rns h�gskolas l�rplattform Canvas", "<!DOCTYPE html> <html> <head>     <meta charset='UTF-8'> </head> <body>     <p>Du har nu ett konto i Canvas. <a href='https://canvas-account-registration.shbiblioteket.se/how-to-log-into-canvas'>Klicka h�r f�r information om hur du loggar in</a></p> <p>Om l�nken inte fungerar, kopiera och klistra in f�ljande i din webbl�sare:<br /> https://canvas-account-registration.shbiblioteket.se/how-to-log-into-canvas</p> </body> </html>");
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
