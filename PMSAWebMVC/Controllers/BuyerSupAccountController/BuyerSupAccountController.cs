using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using PMSAWebMVC.Models;
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
    public class BuyerSupAccountController : Controller
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
        public ActionResult Index()
        {
            return View();
        }

        //供應商公司確認新增後>新增一供應商帳號加入>
        // GET: BuyerSupAccount/Create
        public ActionResult Create()
        {
            BuyerSupAcc_Parent m = new BuyerSupAcc_Parent();
            return View(m);
        }

        // POST: BuyerSupAccount/Create
        [HttpPost]
        public ActionResult Create(BuyerSupAcc_Parent m)
        {
            try
            {
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

                db.SaveChanges();

                //user
                ApplicationUser user = new ApplicationUser();
                user.Id = Guid.NewGuid().ToString();
                user.Email = m.BuyerSupAccount_CreateViewModel.Email;
                user.PasswordHash = hashpwd;
                UserManager.UpdateSecurityStamp(user.Id);
                user.PhoneNumber = m.BuyerSupAccount_CreateViewModel.Mobile;
                user.UserName = SupAccIDstr;
                user.RealName = m.BuyerSupAccount_CreateViewModel.ContactName;
                user.LastPasswordChangedDate = null;

                UserManager.Create(user);
                ViewBag.msg = "新增成功!";
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
                ViewBag.msg = "對不起，新增失敗，請檢查網路連線再重試一次";
                return View();
            }
            return View("Index");
        }

        public async Task<ActionResult> getAllSupAccToIndexAjax()
        {
            List<ApplicationUser> usersWithSupplierAccountID = await UserManager.Users.Where(x => x.UserName.Contains("S")).ToListAsync();
            List<object> sups = new List<object>();
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
                    CreateDate = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.CreateDate).FirstOrDefaultAsync() == null ? null : JsonConvert.SerializeObject(await db.Employee.Where(e => e.EmployeeID == x.UserName).Select(e => e.CreateDate).FirstOrDefaultAsync(), js),
                    SendLetterDate = await supAcc.Where(e => e.SupplierAccountID == x.UserName).Select(e => e.SendLetterDate).FirstOrDefaultAsync() == null ? null : JsonConvert.SerializeObject(await db.Employee.Where(e => e.EmployeeID == x.UserName).Select(e => e.SendLetterDate).FirstOrDefaultAsync(), js),
                    LastPasswordChangedDate = await UserManager.Users.Where(e => e.UserName == x.UserName).Select(e => e.LastPasswordChangedDate).FirstOrDefaultAsync() == null ? null : JsonConvert.SerializeObject(await UserManager.Users.Where(e => e.UserName == x.UserName).Select(e => e.LastPasswordChangedDate).FirstOrDefaultAsync(), js)
                };
                sups.Add(user);
            }
            return Json(sups, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getAllSupInfoNoContactOnlySupInfoToIndexAjax()
        {
            var supInfos = db.SupplierInfo.Select(x => x.SupplierCode).ToList();
            var supAccs = db.SupplierAccount.Select(x => x.SupplierCode).ToList();
            var onlySupInfos = supInfos.Except(supAccs);

            List<SupplierInfo> NoContactOnlySupInfo = new List<SupplierInfo>();

            foreach (var os in onlySupInfos)
            {
                NoContactOnlySupInfo = db.SupplierInfo.Where(x => x.SupplierCode == os).ToList();
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
            return Json(list, JsonRequestBehavior.AllowGet);
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
        private async Task updateTable(ApplicationUser user, SupplierAccount sa, SupplierInfo supInfo, SupplierRating supRate)
        {
            //4.存到資料庫
            //更新此 user table
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            await UserManager.UpdateAsync(user);
            var ctx = store.Context;
            await ctx.SaveChangesAsync();
            //sa table
            db.Entry(sa).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }
    }
}