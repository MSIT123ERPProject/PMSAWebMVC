using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using PMSAWebMVC.Models;
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
    [Authorize]
    public class UsersAdminController : BaseController
    {
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
                    SendLetterStatus = await db.Employee.Where(e => e.EmployeeID == user.UserName).Select(e => e.SendLetterStatus).FirstOrDefaultAsync(),
                    //取得 所有Role列表，若有出現在該userA中 Selected = true
                    RolesList = RoleManager.Roles.ToList().Select(r => new
                    {
                        Selected = userRoles.Contains(r.Name),
                        Text = r.Description,
                        Value = r.Name
                    })
                };

                return Json(userA, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> IndexAjax()
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
                    SendLetterStatus = await db.Employee.Where(e => e.EmployeeID == x.UserName).Select(e => e.SendLetterStatus).FirstOrDefaultAsync()
                };
                emps.Add(user);
            }
            return Json(emps, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Users/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                // Process normally:
                var user = await UserManager.FindByIdAsync(id);
                ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);
                return View(user);
            }
            // Return Error:
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        //
        // GET: /Users/Create
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            ViewBag.empID = new SelectList(db.Employee, "EmployeeID", "EmployeeID");
            return View();
        }

        private PMSAEntities db = new PMSAEntities();

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

        //管理員新增採購員帳號
        // POST: /Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel model, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                ApplicationDbContext context = new ApplicationDbContext();
                //ApplicationRoleStore roleStore = new ApplicationRoleStore(context);
                //ApplicationRoleManager roleManager = new ApplicationRoleManager(roleStore);
                //新增 user 資料
                var user = new ApplicationUser
                {
                    //EmployeeID = model.EmployeeID,
                    UserName = model.EmployeeID,
                    //realName = model.realName,
                    Email = model.Email,
                    //AccountStatus = "R",
                    //CreateDate = DateTime.Now,
                    //ModifiedDate = DateTime.Now,
                    //SendLetterStatus = "Y",
                    //SendLetterDate = DateTime.Now,
                    SecurityStamp = Guid.NewGuid().ToString("D")
                };
                // TODO
                // 密碼寫死先統一 P@ssw0rd
                // string pwd = generateFirstPwd();
                string pwd = "P@ssw0rd";
                var result = await UserManager.CreateAsync(user, pwd);
                //ViewBag.RoleId = new SelectList(await roleManager.Roles.ToListAsync(), "Name", "Name");
                // 如果成功新增 進行驗證
                if (result.Succeeded)
                {
                    // 新增成功才有 userID(流水號)
                    // 如果選擇不為空
                    if (selectedRoles != null)
                    {
                        // 將 user 加入角色池  //Add User to the selected Roles
                        var resultSelect = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
                        // 加入失敗的話 讓他可以選 RoleId
                        if (!resultSelect.Succeeded)
                        {
                            ModelState.AddModelError("", resultSelect.Errors.First());
                            //ViewBag.RoleId = new SelectList(await roleManager.Roles.ToListAsync(), "Name", "Name");
                            return View();
                        }
                    }
                    //?????
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // TODO 帳戶確認及密碼重設
                    // 如需如何進行帳戶確認及密碼重設的詳細資訊，請前往 https://go.microsoft.com/fwlink/?LinkID=320771
                    // 傳送包含此連結的電子郵件
                    var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("PMSAWebMVC");
                    //UserManager.UserTokenProvider = new Microsoft.AspNet.Identity.Owin.DataProtectorTokenProvider<ApplicationUser, int>(provider.Create("TokenName"));
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "確認您的帳戶", $"<h1>請按一下此連結確認您的帳戶</h1> <a href='{callbackUrl}'>這裏</a><h5>您好，您的密碼是：</h5><h5>{pwd}</h5>");

                    ViewBag.Link = callbackUrl;
                    return View("DisplayEmail");
                }
            }
            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        //
        // GET: /Users/Edit/1
        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                var userRoles = await UserManager.GetRolesAsync(user.Id);
                return View(new EditUserViewModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                    {
                        Selected = userRoles.Contains(x.Name),
                        Text = x.Name,
                        Value = x.Name
                    })
                });
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Email,Id")] EditUserViewModel editUser, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                user.UserName = editUser.Email;
                user.Email = editUser.Email;

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Something failed.");
            return View();
        }

        //
        // GET: /Users/Delete/5
        [HttpGet]
        public async Task<ActionResult> Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                return View(user);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        //
        // POST: /Users/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    var user = await UserManager.FindByIdAsync(id);
                    if (user == null)
                    {
                        return HttpNotFound();
                    }
                    var result = await UserManager.DeleteAsync(user);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First());
                        return View();
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            return View();
        }
    }
}