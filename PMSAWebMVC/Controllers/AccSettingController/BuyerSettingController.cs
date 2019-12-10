using Microsoft.AspNet.Identity.Owin;
using PMSAWebMVC.Models;
using PMSAWebMVC.ViewModels.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.Controllers.AccSettingController
{
    public class BuyerSettingController : BaseController
    {
        private readonly PMSAEntities db = new PMSAEntities();

        //你原本的建構子不要刪掉
        public BuyerSettingController()
        {
        }

        //建構子多載
        public BuyerSettingController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        // 屬性
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Buyer
        public ActionResult Index()
        {
            var userId = User.Identity.Name;
            var userData = UserManager.Users.Where(x => x.UserName == userId).SingleOrDefault();
            var empData = db.Employee.Where(x => x.EmployeeID == userId).SingleOrDefault();
            var companyData = db.CompanyInfo.Where(x => x.CompanyCode == empData.CompanyCode).SingleOrDefault();
            Buyer_parent m = new Buyer_parent();
            BuyerCompInfoViewModel info = new BuyerCompInfoViewModel();
            BuyerSettingViewModel buyer = new BuyerSettingViewModel();
            info.CompanyName = companyData.CompanyName;
            info.TaxID = companyData.TaxID;
            info.Tel = companyData.Tel;
            info.Address = companyData.Address;
            info.Email = companyData.Email;

            buyer.EmployeeID = User.Identity.Name;
            buyer.Name = userData.RealName;
            buyer.Email = userData.Email;
            buyer.Tel = empData.Tel;
            buyer.Mobile = empData.Mobile;
            buyer.EnableTwoFactorAuth = userData.TwoFactorEnabled;

            m.BuyerCompInfoViewModel = info;
            m.BuyerSettingViewModel = buyer;
            return View(m);
        }

        // POST: Buyer/Create
        [HttpPost]
        public ActionResult Index(Buyer_parent m)
        {
            try
            {
                m.BuyerSettingViewModel.EmployeeID = User.Identity.Name;

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}