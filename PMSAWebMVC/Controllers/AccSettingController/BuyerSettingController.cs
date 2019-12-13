using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PMSAWebMVC.Models;
using PMSAWebMVC.ViewModels.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpGet]
        public async Task<JsonResult> isEmailConfirmed()
        {
            string LognId = User.Identity.GetUserId();
            var isEmailConfirmed = await UserManager.IsEmailConfirmedAsync(LognId);
            if (isEmailConfirmed == false)
            {
                string str = "您的信箱尚未驗證，不能開啟雙因素驗證功能";
                var obj = new { text = str, isEmailConfirmed = false };
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string str = "信箱已驗證，可以開啟雙因素驗證功能";
                var obj = new { text = str, isEmailConfirmed = true };
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> sendMailConfirm()
        {
            string LognId = User.Identity.GetUserId();
            //寄驗證信
            // 傳送包含此連結的電子郵件
            var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("PMSAWebMVC");
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(LognId);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = LognId, code = code }, protocol: Request.Url.Scheme);
            //寄信
            await UserManager.SendEmailAsync(LognId, "信箱驗證", $"<a href='{callbackUrl}'>請點此驗證信箱</a>");
            TempData["Sended"] = "已寄送!請到信箱收信!";
            return RedirectToAction("Index");
        }

        // POST: Buyer/Create
        [HttpPost]
        public async Task<ActionResult> Index(Buyer_parent m)
        {
            try
            {
                //m.BuyerSettingViewModel.EmployeeID = User.Identity.Name;
                var userId = User.Identity.Name;
                var userData = UserManager.Users.Where(x => x.UserName == userId).SingleOrDefault();
                var empData = db.Employee.Where(x => x.EmployeeID == userId).SingleOrDefault();
                var companyData = db.CompanyInfo.Where(x => x.CompanyCode == empData.CompanyCode).SingleOrDefault();
                BuyerCompInfoViewModel info = new BuyerCompInfoViewModel();
                BuyerSettingViewModel buyer = new BuyerSettingViewModel();

                //姓名
                userData.RealName = m.BuyerSettingViewModel.Name;
                empData.Name = m.BuyerSettingViewModel.Name;
                //信箱
                string holder = userData.Email;
                userData.Email = m.BuyerSettingViewModel.Email;
                empData.Email = m.BuyerSettingViewModel.Email;
                //如果信箱有改，驗證要重置為false
                if (holder != m.BuyerSettingViewModel.Email)
                {
                    userData.EmailConfirmed = false;
                }
                //手機
                userData.PhoneNumber = m.BuyerSettingViewModel.Mobile;
                empData.Mobile = m.BuyerSettingViewModel.Mobile;
                //市話
                empData.Tel = m.BuyerSettingViewModel.Tel;
                //雙因素
                //判斷Email是否通過驗證
                userData.TwoFactorEnabled = m.BuyerSettingViewModel.EnableTwoFactorAuth;

                var r = await UserManager.UpdateAsync(userData);
                db.Entry(empData).State = System.Data.Entity.EntityState.Modified;
                var r2 = await db.SaveChangesAsync();
                if (r.Succeeded && r2 > 0)
                {
                    TempData["SuccessMsg"] = "修改成功!";
                    return RedirectToAction("Index");
                }
                TempData["ErrorMsg"] = "修改失敗， " + string.Join("、", r.Errors) + " 請檢查網路連線再試一次。";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["ErrorMsg"] = "修改失敗，不可空值，並請檢查網路連線再試一次...";
                return RedirectToAction("Index");
            }
        }
    }
}