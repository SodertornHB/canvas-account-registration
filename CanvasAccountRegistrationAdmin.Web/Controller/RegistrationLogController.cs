
//--------------------------------------------------------------------------------------------------------------------
// Warning! This is an auto generated file. Changes may be overwritten. 
// Generator version: 0.0.1.0
//--------------------------------------------------------------------------------------------------------------------

using AutoMapper;
using CanvasAccountRegistration.Logic.Model;
using CanvasAccountRegistration.Logic.Services;
using CanvasAccountRegistration.Web.ViewModel;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CanvasAccountRegistration.Web.Controllers
{
    public partial class RegistrationLogController : Controller
    {
        private readonly ILogger<RegistrationLogController> logger;
        private readonly IRegistrationLogService service;
        private readonly IMapper mapper;

        public RegistrationLogController(ILogger<RegistrationLogController> logger, 
        IRegistrationLogService service, 
        IMapper mapper)
        {
            this.logger = logger;
            this.service = service;
            this.mapper = mapper;
        }

        public virtual async Task<IActionResult> Index()
        {
            var list = await service.GetAll();
            var viewModels = mapper.Map<IEnumerable<RegistrationLogViewModel>>(list);
            return View(viewModels.OrderByDescending(x => x.Id));
        }
        
        public ActionResult Create()
        {
            return View(new RegistrationLogViewModel());
        }

        [HttpPost]
        public virtual async Task<ActionResult> Create([FromForm]RegistrationLogViewModel viewModel)
        {
            var model = mapper.Map<RegistrationLog>(viewModel);
            await service.Insert(model);
            return RedirectToAction(nameof(Index));
        }

        public virtual async Task<ActionResult> Edit(int id)
        {
            var entity = await service.Get(id);
            return View(mapper.Map<RegistrationLogViewModel>(entity));
        }


        [HttpPost]
        public virtual async Task<ActionResult> Edit([FromForm]RegistrationLogViewModel viewModel)
        {
            var model = mapper.Map<RegistrationLog>(viewModel);
            await service.Update(model);
            return RedirectToAction(nameof(Index));         
        }

        public virtual async Task<ActionResult> Remove(int id)
        {
            var entity = await service.Get(id);
            return View(mapper.Map<RegistrationLogViewModel>(entity));        
        }

        [HttpPost]
        public virtual async Task<ActionResult> Remove([FromForm]RegistrationLogViewModel viewModel)
        {
            var model = mapper.Map<RegistrationLog>(viewModel);
            await service.Delete(viewModel.Id);
            return RedirectToAction(nameof(Index));         
        }
    }
}