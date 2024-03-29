﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using PMSAWebMVC.Models;
using PMSAWebMVC.ViewModels.BuyerSupAccount;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.Controllers.BuyerSupAccountController
{
    [Authorize(Roles = "Buyer, Manager")]
    public class SupInfoCompanyController : BaseController
    {
        private PMSAEntities db = new PMSAEntities();
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public SupInfoCompanyController()
        {
        }

        public SupInfoCompanyController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

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

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        // GET: SupInfoCompany
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SupInfoViewModel m)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var maxThanOID = db.SupplierInfo.Select(x => x.SupplierInfoOID).Max() + 1;
                    string SupCodestr = String.Format("S{0:00000}", Convert.ToDouble(maxThanOID));
                    var supInfo = db.SupplierInfo;
                    SupplierInfo s = new SupplierInfo();
                    s.SupplierCode = SupCodestr;
                    s.SupplierName = m.SupplierName;
                    s.TaxID = m.TaxID;
                    s.Address = m.Address;
                    s.Email = m.Email;
                    s.Tel = m.Tel;
                    s.SupplierRatingOID = m.SupplierRatingOID;

                    supInfo.Add(s);
                    var result = db.SaveChanges();

                    if (result > 0)
                    {
                        TempData["Success"] = $"{s.SupplierName} 更新成功";
                        return View("Index");
                    }
                    else
                    {
                        ViewBag.Error = "對不起，伺服器發生錯誤，請再試一次。";
                        return View(m);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"對不起，伺服器發生錯誤: {ex.Message}，請再試一次。";
            }
            return View(m);
        }

        //==========================================================================
        //Ajax
        //SupAcc
        public async Task<ActionResult> getAllSupAccToIndexAjax()
        {
            List<object> sups = new List<object>();
            var supInfo = db.SupplierInfo;
            var supAcc = db.SupplierAccount;
            var rating = db.SupplierRating;
            //datatime 要轉型
            var js = new JsonSerializerSettings()
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };
            //判斷登入者是誰顯示專屬廠商
            string LoginAccId = User.Identity.GetUserName();
            string LognId = User.Identity.GetUserId();

            //Manager
            if (UserManager.IsInRole(LognId, "Manager"))
            {
                List<ApplicationUser> usersWithSupplierAccountID = await UserManager.Users.Where(x => x.UserName.Contains("S")).ToListAsync();
                foreach (var x in usersWithSupplierAccountID)
                {
                    var user = new
                    {
                        SupplierCode = await supInfo.Join(supAcc, s => s.SupplierCode, sa => sa.SupplierCode, (s, sa) => new { SupplierCode = s.SupplierCode, SupplierAccountID = sa.SupplierAccountID }).Where(y => y.SupplierAccountID == x.UserName).Select(y => y.SupplierCode).FirstOrDefaultAsync(),
                        SupplierName = await supInfo.Join(supAcc, s => s.SupplierCode, sa => sa.SupplierCode, (s, sa) => new { SupplierName = s.SupplierName, SupplierAccountID = sa.SupplierAccountID }).Where(y => y.SupplierAccountID == x.UserName).Select(y => y.SupplierName).FirstOrDefaultAsync(),
                        Email = x.Email,
                        Tel = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.Tel).FirstOrDefaultAsync(),
                        TaxID = await supInfo.Join(supAcc, s => s.SupplierCode, sa => sa.SupplierCode, (s, sa) => new { TaxID = s.TaxID, SupplierAccountID = sa.SupplierAccountID }).Where(y => y.SupplierAccountID == x.UserName).Select(y => y.TaxID).FirstOrDefaultAsync(),
                        SupplierRatingOID = await supInfo.Join(supAcc, s => s.SupplierCode, sa => sa.SupplierCode, (s, sa) => new { SupplierRatingOID = s.SupplierRatingOID, SupplierAccountID = sa.SupplierAccountID }).Where(y => y.SupplierAccountID == x.UserName).Select(y => y.SupplierRatingOID).FirstOrDefaultAsync(),
                        Address = await supInfo.Join(supAcc, s => s.SupplierCode, sa => sa.SupplierCode, (s, sa) => new { Address = s.Address, SupplierAccountID = sa.SupplierAccountID }).Where(y => y.SupplierAccountID == x.UserName).Select(y => y.Address).FirstOrDefaultAsync(),
                    };
                    sups.Add(user);
                }
            }
            //Buyer
            else if (UserManager.IsInRole(LognId, "Buyer"))
            {
                //供應商帳號
                var usersWithSupplierAccountID = UserManager.Users.Where(x => x.UserName.Contains("S")).ToList();
                var usersOfsupAccfromCreator = usersWithSupplierAccountID.Join(supAcc, u => u.UserName, sa => sa.SupplierAccountID, (u, sa) => new
                {
                    UserName = u.UserName,
                    Email = u.Email,
                    RealName = u.RealName,
                    CreatorEmployeeID = sa.CreatorEmployeeID,
                    SupplierAccountID = sa.SupplierAccountID,
                    SupplierCode = sa.SupplierCode
                }).Where(y => y.CreatorEmployeeID == LoginAccId);

                foreach (var x in usersOfsupAccfromCreator)
                {
                    var user = new
                    {
                        SupplierCode = await supInfo.Join(supAcc, s => s.SupplierCode, sa => sa.SupplierCode, (s, sa) => new { SupplierCode = s.SupplierCode, SupplierAccountID = sa.SupplierAccountID }).Where(y => y.SupplierAccountID == x.UserName).Select(y => y.SupplierCode).FirstOrDefaultAsync(),
                        SupplierName = await supInfo.Join(supAcc, s => s.SupplierCode, sa => sa.SupplierCode, (s, sa) => new { SupplierName = s.SupplierName, SupplierAccountID = sa.SupplierAccountID }).Where(y => y.SupplierAccountID == x.UserName).Select(y => y.SupplierName).FirstOrDefaultAsync(),
                        Email = x.Email,
                        Tel = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.Tel).FirstOrDefaultAsync(),
                        TaxID = await supInfo.Join(supAcc, s => s.SupplierCode, sa => sa.SupplierCode, (s, sa) => new { TaxID = s.TaxID, SupplierAccountID = sa.SupplierAccountID }).Where(y => y.SupplierAccountID == x.UserName).Select(y => y.TaxID).FirstOrDefaultAsync(),
                        SupplierRatingOID = await supInfo.Join(supAcc, s => s.SupplierCode, sa => sa.SupplierCode, (s, sa) => new { SupplierRatingOID = s.SupplierRatingOID, SupplierAccountID = sa.SupplierAccountID }).Where(y => y.SupplierAccountID == x.UserName).Select(y => y.SupplierRatingOID).FirstOrDefaultAsync(),
                        Address = await supInfo.Join(supAcc, s => s.SupplierCode, sa => sa.SupplierCode, (s, sa) => new { Address = s.Address, SupplierAccountID = sa.SupplierAccountID }).Where(y => y.SupplierAccountID == x.UserName).Select(y => y.Address).FirstOrDefaultAsync(),
                    };
                    sups.Add(user);
                }
            }
            return Json(sups, JsonRequestBehavior.AllowGet);
        }

        //SupInfoNoContactOnlySupInfo
        public JsonResult getAllSupInfoNoContactOnlySupInfoToIndexAjax()
        {
            var supInfos = db.SupplierInfo.Select(x => x.SupplierCode);
            var supAccs = db.SupplierAccount.Select(x => x.SupplierCode);
            var onlySupInfos = supInfos.Except(supAccs);

            List<SupplierInfo> NoContactOnlySupInfo = new List<SupplierInfo>();

            foreach (var os in onlySupInfos)
            {
                var s = db.SupplierInfo.Where(x => x.SupplierCode == os).FirstOrDefault();
                NoContactOnlySupInfo.Add(s);
            }
            //加jsonignore加到快崩潰..用匿名物件避開TMD的導覽屬性
            List<object> list = new List<object>();
            foreach (var s in NoContactOnlySupInfo)
            {
                var company = new
                {
                    SupplierCode = s.SupplierCode,
                    SupplierName = s.SupplierName,
                    TaxID = s.TaxID,
                    Address = s.Address,
                    Email = s.Email,
                    Tel = s.Tel,
                    SupplierRatingOID = s.SupplierRatingOID
                };
                list.Add(company);
            }
            if (list == null)
            {
                return Json(new EmptyResult(), JsonRequestBehavior.AllowGet);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}