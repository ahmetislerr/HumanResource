﻿using HumanResource.Application.Models.DTOs.CompanyManagerDTO;
using HumanResource.Application.Models.VMs.EmailVM;
using HumanResource.Application.Services.AccountServices;
using HumanResource.Application.Services.AddressService;
using HumanResource.Application.Services.AdvanceService;
using HumanResource.Application.Services.CompanyManagerService;
using HumanResource.Application.Services.EmailSenderService;
using HumanResource.Application.Services.ExpenseService;
using HumanResource.Application.Services.LeaveServices;
using HumanResource.Application.Services.PersonelService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HumanResource.Presentation.Areas.CompanyManager.Controllers
{
    [Area("CompanyManager")]
    [Authorize(Roles = "CompanyManager")]
    public class CompanyManagerController : Controller
    {
        private readonly ICompanyManagerService _companyManagerService;
        private readonly IPersonelService _personelService;
        private readonly IAddressService _addressService;
        private readonly IEmailService _emailService;
        private readonly IAccountServices _accountService;
        private readonly IAdvanceService _advanceService;
        private readonly IExpenseServices _expenseService;
        private readonly ILeaveService _leaveService;

        public CompanyManagerController(ICompanyManagerService companyManagerService, IPersonelService personelService, IAddressService addressService, IEmailService emailService, IAccountServices accountService, IAdvanceService advanceService, IExpenseServices expenseService, ILeaveService leaveService)
        {
            _companyManagerService = companyManagerService;
            _personelService = personelService;
            _addressService = addressService;
            _emailService = emailService;
            _accountService = accountService;
            _advanceService = advanceService;
            _expenseService = expenseService;
            _leaveService = leaveService;
        }

        public async Task<IActionResult> Employees()
        {
            ViewBag.Personel = await _personelService.GetPersonel(User.Identity.Name);
            return View(await _companyManagerService.GetEmployees());
        }


        public async Task<IActionResult> Create()
        {
            ViewBag.Personel = await _personelService.GetPersonel(User.Identity.Name);
            ViewBag.Departments = new SelectList(await _companyManagerService.GetDepartments(), "Id", "Name");
            ViewBag.Titles = new SelectList(await _companyManagerService.GetTitles(), "Id", "Name");
            ViewBag.CompanyManagers = new SelectList(await _companyManagerService.GetCompanyManagers(), "Id", "FullName");
            ViewBag.Cities = new SelectList(await _addressService.GetCities(), "Id", "Name");
            ViewBag.Districts = new SelectList(await _addressService.GetDistricts(), "Id", "Name");
            ViewBag.BaseUrl = Request.Scheme + "://" + HttpContext.Request.Host.ToString();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeDTO model)
        {
            if (ModelState.IsValid)
            {
                var result = await _companyManagerService.CreateEmployee(model);
                if (!result.Errors.Any())
                {
                    if (result.Result.Succeeded)
                    {

                        var conformationLink = Url.Action("ConfirmEmail", "Account", new { token = result.Token, email = result.Email, Area = "" }, Request.Scheme);

                        var message = new Message(result.Email, "Conformation Email Link", $"Welcome to our human resources platform. Please click the link to activate your account. {conformationLink!}   You can use your email and the password below to login to the platform.     {result.Password}");
                        _emailService.SendEmail(message);

                        return RedirectToAction("employees", "companymanager", new { Area = "companymanager" });
                    }
                }

                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }

            }
            ViewBag.Personel = await _personelService.GetPersonel(User.Identity.Name);
            ViewBag.Departments = new SelectList(await _companyManagerService.GetDepartments(), "Id", "Name");
            ViewBag.Titles = new SelectList(await _companyManagerService.GetTitles(), "Id", "Name");
            ViewBag.CompanyManagers = new SelectList(await _companyManagerService.GetCompanyManagers(), "Id", "FullName");
            ViewBag.Cities = new SelectList(await _addressService.GetCities(), "Id", "Name");
            ViewBag.Districts = new SelectList(await _addressService.GetDistricts(), "Id", "Name");
            ViewBag.BaseUrl = Request.Scheme + "://" + HttpContext.Request.Host.ToString();
            return View(model);
        }


        public async Task<IActionResult> Update(Guid id)
        {
            ViewBag.Personel = await _personelService.GetPersonel(User.Identity.Name);
            ViewBag.Departments = new SelectList(await _companyManagerService.GetDepartments(), "Id", "Name");
            ViewBag.Titles = new SelectList(await _companyManagerService.GetTitles(), "Id", "Name");
            ViewBag.CompanyManagers = new SelectList(await _companyManagerService.GetCompanyManagers(), "Id", "FullName");
            ViewBag.Cities = new SelectList(await _addressService.GetCities(), "Id", "Name");
            ViewBag.Districts = new SelectList(await _addressService.GetDistricts(), "Id", "Name");
            var model = await _companyManagerService.GetByUserName(id);
            model.BaseUrl = Request.Scheme + "://" + HttpContext.Request.Host.ToString();
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateEmployeeDTO model)
        {
            if (ModelState.IsValid)
            {
                var result = await _companyManagerService.UpdateEmployee(model);
                if (result.Succeeded)
                    return RedirectToAction("employees");
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);

                } 
            }

            ViewBag.Personel = await _personelService.GetPersonel(User.Identity.Name);
            ViewBag.Departments = new SelectList(await _companyManagerService.GetDepartments(), "Id", "Name");
            ViewBag.Titles = new SelectList(await _companyManagerService.GetTitles(), "Id", "Name");
            ViewBag.CompanyManagers = new SelectList(await _companyManagerService.GetCompanyManagers(), "Id", "FullName");
            ViewBag.Cities = new SelectList(await _addressService.GetCities(), "Id", "Name");
            ViewBag.Districts = new SelectList(await _addressService.GetDistricts(), "Id", "Name");
            model.BaseUrl = Request.Scheme + "://" + HttpContext.Request.Host.ToString();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(IFormCollection collection)
        {
            Guid id = Guid.Parse(collection["id"]);
            await _companyManagerService.Delete(id);
            TempData["success"] = "Employee was deleted succesfully.";
            return RedirectToAction("employees", "companymanager", new { Area = "companymanager" });
        }

        public async Task<IActionResult> Departments()
        {
            ViewBag.Personel = await _personelService.GetPersonel(User.Identity.Name);
            return View(await _companyManagerService.GetDepartments());
        }


        public async Task<IActionResult> Titles()
        {
            ViewBag.Personel = await _personelService.GetPersonel(User.Identity.Name);
            return View(await _companyManagerService.GetTitles());
        }

        public async Task<IActionResult> LeaveRequests()
        {
            ViewBag.Personel = await _personelService.GetPersonel(User.Identity.Name);
            return View(await _companyManagerService.GetPersonelLeaveRequests( await _personelService.GetPersonelId(User.Identity.Name)));
        }
        public async Task<IActionResult> AdvanceRequests()
        {
            ViewBag.Personel = await _personelService.GetPersonel(User.Identity.Name);
            return View(await _companyManagerService.GetPersonelAdvanceRequests(await _personelService.GetPersonelId(User.Identity.Name)));
        }
        public async Task<IActionResult> ExpenseRequests()
        {
            ViewBag.Personel = await _personelService.GetPersonel(User.Identity.Name);
            return View(await _companyManagerService.GetPersonelExpenseRequests(await _personelService.GetPersonelId(User.Identity.Name)));
        }

        public async Task<IActionResult> AdvanceRequestDetail(int id)
        {
            ViewBag.Personel = await _personelService.GetPersonel(User.Identity.Name);
            return View(await _advanceService.AdvanceDetail(id));
        }

        public async Task<IActionResult> ExpenseRequestDetail(int id)
        {
            ViewBag.Personel = await _personelService.GetPersonel(User.Identity.Name);
            return View(await _expenseService.ExpenseDetail(id));
        }

        public async Task<IActionResult> LeaveRequestDetail(int id)
        {
            ViewBag.Personel = await _personelService.GetPersonel(User.Identity.Name);
            return View(await _leaveService.LeaveDetail(id));
        }



    }
}
