using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using PMSAWebMVC.Models;
using PMSAWebMVC.Services;
using PMSAWebMVC.ViewModels.RolesAdmin;
using PMSAWebMVC.ViewModels.UsersAdmin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersAdminController : BaseController
    {
        private PMSAEntities db = new PMSAEntities();

        public UsersAdminController()
        {
        }

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
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

        //===========================================================================
        [HttpPost]
        public JsonResult GetUsersFromEmp(string EmpID)
        {
            List<System.Collections.Generic.KeyValuePair<string, string[]>> items = new List<KeyValuePair<string, string[]>>();

            if (!string.IsNullOrWhiteSpace(EmpID))
            {
                var Emps = this.GetEmpID(EmpID);
                if (Emps.Count() > 0)
                {
                    foreach (var Emp in Emps)
                    {
                        items.Add(new KeyValuePair<string, string[]>(
                            Emp.EmployeeID.ToString(), new string[] { Emp.EmployeeID, Emp.Name, Emp.Email, Emp.Mobile, Emp.Tel }
                        ));
                    }
                }
            }
            return this.Json(items);
        }

        private IEnumerable<Employee> GetEmpID(string EmpID)
        {
            using (PMSAEntities db = new PMSAEntities())
            {
                var query = db.Employee.Where(x => x.EmployeeID == EmpID);
                return query.ToList();
            }
        }

        //==========================================================================
        //
        // GET: /Users/
        public async Task<ActionResult> Index()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> getEmpByIDIndexAjax(string EmpId)
        {
            var usersWithEmpID = await UserManager.Users.Where(x => x.UserName.Contains("C") && x.UserName == EmpId).FirstOrDefaultAsync();
            PMSAEntities db = new PMSAEntities();

            //取得此 empId 找Role 加到 RolesList
            var user = await UserManager.FindByIdAsync(usersWithEmpID.Id);
            var userRoles = await UserManager.GetRolesAsync(user.Id);

            //datatime 要轉型
            var js = new JsonSerializerSettings()
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            var userA = new
            {
                EmployeeID = user.UserName,
                Name = user.RealName,
                Role = await UserManager.GetRolesAsync(user.Id),
                Email = user.Email,
                Mobile = await db.Employee.Where(e => e.EmployeeID == user.UserName).Select(e => e.Mobile).FirstOrDefaultAsync(),
                Tel = await db.Employee.Where(e => e.EmployeeID == user.UserName).Select(e => e.Tel).FirstOrDefaultAsync(),
                AccountStatus = await db.Employee.Where(e => e.EmployeeID == user.UserName).Select(e => e.AccountStatus).FirstOrDefaultAsync(),
                ModifiedDate = JsonConvert.SerializeObject(await db.Employee.Where(e => e.EmployeeID == user.UserName).Select(e => e.ModifiedDate).FirstOrDefaultAsync(), js),
                ManagerID = await db.Employee.Where(e => e.EmployeeID == user.UserName).Select(e => e.ManagerID).FirstOrDefaultAsync(),
                CreateDate = await db.Employee.Where(e => e.EmployeeID == user.UserName).Select(e => e.CreateDate).FirstOrDefaultAsync() == null ? null : JsonConvert.SerializeObject(await db.Employee.Where(e => e.EmployeeID == user.UserName).Select(e => e.CreateDate).FirstOrDefaultAsync(), js),
                SendLetterDate = await db.Employee.Where(e => e.EmployeeID == user.UserName).Select(e => e.SendLetterDate).FirstOrDefaultAsync() == null ? null : JsonConvert.SerializeObject(await db.Employee.Where(e => e.EmployeeID == user.UserName).Select(e => e.SendLetterDate).FirstOrDefaultAsync(), js),
                LastPasswordChangedDate = await UserManager.Users.Where(e => e.UserName == user.UserName).Select(e => e.LastPasswordChangedDate).FirstOrDefaultAsync() == null ? null : JsonConvert.SerializeObject(await UserManager.Users.Where(e => e.UserName == user.UserName).Select(e => e.LastPasswordChangedDate).FirstOrDefaultAsync(), js),
                //取得 所有Role列表，若有出現在該userA中 Selected = true
                RolesList = RoleManager.Roles.ToList().Select(r => new
                {
                    Selected = userRoles.Contains(r.Name),
                    Text = r.Description,
                    Value = r.Name
                }),
                EmailConfirm = user.EmailConfirmed
            };

            return Json(userA, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> getAllEmpToIndexAjax()
        {
            List<ApplicationUser> usersWithEmpID = await UserManager.Users.Where(x => x.UserName.Contains("C")).ToListAsync();
            //return View(await UserManager.Users.ToListAsync());
            PMSAEntities db = new PMSAEntities();
            List<object> emps = new List<object>();
            //datatime 要轉型
            var js = new JsonSerializerSettings()
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };
            foreach (var x in usersWithEmpID)
            {
                var user = new
                {
                    EmployeeID = x.UserName,
                    Name = x.RealName,
                    Role = await UserManager.GetRolesAsync(x.Id),
                    Email = x.Email,
                    Mobile = await db.Employee.Where(e => e.EmployeeID == x.UserName).Select(e => e.Mobile).FirstOrDefaultAsync(),
                    Tel = await db.Employee.Where(e => e.EmployeeID == x.UserName).Select(e => e.Tel).FirstOrDefaultAsync(),
                    AccountStatus = await db.Employee.Where(e => e.EmployeeID == x.UserName).Select(e => e.AccountStatus).FirstOrDefaultAsync(),
                    ModifiedDate = JsonConvert.SerializeObject(await db.Employee.Where(e => e.EmployeeID == x.UserName).Select(e => e.ModifiedDate).FirstOrDefaultAsync(), js),
                    ManagerID = await db.Employee.Where(e => e.EmployeeID == x.UserName).Select(e => e.ManagerID).FirstOrDefaultAsync(),
                    CreateDate = await db.Employee.Where(e => e.EmployeeID == x.UserName).Select(e => e.CreateDate).FirstOrDefaultAsync() == null ? null : JsonConvert.SerializeObject(await db.Employee.Where(e => e.EmployeeID == x.UserName).Select(e => e.CreateDate).FirstOrDefaultAsync(), js),
                    SendLetterDate = await db.Employee.Where(e => e.EmployeeID == x.UserName).Select(e => e.SendLetterDate).FirstOrDefaultAsync() == null ? null : JsonConvert.SerializeObject(await db.Employee.Where(e => e.EmployeeID == x.UserName).Select(e => e.SendLetterDate).FirstOrDefaultAsync(), js),
                    LastPasswordChangedDate = await UserManager.Users.Where(e => e.UserName == x.UserName).Select(e => e.LastPasswordChangedDate).FirstOrDefaultAsync() == null ? null : JsonConvert.SerializeObject(await UserManager.Users.Where(e => e.UserName == x.UserName).Select(e => e.LastPasswordChangedDate).FirstOrDefaultAsync(), js),
                    EmailConfirm = x.EmailConfirmed
                };
                emps.Add(user);
            }
            return Json(emps, JsonRequestBehavior.AllowGet);
        }

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

        //管理員修改採購員帳號(啟用停用/角色)
        // POST: /Users/Create
        //TODO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> updateEmployeeUsersAtIndex(string EmpId, bool AccStatus, bool sendResetMail, params string[] selectedRole)
        {
            var user = await UserManager.Users.Where(x => x.UserName.Contains("C") && x.UserName == EmpId).SingleOrDefaultAsync();

            //role table
            //角色更新到db
            //更新db角色
            var userRoles = await UserManager.GetRolesAsync(user.Id);
            selectedRole = selectedRole ?? new string[] { };

            var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());
            result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

            //更新此 user
            //var r3 = await UserManager.UpdateAsync(user);

            //啟用 AccStatus==true//先判斷DB是否 D -> RE // && db.Employee.Where(x => x.AccountStatus == "D" && x.EmployeeID == EmpId).Any()
            if (AccStatus == true)
            {
                //如果勾選寄信重設密碼
                if (sendResetMail == true)
                {
                    // 帳戶確認及密碼重設 //user emp confirmemail // user emp pwd // emp SendLetterStatus="Y" SendLetterDate datetime.now 已重設還沒存 db
                    await sendMailatIndex(user, EmpId);
                }

                var emp = db.Employee.Where(x => x.EmployeeID == EmpId).SingleOrDefault();
                emp.ModifiedDate = DateTime.Now;
                //更新紀錄狀態的欄位
                await AccStatusReset(EmpId);
            }
            //停用
            else if (AccStatus == false)
            {
                var emp = db.Employee.Where(x => x.EmployeeID == EmpId).SingleOrDefault();
                emp.ModifiedDate = DateTime.Now;
                await AccStatusDisable(EmpId);
            }

            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            return View("Index");
        }

        // 帳戶確認及密碼重設
        [HttpPost]
        public async Task sendMail(string EmpId)
        {
            var user = await UserManager.Users.Where(x => x.UserName.Contains("C") && x.UserName == EmpId).SingleOrDefaultAsync();

            //emp table user table
            //重設資料庫該 user 密碼 並 hash 存入 db
            //重設db密碼
            //1.重設 user 密碼
            string pwd = generateFirstPwd();
            await UserManager.UpdateSecurityStampAsync(user.Id);
            user.PasswordHash = UserManager.PasswordHasher.HashPassword(pwd);
            user.LastPasswordChangedDate = null;
            await UserManager.UpdateAsync(user);

            var emp = db.Employee.Where(x => x.EmployeeID == EmpId).SingleOrDefault();
            emp.PasswordHash = user.PasswordHash;

            // 傳送包含此連結的電子郵件
            var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("PMSAWebMVC");
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            string tempMail = System.IO.File.ReadAllText(Server.MapPath(@"~\Views\Shared\ResetPwdEmailTemplate.html"));
            // 經測試 gmail 不支援 uri data image 所以用網址傳圖比較保險
            string img = "https://ci5.googleusercontent.com/proxy/4OJ0k4udeu09Coqzi7ZQRlKXsHTtpTKlg0ungn0aWQAQs2j1tTS6Q6e8E0dZVW2qsbzD1tod84Zbsx62gMgHLFGWigDzFOPv1qBrzhyFIlRYJWSMWH8=s0-d-e1-ft#https://app.flashimail.com/rest/images/5d8108c8e4b0f9c17e91fab7.jpg";
            string MailBody = MembersDBService.getMailBody(tempMail, img, callbackUrl, pwd);
            //寄信
            await UserManager.SendEmailAsync(user.Id, "重設您的密碼", MailBody);

            //3.更新db寄信相關欄位
            //SendLetterDate
            emp.SendLetterDate = DateTime.Now;
            //SendLetterStatus
            emp.SendLetterStatus = "S";

            //更新狀態欄位 user emo table
            await AccStatusReset(EmpId);
        }

        // 帳戶確認及密碼重設
        private async Task sendMailatIndex(ApplicationUser user, string EmpId)
        {
            //emp table user table
            //重設資料庫該 user 密碼 並 hash 存入 db
            //重設db密碼
            //1.重設 user 密碼
            string pwd = generateFirstPwd();
            await UserManager.UpdateSecurityStampAsync(user.Id);
            user.PasswordHash = UserManager.PasswordHasher.HashPassword(pwd);
            user.LastPasswordChangedDate = null;
            await UserManager.UpdateAsync(user);

            var emp = db.Employee.Where(x => x.EmployeeID == EmpId).SingleOrDefault();
            emp.PasswordHash = user.PasswordHash;

            // 如需如何進行帳戶確認及密碼重設的詳細資訊，請前往 https://go.microsoft.com/fwlink/?LinkID=320771
            // 傳送包含此連結的電子郵件
            var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("PMSAWebMVC");
            //更改密碼要在code之前不然他是拿UpdateSecurityStampAsync 來生code的
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            string tempMail = System.IO.File.ReadAllText(Server.MapPath(@"~\Views\Shared\ResetPwdEmailTemplate.html"));
            // 經測試 gmail 不支援 uri data image 所以用網址傳圖比較保險
            string img = "https://ci5.googleusercontent.com/proxy/4OJ0k4udeu09Coqzi7ZQRlKXsHTtpTKlg0ungn0aWQAQs2j1tTS6Q6e8E0dZVW2qsbzD1tod84Zbsx62gMgHLFGWigDzFOPv1qBrzhyFIlRYJWSMWH8=s0-d-e1-ft#https://app.flashimail.com/rest/images/5d8108c8e4b0f9c17e91fab7.jpg";
            string MailBody = MembersDBService.getMailBody(tempMail, img, callbackUrl, pwd);

            //寄信
            await UserManager.SendEmailAsync(user.Id, "重設您的密碼", MailBody);

            //3.更新db寄信相關欄位
            //SendLetterDate
            emp.SendLetterDate = DateTime.Now;
            //SendLetterStatus
            emp.SendLetterStatus = "S";

            //更新狀態欄位 user emo table
            await AccStatusReset(EmpId);
        }

        //Disable 帳號為停用時 LastPasswordChangedDate = null /  Role 重設為 NewEmployee
        //R -> D
        private async Task AccStatusDisable(string EmpId)
        {
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var user = await UserManager.FindByNameAsync(EmpId);

            var emp = db.Employee.Where(x => x.EmployeeID == EmpId).SingleOrDefault();
            emp.AccountStatus = "D";

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

            //信箱驗證也要重置為0
            user.EmailConfirmed = false;

            //更新此 user
            await UserManager.UpdateAsync(user);

            //4.存到資料庫
            //更新此 user table
            await updateEmpUserTable(user, emp);
        }

        //Reset 帳號為重啟時 LastPasswordChangedDate = null /  SendLetterDate / resetPwd 和 寄信
        //D -> R
        private async Task AccStatusReset(string EmpId)
        {
            var user = await UserManager.FindByNameAsync(EmpId);
            var emp = db.Employee.Where(x => x.EmployeeID == EmpId).SingleOrDefault();

            //有寄過信了
            //if (emp.SendLetterDate != null)
            //{
            //    DateTime s = (DateTime)emp.SendLetterDate;
            //    var tokenOverTime = (DateTime.Now - s).TotalSeconds > s.AddDays(1).Second;
            //    ////檢查token是否過期 //過期才重發密碼x -> 想發就發才對
            //    // 如果過期 R-> D
            //    // token過期 / 沒登入過
            //    if (tokenOverTime && user.LastPasswordChangedDate == null)
            //    {
            //        emp.AccountStatus = "D";
            //        await AccStatusDisable(EmpId);
            //    }
            //}

            //emp table
            //寄信成功狀態設為 R
            emp.AccountStatus = "R";

            //4.存到資料庫
            //更新此 user table
            await updateEmpUserTable(user, emp);
        }

        //4.存到資料庫
        //更新此 user table
        //await updateEmpUserTable(user, emp);
        private async Task updateEmpUserTable(ApplicationUser user, Employee emp)
        {
            //4.存到資料庫
            //更新此 user table
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            await UserManager.UpdateAsync(user);
            var ctx = store.Context;
            await ctx.SaveChangesAsync();
            //Emp table
            db.Entry(emp).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }

        //TODO ConfirmEmail ResetPwd
        //Enable 帳號為啟用時
        //R -> E
        private async Task AccStatusEnable(string EmpId)
        {
            var emp = db.Employee.Where(x => x.EmployeeID == EmpId).SingleOrDefault();

            //確認信箱 ConfirmEmail 1成功 -> 登入後 ResetPwd 重設密碼成功 更新 LastPasswordChangedDate = DateTime.Now
            //若 Confirmemail 0 回倒 ResetPwd 頁面

            emp.AccountStatus = "E";

            //4.存到資料庫
            //Emp table
            db.Entry(emp).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }
    }
}