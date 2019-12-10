using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using PMSAWebMVC.Models;
using PMSAWebMVC.Services;
using PMSAWebMVC.ViewModels.BuyerSupAccount;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.Controllers.BuyerSupAccountController
{
    [Authorize(Roles = "Buyer, Manager")]
    public class BuyerSupAccountController : BaseController
    {
        public BuyerSupAccountController()
        {
        }

        private PMSAEntities db = new PMSAEntities();
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public BuyerSupAccountController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
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

        // GET: BuyerSupAccount
        //TODO Manager 可看全部
        public ActionResult Index()
        {
            //主管可看到全部
            //採購員只能看到自己負責的
            return View();
        }

        //供應商公司確認新增後>新增一供應商帳號加入>
        // GET: BuyerSupAccount/Create
        public ActionResult Create()
        {
            BuyerSupAcc_Parent m = new BuyerSupAcc_Parent();
            return View(m);
        }

        [HttpPost]
        public ActionResult CreateSupInfo(SupInfoViewModel SupInfoModel)
        {
            //if (!ModelState.IsValid)
            //{
            //    BuyerSupAcc_Parent p = new BuyerSupAcc_Parent();
            //    p.SupInfoModel = SupInfoModel;
            //    return View("Create", p);
            //}

            //檢查是否有公司可選
            var result = getAllSupInfoNoContactOnlySupInfoToIndexAjax().Data;
            var data = JsonConvert.SerializeObject(result);
            if (data == "[]")
            {
                try
                {
                    //supInfo
                    var maxsupThanOID = db.SupplierInfo.Select(x => x.SupplierInfoOID).Max() + 1;
                    string SupCodestr = String.Format("S{0:00000}", Convert.ToDouble(maxsupThanOID));
                    SupplierInfo supinfo = new SupplierInfo();
                    supinfo.SupplierCode = SupCodestr;
                    supinfo.SupplierName = SupInfoModel.SupplierName;
                    supinfo.TaxID = SupInfoModel.TaxID;
                    supinfo.Tel = SupInfoModel.Tel;
                    supinfo.Email = SupInfoModel.Email;
                    supinfo.Address = SupInfoModel.Address;
                    supinfo.SupplierRatingOID = null;

                    db.SupplierInfo.Add(supinfo);
                    var r1 = db.SaveChanges();
                    if (r1 > 0)
                    {
                        //新增公司後回到view
                        TempData["Success"] = $"{supinfo.SupplierName} 更新成功";
                        return RedirectToAction("Create");
                    }
                    else
                    {
                        ModelState.AddModelError("", "填寫欄位有錯誤");
                        BuyerSupAcc_Parent p = new BuyerSupAcc_Parent();
                        p.SupInfoModel = SupInfoModel;
                        return View("Create", p);
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    var entityError = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
                    var getFullMessage = string.Join("; ", entityError);
                    var exceptionMessage = string.Concat(ex.Message, "errors are: ", getFullMessage);
                    Console.WriteLine(exceptionMessage);
                    if (!ModelState.IsValid)
                    {
                        BuyerSupAcc_Parent p = new BuyerSupAcc_Parent();
                        p.SupInfoModel = SupInfoModel;
                        return View("Create", p);
                    }
                }
            }
            //新增公司後回到view
            TempData["Success"] = "更新成功";
            return RedirectToAction("Create");
        }

        // POST: BuyerSupAccount/Create
        [HttpPost]
        public async Task<ActionResult> Create(BuyerSupAcc_Parent m, string AccStatus)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "填寫欄位有錯誤");
                    return View(m);
                }

                //==================================================================================
                var maxThanOID = db.SupplierAccount.Select(x => x.SupplierAccountOID).Max() + 1;

                string SupAccIDstr = String.Format("SE{0:00000}", Convert.ToDouble(maxThanOID));
                string pwd = generateFirstPwd();
                string hashpwd = UserManager.PasswordHasher.HashPassword(pwd);

                //sa
                SupplierAccount sa = new SupplierAccount();
                sa.SupplierCode = m.BuyerSupAccount_CreateViewModel.SupplierCode;
                sa.SupplierAccountID = SupAccIDstr;
                sa.ContactName = m.BuyerSupAccount_CreateViewModel.ContactName;
                sa.PasswordHash = hashpwd;
                //此欄位無用但非null
                sa.PasswordSalt = "fd357578-7784-4dea-b8c1-4d8b8d290d55";
                sa.Email = m.BuyerSupAccount_CreateViewModel.Email;
                sa.Address = null;
                sa.Mobile = m.BuyerSupAccount_CreateViewModel.Mobile;
                sa.Tel = m.BuyerSupAccount_CreateViewModel.Tel;
                sa.AccountStatus = m.BuyerSupAccount_CreateViewModel.AccountStatus ? "R" : "D";
                sa.CreateDate = DateTime.Now;
                sa.CreatorEmployeeID = User.Identity.GetEmployee().EmployeeID;
                sa.ModifiedDate = DateTime.Now;
                sa.SendLetterStatus = null;
                sa.SendLetterDate = null;

                //user
                ApplicationUser user = new ApplicationUser();
                user.Id = Guid.NewGuid().ToString();
                user.Email = m.BuyerSupAccount_CreateViewModel.Email;
                user.PasswordHash = hashpwd;
                user.PhoneNumber = m.BuyerSupAccount_CreateViewModel.Mobile;
                user.UserName = SupAccIDstr;
                user.RealName = m.BuyerSupAccount_CreateViewModel.ContactName;
                user.LastPasswordChangedDate = null;

                var r1 = UserManager.Create(user);
                db.SupplierAccount.Add(sa);
                //var r2 = UserManager.Update(user);
                if (r1.Succeeded)
                {
                    //確定聯絡人信箱沒重複才可存到Acctable
                    var r2 = db.SaveChanges();
                    //判斷是否要寄信 補寄信
                    if (AccStatus == "on")
                    {
                        await sendMailatIndex(user, user.UserName);
                    }
                    TempData["Success"] = $"更新成功";
                    return View("Index");
                }
                else
                {
                    var err = string.Join(",", r1.Errors);
                    ModelState.AddModelError("", err);
                    return View("Create");
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (DbEntityValidationResult item in ex.EntityValidationErrors)
                {
                    // Get entry

                    DbEntityEntry entry = item.Entry;
                    string entityTypeName = entry.Entity.GetType().Name;

                    // Display or log error messages

                    foreach (DbValidationError subItem in item.ValidationErrors)
                    {
                        string message = string.Format("Error '{0}' occurred in {1} at {2}",
                                 subItem.ErrorMessage, entityTypeName, subItem.PropertyName);
                        Console.WriteLine(message);
                    }
                }
                TempData["Error"] = "對不起，新增失敗，請檢查網路連線再重試一次";
                return View();
            }
        }

        //updateSupAccUsersAtIndex
        //TODO 如何讓對應的人只能更新對應的 SupId
        //TODO 有SupId的地方要再比對一次是不是該登入者負責的
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> updateSupAccUsersAtIndex(string SupId, bool AccStatus, bool sendResetMail)
        {
            var user = await UserManager.Users.Where(x => x.UserName.Contains("S") && x.UserName == SupId).SingleOrDefaultAsync();

            //啟用 AccStatus==true//先判斷DB是否 D -> RE // && db.Employee.Where(x => x.AccountStatus == "D" && x.EmployeeID == EmpId).Any()
            if (AccStatus == true)
            {
                //如果勾選寄信重設密碼
                if (sendResetMail == true)
                {
                    // 帳戶確認及密碼重設 //user emp confirmemail // user emp pwd // emp SendLetterStatus="Y" SendLetterDate datetime.now 已重設還沒存 db
                    await sendMailatIndex(user, SupId);
                }

                var supAcc = db.SupplierAccount.Where(x => x.SupplierAccountID == SupId).SingleOrDefault();
                supAcc.ModifiedDate = DateTime.Now;

                //新增Supplier
                var userRoles = await UserManager.GetRolesAsync(user.Id);
                if (!userRoles.Contains("Supplier"))
                {
                    user.Roles.Clear();
                    var result = await UserManager.AddToRolesAsync(user.Id, "Supplier");
                }
                //更新紀錄狀態的欄位
                await AccStatusReset(SupId);
            }
            //停用
            else if (AccStatus == false)
            {
                var supAcc = db.SupplierAccount.Where(x => x.SupplierAccountID == SupId).SingleOrDefault();
                supAcc.ModifiedDate = DateTime.Now;
                await AccStatusDisable(SupId);
            }

            return View("Index");
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
                        SupplierAccountID = x.UserName,
                        ContactName = x.RealName,
                        Email = x.Email,
                        Mobile = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.Mobile).FirstOrDefaultAsync(),
                        Tel = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.Tel).FirstOrDefaultAsync(),
                        AccountStatus = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.AccountStatus).FirstOrDefaultAsync(),
                        ModifiedDate = JsonConvert.SerializeObject(await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.ModifiedDate).FirstOrDefaultAsync(), js),
                        CreatorEmployeeID = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.CreatorEmployeeID).FirstOrDefaultAsync(),
                        CreateDate = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.CreateDate).FirstOrDefaultAsync() == null ? null : JsonConvert.SerializeObject(await db.SupplierAccount.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.CreateDate).FirstOrDefaultAsync(), js),
                        SendLetterDate = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.SendLetterDate).FirstOrDefaultAsync() == null ? null : JsonConvert.SerializeObject(await db.SupplierAccount.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.SendLetterDate).FirstOrDefaultAsync(), js),
                        LastPasswordChangedDate = await UserManager.Users.Where(e => e.UserName == x.UserName).Select(e => e.LastPasswordChangedDate).FirstOrDefaultAsync() == null ? null : JsonConvert.SerializeObject(await UserManager.Users.Where(e => e.UserName == x.UserName).Select(e => e.LastPasswordChangedDate).FirstOrDefaultAsync(), js)
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
                        SupplierAccountID = x.UserName,
                        ContactName = x.RealName,
                        Email = x.Email,
                        Mobile = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.Mobile).FirstOrDefaultAsync(),
                        Tel = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.Tel).FirstOrDefaultAsync(),
                        AccountStatus = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.AccountStatus).FirstOrDefaultAsync(),
                        ModifiedDate = JsonConvert.SerializeObject(await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.ModifiedDate).FirstOrDefaultAsync(), js),
                        CreatorEmployeeID = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.CreatorEmployeeID).FirstOrDefaultAsync(),
                        CreateDate = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.CreateDate).FirstOrDefaultAsync() == null ? null : JsonConvert.SerializeObject(await db.SupplierAccount.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.CreateDate).FirstOrDefaultAsync(), js),
                        SendLetterDate = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.SendLetterDate).FirstOrDefaultAsync() == null ? null : JsonConvert.SerializeObject(await db.SupplierAccount.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.SendLetterDate).FirstOrDefaultAsync(), js),
                        LastPasswordChangedDate = await UserManager.Users.Where(e => e.UserName == x.UserName).Select(e => e.LastPasswordChangedDate).FirstOrDefaultAsync() == null ? null : JsonConvert.SerializeObject(await UserManager.Users.Where(e => e.UserName == x.UserName).Select(e => e.LastPasswordChangedDate).FirstOrDefaultAsync(), js)
                    };
                    sups.Add(user);
                }
            }
            return Json(sups, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> getManagerAllSupAccToIndexAjax()
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
                    SupplierAccountID = x.UserName,
                    ContactName = x.RealName,
                    Email = x.Email,
                    Mobile = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.Mobile).FirstOrDefaultAsync(),
                    Tel = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.Tel).FirstOrDefaultAsync(),
                    AccountStatus = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.AccountStatus).FirstOrDefaultAsync(),
                    ModifiedDate = JsonConvert.SerializeObject(await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.ModifiedDate).FirstOrDefaultAsync(), js),
                    CreatorEmployeeID = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.CreatorEmployeeID).FirstOrDefaultAsync(),
                    CreateDate = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.CreateDate).FirstOrDefaultAsync() == null ? null : JsonConvert.SerializeObject(await db.SupplierAccount.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.CreateDate).FirstOrDefaultAsync(), js),
                    SendLetterDate = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.SendLetterDate).FirstOrDefaultAsync() == null ? null : JsonConvert.SerializeObject(await db.SupplierAccount.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.SendLetterDate).FirstOrDefaultAsync(), js),
                    LastPasswordChangedDate = await UserManager.Users.Where(e => e.UserName == x.UserName).Select(e => e.LastPasswordChangedDate).FirstOrDefaultAsync() == null ? null : JsonConvert.SerializeObject(await UserManager.Users.Where(e => e.UserName == x.UserName).Select(e => e.LastPasswordChangedDate).FirstOrDefaultAsync(), js)
                };
                sups.Add(user);
            }
            return Json(sups, JsonRequestBehavior.AllowGet);
        }

        //SupCode 拿 supAcc
        public async Task<ActionResult> getAllSupAccBySupCodeToIndexAjax(string id)
        {
            var supSelected = db.SupplierAccount.Where(x => x.SupplierCode == id).FirstOrDefault();
            List<ApplicationUser> usersWithSupplierAccountID = await UserManager.Users.Where(x => x.UserName.Contains("S") && x.UserName == supSelected.SupplierAccountID).ToListAsync();
            var supInfo = db.SupplierInfo;
            var supAcc = db.SupplierAccount;
            var rating = db.SupplierRating;
            //datatime 要轉型
            var js = new JsonSerializerSettings()
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };
            foreach (var x in usersWithSupplierAccountID)
            {
                var user = new
                {
                    SupplierCode = await supInfo.Join(supAcc, s => s.SupplierCode, sa => sa.SupplierCode, (s, sa) => new { SupplierCode = s.SupplierCode, SupplierAccountID = sa.SupplierAccountID }).Where(y => y.SupplierAccountID == x.UserName).Select(y => y.SupplierCode).FirstOrDefaultAsync(),
                    SupplierName = await supInfo.Join(supAcc, s => s.SupplierCode, sa => sa.SupplierCode, (s, sa) => new { SupplierName = s.SupplierName, SupplierAccountID = sa.SupplierAccountID }).Where(y => y.SupplierAccountID == x.UserName).Select(y => y.SupplierName).FirstOrDefaultAsync(),
                    SupplierAccountID = x.UserName,
                    ContactName = x.RealName,
                    Email = x.Email,
                    Mobile = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.Mobile).FirstOrDefaultAsync(),
                    Tel = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.Tel).FirstOrDefaultAsync(),
                    AccountStatus = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.AccountStatus).FirstOrDefaultAsync(),
                    ModifiedDate = JsonConvert.SerializeObject(await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.ModifiedDate).FirstOrDefaultAsync(), js),
                    CreatorEmployeeID = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.CreatorEmployeeID).FirstOrDefaultAsync(),
                    CreateDate = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.CreateDate).FirstOrDefaultAsync() == null ? null : JsonConvert.SerializeObject(await db.SupplierAccount.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.CreateDate).FirstOrDefaultAsync(), js),
                    SendLetterDate = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.SendLetterDate).FirstOrDefaultAsync() == null ? null : JsonConvert.SerializeObject(await db.SupplierAccount.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.SendLetterDate).FirstOrDefaultAsync(), js),
                    LastPasswordChangedDate = await UserManager.Users.Where(e => e.UserName == x.UserName).Select(e => e.LastPasswordChangedDate).FirstOrDefaultAsync() == null ? null : JsonConvert.SerializeObject(await UserManager.Users.Where(e => e.UserName == x.UserName).Select(e => e.LastPasswordChangedDate).FirstOrDefaultAsync(), js)
                };
                return Json(user, JsonRequestBehavior.AllowGet);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
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

        //SupCode 拿 SupInfo
        public ActionResult getSupInfoBySupCodeToIndexAjax(string id)
        {
            var supInfos = db.SupplierInfo.Where(x => x.SupplierCode == id).ToList();

            //加jsonignore加到快崩潰..用匿名物件避開TMD的導覽屬性
            List<object> list = new List<object>();
            foreach (var s in supInfos)
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
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //====================================================================
        //寄信// 帳戶確認及密碼重設
        //用 SupplierAccountID 寄信
        [HttpPost]
        public async Task sendMail(string Id)
        {
            var user = await UserManager.Users.Where(x => x.UserName.Contains("S") && x.UserName == Id).SingleOrDefaultAsync();
            //sa table user table
            //重設資料庫該 user 密碼 並 hash 存入 db
            //重設db密碼
            //1.重設 user 密碼
            string pwd = generateFirstPwd();
            await UserManager.UpdateSecurityStampAsync(user.Id);
            user.PasswordHash = UserManager.PasswordHasher.HashPassword(pwd);
            user.LastPasswordChangedDate = null;

            var SupAcc = db.SupplierAccount.Where(x => x.SupplierAccountID == Id).SingleOrDefault();
            SupAcc.PasswordHash = user.PasswordHash;

            // 傳送包含此連結的電子郵件
            var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("PMSAWebMVC");
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            string tempMail = System.IO.File.ReadAllText(Server.MapPath(@"~\Views\Shared\ResetPwdSupEmailTemplate.html"));
            // 經測試 gmail 不支援 uri data image 所以用網址傳圖比較保險
            string img = "https://ci5.googleusercontent.com/proxy/4OJ0k4udeu09Coqzi7ZQRlKXsHTtpTKlg0ungn0aWQAQs2j1tTS6Q6e8E0dZVW2qsbzD1tod84Zbsx62gMgHLFGWigDzFOPv1qBrzhyFIlRYJWSMWH8=s0-d-e1-ft#https://app.flashimail.com/rest/images/5d8108c8e4b0f9c17e91fab7.jpg";

            string SupAccIDstr = user.UserName;
            string MailBody = MembersDBService.getMailBody(tempMail, img, callbackUrl, pwd, SupAccIDstr);

            //寄信
            await UserManager.SendEmailAsync(user.Id, "重設您的密碼", MailBody);

            //3.更新db寄信相關欄位
            //SendLetterDate
            SupAcc.SendLetterDate = DateTime.Now;
            //SendLetterStatus
            SupAcc.SendLetterStatus = "S";

            await updateTable(user, SupAcc);

            //新增Supplier
            var userRoles = await UserManager.GetRolesAsync(user.Id);
            if (!userRoles.Contains("Supplier"))
            {
                user.Roles.Clear();
                var result = await UserManager.AddToRolesAsync(user.Id, "Supplier");
            }

            //更新狀態欄位 user sa table
            await AccStatusReset(Id);
        }

        // 帳戶確認及密碼重設
        //用 SupplierAccountID 寄信
        private async Task sendMailatIndex(ApplicationUser user, string Id)
        {
            //sa table user table
            //重設資料庫該 user 密碼 並 hash 存入 db
            //重設db密碼
            //1.重設 user 密碼
            string pwd = generateFirstPwd();
            await UserManager.UpdateSecurityStampAsync(user.Id);
            user.PasswordHash = UserManager.PasswordHasher.HashPassword(pwd);
            user.LastPasswordChangedDate = null;

            var supAcc = db.SupplierAccount.Where(x => x.SupplierAccountID == Id).SingleOrDefault();
            supAcc.PasswordHash = user.PasswordHash;

            // 如需如何進行帳戶確認及密碼重設的詳細資訊，請前往 https://go.microsoft.com/fwlink/?LinkID=320771
            // 傳送包含此連結的電子郵件
            var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("PMSAWebMVC");
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            string tempMail = System.IO.File.ReadAllText(Server.MapPath(@"~\Views\Shared\ResetPwdSupEmailTemplate.html"));
            // 經測試 gmail 不支援 uri data image 所以用網址傳圖比較保險
            string img = "https://ci5.googleusercontent.com/proxy/4OJ0k4udeu09Coqzi7ZQRlKXsHTtpTKlg0ungn0aWQAQs2j1tTS6Q6e8E0dZVW2qsbzD1tod84Zbsx62gMgHLFGWigDzFOPv1qBrzhyFIlRYJWSMWH8=s0-d-e1-ft#https://app.flashimail.com/rest/images/5d8108c8e4b0f9c17e91fab7.jpg";
            string SupAccIDstr = user.UserName;
            string MailBody = MembersDBService.getMailBody(tempMail, img, callbackUrl, pwd, SupAccIDstr);

            //寄信
            await UserManager.SendEmailAsync(user.Id, "重設您的密碼", MailBody);

            //3.更新db寄信相關欄位
            //SendLetterDate
            supAcc.SendLetterDate = DateTime.Now;
            //SendLetterStatus
            supAcc.SendLetterStatus = "S";

            //新增Supplier
            var userRoles = await UserManager.GetRolesAsync(user.Id);
            if (!userRoles.Contains("Supplier"))
            {
                user.Roles.Clear();
                var result = await UserManager.AddToRolesAsync(user.Id, "Supplier");
            }

            await updateTable(user, supAcc);
            //更新狀態欄位 user supAcc table
            await AccStatusReset(Id);
        }

        //Disable 帳號為停用時 LastPasswordChangedDate = null /  Role 重設為 NewEmployee
        //R -> D
        //用 SupplierAccountID
        private async Task AccStatusDisable(string Id)
        {
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var user = await UserManager.FindByNameAsync(Id);

            var supAcc = db.SupplierAccount.Where(x => x.SupplierAccountID == Id).SingleOrDefault();
            supAcc.AccountStatus = "D";

            //Disable 帳號為停用時 LastPasswordChangedDate = null /  Role 重設為 NewEmployee
            user.LastPasswordChangedDate = null;
            user.Roles.Clear();
            var role = RoleManager.FindByName("NewEmployee");
            IdentityUserRole r = new IdentityUserRole()
            {
                RoleId = role.Id,
                UserId = user.Id
            };
            user.Roles.Add((IdentityUserRole)r);

            //更新此 user
            var result = await UserManager.UpdateAsync(user);

            //4.存到資料庫
            //更新此 user tablevar resultOfupdate =
            await updateTable(user, supAcc);
            //if (result.Succeeded && resultOfupdate > 0)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }

        //Reset 帳號為重啟時 LastPasswordChangedDate = null /  SendLetterDate / resetPwd 和 寄信
        //D -> R
        //用 SupplierAccountID
        private async Task AccStatusReset(string Id)
        {
            var user = await UserManager.FindByNameAsync(Id);
            var supAcc = db.SupplierAccount.Where(x => x.SupplierAccountID == Id).SingleOrDefault();

            //supAcc table
            //寄信成功狀態設為 R
            supAcc.AccountStatus = "R";

            //4.存到資料庫
            //更新此 user table var resultOfupdate =
            await updateTable(user, supAcc);
            //if (result.Succeeded && resultOfupdate > 0)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }

        //====================================================================
        public string generateFirstPwd()
        {
            //salt: 隨機位元組=>演算法=>計算後給值
            RNGCryptoServiceProvider rngbyte = new RNGCryptoServiceProvider();
            byte[] bytesalt = new byte[8];
            rngbyte.GetBytes(bytesalt);
            string salty = Convert.ToBase64String(bytesalt);
            Regex regex = new Regex(@"(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])");
            if (regex.IsMatch(salty))
            {
                return salty;
            }
            else
            {
                return generateFirstPwd();
            }
        }

        //4.存到資料庫
        //更新此 user table
        private async Task updateTable(ApplicationUser user, SupplierAccount sa)
        {
            //更新此 user table
            await UserManager.UpdateAsync(user);
            //sa table
            db.Entry(sa).State = EntityState.Modified;
            int u2 = await db.SaveChangesAsync();
        }
    }
}