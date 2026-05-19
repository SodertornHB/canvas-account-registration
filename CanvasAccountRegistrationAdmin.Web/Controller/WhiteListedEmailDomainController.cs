using AutoMapper;
using CanvasAccountRegistration.Logic.Model;
using CanvasAccountRegistration.Logic.Services;
using CanvasAccountRegistration.Web.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanvasAccountRegistration.Web.Controllers
{
    public class WhiteListedEmailDomainController : Controller
    {
        private readonly ILogger<WhiteListedEmailDomainController> logger;
        private readonly IWhiteListedEmailDomainService service;
        private readonly IMapper mapper;

        public WhiteListedEmailDomainController(
            ILogger<WhiteListedEmailDomainController> logger,
            IWhiteListedEmailDomainService service,
            IMapper mapper)
        {
            this.logger = logger;
            this.service = service;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var items = await service.GetAll();
            var viewModels = mapper.Map<IEnumerable<WhiteListedEmailDomainViewModel>>(items);
            return View(viewModels.OrderBy(x => x.Domain));
        }

        public IActionResult Create()
        {
            return View(new WhiteListedEmailDomainViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] WhiteListedEmailDomainViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);
            var model = mapper.Map<WhiteListedEmailDomain>(viewModel);
            model.CreatedOn = DateTime.UtcNow;
            await service.Insert(model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var entity = await service.Get(id);
            return View(mapper.Map<WhiteListedEmailDomainViewModel>(entity));
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] WhiteListedEmailDomainViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);
            var model = mapper.Map<WhiteListedEmailDomain>(viewModel);
            await service.Update(model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int id)
        {
            var entity = await service.Get(id);
            return View(mapper.Map<WhiteListedEmailDomainViewModel>(entity));
        }

        [HttpPost]
        public async Task<IActionResult> Remove([FromForm] WhiteListedEmailDomainViewModel viewModel)
        {
            await service.Delete(viewModel.Id);
            return RedirectToAction(nameof(Index));
        }
    }
}
