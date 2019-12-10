using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PMSAWebMVC.Controllers;
using PMSAWebMVC.Models;
using PMSAWebMVC.ViewModels.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.Areas.SupplierArea.Controllers
{
    public class SupSettingController : BaseController
    {
        private readonly PMSAEntities db = new PMSAEntities();

        //你原本的建構子不要刪掉
        public SupSettingController()
        {
        }

        //建構子多載
        public SupSettingController(ApplicationUserManager userManager)
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

        // GET: SupSetting
        public ActionResult Index()
        {
            var userId = User.Identity.Name;
            var userData = UserManager.Users.Where(x => x.UserName == userId).SingleOrDefault();
            var supData = db.SupplierAccount.Where(x => x.SupplierAccountID == userId).SingleOrDefault();
            var supInfoData = db.SupplierInfo.Where(x => x.SupplierCode == supData.SupplierCode).SingleOrDefault();
            Sup_parent m = new Sup_parent();
            SupInfoViewModel info = new SupInfoViewModel();
            SupSettingViewModel sup = new SupSettingViewModel();
            info.SupplierName = supInfoData.SupplierName;
            info.TaxID = supInfoData.TaxID;
            info.Tel = supInfoData.Tel;
            info.Address = supInfoData.Address;
            info.Email = supInfoData.Email;

            sup.SupplierAccountID = User.Identity.Name;
            sup.ContactName = userData.RealName;
            sup.Email = userData.Email;
            sup.Tel = supData.Tel;
            sup.Mobile = supData.Mobile;
            sup.EnableTwoFactorAuth = userData.TwoFactorEnabled;

            m.SupInfoViewModel = info;
            m.SupSettingViewModel = sup;
            return View(m);
        }

        // POST: SupSetting/Create
        [HttpPost]
        public async Task<ActionResult> Index(Sup_parent m)
        {
            try
            {
                var userId = User.Identity.Name;
                var userData = UserManager.Users.Where(x => x.UserName == userId).SingleOrDefault();
                var supData = db.SupplierAccount.Where(x => x.SupplierAccountID == userId).SingleOrDefault();
                var supInfoData = db.SupplierInfo.Where(x => x.SupplierCode == supData.SupplierCode).SingleOrDefault();
                SupInfoViewModel info = new SupInfoViewModel();
                SupSettingViewModel sup = new SupSettingViewModel();

                //姓名
                userData.RealName = m.SupSettingViewModel.ContactName;
                supData.ContactName = m.SupSettingViewModel.ContactName;
                //信箱
                string holder = userData.Email;
                userData.Email = m.SupSettingViewModel.Email;
                supData.Email = m.SupSettingViewModel.Email;
                //如果信箱有改，驗證要重置為false
                if (holder != m.SupSettingViewModel.Email)
                {
                    userData.EmailConfirmed = false;
                }
                //手機
                userData.PhoneNumber = m.SupSettingViewModel.Mobile;
                supData.Mobile = m.SupSettingViewModel.Mobile;
                //市話
                supData.Tel = m.SupSettingViewModel.Tel;
                //雙因素
                //判斷Email是否通過驗證
                userData.TwoFactorEnabled = m.SupSettingViewModel.EnableTwoFactorAuth;

                var r = await UserManager.UpdateAsync(userData);
                db.Entry(supData).State = System.Data.Entity.EntityState.Modified;
                var r2 = await db.SaveChangesAsync();
                if (r.Succeeded && r2 > 0)
                {
                    TempData["SuccessMsg"] = "修改成功!";
                    return RedirectToAction("Index");
                }
                TempData["ErrorMsg"] = "修改失敗，請檢查網路連線再試一次。";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["ErrorMsg"] = "修改失敗，不可空值，並請檢查網路連線再試一次...";
                return RedirectToAction("Index");
            }
        }

        //公司資料修改
        [HttpPost]
        public async Task<ActionResult> IndexRight(Sup_parent m)
        {
            try
            {
                var userId = User.Identity.Name;
                var userData = UserManager.Users.Where(x => x.UserName == userId).SingleOrDefault();
                var supData = db.SupplierAccount.Where(x => x.SupplierAccountID == userId).SingleOrDefault();
                var supInfoData = db.SupplierInfo.Where(x => x.SupplierCode == supData.SupplierCode).SingleOrDefault();
                SupInfoViewModel info = new SupInfoViewModel();

                //供應商名稱
                supInfoData.SupplierName = m.SupInfoViewModel.SupplierName;

                //統一編號
                supInfoData.TaxID = m.SupInfoViewModel.TaxID;

                //公司信箱
                supInfoData.Email = m.SupInfoViewModel.Email;

                //公司市話
                supInfoData.Tel = m.SupInfoViewModel.Tel;

                //公司地址
                supInfoData.Address = m.SupInfoViewModel.Address;

                db.Entry(supData).State = System.Data.Entity.EntityState.Modified;
                var r2 = await db.SaveChangesAsync();
                if (r2 > 0)
                {
                    TempData["SuccessMsg"] = "修改成功!";
                    return RedirectToAction("Index");
                }
                TempData["ErrorMsg"] = "修改失敗，請檢查網路連線再試一次。";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["ErrorMsg"] = "修改失敗，不可空值，並請檢查網路連線再試一次...";
                return RedirectToAction("Index");
            }
        }

        //===============================================================================

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
    }
}